namespace wUAL.WPF.Models.ProgressBar
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using Torrent;
    using Torrent.Converters;
    using Torrent.Extensions;
    using Torrent.Helpers.Utils;
    using WPF.Models.Base;

    public class ProgressBarStyle //: SelectorModelsBase<UTorrentJob, ProgressBarModel, ProgressBar.States, ProgressBar.Element, ProgressBar.Template>.SelectorStyle { }
        : ProgressBarModels.SelectorStyle { }
    public class ProgressBarTemplate : ProgressBarModels.SelectorTemplate { }
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class ProgressBarModel : ISelectorModel<ProgressBar.States, ProgressBar.Template>
    {
        private LinearGradientBrush _foreground;
        public ProgressBar.States Key { get; set; }
            = ProgressBar.States.Default;
        public ProgressBar.Template Value { get; set; }
        public ProgressBar.Template Template
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

        public SolidColorBrush StrokeBrush
            => new SolidColorBrush(this.Stroke);

        public RadialGradientBrush StrokeGradient
            => ColorUtils.CreateRadialGradient(this.StrokeBrush);

        public LinearGradientBrush Fill
            => this.Foreground;
        
        #region Interfaces
        #region Interfaces: IDebuggerDisplay
        [DebuggerNonUserCode]
        public override string ToString()
            => DebuggerDisplaySimple();
        public string DebuggerDisplay(int level = 1)
        => $"<{this.GetType().GetFriendlyName()}> {DebuggerDisplaySimple(level)}";
        public string DebuggerDisplaySimple(int level = 1)
        => $"{this.Key} [{this.Template}]";
        #endregion
        #endregion
    }
}