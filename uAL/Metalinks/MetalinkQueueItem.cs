using System;
using System.IO;
using System.ComponentModel;
using Torrent.Queue;
namespace uAL.Metalinks
{
    public class MetalinkQueueItem : QueueItemBase
    {
        FileInfo _metalink;
        FileInfo _torrent;
        Uri _uri;

        public string Category {
            get { return QueueItem.Label; }
            set { QueueItem.Label = value; OnPropertyChanged(nameof(Category)); }
        }
        public FileInfo Metalink
        {
            get { return _metalink; }
            set { _metalink = value; OnPropertyChanged(nameof(Metalink), nameof(MetalinkName)); }
        }
        public FileInfo Torrent
        {
            get { return _torrent; }
            set { _torrent = value; OnPropertyChanged(nameof(Torrent), nameof(TorrentName)); }
        }
        public Uri Uri
        {
            get { return _uri; }
            set { _uri = value; OnPropertyChanged(nameof(Uri)); }
        }
        public int Number {
        	get { return QueueItem.Number; }
        	set { QueueItem.Number = value; OnPropertyChanged(nameof(Number)); }
        }
        public QueueStatusMember Status
        {
            get { return QueueItem.Status; }
            set { QueueItem.Status = value; OnPropertyChanged(nameof(Status)); }
        }
        public string MetalinkName => Path.GetFileNameWithoutExtension(Metalink.FullName);
        string torrentName => Path.GetFileNameWithoutExtension(Torrent.FullName);
        public string TorrentName {
            get { return torrentName; }
            set { if (value != torrentName) { Torrent = new FileInfo(Path.Combine(Torrent.DirectoryName, value, Torrent.Extension)); } }
        }

        public MetalinkQueueItem() { }
        public MetalinkQueueItem(string filename, string category, Uri uri, FileInfo torrentFileInfo, int number = 0)
        {
        	Metalink = new FileInfo(filename);
            Category = category;
            Uri = uri;
            Torrent = torrentFileInfo;
            Status = QueueStatus.Queued;
            Number = number;
            QueueItem.SetName(() => TorrentName);
        }
    }


}
