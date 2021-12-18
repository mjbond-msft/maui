﻿using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;
using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui.Controls.Platform
{
	public static class LabelExtensions
	{
		public static void UpdateText(this UILabel nativeLabel, Label label)
		{
			switch (label.TextType)
			{
				case TextType.Html:
					nativeLabel.UpdateTextHtml(label);
					break;

				default:
					if (label.FormattedText != null)
						nativeLabel.AttributedText = label.FormattedText.ToAttributed(label);
					else
						nativeLabel.Text = TextTransformUtilites.GetTransformedText(label.Text, label.TextTransform);
					break;
			}
		}
	}
}
