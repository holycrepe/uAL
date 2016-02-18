namespace wUAL.Queue
{
    public class QueueWorkerOptions
    {
        public class GridViewOptions
        {
            public bool PrepareItems;
        }

        public GridViewOptions GridView = new GridViewOptions();
        public bool FileSystemMonitorRequired;
        public bool ClearFileSearchCache;
        public bool DoReportOperationComplete;

        public QueueWorkerOptions(bool clearFileSearchCache = false, bool gridViewPrepareItems = false,
                                  bool fileSystemMonitorRequired = true, bool doReportOperationComplete=true)
        {
            GridView.PrepareItems = gridViewPrepareItems;
            FileSystemMonitorRequired = fileSystemMonitorRequired;
            ClearFileSearchCache = clearFileSearchCache;
            DoReportOperationComplete = doReportOperationComplete;
        }
    }
}
