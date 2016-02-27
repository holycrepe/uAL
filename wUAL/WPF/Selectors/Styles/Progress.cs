using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Torrent.Queue;
using uAL.Extensions;
using wUAL.UserControls;
using wUAL.WPF.Selectors;

namespace wUAL.WPF.Styles
{
    using Torrent;
    using Torrent.Extensions;
    using Torrent.Helpers.Utils;
    using static Selectors.Selectors;
    using static Selectors.Selectors.Progress;
    public class ProgressStyleSelector : StyleSelectorBase<UTorrentJob>
    {

        #region Overrides of StyleSelectorBase<UTorrentJob>

        protected override void MakeStyles(string name)
            => Styles.Add(name, GenerateForegroundStyle(name));        

        protected override string[] Names { get; }
            = Progress.Names;
        protected override SelectorBase<UTorrentJob> Selectors { get; }
            = Selector;
        protected override string KeyBase { get; }
            = Key;
        #endregion

    }

    public class ProgressLabelStyleSelector : ProgressStyleSelector
    {
        protected override void MakeStyles(string name)
        {
            var brush = GetResource<SolidColorBrush>(Control.ForegroundProperty, name);
            var stroke = ColorUtils.CreateRadialGradient(brush);
            var fill = Progress.Style.GetResource<LinearGradientBrush>(Control.ForegroundProperty, name);
            var style = GenerateStyle()
                .AddSetter(OutlinedTextBlock.FillProperty, fill)
                .AddSetter(OutlinedTextBlock.StrokeProperty, stroke);
            this.Styles[name] = style;
            Content[name] = name + "...";
        }
        protected override string KeySuffix { get; }
            = "Label";
    }
}
