using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Torrent.Queue;
using wUAL.WPF.Selectors;

namespace wUAL
{
    using Torrent.Extensions;
    using WPF.Styles;
    using static Selectors;
    public abstract class QueueStatusStyleSelector : StyleSelectorBase<QueueStatusMember>
    {
        public QueueStatusStyleSelector()
        {
            Styles.Concat (new Dictionary<string, Style> {
                { "Ready", GenerateStyle("#004080")},
                { "Queued", GenerateStyle("#2D5340")},
                { "Active", GenerateStyle("#00264d")},
                { "Success", GenerateStyle("#004D26")},
                { "Error", GenerateStyle("#990000")},
                { "TorrentError", GenerateStyle("#FF0000")},
                { "Invalid", GenerateStyle("#CCCCCC")},
                { "Disabled", GenerateStyle("#999966")},
                { "Dupe", GenerateStyle("#CC0000")},
                { "QueueDupe", GenerateStyle("#B32400")},
                { "TorrentDupe", GenerateStyle("#B32400")},
                { "InProgress", GenerateStyle("#008040")},
                { "NoLabel", GenerateStyle("#E60000")}
            });
        }
        protected override SelectorBase<QueueStatusMember> Selectors { get; }
            = QueueStatusSelector;
        protected override string KeyBase { get; }
            = "GridViewRowStyle";
    }
}
