using System;
using uAL.Properties.Settings.ToggleSettings;
using Torrent.Queue;

namespace uAL.Queue
{
    public class QueueWorkerState : QueueWorkerState<QueueItemBase>
    {
        public QueueWorkerState(MonitorTypes fileType, QueueStatusMember status = null, string fileName = null,
                                string label = null, QueueItemBase queueItem = null, int number = 0)
            : base(fileType, status, fileName, label, queueItem, number) {}

        //public QueueWorkerState(QueueToggleStatus fileType, int total, QueueStatusMember status = null, string fileName = null, string label = null, int fileNumber = 0, QueueItemBase queueItem = null)
        //    : base(fileType, total, status, fileName, label, fileNumber, queueItem)
        //{ }
    }
}
