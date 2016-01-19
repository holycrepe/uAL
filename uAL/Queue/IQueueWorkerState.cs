using Torrent.Queue;
using uAL.Properties.Settings.ToggleSettings;

namespace uAL.Queue
{
    public interface IQueueWorkerState<TQueueItem> where TQueueItem : QueueItemBase
    {
        QueueToggleStatus FileType { get; }
        string Label { get; }
        string Name { get; }
        int Number { get; }
        int Progress { get; }
        TQueueItem QueueItem { get; }
        QueueStatusMember Status { get; }
        int Total { get; }

        QueueWorkerState Cast();
        QueueWorkerState<TQueueItem> New(TQueueItem newQueueItem);
        QueueWorkerState<TQueueItem> New(QueueStatusMember status, string fileName, string label, int fileNumber = 0, TQueueItem queueItem = null);
    }
}