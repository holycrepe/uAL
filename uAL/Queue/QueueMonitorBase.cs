using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Torrent.Queue;
namespace uAL.Queue
{
	public abstract class QueueMonitorBase : IQueueMonitor
	{
		public static QueueOnProgressChangedHandler<TQueueItem> QueueOnProgressChangedHandlerPromoter<TQueueItem>(QueueOnProgressChangedHandler OnProgressChanged)  
			where TQueueItem : QueueItemBase
		{
			return typedState =>  {
				var state = typedState.Cast();
				OnProgressChanged(state);
			};
		}

		public static IEnumerable<TQueueItem> GetDataControlItems<TQueueItem>(DataControl grid)
		{
			return grid.SelectedItems.Count == 0 ? (IEnumerable<TQueueItem>)grid.ItemsSource : (IEnumerable<TQueueItem>)grid.SelectedItems;
		}

		// disable once ConvertToAutoProperty
		public FileSystemWatcher Watcher {
			get { return watcher; }
			set { watcher = value; }
		}

		// disable once StaticFieldInGenericType
		static FileSystemWatcher watcher;

		bool _disposed = false;

		#region Queue
		public abstract Task Start(bool logStartup = false);

		public abstract void Clear();

		public abstract int Count {
			get;
		}

		#endregion
		#region Queue: New Items
		public void AddToQueue(object newQueueItemObject)
		{
			NewQueueItem(newQueueItemObject, true);
		}

		public void QueueItemChanged(object newQueueItemObject)
		{
			NewQueueItem(newQueueItemObject, false);
		}

		internal abstract void NewQueueItem(object newQueueItemObject, bool addToQueue);

		#endregion
		#region Process Queue
		public Task ProcessQueueBase(DataControl grid, QueueOnProgressChangedHandler OnProcessQueueComplete = null)
		{
			return ProcessQueueBase(GetDataControlItems<object>(grid), OnProcessQueueComplete);
		}

		public abstract Task ProcessQueueBase(IEnumerable<object> SelectedItems, QueueOnProgressChangedHandler OnProcessQueueComplete = null);
		#endregion
		#region Queue All Files
		public Task QueueAllFiles()
		{
			return QueueAllFilesBase(false, null);
		}

		public Task QueueAllFilesBase(QueueOnProgressChangedHandler OnProgressChanged)
		{
			return QueueAllFilesBase(false, OnProgressChanged);
		}

		public abstract Task QueueAllFilesBase(bool isStartup, QueueOnProgressChangedHandler OnProgressChanged = null);
		#endregion
		#region Interfaces: IDisposable
		public void Dispose()
		{
			Dispose(true);
			// Use SupressFinalize in case a subclass 
			// of this type implements a finalizer.
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed) {
				if (disposing) {
					// Clear all property values that maybe have been set
					// when the class was instantiated
					if (Watcher != null) {
						Watcher.Dispose();
					}
				}
				// Indicate that the instance has been disposed.
				_disposed = true;
			}
		}
	#endregion
	}
}

