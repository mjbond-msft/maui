﻿using System;
using Android.Views;
using AndroidX.Core.View;
using AndroidX.Core.Widget;
using NativeView = Android.Views.View;

namespace Microsoft.Maui.Handlers
{
	public partial class ViewHandler
	{
		partial void DisconnectingHandler(NativeView nativeView)
		{
			if (nativeView.IsAlive()
				&& ViewCompat.GetAccessibilityDelegate(nativeView) is MauiAccessibilityDelegateCompat ad)
			{
				ad.Handler = null;
				ViewCompat.SetAccessibilityDelegate(nativeView, null);
			}
		}

		void OnRootViewSet(object? sender, EventArgs e)
		{
			UpdateValue(nameof(IToolbarElement.Toolbar));
		}

		static partial void MappingFrame(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateAnchorX(view);
			handler.GetWrappedNativeView()?.UpdateAnchorY(view);
		}

		public static void MapTranslationX(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateTranslationX(view);
		}

		public static void MapTranslationY(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateTranslationY(view);
		}

		public static void MapScale(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateScale(view);
		}

		public static void MapScaleX(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateScaleX(view);
		}

		public static void MapScaleY(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateScaleY(view);
		}

		public static void MapRotation(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateRotation(view);
		}

		public static void MapRotationX(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateRotationX(view);
		}

		public static void MapRotationY(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateRotationY(view);
		}

		public static void MapAnchorX(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateAnchorX(view);
		}

		public static void MapAnchorY(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateAnchorY(view);
		}

		static partial void MappingSemantics(IViewHandler handler, IView view)
		{
			if (handler.NativeView == null)
				return;

			var accessibilityDelegate = ViewCompat.GetAccessibilityDelegate(handler.NativeView as View) as MauiAccessibilityDelegateCompat;

			if (handler.NativeView is not NativeView nativeView)
				return;

			nativeView = nativeView.GetSemanticNativeElement();

			var desc = view.Semantics?.Description;
			var hint = view.Semantics?.Hint;

			// We use MauiAccessibilityDelegateCompat to fix the issue of AutomationId breaking accessibility
			// Because AutomationId gets set on the contentDesc we have to clear that out on the accessibility node via
			// the use of our MauiAccessibilityDelegateCompat
			if (!string.IsNullOrWhiteSpace(hint) ||
				!string.IsNullOrWhiteSpace(desc) ||
				!string.IsNullOrWhiteSpace(view.AutomationId))
			{
				if (accessibilityDelegate == null)
				{
					var currentDelegate = ViewCompat.GetAccessibilityDelegate(nativeView);
					if (currentDelegate is MauiAccessibilityDelegateCompat)
						currentDelegate = null;

					accessibilityDelegate = new MauiAccessibilityDelegateCompat(currentDelegate)
					{
						Handler = handler
					};

					ViewCompat.SetAccessibilityDelegate(nativeView, accessibilityDelegate);
				}

				if (!string.IsNullOrWhiteSpace(hint) ||
					!string.IsNullOrWhiteSpace(desc))
				{
					nativeView.ImportantForAccessibility = ImportantForAccessibility.Yes;
				}
			}
			else if (accessibilityDelegate != null)
			{
				ViewCompat.SetAccessibilityDelegate(nativeView, null);
			}
		}

		public static void MapToolbar(IViewHandler handler, IView view)
		{
			if (handler.VirtualView is not IToolbarElement te || te.Toolbar == null)
				return;

			var rootManager = handler.MauiContext?.GetNavigationRootManager();
			rootManager?.SetToolbarElement(te);

			var nativeView = handler.NativeView as View;
			if (nativeView == null)
				return;

			_ = handler.MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");
			var appbarLayout = nativeView.FindViewById<ViewGroup>(Microsoft.Maui.Resource.Id.navigationlayout_appbar);

			if (appbarLayout == null)
				appbarLayout = rootManager?.RootView?.FindViewById<ViewGroup>(Microsoft.Maui.Resource.Id.navigationlayout_appbar);

			var nativeToolBar = te.Toolbar?.ToNative(handler.MauiContext, true);

			if (appbarLayout == null || nativeToolBar == null)
			{
				return;
			}

			if (nativeToolBar.Parent == appbarLayout)
			{
				return;
			}

			appbarLayout.AddView(nativeToolBar, 0);
		}


		internal static void MapToolbar(IElementHandler handler, IToolbarElement te)
		{
			if (te.Toolbar == null)
				return;

			var rootManager = handler.MauiContext?.GetNavigationRootManager();
			rootManager?.SetToolbarElement(te);

			var nativeView = handler.NativeView as View;

			_ = handler.MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");
			var appbarLayout = nativeView?.FindViewById<ViewGroup>(Microsoft.Maui.Resource.Id.navigationlayout_appbar) ??
				rootManager?.RootView?.FindViewById<ViewGroup>(Microsoft.Maui.Resource.Id.navigationlayout_appbar);

			var nativeToolBar = te.Toolbar?.ToNative(handler.MauiContext, true);

			if (appbarLayout == null)
			{
				return;
			}

			if (appbarLayout.ChildCount > 0 &&
				appbarLayout.GetChildAt(0) == nativeToolBar)
			{
				return;
			}

			appbarLayout.AddView(nativeToolBar, 0);
		}


	}
}