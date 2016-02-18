using System.IO;
using Torrent.Helpers.Utils;
using Torrent;
using Torrent.Queue;

namespace uAL.Torrents
{
    using Torrent.Enums;
    using Torrent.Helpers.StringHelpers;
    using PostSharp.Patterns.Model;
    using System.ComponentModel;

    [NotifyPropertyChanged]
    public class TorrentQueueItem : TorrentItem, INotifyPropertyChanged
    {
        public int Number
        {
            get { return this._queueItem.Number; }
            private set { this._queueItem.Number = value; }
        }

        public FileInfo Dupe { get; set; }

        public QueueStatusMember Status
        {
            get { return _queueItem.Status; }
            set
            {
                value = value ?? QueueStatus.Uninitialized;
                if (_queueItem.Status != value) {
                    _queueItem.Status = value;
                }
            }
        }

        public TorrentQueueItem(string oldRootDirectory, string newRootDirectory, string fileName, int fileNumber)
            : this(oldRootDirectory, newRootDirectory, fileName, label: null, fileNumber: fileNumber) {}

        public TorrentQueueItem(string oldRootDirectory, string newRootDirectory, string fileName,
                                TorrentLabel label = null, QueueStatusMember status = null, int fileNumber = 0)
            : base(oldRootDirectory, newRootDirectory, fileName, label)
        {
            Status = status;
            Number = fileNumber;
            _queueItem.PropertyChanged += (s, e) =>
                                          {
                                              LogQueueItemChanged(e.PropertyName);
                                              this.OnPropertyChanged(e.PropertyName, nameof(QueueItem));
                                          };
        }

        #region Logging
        [System.Diagnostics.Conditional("LOG_QUEUE_ITEM_CHANGED"), System.Diagnostics.Conditional("LOG_ALL")]
        public void LogQueueItemChanged(string propertyName)
            => Log("Δ ", propertyName);
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        public void Log(string prefix = "+", string text = null, PadDirection textPadDirection = PadDirection.Default,
                        string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default,
                        string titleSuffix = null, int random = 0)
        {
#if DEBUG || TRACE_EXT
            var fileNumber = (prefix == "+" ? Number : 0);
            if (fileNumber == 0 || fileNumber%100 == 1) {
                LogUtils.Log(
                             (fileNumber > 0 ? (fileNumber + ".").PadRight(4) + " " : "") + (prefix ?? " ") + text,
                             this.Status.ToString() , this.Base, this.TorrentName, textPadDirection, textSuffix,
                             titlePadDirection, titleSuffix, random);
            }
#endif
        }

        #endregion
    }
}
