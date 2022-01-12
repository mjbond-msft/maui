using System;
using System.Runtime.CompilerServices;
using Android.Content;
using Android.Views;
using Microsoft.Maui.Graphics;
using static Microsoft.Maui.Primitives.Dimension;

namespace Microsoft.Maui.Handlers
{
	public partial class ViewHandler<TVirtualView, TNativeView> : INativeViewHandler
	{
		View? INativeViewHandler.NativeView => this.GetWrappedNativeView();
		View? INativeViewHandler.ContainerView => ContainerView;

		public Context Context => MauiContext?.Context ?? throw new InvalidOperationException($"Context cannot be null here");

		public override void NativeArrange(Rectangle frame)
		{
			var nativeView = this.GetWrappedNativeView();

			if (nativeView == null || MauiContext == null || Context == null)
			{
				return;
			}

			if (frame.Width < 0 || frame.Height < 0)
			{
				// This is a legacy layout value from Controls, nothing is actually laying out yet so we just ignore it
				return;
			}

			var left = Context.ToPixels(frame.Left);
			var top = Context.ToPixels(frame.Top);
			var bottom = Context.ToPixels(frame.Bottom);
			var right = Context.ToPixels(frame.Right);

			nativeView.Layout((int)left, (int)top, (int)right, (int)bottom);

			Invoke(nameof(IView.Frame), frame);
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			var nativeView = this.GetWrappedNativeView();

			if (nativeView == null || VirtualView == null || Context == null)
			{
				return Size.Zero;
			}

			// Create a spec to handle the native measure
			var widthSpec = CreateMeasureSpec(widthConstraint, VirtualView.Width, VirtualView.MaximumWidth);
			var heightSpec = CreateMeasureSpec(heightConstraint, VirtualView.Height, VirtualView.MaximumHeight);

			nativeView.Measure(widthSpec, heightSpec);

			// Convert back to xplat sizes for the return value
			return Context.FromPixels(nativeView.MeasuredWidth, nativeView.MeasuredHeight);
		}

		int CreateMeasureSpec(double constraint, double explicitSize, double maximumSize)
		{
			var mode = MeasureSpecMode.AtMost;

			if (IsExplicitSet(explicitSize))
			{
				// We have a set value (i.e., a Width or Height)
				mode = MeasureSpecMode.Exactly;
				constraint = explicitSize;
			}
			else if (IsMaximumSet(maximumSize))
			{
				mode = MeasureSpecMode.AtMost;
				constraint = maximumSize;
			}
			else if (double.IsInfinity(constraint))
			{
				// We've got infinite space; we'll leave the size up to the native control
				mode = MeasureSpecMode.Unspecified;
				constraint = 0;
			}

			// Convert to a native size to create the spec for measuring
			var deviceConstraint = (int)Context!.ToPixels(constraint);

			return mode.MakeMeasureSpec(deviceConstraint);
		}

		protected override void SetupContainer()
		{
			if (Context == null || NativeView == null || ContainerView != null)
				return;

			var oldParent = (ViewGroup?)NativeView.Parent;

			var oldIndex = oldParent?.IndexOfChild(NativeView);
			oldParent?.RemoveView(NativeView);

			ContainerView ??= new WrapperView(Context);
			((ViewGroup)ContainerView).AddView(NativeView);

			if (oldIndex is int idx && idx >= 0)
				oldParent?.AddView(ContainerView, idx);
			else
				oldParent?.AddView(ContainerView);
		}

		protected override void RemoveContainer()
		{
			if (Context == null || NativeView == null || ContainerView == null || NativeView.Parent != ContainerView)
				return;

			var oldParent = (ViewGroup?)ContainerView.Parent;

			var oldIndex = oldParent?.IndexOfChild(ContainerView);
			oldParent?.RemoveView(ContainerView);

			((ViewGroup)ContainerView).RemoveAllViews();
			ContainerView = null;

			if (oldIndex is int idx && idx >= 0)
				oldParent?.AddView(NativeView, idx);
			else
				oldParent?.AddView(NativeView);
		}
	}
}
