﻿using System;

namespace Microsoft.Maui.Handlers
{
	public partial class RadioButtonHandler : ViewHandler<IRadioButton, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();
		public static void MapIsChecked(RadioButtonHandler handler, IRadioButton radioButton)
		{
		}

		public static void MapContent(RadioButtonHandler handler, IRadioButton radioButton)
		{
		}

		public static void MapTextColor(RadioButtonHandler handler, ITextStyle textStyle)
		{
		}

		public static void MapCharacterSpacing(RadioButtonHandler handler, ITextStyle textStyle)
		{
		}

		public static void MapFont(RadioButtonHandler handler, ITextStyle textStyle)
		{
		}
	}
}