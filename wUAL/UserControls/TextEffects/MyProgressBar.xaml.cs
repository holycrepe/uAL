using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wUAL.UserControls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using PostSharp.Patterns.Model;
    using Puchalapalli.Extensions.Primitives;
    using Torrent.Extensions;
    using Torrent.Extensions.Collection;
    using Torrent.Extensions.UI;
    using Torrent.Helpers.Utils;
    using Torrent.Infrastructure;

    /// <summary>
    /// Interaction logic for MyProgressBar.xaml
    /// </summary>
    [ContentProperty(nameof(Value))]
    [NotifyPropertyChanged]
    public partial class MyProgressBar : UserControl, INotifyPropertyChanged
    {
        public MyProgressBar()
        {
            InitializeComponent();
        }
        #region Dependency Properties
        #region Dependency Property Methods
        protected void SetValueDp(DependencyProperty property, object value, [CallerMemberName] string propertyName = null)
        {
            this.SetValue(property, value);
            OnPropertiesChanged(propertyName, property.Name == propertyName ? string.Empty : property.Name);
        }

        protected static void OnProgressChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            => (dependencyObject as MyProgressBar)?.OnDependencyPropertyChanged(e, nameof(Value), nameof(Text), nameof(Minimum), nameof(Maximum), nameof(Format));
        protected static void BuildEffects(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            => (dependencyObject as MyProgressBar)?.SetInnerEffect().SetOuterEffect();

        protected static void SetInnerEffect(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            => (dependencyObject as MyProgressBar)?.SetInnerEffect();
        protected static void SetOuterEffect(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            => (dependencyObject as MyProgressBar)?.SetOuterEffect();

        protected GlowingTextBlock SetInnerEffect()
            => this.GlowingTextBlock?.SetInnerEffect();

        protected GlowingTextBlock SetOuterEffect()
            => this.GlowingTextBlock?.SetOuterEffect();

        #endregion
        #region Dependency Properties
        #region Dependency Properties: ProgressBar

        #region Dependency Property: `Value`
        /// <summary>
        /// Identifies the `Value` Dependency Property
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
             nameof(Value),
             typeof(double),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, OnProgressChanged));

        /// <summary>
        /// Gets or sets the `Value` Dependency Property
        /// </summary>
        [SafeForDependencyAnalysis]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        #endregion

        #region Dependency Property: `Minimum`
        /// <summary>
        /// Identifies the `Minimum` Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
             nameof(Minimum),
             typeof(double),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, OnProgressChanged));

        /// <summary>
        /// Gets or sets the `Minimum` Dependency Property
        /// </summary>
        [SafeForDependencyAnalysis]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        #endregion

        #region Dependency Property: `Maximum`
        /// <summary>
        /// Identifies the `Maximum` Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
             nameof(Maximum),
             typeof(double),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsRender, OnProgressChanged));

        /// <summary>
        /// Gets or sets the `Maximum` Dependency Property
        /// </summary>
        [SafeForDependencyAnalysis]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        #endregion

        #region Dependency Property: `Format`
        /// <summary>
        /// Identifies the `Format` Dependency Property
        /// </summary>
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
             nameof(Format),
             typeof(string),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnProgressChanged));

        /// <summary>
        /// Gets or sets the `Format` Dependency Property
        /// </summary>
        [SafeForDependencyAnalysis]
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }
        #endregion


        #region Dependency Property: `Suffix`
        /// <summary>
        /// Identifies the `Suffix` Dependency Property
        /// </summary>
        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register(
             nameof(Suffix),
             typeof(string),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(" Complete", FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the `Suffix` Dependency Property
        /// </summary>
        [SafeForDependencyAnalysis]
        public string Suffix
        {
            get { return (string)GetValue(SuffixProperty); }
            set { SetValue(SuffixProperty, value); }
        }
        #endregion
        #endregion

        #region Dependency Property: `TextHeight`
        /// <summary>
        /// Identifies the `TextHeight` Dependency Property
        /// </summary>
        public static readonly DependencyProperty TextHeightProperty = DependencyProperty.Register(
             nameof(TextHeight),
             typeof(double),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsRender, OnProgressChanged));

        /// <summary>
        /// Gets or sets the `TextHeight` Dependency Property
        /// </summary>
        [SafeForDependencyAnalysis]
        public double TextHeight
        {
            get { return (double)GetValue(TextHeightProperty); }
            set { SetValue(TextHeightProperty, value); }
        }
        #endregion
        #region Dependency Properties: Effects
        #region EffectColor Dependency Property
        public static readonly DependencyProperty EffectColorProperty = DependencyProperty.Register(
             nameof(EffectColor),
             typeof(Color?),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, BuildEffects));
        [SafeForDependencyAnalysis]
        public Color EffectColor
        {
            get { return (Color?)GetValue(EffectColorProperty) ?? this.Stroke.GetColor(); }
            set { SetValue(EffectColorProperty, value); }
        }
        #endregion
        #region BlurRadius Dependency Property
        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
             nameof(BlurRadius),
             typeof(double),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender, SetInnerEffect));
        [SafeForDependencyAnalysis]
        public double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }
        public static readonly DependencyProperty OuterBlurRadiusProperty = DependencyProperty.Register(
             nameof(OuterBlurRadius),
             typeof(double),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(-1d, FrameworkPropertyMetadataOptions.AffectsRender, SetOuterEffect));
        [SafeForDependencyAnalysis]
        public double OuterBlurRadius
        {
            get { var value = (double)GetValue(OuterBlurRadiusProperty); return value < 0 ? this.BlurRadius : value; }
            set { SetValue(OuterBlurRadiusProperty, value); }
        }
        #endregion
        #region ShadowDepth Dependency Property
        public static readonly DependencyProperty ShadowDepthProperty = DependencyProperty.Register(
             nameof(ShadowDepth),
             typeof(double),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, SetInnerEffect));
        [SafeForDependencyAnalysis]
        public double ShadowDepth
        {
            get { return (double)GetValue(ShadowDepthProperty); }
            set
            {
                SetValue(ShadowDepthProperty, value);
            }
        }
        public static readonly DependencyProperty OuterShadowDepthProperty = DependencyProperty.Register(
             nameof(OuterShadowDepth),
             typeof(double),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(-1d, FrameworkPropertyMetadataOptions.AffectsRender, SetOuterEffect));
        [SafeForDependencyAnalysis]
        public double OuterShadowDepth
        {
            get { var value = (double)GetValue(OuterShadowDepthProperty); return value < 0 ? this.ShadowDepth : value; }
            set
            {
                SetValue(OuterShadowDepthProperty, value);
            }
        }
        #endregion
        #region EffectOpacity Dependency Property
        public static readonly DependencyProperty EffectOpacityProperty = DependencyProperty.Register(
             nameof(EffectOpacity),
             typeof(double),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender, SetInnerEffect));
        [SafeForDependencyAnalysis]
        public double EffectOpacity
        {
            get { return (double)GetValue(EffectOpacityProperty); }
            set { SetValue(EffectOpacityProperty, value); }
        }
        public static readonly DependencyProperty OuterEffectOpacityProperty = DependencyProperty.Register(
             nameof(OuterEffectOpacity),
             typeof(double),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(-1d, FrameworkPropertyMetadataOptions.AffectsRender, SetOuterEffect));
        [SafeForDependencyAnalysis]
        public double OuterEffectOpacity
        {
            get { var value = (double)GetValue(OuterEffectOpacityProperty); return value < 0 ? this.EffectOpacity : value; }
            set { SetValue(OuterEffectOpacityProperty, value); }
        }
        #endregion
        #region InnerEffect Dependency Property
        public static readonly DependencyProperty InnerEffectProperty = DependencyProperty.Register(
             nameof(InnerEffect),
             typeof(Effect),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        [SafeForDependencyAnalysis]
        public Effect InnerEffect
        {
            get { return (Effect)GetValue(InnerEffectProperty); }
            set { SetValue(InnerEffectProperty, value); }
        }
        #endregion
        #region OuterEffect Dependency Property
        public static readonly DependencyProperty OuterEffectProperty = DependencyProperty.Register(
             nameof(OuterEffect),
             typeof(Effect),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        [SafeForDependencyAnalysis]
        public Effect OuterEffect
        {
            get { return (Effect)GetValue(OuterEffectProperty); }
            set { SetValue(OuterEffectProperty, value); }
        }
        #endregion
        #region OuterEffectDepth Dependency Property

        /// <summary>
        /// Gets or sets the `OuterEffectDepth` Dependency Property
        /// </summary>
        [SafeForDependencyAnalysis]
        public TextBlockOuterEffectDepth EffectDepth
        {
            get { return (TextBlockOuterEffectDepth)GetValue(EffectDepthProperty); }
            set { SetValue(EffectDepthProperty, value); }
        }

        /// <summary>
        /// Identifies the `OuterEffectDepth` Dependency Property
        /// </summary>
        public static readonly DependencyProperty EffectDepthProperty =
            DependencyProperty.Register(nameof(EffectDepth), typeof(TextBlockOuterEffectDepth),
              typeof(MyProgressBar), new FrameworkPropertyMetadata(TextBlockOuterEffectDepth.OuterBorder, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion
        #endregion
        #region Dependency Property Definitions        
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
             nameof(Fill),
             typeof(Brush),
             typeof(MyProgressBar),
             new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            nameof(Stroke),
            typeof(Brush),
            typeof(MyProgressBar),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            nameof(StrokeThickness),
            typeof(double),
            typeof(MyProgressBar),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            nameof(TextAlignment),
            typeof(TextAlignment),
            typeof(MyProgressBar),
            new FrameworkPropertyMetadata(TextAlignment.Center, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register(
            nameof(TextDecorations),
            typeof(TextDecorationCollection),
            typeof(MyProgressBar),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
            nameof(TextTrimming),
            typeof(TextTrimming),
            typeof(MyProgressBar),
            new FrameworkPropertyMetadata(TextTrimming.CharacterEllipsis, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
            nameof(TextWrapping),
            typeof(TextWrapping),
            typeof(MyProgressBar),
            new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Dependency Property Accessors
        [SafeForDependencyAnalysis]
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        [SafeForDependencyAnalysis]
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        [SafeForDependencyAnalysis]
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        [SafeForDependencyAnalysis]        
        public string Text
            => PercentConverter.Convert(this.Value, this.Minimum, this.Maximum, this.Format, this.Suffix);

        [SafeForDependencyAnalysis]
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
        [SafeForDependencyAnalysis]
        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)this.GetValue(TextDecorationsProperty); }
            set { this.SetValue(TextDecorationsProperty, value); }
        }
        [SafeForDependencyAnalysis]
        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }
        [SafeForDependencyAnalysis]
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        public static readonly DependencyProperty MergeShapesProperty
            = DependencyProperty.Register(nameof(MergeShapes),
                typeof(bool),
                typeof(MyProgressBar),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        [SafeForDependencyAnalysis]
        public bool MergeShapes
        {
            get { return (bool)GetValue(MergeShapesProperty); }
            set { SetValue(MergeShapesProperty, value); }
        }
        #endregion
        #endregion
        #endregion
        #region Interfaces
        #region Interfaces: IPropertyChanged

        private void OnPropertyChangedLocal(string propertyName)
            => LogUtils.Log(nameof(MyProgressBar), $"Δ {propertyName}");
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs e, params string[] propertyNames)
            => OnPropertiesChanged(propertyNames.Before(e.Property.Name));
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => OnPropertiesChanged(propertyName.TrimStart("Set").TrimStart("Get"));
        public virtual void OnPropertiesChanged(params string[] propertyNames)
            => NotifyPropertyChangedBase.DoOnPropertyChanged(this, PropertyChanged, OnPropertyChangedLocal, propertyNames);
        #endregion

        #endregion

        private void LayoutRoot_MouseUp(object sender, MouseButtonEventArgs e)
            => Debugger.Break();
        
    }
}
