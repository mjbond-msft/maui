﻿using System;
using System.Linq;
using CoreGraphics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.CoreGraphics;
using Microsoft.Maui.Graphics.Native;
using UIKit;

namespace Microsoft.Maui
{
	public partial class WindowOverlay
	{
		PassthroughView? _passthroughView;
		IDisposable? _frameObserver;
		NativeGraphicsView? _graphicsView;

		public virtual bool Initialize()
		{
			if (IsNativeViewInitialized)
				return true;

			var nativeLayer = Window?.GetNative(true);
			if (nativeLayer is not UIWindow nativeWindow)
				return false;

			if (nativeWindow?.RootViewController?.View == null)
				return false;

			// Create a passthrough view for holding the canvas and other diagnostics tools.
			_passthroughView = new PassthroughView(this, nativeWindow.RootViewController.View.Frame);

			_graphicsView = new NativeGraphicsView(_passthroughView.Frame, this, new DirectRenderer());
			_graphicsView.AutoresizingMask = UIViewAutoresizing.All;

			_passthroughView.AddSubview(_graphicsView);

			if (_graphicsView == null)
			{
				return false;
			}

			// Any time the frame gets a new value, we need to update and invalidate the canvas.
			_frameObserver = nativeLayer.AddObserver("frame", Foundation.NSKeyValueObservingOptions.New, FrameAction);
			// Disable the graphics view from user input.
			// This will be handled by the passthrough view.
			_graphicsView.UserInteractionEnabled = false;

			// Make the canvas view transparent.
			_graphicsView.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.0f);

			// Add the passthrough view to the front of the stack.
			nativeWindow.RootViewController.View.AddSubview(_passthroughView);
			nativeWindow.RootViewController.View.BringSubviewToFront(_passthroughView);

			// Any time the passthrough view is touched, handle it.
			_passthroughView.OnTouch += UIViewOnTouch;
			IsNativeViewInitialized = true;
			return IsNativeViewInitialized;
		}

		/// <inheritdoc/>
		public void Invalidate()
		{
			_graphicsView?.InvalidateIntrinsicContentSize();
			_graphicsView?.InvalidateDrawable();
		}

		/// <summary>
		/// Deinitializes the native event hooks and handlers used to drive the overlay.
		/// </summary>
		void DeinitializeNativeDependencies()
		{
			_frameObserver?.Dispose();
			_passthroughView?.RemoveFromSuperview();
			_passthroughView?.Dispose();
			IsNativeViewInitialized = false;
		}

		void UIViewOnTouch(object? sender, CGPoint e) =>
			OnTappedInternal(new Point(e.X, e.Y));

		void FrameAction(Foundation.NSObservedChange obj)
		{
			HandleUIChange();
			Invalidate();
		}

		class PassthroughView : UIView
		{
			/// <summary>
			/// Event Handler for handling on touch events on the Passthrough View.
			/// </summary>
			public event EventHandler<CGPoint>? OnTouch;

			WindowOverlay overlay;

			/// <summary>
			/// Initializes a new instance of the <see cref="PassthroughView"/> class.
			/// </summary>
			/// <param name="overlay">The Window Overlay.</param>
			/// <param name="frame">Base Frame.</param>
			public PassthroughView(WindowOverlay windowOverlay, CGRect frame)
				: base(frame)
			{
				overlay = windowOverlay;
			}

			public override bool PointInside(CGPoint point, UIEvent? uievent)
			{
				// If we don't have a UI event, return.
				if (uievent == null)
					return false;

				if (uievent.Type == UIEventType.Hover)
					return false;

				// If we are not pressing down, return.
				if (uievent.Type != UIEventType.Touches)
					return false;

				var disableTouchEvent = false;

				if (overlay.DisableUITouchEventPassthrough)
					disableTouchEvent = true;
				else if (overlay.EnableDrawableTouchHandling)
					disableTouchEvent = overlay.WindowElements.Any(n => n.Contains(new Point(point.X, point.Y)));

				OnTouch?.Invoke(this, point);
				return disableTouchEvent;
			}
		}
	}
}