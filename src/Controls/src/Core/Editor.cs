using System;
using System.ComponentModel;
using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Controls
{
	/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="Type[@FullName='Microsoft.Maui.Controls.Editor']/Docs" />
	public partial class Editor : InputView, IEditorController, IFontElement, ITextAlignmentElement, IElementConfiguration<Editor>
	{
		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='TextProperty']/Docs" />
		public new static readonly BindableProperty TextProperty = InputView.TextProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='FontFamilyProperty']/Docs" />
		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='FontSizeProperty']/Docs" />
		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='FontAttributesProperty']/Docs" />
		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='FontAutoScalingEnabledProperty']/Docs" />
		public static readonly BindableProperty FontAutoScalingEnabledProperty = FontElement.FontAutoScalingEnabledProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='TextColorProperty']/Docs" />
		public new static readonly BindableProperty TextColorProperty = InputView.TextColorProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='CharacterSpacingProperty']/Docs" />
		public new static readonly BindableProperty CharacterSpacingProperty = InputView.CharacterSpacingProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='PlaceholderProperty']/Docs" />
		public new static readonly BindableProperty PlaceholderProperty = InputView.PlaceholderProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='PlaceholderColorProperty']/Docs" />
		public new static readonly BindableProperty PlaceholderColorProperty = InputView.PlaceholderColorProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='IsTextPredictionEnabledProperty']/Docs" />
		public static readonly BindableProperty IsTextPredictionEnabledProperty = BindableProperty.Create(nameof(IsTextPredictionEnabled), typeof(bool), typeof(Editor), true, BindingMode.Default);

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='AutoSizeProperty']/Docs" />
		public static readonly BindableProperty AutoSizeProperty = BindableProperty.Create(nameof(AutoSize), typeof(EditorAutoSizeOption), typeof(Editor), defaultValue: EditorAutoSizeOption.Disabled, propertyChanged: (bindable, oldValue, newValue)
			=> ((Editor)bindable)?.UpdateAutoSizeOption());

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='HorizontalTextAlignmentProperty']/Docs" />
		public static readonly BindableProperty HorizontalTextAlignmentProperty = TextAlignmentElement.HorizontalTextAlignmentProperty;

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='VerticalTextAlignmentProperty']/Docs" />
		public static readonly BindableProperty VerticalTextAlignmentProperty = TextAlignmentElement.VerticalTextAlignmentProperty;

		readonly Lazy<PlatformConfigurationRegistry<Editor>> _platformConfigurationRegistry;

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='AutoSize']/Docs" />
		public EditorAutoSizeOption AutoSize
		{
			get { return (EditorAutoSizeOption)GetValue(AutoSizeProperty); }
			set { SetValue(AutoSizeProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='FontAttributes']/Docs" />
		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontAttributesProperty); }
			set { SetValue(FontAttributesProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='IsTextPredictionEnabled']/Docs" />
		public bool IsTextPredictionEnabled
		{
			get { return (bool)GetValue(IsTextPredictionEnabledProperty); }
			set { SetValue(IsTextPredictionEnabledProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='FontFamily']/Docs" />
		public string FontFamily
		{
			get { return (string)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='FontSize']/Docs" />
		[System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='HorizontalTextAlignment']/Docs" />
		public TextAlignment HorizontalTextAlignment
		{
			get { return (TextAlignment)GetValue(HorizontalTextAlignmentProperty); }
			set { SetValue(HorizontalTextAlignmentProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='VerticalTextAlignment']/Docs" />
		public TextAlignment VerticalTextAlignment
		{
			get { return (TextAlignment)GetValue(VerticalTextAlignmentProperty); }
			set { SetValue(VerticalTextAlignmentProperty, value); }
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='FontAutoScalingEnabled']/Docs" />
		public bool FontAutoScalingEnabled
		{
			get => (bool)GetValue(FontAutoScalingEnabledProperty);
			set => SetValue(FontAutoScalingEnabledProperty, value);
		}

		protected void UpdateAutoSizeOption()
		{
			if (AutoSize == EditorAutoSizeOption.TextChanges && this.IsShimmed())
			{
				InvalidateMeasure();
			}
		}

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue) =>
			UpdateAutoSizeOption();

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue) =>
			UpdateAutoSizeOption();

		void IFontElement.OnFontChanged(Font oldValue, Font newValue) =>
			UpdateAutoSizeOption();

		double IFontElement.FontSizeDefaultValueCreator() =>
			Device.GetNamedSize(NamedSize.Default, (Editor)this);

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue) =>
			UpdateAutoSizeOption();

		void IFontElement.OnFontAutoScalingEnabledChanged(bool oldValue, bool newValue) =>
			UpdateAutoSizeOption();

		public event EventHandler Completed;

		public Editor()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Editor>>(() => new PlatformConfigurationRegistry<Editor>(this));
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='On']/Docs" />
		public IPlatformElementConfiguration<T, Editor> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='SendCompleted']/Docs" />
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendCompleted()
			=> Completed?.Invoke(this, EventArgs.Empty);

		protected override void OnTextChanged(string oldValue, string newValue)
		{
			base.OnTextChanged(oldValue, newValue);

			if (AutoSize == EditorAutoSizeOption.TextChanges)
			{
				InvalidateMeasure();
			}
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/Editor.xml" path="//Member[@MemberName='OnHorizontalTextAlignmentPropertyChanged']/Docs" />
		public void OnHorizontalTextAlignmentPropertyChanged(TextAlignment oldValue, TextAlignment newValue)
		{
		}
	}
}
