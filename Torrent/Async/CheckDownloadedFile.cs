using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torrent.Async
{
    using Infrastructure.EAP;
    using PostSharp.Patterns.Model;
    using Queue;
    using System.ComponentModel;
    public static class Workers
    {
        static DownloadedFileChecker _downloadedFileChecker;
        public static DownloadedFileChecker DownloadedFileChecker
            => (_downloadedFileChecker ?? (_downloadedFileChecker = new DownloadedFileChecker()));
    }
    public class DownloadedFileCheckerArguments
    {
        public string Name { get; set; }
        public string RootDirectory { get; set; }
        public DownloadedFileCheckerArguments(string name, string rootDirectory)
        {
            Name = name;
            RootDirectory = rootDirectory;
        }
    }

    public partial class DownloadedFileChecker : AsyncQueuedWorker<DownloadedFileChecker, TorrentInfo, DownloadedFileCheckerArguments, double>
    {
        protected override double DoWork(Operation asyncOp)
        {
            var arguments = asyncOp.Arguments;
            var item = asyncOp.Item;
            item.Status = QueueStatus.Active;
            item.Progress = 0;
            NotifyPropertyChangedServices.RaiseEventsImmediate(item);
            TorrentInfo.ProgressChangedEventHandler progressChanged = p =>
            {
                this.ReportProgress(item.Progress = p, asyncOp);
                NotifyPropertyChangedServices.RaiseEventsImmediate(item);
            };
            var percent = item.PercentComplete = item.CheckFile(progressChanged, arguments.Name, arguments.RootDirectory);
            item.Status = percent == 100 ? QueueStatus.Success : QueueStatus.Incomplete;
            return percent;
        }
        public override void OnQueueWorkerAsync(Operation asyncOp)
            => asyncOp.Item.Status = QueueStatus.Queued;
        #region Usages
        #region Uages: UTorrentJob
        public Operation Check(UTorrentJob item, Events.ProgressChangedHandler onProgressChanged = null, Events.CompletedHandler onRunWorkerCompleted = null)
            => Check(item, item.Torrent.Largest.FullName, onProgressChanged, onRunWorkerCompleted);
        public Operation Check(UTorrentJob item, string name, Events.ProgressChangedHandler onProgressChanged = null, Events.CompletedHandler onRunWorkerCompleted = null)
            => Check(item, name, item.Path, onProgressChanged, onRunWorkerCompleted);
        public Operation CheckOriginalPath(UTorrentJob item, string name, Events.ProgressChangedHandler onProgressChanged = null, Events.CompletedHandler onRunWorkerCompleted = null)
            => Check(item, name, item.OriginalPath, onProgressChanged, onRunWorkerCompleted);
        public Operation CheckNewPath(UTorrentJob item, string name, Events.ProgressChangedHandler onProgressChanged = null, Events.CompletedHandler onRunWorkerCompleted = null)
            => Check(item, name, item.NewPath, onProgressChanged, onRunWorkerCompleted);
        //public Operation Check(UTorrentJob item, string name, string directory, Events.ProgressChangedHandler onProgressChanged = null, Events.CompletedHandler onRunWorkerCompleted = null)
        //    => QueueWorkerAsync(item.Torrent, new DownloadedFileCheckerArguments(name, directory), onProgressChanged, onRunWorkerCompleted);
        #endregion
        #region Uages: TorrentInfo
        public Operation Check(TorrentInfo item, Events.ProgressChangedHandler onProgressChanged = null, Events.CompletedHandler onRunWorkerCompleted = null)
            => Check(item, item.Name, onProgressChanged, onRunWorkerCompleted);
        public Operation Check(TorrentInfo item, string name, Events.ProgressChangedHandler onProgressChanged = null, Events.CompletedHandler onRunWorkerCompleted = null)
            => Check(item, name, item.RootDirectory, onProgressChanged, onRunWorkerCompleted);
        public Operation Check(TorrentInfo item, string name, string directory, Events.ProgressChangedHandler onProgressChanged = null, Events.CompletedHandler onRunWorkerCompleted = null)
            => QueueWorkerAsync(item, new DownloadedFileCheckerArguments(name, directory), onProgressChanged, onRunWorkerCompleted);
        #endregion
        #endregion
    }
}
