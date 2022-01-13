﻿namespace Microsoft.Maui.DeviceTests.Stubs
{
	public partial class RadioButtonStub : StubBase, IRadioButton
	{
		public bool IsChecked { get; set; }

		public TextType TextType { get; set; } = TextType.Text;

		public Color TextColor { get; set; }

		public double CharacterSpacing { get; set; }

		public Font Font { get; set; }

		public object Content { get; set; }

		public IView PresentedContent { get; set; }

		public Thickness Padding { get; set; }

		public Size CrossPlatformArrange(Rectangle bounds)
		{
			return bounds.Size;
		}

		public Size CrossPlatformMeasure(double widthConstraint, double heightConstraint)
		{
			return new Size(widthConstraint, heightConstraint);
		}
	}
}