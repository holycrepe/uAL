using System;
using System.IO;
using System.ComponentModel;
using Torrent.Queue;

namespace uAL.UTorrentJobs
{
    using System.Collections.Generic;
    using PostSharp.Patterns.Model;
    using Torrent.Enums;
    using Torrent.Helpers.Utils;
    using System.Net;
    using Torrent;
    [NotifyPropertyChanged]
    public class UTorrentJobQueueItem : QueueItemBase
    {
        protected override Dictionary<string, string[]> DerivedProperties { get; }
            = new Dictionary<string, string[]>
            {
                [nameof(CategoryNumber)] =
                    new[]
                    {
                        nameof(Category),
                        nameof(UTorrentJobName)
                    }
            };
        public UTorrentJob Job { get; set; }
        public string Category
        {
            get { return this._queueItem?.Label; }
            set { this._queueItem.Label = value; }
        }

        public FileInfo UTorrentJob { get; set; }
        public FileInfo Torrent { get; set; }
        public Uri Uri { get; set; }

        public int Number
        {
            get { return this._queueItem.Number; }
            set { this._queueItem.Number = value; }
        }

        public QueueStatusMember Status
        {
            get { return this._queueItem.Status; }
            set { this._queueItem.Status = value; }
        }

        internal WebClient Client { get; set; }

        protected string Base
            => _queueItem?.Label ?? this.UTorrentJobName;

        [SafeForDependencyAnalysis]
        public string UTorrentJobName =>
            Path.GetFileNameWithoutExtension(this.UTorrentJob.FullName);

        [SafeForDependencyAnalysis]
        string torrentName =>
            Path.GetFileNameWithoutExtension(this.Torrent.FullName);

        [SafeForDependencyAnalysis]
        public string TorrentName
        {
            get { return this.torrentName; }
            set
            {
                if (value != this.torrentName)
                {
                    this.Torrent = FileUtils.GetInfo(Path.Combine(this.Torrent.DirectoryName, value, this.Torrent.Extension));
                }
            }
        }

        [IgnoreAutoChangeNotification]
        public int CategoryNumber
        {
            get
            {
                var baseName = this.UTorrentJobName.Replace(this.Category.Replace(": ", "-"), "").Trim('-');
                int num;
                return string.IsNullOrEmpty(baseName) ? 1 : (int.TryParse(baseName, out num) ? num : 1);
            }
        }

        #region Constructor
        public UTorrentJobQueueItem() { }
        public UTorrentJobQueueItem(UTorrentJob job)
        {
            this.Job = job;
        }

        public UTorrentJobQueueItem(FileInfo uTorrentJob, string category, Uri uri, FileInfo torrentFileInfo, int number = 0)
        {
            UTorrentJob = uTorrentJob;
            Category = category;
            Uri = uri;
            Torrent = torrentFileInfo;
            Status = QueueStatus.Queued;
            Number = number;
            this._queueItem.SetName(() => TorrentName);
            _queueItem.PropertyChanged += (s, e) =>
            {
                LogQueueItemChanged(e.PropertyName);
                this.OnPropertyChanged(e.PropertyName, nameof(QueueItem));
            };
        }

        public UTorrentJobQueueItem(string filename, string category, Uri uri, FileInfo torrentFileInfo, int number = 0)
            : this(FileUtils.GetInfo(filename), category, uri, torrentFileInfo, number)
        { }

        #endregion

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
            if (fileNumber == 0 || fileNumber % 100 == 1)
            {
                LogUtils.Log(
                             (fileNumber > 0 ? (fileNumber + ".").PadRight(4) + " " : "") + (prefix ?? " ") + text,
                             this.Status.ToString(), this.Base, this.TorrentName, textPadDirection, textSuffix,
                             titlePadDirection, titleSuffix, random);
            }
#endif
        }

        #endregion
    }
}
