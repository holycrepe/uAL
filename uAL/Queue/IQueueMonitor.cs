using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Torrent.Queue;

namespace uAL.Queue
{
    public interface IQueueMonitor : IDisposable
    {
        void Clear();
        int Count { get; }

        FileSystemWatcher Watcher { get; set; }
        Task Start(bool logStartup = false);
        void AddToQueue(object newQueueItemObject);
        void QueueItemChanged(object newQueueItemObject);

        Task QueueAllFiles();
        Task QueueAllFilesBase(QueueOnStartHandler OnStart, QueueOnProgressChangedHandler OnProgressChanged);

        Task QueueAllFilesBase(bool isStartup, QueueOnStartHandler OnStart,
                               QueueOnProgressChangedHandler OnProgressChanged = null);

        Task ProcessQueueBase(IEnumerable<object> SelectedItems, QueueOnStartHandler OnStart,
                              QueueOnProgressChangedHandler OnProcessQueueComplete = null,
                              QueueWorkerOnCompleteHandler OnQueueWorkerComplete = null);

        Task ProcessQueueBase(DataControl grid, QueueOnStartHandler OnStart,
                              QueueOnProgressChangedHandler OnProcessQueueComplete = null,
                              QueueWorkerOnCompleteHandler OnQueueWorkerComplete = null);
    }

    public interface IQueueMonitor<TQueueItem> : IQueueMonitor where TQueueItem : QueueItemBase
    {
        Task QueueAllFiles(QueueOnStartHandler OnStart, QueueOnProgressChangedHandler<TQueueItem> OnProgressChanged);

        Task QueueAllFiles(bool isStartup, QueueOnStartHandler OnStart,
                           QueueOnProgressChangedHandler<TQueueItem> OnProgressChanged = null);

        Task ProcessQueue(IEnumerable<object> SelectedItems, QueueOnStartHandler OnStart,
                          QueueOnProgressChangedHandler<TQueueItem> OnProcessQueueComplete = null,
                          QueueWorkerOnCompleteHandler OnQueueWorkerComplete = null);

        Task ProcessQueue(DataControl grid, QueueOnStartHandler OnStart,
                          QueueOnProgressChangedHandler<TQueueItem> OnProcessQueueComplete = null,
                          QueueWorkerOnCompleteHandler OnQueueWorkerComplete = null);
    }
}
