﻿using System.Linq;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Win2D;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Microsoft.Maui
{
	public partial class WindowOverlay
	{
		W2DGraphicsView? _graphicsView;
		Frame? _frame;
		Panel? _panel;
		FrameworkElement? _nativeElement;

		/// <inheritdoc/>
		public virtual bool Initialize()
		{
			if (IsNativeViewInitialized)
				return true;

			if (Window?.Content == null)
				return false;

			_nativeElement = Window.Content.GetNative(true);
			if (_nativeElement == null)
				return false;
			var handler = Window.Handler as WindowHandler;
			if (handler?.NativeView is not Window _window)
				return false;

			_panel = _window.Content as Panel;
			// Capture when the frame is navigating.
			// When it is, we will clear existing adorners.
			if (_nativeElement is Frame frame)
			{
				_frame = frame;
				_frame.Navigating += FrameNavigating;
			}

			_graphicsView = new W2DGraphicsView() { Drawable = this };
			if (_graphicsView == null)
				return false;

			_nativeElement.Tapped += ViewTapped;
			_nativeElement.PointerMoved += PointerMoved;
			_graphicsView.Tapped += ViewTapped;
			_graphicsView.PointerMoved += PointerMoved;

			_graphicsView.SetValue(Canvas.ZIndexProperty, 99);
			_graphicsView.IsHitTestVisible = false;
			_graphicsView.Visibility = UI.Xaml.Visibility.Collapsed;

			_panel?.Children.Add(_graphicsView);
			
			IsNativeViewInitialized = true;
			return IsNativeViewInitialized;
		}

		/// <inheritdoc/>
		public void Invalidate()
		{
			if (_graphicsView is null)
				return;

			// Hide the visibility of the graphics view if there are no drawn elements.
			// This way, the In-App Toolbar will work as expected.
			_graphicsView.Visibility = WindowElements.Any() ? UI.Xaml.Visibility.Visible : UI.Xaml.Visibility.Collapsed;
			_graphicsView.Invalidate();
		}

		/// <summary>
		/// Deinitializes the native event hooks and handlers used to drive the overlay.
		/// </summary>
		void DeinitializeNativeDependencies()
		{
			if (_frame != null)
				_frame.Navigating -= FrameNavigating;
			if (_panel != null)
				_panel.Children.Remove(_graphicsView);
			if (_nativeElement != null)
			{
				_nativeElement.Tapped -= ViewTapped;
				_nativeElement.PointerMoved -= PointerMoved;
			}
			if (_graphicsView != null)
			{
				_graphicsView.Tapped -= ViewTapped;
				_graphicsView.PointerMoved -= PointerMoved;
			}
			_graphicsView = null;
			IsNativeViewInitialized = false;
		}

		void PointerMoved(object sender, UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			if (!EnableDrawableTouchHandling)
				return;

			if (!_windowElements.Any())
				return;

			if (_graphicsView == null)
				return;

			var pointerPoint = e.GetCurrentPoint(_graphicsView);
			if (pointerPoint == null)
				return;

			_graphicsView.IsHitTestVisible = _windowElements.Any(n => n.Contains(new Point(pointerPoint.Position.X, pointerPoint.Position.Y)));
		}

		void FrameNavigating(object sender, UI.Xaml.Navigation.NavigatingCancelEventArgs e)
		{
			HandleUIChange();
			Invalidate();
		}

		void ViewTapped(object sender, UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			if (e == null)
				return;
			var position = e.GetPosition(_graphicsView);
			var point = new Point(position.X, position.Y);

			if (DisableUITouchEventPassthrough)
				e.Handled = true;
			else if (EnableDrawableTouchHandling)
				e.Handled = _windowElements.Any(n => n.Contains(point));

			OnTappedInternal(point);
		}

		partial void OnDisableUITouchEventPassthroughSet()
		{
			if (_graphicsView != null)
				_graphicsView.IsHitTestVisible = DisableUITouchEventPassthrough;
		}
	}
}