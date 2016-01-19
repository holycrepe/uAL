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
		Task QueueAllFilesBase(QueueOnProgressChangedHandler OnProgressChanged);
		Task QueueAllFilesBase(bool isStartup, QueueOnProgressChangedHandler OnProgressChanged = null);
		Task ProcessQueueBase(IEnumerable<object> SelectedItems, QueueOnProgressChangedHandler OnProcessQueueComplete = null);
		Task ProcessQueueBase(DataControl grid, QueueOnProgressChangedHandler OnProcessQueueComplete = null);						
	}
	
	public interface IQueueMonitor<TQueueItem> : IQueueMonitor where TQueueItem : QueueItemBase
	{
		Task QueueAllFiles(QueueOnProgressChangedHandler<TQueueItem> OnProgressChanged);

		Task QueueAllFiles(bool isStartup, QueueOnProgressChangedHandler<TQueueItem> OnProgressChanged = null);

		Task ProcessQueue(IEnumerable<object> SelectedItems, QueueOnProgressChangedHandler<TQueueItem> OnProcessQueueComplete = null);

		Task ProcessQueue(DataControl grid, QueueOnProgressChangedHandler<TQueueItem> OnProcessQueueComplete = null);
	}
}

