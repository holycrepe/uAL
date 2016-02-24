using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Torrent.Converters;
using Torrent.Helpers.Utils;
using wUAL.WPF.Selectors.Models.Base;

namespace wUAL.WPF.Selectors.Models.ProgressBar
{
    using static ProgressBar;
    using static ProgressBar.States;
    public class ProgressBarStyle : ProgressBarModels.SelectorStyle { }
    public class ProgressBarTemplate : ProgressBarModels.SelectorTemplate { }
    public class ProgressBarModel : ISelectorModel<States, Template>
    {
        private LinearGradientBrush _foreground;
        public States Key { get; set; }
            = Default;
        public Template Value { get; set; }
        public Template Template
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
        public Color Stroke { get; set; }
        public object Label { get; set; } = null;
        [TypeConverter(typeof(ColorCollectionTypeConverter))]
        public Collection<Color> Colors { get; set; }

        public LinearGradientBrush Foreground
        {
            get
            {
                return this._foreground ?? (this.Colors == null ? null : (this._foreground
                = new LinearGradientBrush(new GradientStopCollection
                {
                    new GradientStop(this.Colors[0], 0),
                    new GradientStop(this.Colors[1], 0.8),
                    new GradientStop(this.Colors[2], 1.0)
                }, 
                new Point(0, 0.5), new Point(1, 0.5))));
            }
            set { this._foreground = value; }
        }

        private SolidColorBrush StrokeBrush
            => new SolidColorBrush(this.Stroke);

        public RadialGradientBrush StrokeGradient
            => ColorUtils.CreateRadialGradient(this.StrokeBrush);

        public LinearGradientBrush Fill
            => this.Foreground;
    }
}