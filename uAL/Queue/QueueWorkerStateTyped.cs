namespace uAL.Queue
{
    using System;
    using Properties.Settings.ToggleSettings;
    using Torrent.Queue;

    public class QueueWorkerState<TQueueItem> : IQueueWorkerState<TQueueItem> where TQueueItem : QueueItemBase
    {
        public QueueToggleStatus FileType { get; private set; }
        public TQueueItem QueueItem { get; private set; }
        public string Name { get; private set; }
        public string Label { get; private set; }
        public int Number { get; private set; }
        public int Total { get; private set; }
        public int Progress => Convert.ToInt32(Convert.ToDouble(Number) / Total * 100);
        public QueueStatusMember Status { get; private set; }       
        public QueueWorkerState(QueueToggleStatus fileType, int total, QueueStatusMember status = null, string fileName = null, string label = null, int fileNumber = 0, TQueueItem queueItem = null)
        {
            FileType = fileType;
            Total = total;
            Status = status ?? QueueStatus.Uninitialized;
            Name = fileName;
            Label = label;
            Number = fileNumber;
            QueueItem = queueItem;
        }
		
        public QueueWorkerState<TQueueItem> New(QueueStatusMember status, string fileName, string label, int fileNumber = 0, TQueueItem queueItem = null) {
            return new QueueWorkerState<TQueueItem>(FileType, Total, status, fileName, label, fileNumber, queueItem);
        }
        public QueueWorkerState<TQueueItem> New(TQueueItem newQueueItem)
        {
            return New(newQueueItem.QueueItem.Status, newQueueItem.QueueItem.Name, newQueueItem.QueueItem.Label, newQueueItem.QueueItem.Number, newQueueItem);
        }
        public QueueWorkerState Cast() {
            return QueueItem == null ? Cast(FileType, Total, Status, Name, Label, Number, QueueItem) : Cast(FileType, Total, QueueItem);
        }
        public static QueueWorkerState Cast(QueueToggleStatus fileType, int total, QueueStatusMember status, string fileName, string label, int fileNumber = 0, TQueueItem queueItem = null) {
            return new QueueWorkerState(fileType, total, status, fileName, label, fileNumber, queueItem);
        }
        public static QueueWorkerState Cast(QueueToggleStatus fileType, int total, TQueueItem newQueueItem)
        {
            return new QueueWorkerState(fileType, total, newQueueItem.QueueItem.Status, newQueueItem.QueueItem.Name, newQueueItem.QueueItem.Label, newQueueItem.QueueItem.Number, newQueueItem);
        }
        //      public static implicit operator QueueWorkerState(QueueWorkerState<TQueueItem> self) {
        //      	return self.Cast();
        //}
    }
}