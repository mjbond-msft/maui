using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls
{
	public partial class Button : View, IFontElement, ITextElement, IBorderElement, IButtonController, IElementConfiguration<Button>, IPaddingElement, IImageController, IViewController, IButtonElement, IImageElement
	{
		const double DefaultSpacing = 10;

		public static readonly BindableProperty CommandProperty = ButtonElement.CommandProperty;

		public static readonly BindableProperty CommandParameterProperty = ButtonElement.CommandParameterProperty;

		public static readonly BindableProperty ContentLayoutProperty = BindableProperty.Create(
			nameof(ContentLayout), typeof(ButtonContentLayout), typeof(Button), new ButtonContentLayout(ButtonContentLayout.ImagePosition.Left, DefaultSpacing),
			propertyChanged: (bindable, oldVal, newVal) => ((Button)bindable).InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged));

		public static readonly BindableProperty TextProperty = BindableProperty.Create(
			nameof(Text), typeof(string), typeof(Button), null,
			propertyChanged: (bindable, oldVal, newVal) => ((Button)bindable).InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged));

		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;

		public static readonly BindableProperty CharacterSpacingProperty = TextElement.CharacterSpacingProperty;

		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		public static readonly BindableProperty TextTransformProperty = TextElement.TextTransformProperty;

		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		public static readonly BindableProperty FontAutoScalingEnabledProperty = FontElement.FontAutoScalingEnabledProperty;

		public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(Button), -1d);

		public static readonly BindableProperty BorderColorProperty = BorderElement.BorderColorProperty;

		public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(Button), defaultValue: BorderElement.DefaultCornerRadius);

		public static readonly BindableProperty ImageSourceProperty = ImageElement.ImageSourceProperty;

		public static readonly BindableProperty PaddingProperty = PaddingElement.PaddingProperty;

		public static readonly BindableProperty LineBreakModeProperty = BindableProperty.Create(
			nameof(LineBreakMode), typeof(LineBreakMode), typeof(Button), LineBreakMode.NoWrap,
			propertyChanged: (bindable, oldvalue, newvalue) => ((Button)bindable).InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged));

		public Thickness Padding
		{
			get { return (Thickness)GetValue(PaddingElement.PaddingProperty); }
			set { SetValue(PaddingElement.PaddingProperty, value); }
		}

		Thickness IPaddingElement.PaddingDefaultValueCreator() => new Thickness(double.NaN);

		public LineBreakMode LineBreakMode
		{
			get { return (LineBreakMode)GetValue(LineBreakModeProperty); }
			set { SetValue(LineBreakModeProperty, value); }
		}

		void IPaddingElement.OnPaddingPropertyChanged(Thickness oldValue, Thickness newValue)
		{
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);
		}


		internal static readonly BindablePropertyKey IsPressedPropertyKey = BindableProperty.CreateReadOnly(nameof(IsPressed), typeof(bool), typeof(Button), default(bool));
		public static readonly BindableProperty IsPressedProperty = IsPressedPropertyKey.BindableProperty;


		readonly Lazy<PlatformConfigurationRegistry<Button>> _platformConfigurationRegistry;

		public Color BorderColor
		{
			get { return (Color)GetValue(BorderElement.BorderColorProperty); }
			set { SetValue(BorderElement.BorderColorProperty, value); }
		}

		public int CornerRadius
		{
			get { return (int)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}

		public double BorderWidth
		{
			get { return (double)GetValue(BorderWidthProperty); }
			set { SetValue(BorderWidthProperty, value); }
		}

		public ButtonContentLayout ContentLayout
		{
			get { return (ButtonContentLayout)GetValue(ContentLayoutProperty); }
			set { SetValue(ContentLayoutProperty, value); }
		}

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public object CommandParameter
		{
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public ImageSource ImageSource
		{
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextElement.TextColorProperty); }
			set { SetValue(TextElement.TextColorProperty, value); }
		}

		public double CharacterSpacing
		{
			get { return (double)GetValue(TextElement.CharacterSpacingProperty); }
			set { SetValue(TextElement.CharacterSpacingProperty, value); }
		}

		bool IButtonElement.IsEnabledCore
		{
			set { SetValueCore(IsEnabledProperty, value); }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendClicked() => ButtonElement.ElementClicked(this, this);

		public bool IsPressed => (bool)GetValue(IsPressedProperty);

		[EditorBrowsable(EditorBrowsableState.Never)]
		void IButtonElement.SetIsPressed(bool isPressed) => SetValue(IsPressedPropertyKey, isPressed);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendPressed() => ButtonElement.ElementPressed(this, this);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendReleased() => ButtonElement.ElementReleased(this, this);

		[EditorBrowsable(EditorBrowsableState.Never)]
		void IButtonElement.PropagateUpClicked() => Clicked?.Invoke(this, EventArgs.Empty);

		[EditorBrowsable(EditorBrowsableState.Never)]
		void IButtonElement.PropagateUpPressed() => Pressed?.Invoke(this, EventArgs.Empty);

		[EditorBrowsable(EditorBrowsableState.Never)]
		void IButtonElement.PropagateUpReleased() => Released?.Invoke(this, EventArgs.Empty);

		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontAttributesProperty); }
			set { SetValue(FontAttributesProperty, value); }
		}

		public string FontFamily
		{
			get { return (string)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}

		[System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		public bool FontAutoScalingEnabled
		{
			get => (bool)GetValue(FontAutoScalingEnabledProperty);
			set => SetValue(FontAutoScalingEnabledProperty, value);
		}

		public TextTransform TextTransform
		{
			get => (TextTransform)GetValue(TextTransformProperty);
			set => SetValue(TextTransformProperty, value);
		}

		public event EventHandler Clicked;
		public event EventHandler Pressed;

		public event EventHandler Released;

		public Button()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Button>>(() => new PlatformConfigurationRegistry<Button>(this));
		}

		public IPlatformElementConfiguration<T, Button> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		protected internal override void ChangeVisualState()
		{
			if (IsEnabled && IsPressed)
			{
				VisualStateManager.GoToState(this, ButtonElement.PressedVisualState);
			}
			else
			{
				base.ChangeVisualState();
			}
		}

		protected override void OnBindingContextChanged()
		{
			ImageSource image = ImageSource;
			if (image != null)
				SetInheritedBindingContext(image, BindingContext);

			base.OnBindingContextChanged();
		}

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue) =>
			HandleFontChanged();

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue) =>
			HandleFontChanged();

		double IFontElement.FontSizeDefaultValueCreator() =>
			this.GetDefaultFontSize();

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue) =>
			HandleFontChanged();

		void IFontElement.OnFontChanged(Font oldValue, Font newValue) =>
			HandleFontChanged();

		void IFontElement.OnFontAutoScalingEnabledChanged(bool oldValue, bool newValue) =>
			HandleFontChanged();

		void HandleFontChanged()
		{
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);
		}

		Aspect IImageElement.Aspect => Aspect.AspectFit;
		ImageSource IImageElement.Source => ImageSource;
		bool IImageElement.IsOpaque => false;


		void IImageElement.RaiseImageSourcePropertyChanged() => OnPropertyChanged(ImageSourceProperty.PropertyName);

		int IBorderElement.CornerRadiusDefaultValue => (int)CornerRadiusProperty.DefaultValue;

		Color IBorderElement.BorderColorDefaultValue => (Color)BorderColorProperty.DefaultValue;

		double IBorderElement.BorderWidthDefaultValue => (double)BorderWidthProperty.DefaultValue;

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}

		void ITextElement.OnCharacterSpacingPropertyChanged(double oldValue, double newValue)
		{
			InvalidateMeasure();
		}


		void IBorderElement.OnBorderColorPropertyChanged(Color oldValue, Color newValue)
		{
		}

		bool IImageController.GetLoadAsAnimation() => false;
		bool IImageElement.IsLoading => false;

		bool IImageElement.IsAnimationPlaying => false;

		void IImageElement.OnImageSourceSourceChanged(object sender, EventArgs e) =>
			ImageElement.ImageSourceSourceChanged(this, e);

		void IButtonElement.OnCommandCanExecuteChanged(object sender, EventArgs e) =>
			ButtonElement.CommandCanExecuteChanged(this, EventArgs.Empty);

		void IImageController.SetIsLoading(bool isLoading)
		{
		}

		bool IBorderElement.IsCornerRadiusSet() => IsSet(CornerRadiusProperty);
		bool IBorderElement.IsBackgroundColorSet() => IsSet(BackgroundColorProperty);
		bool IBorderElement.IsBackgroundSet() => IsSet(BackgroundProperty);
		bool IBorderElement.IsBorderColorSet() => IsSet(BorderColorProperty);
		bool IBorderElement.IsBorderWidthSet() => IsSet(BorderWidthProperty);

		void ITextElement.OnTextTransformChanged(TextTransform oldValue, TextTransform newValue)
			=> InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		public virtual string UpdateFormsText(string source, TextTransform textTransform)
			=> TextTransformUtilites.GetTransformedText(source, textTransform);

		[DebuggerDisplay("Image Position = {Position}, Spacing = {Spacing}")]
		[System.ComponentModel.TypeConverter(typeof(ButtonContentTypeConverter))]
		public sealed class ButtonContentLayout
		{
			public enum ImagePosition
			{
				Left,
				Top,
				Right,
				Bottom
			}

			public ButtonContentLayout(ImagePosition position, double spacing)
			{
				Position = position;
				Spacing = spacing;
			}

			public ImagePosition Position { get; }

			public double Spacing { get; }

			public override string ToString() => $"Image Position = {Position}, Spacing = {Spacing}";
		}

		public sealed class ButtonContentTypeConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
				=> sourceType == typeof(string);

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
				=> false;

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				var strValue = value?.ToString();
				if (strValue == null)
					throw new InvalidOperationException($"Cannot convert null into {typeof(ButtonContentLayout)}");

				string[] parts = strValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

				if (parts.Length != 1 && parts.Length != 2)
					throw new InvalidOperationException($"Cannot convert \"{strValue}\" into {typeof(ButtonContentLayout)}");

				double spacing = DefaultSpacing;
				var position = ButtonContentLayout.ImagePosition.Left;

				var spacingFirst = char.IsDigit(parts[0][0]);

				int positionIndex = spacingFirst ? (parts.Length == 2 ? 1 : -1) : 0;
				int spacingIndex = spacingFirst ? 0 : (parts.Length == 2 ? 1 : -1);

				if (spacingIndex > -1)
					spacing = double.Parse(parts[spacingIndex]);

				if (positionIndex > -1)
					position = (ButtonContentLayout.ImagePosition)Enum.Parse(typeof(ButtonContentLayout.ImagePosition), parts[positionIndex], true);

				return new ButtonContentLayout(position, spacing);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
				=> throw new NotSupportedException();
		}
	}
}
