using System.Windows;
using uAL.Torrents;
using uAL.Metalinks;
using Torrent;

namespace wUAL
{
    public class TorrentQueueStyle : QueueStatusStyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
            => SelectStyle((item as TorrentQueueItem)?.Status);
    }
    public class MetalinkQueueStyle : QueueStatusStyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
            => SelectStyle((item as MetalinkQueueItem)?.Status);
    }
    public class UTorrentJobQueueStyle : QueueStatusStyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
            => SelectStyle((item as UTorrentJob)?.Status);
    }
}