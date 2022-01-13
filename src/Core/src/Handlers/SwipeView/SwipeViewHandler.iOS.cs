using System;
using System.Threading.Tasks;
using UIKit;

namespace Microsoft.Maui.Handlers
{
	public partial class SwipeViewHandler : ViewHandler<ISwipeView, UIView>
	{
		protected override UIView CreateNativeView()
		{
			throw new NotImplementedException();
		}

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
		}

		public static void MapSwipeTransitionMode(ISwipeViewHandler handler, ISwipeView swipeView)
		{
		}

		public static void MapRequestOpen(ISwipeViewHandler handler, ISwipeView swipeView, object? args)
		{
			if (args is not SwipeViewOpenRequest request)
			{
				return;
			}
		}

		public static void MapRequestClose(ISwipeViewHandler handler, ISwipeView swipeView, object? args)
		{
			if (args is not SwipeViewCloseRequest request)
			{
				return;
			}
		}

		public static void MapLeftItems(ISwipeViewHandler handler, ISwipeView view)
		{
		}
		public static void MapTopItems(ISwipeViewHandler handler, ISwipeView view)
		{
		}
		public static void MapRightItems(ISwipeViewHandler handler, ISwipeView view)
		{
		}
		public static void MapBottomItems(ISwipeViewHandler handler, ISwipeView view)
		{
		}
	}
}