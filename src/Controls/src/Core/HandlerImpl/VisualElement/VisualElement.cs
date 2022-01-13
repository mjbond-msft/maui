﻿using Microsoft.Maui.Handlers;

namespace Microsoft.Maui.Controls
{
	/// <include file="../../../../docs/Microsoft.Maui.Controls/VisualElement.xml" path="Type[@FullName='Microsoft.Maui.Controls.VisualElement']/Docs" />
	public partial class VisualElement
	{
		/// <include file="../../../../docs/Microsoft.Maui.Controls/VisualElement.xml" path="//Member[@MemberName='ControlsVisualElementMapper']/Docs" />
		public static IPropertyMapper<IView, ViewHandler> ControlsVisualElementMapper = new PropertyMapper<View, ViewHandler>(Element.ControlsElementMapper)
		{
			[nameof(BackgroundColor)] = MapBackgroundColor,
		};

		internal static void RemapForControls()
		{
			ViewHandler.ViewMapper = ControlsVisualElementMapper;
		}

		/// <include file="../../../../docs/Microsoft.Maui.Controls/VisualElement.xml" path="//Member[@MemberName='MapBackgroundColor']/Docs" />
		public static void MapBackgroundColor(ViewHandler handler, View view)
		{
			handler.UpdateValue(nameof(Background));
		}
	}
}
