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
    using Torrent.Extensions;
    using Torrent.Extensions.UI;
    using Torrent.Helpers.Utils;
    using Torrent.Infrastructure;
    using static Torrent.Infrastructure.NotifyPropertyChangedBase;
    /// <summary>
    /// Interaction logic for GlowingTextBlock.xaml
    /// </summary>
    [ContentProperty(nameof(Text))]
    [NotifyPropertyChanged]
    public partial class GlowingTextBlock : UserControl, INotifyPropertyChanged
    {
        #region Public Properties
        #region Public Properties: Accessors

        [SafeForDependencyAnalysis]
        public Effect ElementEffect
        {
            get
            {
                if (Depends.Guard)
                    Depends.On(this.InnerEffect, this.EffectDepth, this.BlurRadius, this.ShadowDepth,
                        this.EffectOpacity, this.EffectColor, this.Stroke);
                return GetEffect(TextBlockOuterEffectDepth.Element);
            }
        }

        [SafeForDependencyAnalysis]
        public Effect InnerBorderEffect
        {
            get
            {
                if (Depends.Guard)
                    Depends.On(this.InnerEffect, this.EffectDepth, this.BlurRadius, this.ShadowDepth,
                        this.EffectOpacity, this.EffectColor, this.Stroke);
                return GetEffect(TextBlockOuterEffectDepth.InnerBorder);
            }
        }

        [SafeForDependencyAnalysis]
        public Effect OuterBorderEffect
        {
            get
            {
                if (Depends.Guard)
                {
                    Depends.On(this.InnerEffect, this.EffectColor, this.Stroke,
                               this.EffectDepth, this.BlurRadius, this.ShadowDepth, this.EffectOpacity);
                    Depends.On(this.OuterEffect, this.OuterBlurRadius, this.OuterShadowDepth, this.OuterEffectOpacity);
                }
                return GetEffect(TextBlockOuterEffectDepth.OuterBorder);
            }
        }
        #endregion
        #region Public Properties: Methods
        protected Effect GetEffect(TextBlockOuterEffectDepth depth)
        {
            var outerDepth = Convert.ToInt32(this.EffectDepth);
            if (depth == TextBlockOuterEffectDepth.Element)
                return this.EffectDepth == TextBlockOuterEffectDepth.Element ? this.OuterEffect : this.InnerEffect;
            return this.EffectDepth >= depth ? OuterEffect : null;
        }
        protected static DropShadowEffect BuildDropShadowEffect(Color color, double blur, double shadow=0, double opacity=1)
            => new DropShadowEffect
            {
                Color = color,
                BlurRadius = blur,
                Opacity = opacity,
                ShadowDepth = shadow
            };

        #endregion
        #endregion
        public GlowingTextBlock()
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
        protected static void BuildEffects(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            => (dependencyObject as GlowingTextBlock)?.SetInnerEffect().SetOuterEffect();

        protected static void SetInnerEffect(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            => (dependencyObject as GlowingTextBlock)?.SetInnerEffect();
        protected static void SetOuterEffect(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            => (dependencyObject as GlowingTextBlock)?.SetOuterEffect();

        public GlowingTextBlock SetInnerEffect()
        {
            InnerEffect = BuildDropShadowEffect(EffectColor, BlurRadius, ShadowDepth, EffectOpacity);            
            OnPropertyChanged();
            return this;
        }
        public GlowingTextBlock SetOuterEffect()
        {
            OuterEffect = BuildDropShadowEffect(EffectColor, OuterBlurRadius, OuterShadowDepth, OuterEffectOpacity);
            OnPropertyChanged();
            return this;
        }
        #endregion
        #region Dependency Properties
        #region Dependency Properties: Effects
        #region EffectColor Dependency Property
        public static readonly DependencyProperty EffectColorProperty = DependencyProperty.Register(
             nameof(EffectColor),
             typeof(Color?),
             typeof(GlowingTextBlock),
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
             typeof(GlowingTextBlock),
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
             typeof(GlowingTextBlock),
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
             typeof(GlowingTextBlock),
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
             typeof(GlowingTextBlock),
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
             typeof(GlowingTextBlock),
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
             typeof(GlowingTextBlock),
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
             typeof(GlowingTextBlock),
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
             typeof(GlowingTextBlock),
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
              typeof(GlowingTextBlock), new FrameworkPropertyMetadata(TextBlockOuterEffectDepth.OuterBorder, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion
        #endregion
        #region Dependency Property Definitions        
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
             nameof(Fill),
             typeof(Brush),
             typeof(GlowingTextBlock),
             new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            nameof(Stroke),
            typeof(Brush),
            typeof(GlowingTextBlock),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            nameof(StrokeThickness),
            typeof(double),
            typeof(GlowingTextBlock),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(GlowingTextBlock),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            nameof(TextAlignment),
            typeof(TextAlignment),
            typeof(GlowingTextBlock),
            new FrameworkPropertyMetadata(TextAlignment.Center, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register(
            nameof(TextDecorations),
            typeof(TextDecorationCollection),
            typeof(GlowingTextBlock),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
            nameof(TextTrimming),
            typeof(TextTrimming),
            typeof(GlowingTextBlock),
            new FrameworkPropertyMetadata(TextTrimming.CharacterEllipsis, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
            nameof(TextWrapping),
            typeof(TextWrapping),
            typeof(GlowingTextBlock),
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
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
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
                typeof(GlowingTextBlock),
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
            => LogUtils.Log(nameof(GlowingTextBlock), $"Δ {propertyName}");
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs e)
            => OnPropertiesChanged(e.Property.Name);
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => OnPropertiesChanged(propertyName.TrimStart("Set").TrimStart("Get"));
        public virtual void OnPropertiesChanged(params string[] propertyNames)
            => DoOnPropertyChanged(this, PropertyChanged, OnPropertyChangedLocal, propertyNames);
        #endregion

        #endregion

        private void LayoutRoot_MouseUp(object sender, MouseButtonEventArgs e)
            => Debugger.Break();
        
    }
}
