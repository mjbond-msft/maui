using System;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls
{
	/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="Type[@FullName='Microsoft.Maui.Controls.TimePicker']/Docs" />
	public partial class TimePicker : View, IFontElement, ITextElement, IElementConfiguration<TimePicker>
	{
		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='FormatProperty']/Docs" />
		public static readonly BindableProperty FormatProperty = BindableProperty.Create(nameof(Format), typeof(string), typeof(TimePicker), "t");

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='TextColorProperty']/Docs" />
		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='CharacterSpacingProperty']/Docs" />
		public static readonly BindableProperty CharacterSpacingProperty = TextElement.CharacterSpacingProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='TimeProperty']/Docs" />
		public static readonly BindableProperty TimeProperty = BindableProperty.Create(nameof(Time), typeof(TimeSpan), typeof(TimePicker), new TimeSpan(0), BindingMode.TwoWay, (bindable, value) =>
		{
			var time = (TimeSpan)value;
			return time.TotalHours < 24 && time.TotalMilliseconds >= 0;
		});

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='FontFamilyProperty']/Docs" />
		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='FontSizeProperty']/Docs" />
		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='FontAttributesProperty']/Docs" />
		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='FontAutoScalingEnabledProperty']/Docs" />
		public static readonly BindableProperty FontAutoScalingEnabledProperty = FontElement.FontAutoScalingEnabledProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='TextTransformProperty']/Docs" />
		public static readonly BindableProperty TextTransformProperty = TextElement.TextTransformProperty;

		readonly Lazy<PlatformConfigurationRegistry<TimePicker>> _platformConfigurationRegistry;

		public TimePicker()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<TimePicker>>(() => new PlatformConfigurationRegistry<TimePicker>(this));
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='Format']/Docs" />
		public string Format
		{
			get { return (string)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='TextColor']/Docs" />
		public Color TextColor
		{
			get { return (Color)GetValue(TextElement.TextColorProperty); }
			set { SetValue(TextElement.TextColorProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='CharacterSpacing']/Docs" />
		public double CharacterSpacing
		{
			get { return (double)GetValue(TextElement.CharacterSpacingProperty); }
			set { SetValue(TextElement.CharacterSpacingProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='Time']/Docs" />
		public TimeSpan Time
		{
			get { return (TimeSpan)GetValue(TimeProperty); }
			set { SetValue(TimeProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='FontAttributes']/Docs" />
		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontAttributesProperty); }
			set { SetValue(FontAttributesProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='FontFamily']/Docs" />
		public string FontFamily
		{
			get { return (string)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='FontSize']/Docs" />
		[System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='FontAutoScalingEnabled']/Docs" />
		public bool FontAutoScalingEnabled
		{
			get => (bool)GetValue(FontAutoScalingEnabledProperty);
			set => SetValue(FontAutoScalingEnabledProperty, value);
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='TextTransform']/Docs" />
		public TextTransform TextTransform
		{
			get => (TextTransform)GetValue(TextTransformProperty);
			set => SetValue(TextTransformProperty, value);
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='UpdateFormsText']/Docs" />
		public virtual string UpdateFormsText(string source, TextTransform textTransform)
			=> TextTransformUtilites.GetTransformedText(source, textTransform);

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue) =>
			HandleFontChanged();

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue) =>
			HandleFontChanged();

		void IFontElement.OnFontChanged(Font oldValue, Font newValue) =>
			HandleFontChanged();

		double IFontElement.FontSizeDefaultValueCreator() =>
			Device.GetNamedSize(NamedSize.Default, (TimePicker)this);

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue) =>
			HandleFontChanged();

		void IFontElement.OnFontAutoScalingEnabledChanged(bool oldValue, bool newValue) =>
			HandleFontChanged();

		void HandleFontChanged()
		{
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/TimePicker.xml" path="//Member[@MemberName='On']/Docs" />
		public IPlatformElementConfiguration<T, TimePicker> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}

		void ITextElement.OnCharacterSpacingPropertyChanged(double oldValue, double newValue)
		{
			InvalidateMeasure();
		}

		void ITextElement.OnTextTransformChanged(TextTransform oldValue, TextTransform newValue)
			=> InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);
	}
}
