using System;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls
{
	[ContentProperty("Text")]
	public class Span : GestureElement, IFontElement, IStyleElement, ITextElement, ILineHeightElement, IDecorableTextElement
	{
		internal readonly MergedStyle _mergedStyle;

		public Span()
		{
			_mergedStyle = new MergedStyle(GetType(), this);
		}

		public static readonly BindableProperty StyleProperty = BindableProperty.Create(nameof(Style), typeof(Style), typeof(Span), default(Style),
			propertyChanged: (bindable, oldvalue, newvalue) => ((Span)bindable)._mergedStyle.Style = (Style)newvalue, defaultBindingMode: BindingMode.OneWay);

		public static readonly BindableProperty TextDecorationsProperty = DecorableTextElement.TextDecorationsProperty;

		public static readonly BindableProperty TextTransformProperty = TextElement.TextTransformProperty;

		public Style Style
		{
			get { return (Style)GetValue(StyleProperty); }
			set { SetValue(StyleProperty, value); }
		}

		public static readonly BindableProperty BackgroundColorProperty
			= BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(Span), default(Color), defaultBindingMode: BindingMode.OneWay);

		public Color BackgroundColor
		{
			get { return (Color)GetValue(BackgroundColorProperty); }
			set { SetValue(BackgroundColorProperty, value); }
		}

		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;

		public Color TextColor
		{
			get { return (Color)GetValue(TextElement.TextColorProperty); }
			set { SetValue(TextElement.TextColorProperty, value); }
		}

		public static readonly BindableProperty CharacterSpacingProperty = TextElement.CharacterSpacingProperty;

		public double CharacterSpacing
		{
			get { return (double)GetValue(TextElement.CharacterSpacingProperty); }
			set { SetValue(TextElement.CharacterSpacingProperty, value); }
		}

		public TextTransform TextTransform
		{
			get => (TextTransform)GetValue(TextTransformProperty);
			set => SetValue(TextTransformProperty, value);
		}

		public virtual string UpdateFormsText(string source, TextTransform textTransform)
			=> TextTransformUtilites.GetTransformedText(source, textTransform);

		public static readonly BindableProperty TextProperty
			= BindableProperty.Create(nameof(Text), typeof(string), typeof(Span), "", defaultBindingMode: BindingMode.OneWay);

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		public static readonly BindableProperty FontAutoScalingEnabledProperty = FontElement.FontAutoScalingEnabledProperty;

		public static readonly BindableProperty LineHeightProperty = LineHeightElement.LineHeightProperty;

		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontElement.FontAttributesProperty); }
			set { SetValue(FontElement.FontAttributesProperty, value); }
		}

		public string FontFamily
		{
			get { return (string)GetValue(FontElement.FontFamilyProperty); }
			set { SetValue(FontElement.FontFamilyProperty, value); }
		}

		[System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontElement.FontSizeProperty); }
			set { SetValue(FontElement.FontSizeProperty, value); }
		}

		public bool FontAutoScalingEnabled
		{
			get => (bool)GetValue(FontAutoScalingEnabledProperty);
			set => SetValue(FontAutoScalingEnabledProperty, value);
		}

		public TextDecorations TextDecorations
		{
			get { return (TextDecorations)GetValue(TextDecorationsProperty); }
			set { SetValue(TextDecorationsProperty, value); }
		}

		public double LineHeight
		{
			get { return (double)GetValue(LineHeightElement.LineHeightProperty); }
			set { SetValue(LineHeightElement.LineHeightProperty, value); }
		}

		protected override void OnBindingContextChanged()
		{
			this.PropagateBindingContext(GestureRecognizers);
			base.OnBindingContextChanged();
		}

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue)
		{
		}

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue)
		{
		}

		double IFontElement.FontSizeDefaultValueCreator() =>
			this.GetDefaultFontSize();

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue)
		{
		}

		void IFontElement.OnFontChanged(Font oldValue, Font newValue)
		{
		}

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}

		void ITextElement.OnCharacterSpacingPropertyChanged(double oldValue, double newValue)
		{
		}

		void ITextElement.OnTextTransformChanged(TextTransform oldValue, TextTransform newValue)
		{
		}

		void IFontElement.OnFontAutoScalingEnabledChanged(bool oldValue, bool newValue) { }

		internal override void ValidateGesture(IGestureRecognizer gesture)
		{
			switch (gesture)
			{
				case ClickGestureRecognizer click:
				case TapGestureRecognizer tap:
				case null:
					break;
				default:
					throw new InvalidOperationException($"{gesture.GetType().Name} is not supported on a {nameof(Span)}");

			}
		}

		void ILineHeightElement.OnLineHeightChanged(double oldValue, double newValue)
		{
		}
	}
}
