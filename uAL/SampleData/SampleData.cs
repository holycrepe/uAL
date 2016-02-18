using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using uAL.Metalinks;
using uAL.Torrents;
using uAL.Properties;
using System.IO;

namespace uAL.SampleData
{

    public static class MetalinkQuerySampleDataContext
    {
        public static MetalinkQueueItem MetalinkQueueItemSampleData
        {
            get
            {
                var category = @"$\O\Some\Category\Here";
                var metalink = Path.Combine(Settings.ActiveDirectory, @"$\METALINKS\O", "myMetalink.metalink");
                var torrent = Path.Combine(Settings.ActiveDirectory, category, "myTorrentName.torrent");
                var uri = new Uri("http://www.google.com/testing/testing/testing/testing");
                var item = new MetalinkQueueItem(metalink, category, uri, FileUtils.GetInfo(torrent));
                item.Status = (MetalinkDownloadStatus)new Random().Next(1, 4);
                return item;
            }
        }
        public static ObservableCollection<MetalinkQueueItem> MetalinkQueueSampleData
        {
            get
            {
                var queue = new ObservableCollection<MetalinkQueueItem>();
                for (var i = 0; i < 100; i++)
                {
                    queue.Add(MetalinkQueueItemSampleData);
                }
                return queue;
            }
        }
    }
    public static class TorrentQuerySampleDataContext
    {
        public static TorrentQueueItem TorrentQueueItemSampleData
        {
            get
            {
                var category = @"$\O\Some\Category\Here";
                var torrent = Path.Combine(Settings.ActiveDirectory, category, "myTorrentName.torrent");
                var status = (TorrentQueueStatus)new Random().Next(1, 6);
                var item = new TorrentQueueItem(Settings.ActiveDirectory, torrent, category, status);                
                return item;
            }
        }
        public static ObservableCollection<TorrentQueueItem> TorrentQueueSampleData
        {
            get
            {
                var queue = new ObservableCollection<TorrentQueueItem>();
                for (var i = 0; i < 100; i++)
                {
                    queue.Add(TorrentQueueItemSampleData);
                }
                return queue;
            }
        }
    }
}
