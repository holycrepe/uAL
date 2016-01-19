namespace wUAL.Queue
{
    public class QueueWorkerOptions
	{
		public class GridViewOptions
		{
			public bool PrepareItems;			
		}
		public class ProgressChangedOptions {
			public bool AddQueueItem;
			public bool UpdateQueueItem;			
		}

		public readonly GridViewOptions GridView = new GridViewOptions();
		public readonly ProgressChangedOptions OnProgressChanged = new ProgressChangedOptions();
        public bool FileSystemMonitorRequired;

		public QueueWorkerOptions(bool gridViewPrepareItems = false, bool onProgressChangedAddQueueItem = false, bool onProgressChangedUpdateQueueItem = false, bool fileSystemMonitorRequired = true)
		{
			GridView.PrepareItems = gridViewPrepareItems;
			OnProgressChanged.AddQueueItem = onProgressChangedAddQueueItem;
			OnProgressChanged.UpdateQueueItem = onProgressChangedUpdateQueueItem;
            FileSystemMonitorRequired = fileSystemMonitorRequired;
        }
	}
}


