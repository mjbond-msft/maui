using System;
using System.Linq;
using WSwipeControl = Microsoft.UI.Xaml.Controls.SwipeControl;
using WSwipeItems = Microsoft.UI.Xaml.Controls.SwipeItems;
using WSwipeItem = Microsoft.UI.Xaml.Controls.SwipeItem;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Handlers
{
	public partial class SwipeViewHandler : ViewHandler<ISwipeView, WSwipeControl>
	{
		protected override WSwipeControl CreateNativeView() => new();

		public override void SetVirtualView(IView view)
		{
			base.SetVirtualView(view);
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
		}

		public static void MapContent(ISwipeViewHandler handler, ISwipeView view)
		{
			_ = handler.NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = handler.MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");
			_ = handler.TypedVirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");


			if (handler.TypedVirtualView.PresentedContent is not IView presentedView)
				return;

			handler.TypedNativeView.Content = presentedView.ToNative(handler.MauiContext, true);
		}

		public static void MapSwipeTransitionMode(ISwipeViewHandler handler, ISwipeView swipeView)
		{
		}

		public static void MapRequestOpen(ISwipeViewHandler handler, ISwipeView swipeView, object? args)
		{
			if (args is not SwipeViewOpenRequest)
			{
				return;
			}
		}

		public static void MapRequestClose(ISwipeViewHandler handler, ISwipeView swipeView, object? args)
		{
			handler.TypedNativeView.Close();
		}

		public static void MapLeftItems(ISwipeViewHandler handler, ISwipeView view)
		{
			UpdateSwipeMode(view.LeftItems, view, handler.TypedNativeView);
			UpdateSwipeBehaviorOnInvoked(view.LeftItems, view, handler.TypedNativeView);
		}
		public static void MapTopItems(ISwipeViewHandler handler, ISwipeView view)
		{
			UpdateSwipeMode(view.TopItems, view, handler.TypedNativeView);
			UpdateSwipeBehaviorOnInvoked(view.TopItems, view, handler.TypedNativeView);
		}

		public static void MapRightItems(ISwipeViewHandler handler, ISwipeView view)
		{
			UpdateSwipeMode(view.RightItems, view, handler.TypedNativeView);
			UpdateSwipeBehaviorOnInvoked(view.RightItems, view, handler.TypedNativeView);
		}

		public static void MapBottomItems(ISwipeViewHandler handler, ISwipeView view)
		{
			UpdateSwipeMode(view.BottomItems, view, handler.TypedNativeView);
			UpdateSwipeBehaviorOnInvoked(view.BottomItems, view, handler.TypedNativeView);
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (NativeView.Parent != null)
				return base.GetDesiredSize(widthConstraint, heightConstraint);
			else
			{
				if (widthConstraint * heightConstraint == 0)
					return new Size(0, 0);

				double width = Math.Max(0, VirtualView.Width);
				double height = Math.Max(0, VirtualView.Height);
				var result = new Size(width, height);

				double w = VirtualView.Width;
				double h = VirtualView.Height;

				if (w == -1)
					w = widthConstraint;

				if (h == -1)
					h = heightConstraint;

				w = Math.Max(0, w);
				h = Math.Max(0, h);

				// SwipeLayout sometimes crashes when Measure if not previously fully loaded into the VisualTree.
				NativeView.Loaded += (sender, args) => { NativeView.Measure(new global::Windows.Foundation.Size(w, h)); };

				return result;
			}
		}

		static void UpdateSwipeMode(ISwipeItems swipeItems, ISwipeView swipeView, WSwipeControl swipeControl)
		{
			var windowsSwipeItems = GetWindowsSwipeItems(swipeItems, swipeView, swipeControl);

			if (windowsSwipeItems != null)
				windowsSwipeItems.Mode = swipeItems.Mode.ToNative();
		}

		static void UpdateSwipeBehaviorOnInvoked(ISwipeItems swipeItems, ISwipeView swipeView, WSwipeControl swipeControl)
		{
			var windowsSwipeItems = GetWindowsSwipeItems(swipeItems, swipeView, swipeControl);

			if (windowsSwipeItems != null)
				foreach (var windowSwipeItem in windowsSwipeItems.ToList())
					windowSwipeItem.BehaviorOnInvoked = swipeItems.SwipeBehaviorOnInvoked.ToNative();
		}

		static bool IsValidSwipeItems(ISwipeItems? swipeItems)
		{
			return swipeItems?.Count > 0;
		}

		static WSwipeItems CreateSwipeItems(SwipeDirection swipeDirection, ISwipeViewHandler handler)
		{
			var swipeItems = new WSwipeItems();
			var swipeView = handler.TypedVirtualView;

			ISwipeItems? items = null;

			switch (swipeDirection)
			{
				case SwipeDirection.Left:
					items = swipeView.LeftItems;
					break;
				case SwipeDirection.Right:
					items = swipeView.RightItems;
					break;
				case SwipeDirection.Up:
					items = swipeView.TopItems;
					break;
				case SwipeDirection.Down:
					items = swipeView.BottomItems;
					break;
			}

			if (items == null)
				return swipeItems;

			swipeItems.Mode = items.Mode.ToNative();

			foreach (var item in items)
			{
				if (item is ISwipeItemMenuItem formsSwipeItem)
				{
					var windowsSwipeItem = (WSwipeItem)item.ToHandler(handler.MauiContext!);
					windowsSwipeItem.BehaviorOnInvoked = items.SwipeBehaviorOnInvoked.ToNative();
					swipeItems.Add(windowsSwipeItem);
				}
			}

			return swipeItems;
		}

		static WSwipeItems? GetWindowsSwipeItems(ISwipeItems swipeItems, ISwipeView swipeView, WSwipeControl swipeControl)
		{
			if (swipeItems == swipeView.LeftItems)
				return swipeControl.LeftItems;

			if (swipeItems == swipeView.RightItems)
				return swipeControl.RightItems;

			if (swipeItems == swipeView.TopItems)
				return swipeControl.TopItems;

			if (swipeItems == swipeView.BottomItems)
				return swipeControl.BottomItems;

			return null;
		}
	}
}