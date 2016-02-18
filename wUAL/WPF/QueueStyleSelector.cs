using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Torrent.Queue;
using uAL.Metalinks;
using uAL.Torrents;

namespace wUAL
{
    public abstract class QueueStyleSelector : StyleSelector
    {
        Style GetStyle(string name, bool flag = true)
        {
            if (flag && styles.ContainsKey(name)) {
                return styles[name];
            }
            return null;
        }

        protected Style GetStyles(params object[] args)
        {
            for (var i = 0; i < args.Length; i += 2) {
                var style = GetStyle((string) args[i], (bool) args[i + 1]);
                if (style != null) {
                    return style;
                }
            }
            return null;
        }

        protected Style GetStyle(object queueStatus)
        {
            var status = (QueueStatusMember) queueStatus;
            return GetStyles(
                             status.Name, true,
                             status.Title, true,
                             "TorrentError", status.IsTorrentError,
                             "LoadError", status.IsLoadError,
                             "Error", status.IsError,
                             "InProgress", status.IsInProgress,
                             "Dupe", status.IsDupe,
                             "Invalid", status.IsInvalid,
                             "Activatable", status.IsActivatable,
                             "Queued", status.IsQueued,
                             "Success", status.IsSuccess
                );
        }

        static Style MakeStyle(string BackgroundHex)
        {
            Brush Background = new SolidColorBrush((Color) ColorConverter.ConvertFromString(BackgroundHex));
            var style = new Style();
            style.BasedOn = GridViewRowBaseStyle;
            style.TargetType = GridViewRowBaseStyle.TargetType;
            style.Setters.Add(new Setter(Control.BackgroundProperty, Background));
            return style;
        }

        protected static Style GridViewRowBaseStyle = ((Style) Application.Current.FindResource("GridViewRowStyle"));

        static Dictionary<string, Style> styles = new Dictionary<string, Style>
                                                  {
                                                      {"Ready", MakeStyle("#004080")},
                                                      {"Queued", MakeStyle("#2D5340")},
                                                      {"Active", MakeStyle("#00264d")},
                                                      {"Success", MakeStyle("#004D26")},
                                                      {"Error", MakeStyle("#990000")},
                                                      {"TorrentError", MakeStyle("#FF0000")},
                                                      {"Invalid", MakeStyle("#CCCCCC")},
                                                      {"Disabled", MakeStyle("#999966")},
                                                      {"Dupe", MakeStyle("#CC0000")},
                                                      {"QueueDupe", MakeStyle("#B32400")},
                                                      {"TorrentDupe", MakeStyle("#B32400")},
                                                      {"InProgress", MakeStyle("#008040")},
                                                      {"NoLabel", MakeStyle("#E60000")}
                                                  };
    }

    public class MetalinkQueueStyle : QueueStyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var queueItem = item as MetalinkQueueItem;
            return queueItem != null ? GetStyle(queueItem.Status) : null;
        }
    }

    public class TorrentQueueStyle : QueueStyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var queueItem = item as TorrentQueueItem;
            return queueItem != null ? GetStyle(queueItem.Status) : null;
        }
    }
}
