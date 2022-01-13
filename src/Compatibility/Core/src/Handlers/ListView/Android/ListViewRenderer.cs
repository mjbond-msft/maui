using System;
using System.ComponentModel;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Graphics;
using AListView = Android.Widget.ListView;
using AView = Android.Views.View;
using LayoutParams = Android.Views.ViewGroup.LayoutParams;

namespace Microsoft.Maui.Controls.Handlers.Compatibility
{
	public class ListViewRenderer : ViewRenderer<ListView, AListView>
	{
		public static PropertyMapper<ListView, ListViewRenderer> Mapper =
				new PropertyMapper<ListView, ListViewRenderer>(ViewMapper);

		ListViewAdapter _adapter;
		INativeViewHandler _headerRenderer;
		INativeViewHandler _footerRenderer;
		Container _headerView;
		Container _footerView;
		bool _isAttached;
		ScrollToRequestedEventArgs _pendingScrollTo;

		SwipeRefreshLayout _refresh;
		IListViewController Controller => Element;
		ITemplatedItemsView<Cell> TemplatedItemsView => Element;

		ScrollBarVisibility _defaultHorizontalScrollVisibility = 0;
		ScrollBarVisibility _defaultVerticalScrollVisibility = 0;

		public ListViewRenderer() : base(Mapper)
		{
		}

		protected override Size MinimumSize()
		{
			return new Size(40, 40);
		}

		protected virtual SwipeRefreshLayout CreateNativePullToRefresh(Context context)
			=> new SwipeRefreshLayoutWithFixedNestedScrolling(context);

		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();

			if (Control != null)
				Control.NestedScrollingEnabled = (Parent.GetParentOfType<NestedScrollView>() != null);

			_isAttached = true;
			_adapter.IsAttachedToWindow = _isAttached;
			UpdateIsRefreshing(isInitialValue: true);
		}

		protected override void OnDetachedFromWindow()
		{
			base.OnDetachedFromWindow();

			_isAttached = false;
			_adapter.IsAttachedToWindow = _isAttached;
		}

		protected override AListView CreateNativeControl()
		{
			return new AListView(Context);
		}

		public override bool NeedsContainer => true;

		protected override void SetupContainer()
		{
			if (Context == null || NativeView == null || ContainerView != null)
				return;


			var oldParent = (ViewGroup)NativeView.Parent;

			var oldIndex = oldParent?.IndexOfChild(NativeView);
			oldParent?.RemoveView(NativeView);

			if (ContainerView == null)
			{
				_refresh = CreateNativePullToRefresh(Context);
				_refresh.SetOnRefreshListener(new ListViewSwipeRefreshLayoutListener(this));
				ContainerView = _refresh;
			}

			_refresh.AddView(NativeView, new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent));

			if (oldIndex is int idx && idx >= 0)
				oldParent?.AddView(ContainerView, idx);
			else
				oldParent?.AddView(ContainerView);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				((IListViewController)e.OldElement).ScrollToRequested -= OnScrollToRequested;

				if (Control != null)
				{
					// Unhook the adapter from the ListView before disposing of it
					Control.Adapter = null;

					Control.SetOnScrollListener(null);
				}

				if (_adapter != null)
				{
					_adapter.Dispose();
					_adapter = null;
				}
			}

			if (e.NewElement != null)
			{
				AListView nativeListView = Control;

				if(nativeListView == null)
				{
					nativeListView = CreateNativeControl();
					SetNativeControl(nativeListView);
				}

				var ctx = Context;
				_headerView = new Container(ctx);
				nativeListView.AddHeaderView(_headerView, null, false);
				_footerView = new Container(ctx);
				nativeListView.AddFooterView(_footerView, null, false);

				((IListViewController)e.NewElement).ScrollToRequested += OnScrollToRequested;
				Control?.SetOnScrollListener(new ListViewScrollDetector(this));

				nativeListView.DividerHeight = 0;
				nativeListView.Focusable = false;
				nativeListView.DescendantFocusability = DescendantFocusability.AfterDescendants;
				nativeListView.Adapter = _adapter = e.NewElement.IsGroupingEnabled && e.NewElement.OnThisPlatform().IsFastScrollEnabled() ? new GroupedListViewAdapter(Context, nativeListView, e.NewElement) : new ListViewAdapter(Context, nativeListView, e.NewElement);
				_adapter.HeaderView = _headerView;
				_adapter.FooterView = _footerView;
				_adapter.IsAttachedToWindow = _isAttached;

				UpdateHeader();
				UpdateFooter();
				UpdateIsSwipeToRefreshEnabled();
				UpdateFastScrollEnabled();
				UpdateSelectionMode();
				UpdateSpinnerColor();
				UpdateHorizontalScrollBarVisibility();
				UpdateVerticalScrollBarVisibility();
			}
		}

		internal void ClickOn(AView viewCell)
		{
			if (Control == null)
			{
				return;
			}

			var position = Control.GetPositionForView(viewCell);
			var id = Control.GetItemIdAtPosition(position);

			viewCell.PerformHapticFeedback(FeedbackConstants.ContextClick);
			_adapter.OnItemClick(Control, viewCell, position, id);
		}

		internal void LongClickOn(AView viewCell)
		{
			if (Control == null)
			{
				return;
			}

			var position = Control.GetPositionForView(viewCell);
			var id = Control.GetItemIdAtPosition(position);

			viewCell.PerformHapticFeedback(FeedbackConstants.ContextClick);
			_adapter.OnItemLongClick(Control, viewCell, position, id);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == "HeaderElement")
				UpdateHeader();
			else if (e.PropertyName == "FooterElement")
				UpdateFooter();
			else if (e.PropertyName == "RefreshAllowed")
				UpdateIsSwipeToRefreshEnabled();
			else if (e.PropertyName == ListView.IsPullToRefreshEnabledProperty.PropertyName)
				UpdateIsSwipeToRefreshEnabled();
			else if (e.PropertyName == ListView.IsRefreshingProperty.PropertyName)
				UpdateIsRefreshing();
			else if (e.PropertyName == ListView.SeparatorColorProperty.PropertyName || e.PropertyName == ListView.SeparatorVisibilityProperty.PropertyName)
				_adapter.NotifyDataSetChanged();
			else if (e.PropertyName == PlatformConfiguration.AndroidSpecific.ListView.IsFastScrollEnabledProperty.PropertyName)
				UpdateFastScrollEnabled();
			else if (e.PropertyName == ListView.SelectionModeProperty.PropertyName)
				UpdateSelectionMode();
			else if (e.PropertyName == ListView.RefreshControlColorProperty.PropertyName)
				UpdateSpinnerColor();
			else if (e.PropertyName == ScrollView.HorizontalScrollBarVisibilityProperty.PropertyName)
				UpdateHorizontalScrollBarVisibility();
			else if (e.PropertyName == ScrollView.VerticalScrollBarVisibilityProperty.PropertyName)
				UpdateVerticalScrollBarVisibility();
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (double.IsInfinity(heightConstraint))
			{
				if (VirtualView.RowHeight > -1)
				{
					heightConstraint = (int)(_adapter.Count * VirtualView.RowHeight);
				}
				else if (_adapter != null)
				{
					double totalHeight = 0;
					int adapterCount = _adapter.Count;
					for (int i = 0; i < adapterCount; i++)
					{
						var cell = _adapter.GetCellsFromPosition(i, 1)[0];
						if (cell.Height > -1)
						{
							totalHeight += cell.Height;
							continue;
						}

						AView listItem = _adapter.GetView(i, null, NativeView);
						int widthSpec;

						if (double.IsInfinity(widthConstraint))
							widthSpec = MeasureSpecMode.Unspecified.MakeMeasureSpec(0);
						else
							widthSpec = MeasureSpecMode.AtMost.MakeMeasureSpec((int)Context.ToPixels(widthConstraint));

						listItem.Measure(widthSpec, MeasureSpecMode.Unspecified.MakeMeasureSpec(0));
						totalHeight += Context.FromPixels(listItem.MeasuredHeight);
					}

					heightConstraint = totalHeight;
				}
			}

			return base.GetDesiredSize(widthConstraint, heightConstraint);
		}

		public override void NativeArrange(Rectangle frame)
		{
			base.NativeArrange(frame);

			if (_pendingScrollTo != null)
			{
				OnScrollToRequested(this, _pendingScrollTo);
				_pendingScrollTo = null;
			}
		}

		void OnScrollToRequested(object sender, ScrollToRequestedEventArgs e)
		{
			if (!_isAttached)
			{
				_pendingScrollTo = e;
				return;
			}

			Cell cell;
			int scrollPosition;
			var scrollArgs = (ITemplatedItemsListScrollToRequestedEventArgs)e;

			var templatedItems = TemplatedItemsView.TemplatedItems;
			if (Element.IsGroupingEnabled)
			{
				var results = templatedItems.GetGroupAndIndexOfItem(scrollArgs.Group, scrollArgs.Item);
				int indexOfGroup = results.Item1;
				int indexOfItem = results.Item2;

				if (indexOfGroup == -1)
					return;

				int itemIndex = indexOfItem == -1 ? 0 : indexOfItem;

				var group = templatedItems.GetGroup(indexOfGroup);
				if (group.Count == 0)
					cell = group.HeaderContent;
				else
					cell = group[itemIndex];

				//Increment Scroll Position by 1 when Grouping is enabled. Android offsets position of cells when using header.
				scrollPosition = templatedItems.GetGlobalIndexForGroup(group) + itemIndex + 1;
			}
			else
			{
				scrollPosition = templatedItems.GetGlobalIndexOfItem(scrollArgs.Item);
				if (scrollPosition == -1)
					return;

				cell = templatedItems[scrollPosition];
			}

			//Android offsets position of cells when using header
			int realPositionWithHeader = scrollPosition + 1;

			if (e.Position == ScrollToPosition.MakeVisible)
			{
				if (e.ShouldAnimate)
					Control.SmoothScrollToPosition(realPositionWithHeader);
				else
					Control.SetSelection(realPositionWithHeader);
				return;
			}

			int height = Control.Height;
			var cellHeight = (int)cell.RenderHeight;
			if (cellHeight == -1)
			{
				int first = Control.FirstVisiblePosition;
				if (first <= scrollPosition && scrollPosition <= Control.LastVisiblePosition)
					cellHeight = Control.GetChildAt(scrollPosition - first).Height;
				else
				{
					AView view = _adapter.GetView(scrollPosition, null, null);


					view.Measure(MeasureSpecMode.AtMost.MakeMeasureSpec(Control.Width), MeasureSpecMode.Unspecified.MakeMeasureSpec(0));
					cellHeight = view.MeasuredHeight;
				}
			}

			var y = 0;

			if (e.Position == ScrollToPosition.Center)
				y = height / 2 - cellHeight / 2;
			else if (e.Position == ScrollToPosition.End)
				y = height - cellHeight;

			if (e.ShouldAnimate)
				Control.SmoothScrollToPositionFromTop(realPositionWithHeader, y);
			else
				Control.SetSelectionFromTop(realPositionWithHeader, y);
		}

		void UpdateFooter()
		{
			var footer = (VisualElement)Controller.FooterElement;
			if (_footerRenderer != null)
			{
				var reflectableType = _footerRenderer as System.Reflection.IReflectableType;
				var rendererType = reflectableType != null ? reflectableType.GetTypeInfo().AsType() : _footerRenderer.GetType();
				if (footer == null || Registrar.Registered.GetHandlerTypeForObject(footer) != rendererType)
				{
					if (_footerView != null)
						_footerView.Child = null;

					_footerRenderer.VirtualView.Handler?.DisconnectHandler();
					_footerRenderer = null;
				}
			}

			if (footer == null)
				return;

			if (_footerRenderer != null)
				_footerRenderer.SetVirtualView(footer);
			else
			{
				_ = footer.ToNative(Element.FindMauiContext());
				if (_footerView != null)
					_footerView.Child = (INativeViewHandler)footer.Handler;
			}
		}

		void UpdateHeader()
		{
			var header = (VisualElement)Controller.HeaderElement;
			if (_headerRenderer != null)
			{
				var reflectableType = _headerRenderer as System.Reflection.IReflectableType;
				var rendererType = reflectableType != null ? reflectableType.GetTypeInfo().AsType() : _headerRenderer.GetType();
				if (header == null || Registrar.Registered.GetHandlerTypeForObject(header) != rendererType)
				{
					if (_headerView != null)
						_headerView.Child = null;
					_headerRenderer.VirtualView.Handler?.DisconnectHandler();
					_headerRenderer = null;
				}
			}

			if (header == null)
				return;

			if (_headerRenderer != null)
				_headerRenderer.SetVirtualView(header);
			else
			{
				_ = header.ToNative(Element.FindMauiContext());
				if (_headerView != null)
					_headerView.Child = (INativeViewHandler)header.Handler;
			}
		}

		void UpdateIsRefreshing(bool isInitialValue = false)
		{
			if (_refresh != null)
			{
				var isRefreshing = Element.IsRefreshing;
				if (isRefreshing && isInitialValue)
				{
					_refresh.Refreshing = false;
					_refresh.Post(() =>
					{
						if (_refresh.IsDisposed())
							return;

						_refresh.Refreshing = true;
					});
				}
				else
					_refresh.Refreshing = isRefreshing;
			}
		}

		void UpdateIsSwipeToRefreshEnabled()
		{
			if (_refresh != null)
				_refresh.Enabled = Element.IsPullToRefreshEnabled && (Element as IListViewController).RefreshAllowed;
		}

		void UpdateFastScrollEnabled()
		{
			if (Control != null)
			{
				Control.FastScrollEnabled = Element.OnThisPlatform().IsFastScrollEnabled();
			}
		}

		void UpdateSelectionMode()
		{
			if (Control != null)
			{
				if (Element.SelectionMode == ListViewSelectionMode.None)
				{
					Control.ChoiceMode = ChoiceMode.None;
					Element.SelectedItem = null;
				}
				else if (Element.SelectionMode == ListViewSelectionMode.Single)
				{
					Control.ChoiceMode = ChoiceMode.Single;
				}
			}
		}

		void UpdateSpinnerColor()
		{
			if (_refresh != null && Element.RefreshControlColor != null)
				_refresh.SetColorSchemeColors(Element.RefreshControlColor.ToNative());
		}

		void UpdateHorizontalScrollBarVisibility()
		{
			if (_defaultHorizontalScrollVisibility == 0)
			{
				_defaultHorizontalScrollVisibility = Control.HorizontalScrollBarEnabled ? ScrollBarVisibility.Always : ScrollBarVisibility.Never;
			}

			var newHorizontalScrollVisiblility = Element.HorizontalScrollBarVisibility;

			if (newHorizontalScrollVisiblility == ScrollBarVisibility.Default)
			{
				newHorizontalScrollVisiblility = _defaultHorizontalScrollVisibility;
			}

			Control.HorizontalScrollBarEnabled = newHorizontalScrollVisiblility == ScrollBarVisibility.Always;
		}

		void UpdateVerticalScrollBarVisibility()
		{
			if (_defaultVerticalScrollVisibility == 0)
				_defaultVerticalScrollVisibility = Control.VerticalScrollBarEnabled ? ScrollBarVisibility.Always : ScrollBarVisibility.Never;

			var newVerticalScrollVisibility = Element.VerticalScrollBarVisibility;

			if (newVerticalScrollVisibility == ScrollBarVisibility.Default)
				newVerticalScrollVisibility = _defaultVerticalScrollVisibility;

			Control.VerticalScrollBarEnabled = newVerticalScrollVisibility == ScrollBarVisibility.Always;
		}

		internal class Container : ViewGroup
		{
			INativeViewHandler _child;

			public Container(Context context) : base(context)
			{
			}

			public INativeViewHandler Child
			{
				set
				{
					if (_child != null)
						RemoveView(_child.NativeView);

					_child = value;

					if (value != null)
						AddView(value.NativeView);
				}
			}

			protected override void OnLayout(bool changed, int l, int t, int r, int b)
			{
				if (_child?.NativeView == null)
				{
					return;
				}

				_child.NativeView.Layout(l, t, r, b);
			}

			protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
			{
				if (_child?.NativeView == null)
				{
					SetMeasuredDimension(0, 0);
					return;
				}

				_child.NativeView.Measure(widthMeasureSpec, heightMeasureSpec);
				SetMeasuredDimension(_child.NativeView.MeasuredWidth, _child.NativeView.MeasuredHeight);
			}
		}

		class SwipeRefreshLayoutWithFixedNestedScrolling : SwipeRefreshLayout
		{
			float _touchSlop;
			float _initialDownY;
			bool _nestedScrollAccepted;
			bool _nestedScrollCalled;

			public SwipeRefreshLayoutWithFixedNestedScrolling(Context ctx) : base(ctx)
			{
				_touchSlop = ViewConfiguration.Get(ctx).ScaledTouchSlop;
			}

			public override bool OnInterceptTouchEvent(MotionEvent ev)
			{
				if (ev.Action == MotionEventActions.Down)
					_initialDownY = ev.GetAxisValue(Axis.Y);

				var isBeingDragged = base.OnInterceptTouchEvent(ev);

				if (!isBeingDragged && ev.Action == MotionEventActions.Move && _nestedScrollAccepted && !_nestedScrollCalled)
				{
					var y = ev.GetAxisValue(Axis.Y);
					var dy = (y - _initialDownY) / 2;
					isBeingDragged = dy > _touchSlop;
				}

				return isBeingDragged;
			}

			public override void OnNestedScrollAccepted(AView child, AView target, [GeneratedEnum] ScrollAxis axes)
			{
				base.OnNestedScrollAccepted(child, target, axes);
				_nestedScrollAccepted = true;
				_nestedScrollCalled = false;
			}

			public override void OnStopNestedScroll(AView child)
			{
				base.OnStopNestedScroll(child);
				_nestedScrollAccepted = false;
			}

			public override void OnNestedScroll(AView target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed)
			{
				base.OnNestedScroll(target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed);
				_nestedScrollCalled = true;
			}
		}

		class ListViewSwipeRefreshLayoutListener : Java.Lang.Object, SwipeRefreshLayout.IOnRefreshListener
		{
			readonly ListViewRenderer _listViewRenderer;

			public ListViewSwipeRefreshLayoutListener(ListViewRenderer listViewRenderer)
			{
				_listViewRenderer = listViewRenderer;
			}

			public void OnRefresh()
			{
				IListViewController controller = _listViewRenderer.Element;
				controller.SendRefreshing();
			}
		}

		class ListViewScrollDetector : Java.Lang.Object, AbsListView.IOnScrollListener
		{
			class TrackElement
			{
				public TrackElement(int position)
				{
					_position = position;
				}

				readonly int _position;

				AView _trackedView;
				int _trackedViewPrevPosition;
				int _trackedViewPrevTop;

				public void SyncState(AbsListView view)
				{
					if (view.ChildCount > 0)
					{
						_trackedView = GetChild(view);
						_trackedViewPrevTop = GetY();
						_trackedViewPrevPosition = view.GetPositionForView(_trackedView);
					}
				}

				public void Reset()
				{
					_trackedView = null;
				}

				public bool IsSafeToTrack(AbsListView view)
				{
					return _trackedView != null && _trackedView.Parent == view && view.GetPositionForView(_trackedView) == _trackedViewPrevPosition;
				}

				public int GetDeltaY()
				{
					return GetY() - _trackedViewPrevTop;
				}

				AView GetChild(AbsListView view)
				{
					switch (_position)
					{
						case 0:
							return view.GetChildAt(0);
						case 1:
						case 2:
							return view.GetChildAt(view.ChildCount / 2);
						case 3:
							return view.GetChildAt(view.ChildCount - 1);
						default:
							return null;
					}
				}
				int GetY()
				{
					return _position <= 1 ? _trackedView.Bottom : _trackedView.Top;
				}
			}

			readonly ListView _element;
			readonly float _density;
			int _contentOffset;

			public ListViewScrollDetector(ListViewRenderer renderer)
			{
				_element = renderer.Element;
				_density = renderer.Context.Resources.DisplayMetrics.Density;
			}

			void SendScrollEvent(double y)
			{
				var element = _element;
				double offset = Math.Abs(y) / _density;
				var args = new ScrolledEventArgs(0, offset);
				element?.SendScrolled(args);
			}


			readonly TrackElement[] _trackElements =
			{
				new TrackElement(0), // Top view, bottom Y
				new TrackElement(1), // Mid view, bottom Y
				new TrackElement(2), // Mid view, top Y
				new TrackElement(3) // Bottom view, top Y
			};


			public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
			{
				var wasTracked = false;
				foreach (TrackElement t in _trackElements)
				{
					if (!wasTracked)
					{
						if (t.IsSafeToTrack(view))
						{
							wasTracked = true;
							_contentOffset += t.GetDeltaY();
							SendScrollEvent(_contentOffset);
							t.SyncState(view);
						}
						else
						{
							t.Reset();
							t.SyncState(view);
						}
					}
					else
					{
						t.SyncState(view);
					}
				}
			}

			public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
			{
				if (scrollState == ScrollState.TouchScroll || scrollState == ScrollState.Fling)
				{
					foreach (TrackElement t in _trackElements)
					{
						t.SyncState(view);
					}
				}
			}
		}
	}
}
