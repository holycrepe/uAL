using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Torrent.Queue;
using uAL.Properties.Settings.ToggleSettings;

namespace uAL.Queue
{
    using static uAL.Properties.Settings.LibSettings;
    public delegate void QueueOnProgressChangedHandler(QueueWorkerState state);
	public delegate void QueueOnProgressChangedHandler<TQueueItem>(QueueWorkerState<TQueueItem> state) where TQueueItem : QueueItemBase;
		
	public abstract class QueueMonitor<TQueueItem> : QueueMonitorBase, IQueueMonitor<TQueueItem>  where TQueueItem : QueueItemBase
	{
		public static QueueOnProgressChangedHandler<TQueueItem> QueueOnProgressChangedHandlerPromoter(QueueOnProgressChangedHandler OnProgressChanged) {
			return QueueOnProgressChangedHandlerPromoter<TQueueItem>(OnProgressChanged);
		}
        public abstract QueueToggleStatus QueueType { get;  }
        public Toggle TOGGLES => Toggles.GetActiveToggle(QueueType);
        protected string activeDir => ActiveDirectory;
        protected string addedDir => ActiveAddedDirectory;

        #region Process Queue		
        public override Task ProcessQueueBase(IEnumerable<object> SelectedItems, QueueOnProgressChangedHandler OnProcessQueueComplete = null)
		{
			return ProcessQueue(SelectedItems, QueueOnProgressChangedHandlerPromoter(OnProcessQueueComplete));
		}
		public Task ProcessQueue(DataControl grid, QueueOnProgressChangedHandler<TQueueItem> OnProcessQueueComplete = null)
		{        	
			return ProcessQueue(QueueMonitorBase.GetDataControlItems<object>(grid), OnProcessQueueComplete);
		}
		public abstract Task ProcessQueue(IEnumerable<object> SelectedItems, QueueOnProgressChangedHandler<TQueueItem> OnProcessQueueComplete = null);
		#endregion
		#region Queue All Files		
		public override Task QueueAllFilesBase(bool isStartup, QueueOnProgressChangedHandler OnProgressChanged = null)
		{
			return QueueAllFiles(isStartup, QueueOnProgressChangedHandlerPromoter(OnProgressChanged));
		}
		public Task QueueAllFiles(QueueOnProgressChangedHandler<TQueueItem> OnProgressChanged)
		{
			return QueueAllFiles(false, OnProgressChanged);
		}		
		public abstract Task QueueAllFiles(bool isStartup, QueueOnProgressChangedHandler<TQueueItem> OnProgressChanged = null);
		#endregion Queue All Files						

	
	}
}