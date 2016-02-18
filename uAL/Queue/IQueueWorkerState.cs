using Torrent.Queue;
using uAL.Properties.Settings.ToggleSettings;

namespace uAL.Queue
{
    public interface IQueueWorkerState<TQueueItem> where TQueueItem : QueueItemBase
    {
        MonitorTypes FileType { get; }
        string Label { get; }
        string Name { get; }
        int Number { get; }
        //int Total { get; }
        //int Progress { get; }
        TQueueItem QueueItem { get; }
        QueueStatusMember Status { get; }

        QueueWorkerState Cast();
        QueueWorkerState<TQueueItem> New(TQueueItem newQueueItem);

        QueueWorkerState<TQueueItem> New(QueueStatusMember status, string fileName, string label,
                                         TQueueItem queueItem = null, int number = -1);

        //QueueWorkerState<TQueueItem> New(QueueStatusMember status, string fileName, int total, string label, int fileNumber = 0, TQueueItem queueItem = null);
    }
}
