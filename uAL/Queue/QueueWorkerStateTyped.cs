namespace uAL.Queue
{
    using System;
    using Properties.Settings.ToggleSettings;
    using Torrent.Queue;
    using Torrent.Extensions;

    public class QueueWorkerState<TQueueItem> : IQueueWorkerState<TQueueItem> where TQueueItem : QueueItemBase
    {
        public MonitorTypes FileType { get; }
        public TQueueItem QueueItem { get; private set; } = null;
        public string Name { get; private set; } = null;
        public string Label { get; private set; } = null;
        public int Number { get; private set; } = 0;
        //public int Total { get; private set; }
        //public int Progress =>
        //    (int) Number.GetProgress(Total);
        public QueueStatusMember Status { get; private set; } = QueueStatus.Uninitialized;

        public QueueWorkerState(MonitorTypes fileType, TQueueItem queueItem)
        {
            FileType = fileType;
            SetQueueItem(queueItem);
        }

        public QueueWorkerState(MonitorTypes fileType, QueueStatusMember status = null, string fileName = null,
                                string label = null, TQueueItem queueItem = null, int number = -1)
        {
            FileType = fileType;
            Status = status ?? QueueStatus.Uninitialized;
            Name = fileName;
            Label = label;
            QueueItem = queueItem;
            Number = (number == -1 ? queueItem?.QueueItem?.Number : number) ?? 0;
        }

        public void SetQueueItem(TQueueItem queueItem)
        {
            if (queueItem != null) {
                QueueItem = queueItem;
                var item = queueItem.QueueItem;
                if (item != null) {
                    Status = item.Status;
                    Name = item.Name;
                    Label = item.Label;
                    Number = item.Number;
                }
            }
        }

        public QueueWorkerState<TQueueItem> New(QueueStatusMember status, string fileName, string label,
                                                TQueueItem queueItem = null, int number = -1)
            => new QueueWorkerState<TQueueItem>(FileType, status, fileName, label, queueItem, number);

        public QueueWorkerState<TQueueItem> New(TQueueItem newQueueItem)
            =>
                New(newQueueItem.QueueItem.Status, newQueueItem.QueueItem.Name, newQueueItem.QueueItem.Label,
                    newQueueItem, newQueueItem.QueueItem.Number);

        public QueueWorkerState Cast()
            => QueueItem == null ? Cast(FileType, Status, Name, Label, QueueItem) : Cast(FileType, QueueItem);

        public static QueueWorkerState Cast(MonitorTypes fileType, QueueStatusMember status, string fileName,
                                            string label, TQueueItem queueItem = null, int number = -1)
            => new QueueWorkerState(fileType, status, fileName, label, queueItem, number);

        public static QueueWorkerState Cast(MonitorTypes fileType, TQueueItem newQueueItem)
            =>
                new QueueWorkerState(fileType, newQueueItem.QueueItem.Status, newQueueItem.QueueItem.Name,
                                     newQueueItem.QueueItem.Label, newQueueItem, newQueueItem.QueueItem.Number);

        //      public static implicit operator QueueWorkerState(QueueWorkerState<TQueueItem> self) {
        //      	return self.Cast();
        //}

        //public QueueWorkerState(QueueToggleStatus fileType, int total, QueueStatusMember status = null, string fileName = null, string label = null, int fileNumber = 0, TQueueItem queueItem = null)
        //{
        //    FileType = fileType;
        //    Total = total;
        //    Status = status ?? QueueStatus.Uninitialized;
        //    Name = fileName;
        //    Label = label;
        //    Number = fileNumber;
        //    QueueItem = queueItem;
        //}
        //public QueueWorkerState<TQueueItem> New(QueueStatusMember status, string fileName, string label, int fileNumber = 0, TQueueItem queueItem = null) 
        //    => QueueWorkerState<TQueueItem>(FileType, Total, status, fileName, label, fileNumber, queueItem);
        //public QueueWorkerState Cast()
        //    => QueueItem == null ? Cast(FileType, Total, Status, Name, Label, Number, QueueItem) : Cast(FileType, Total, QueueItem);
        //public static QueueWorkerState Cast(QueueToggleStatus fileType, int total, QueueStatusMember status, string fileName, string label, int fileNumber = 0, TQueueItem queueItem = null)
        //{
        //    return new QueueWorkerState(fileType, total, status, fileName, label, fileNumber, queueItem);
        //}
        //public static QueueWorkerState Cast(QueueToggleStatus fileType, int total, TQueueItem newQueueItem)
        //{
        //    return new QueueWorkerState(fileType, total, newQueueItem.QueueItem.Status, newQueueItem.QueueItem.Name, newQueueItem.QueueItem.Label, newQueueItem.QueueItem.Number, newQueueItem);
        //}
    }
}
