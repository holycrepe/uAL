using System.ComponentModel;
using Torrent.Queue;
using uAL.Queue;

namespace wUAL.Queue
{
    /// <summary>
    /// Description of QueueWorker.
    /// </summary>
    public class DoQueueWorkEventArgs<TArgument> : DoWorkEventArgs
    {
        readonly TArgument argument;

        new TArgument Argument => argument;

        public DoQueueWorkEventArgs(TArgument argument) : base(null) { this.argument = argument; }
    }

    public class QueueProgressChangedEventArgs<TQueueItem> : ProgressChangedEventArgs where TQueueItem : QueueItemBase
    {
        protected readonly QueueWorkerState<TQueueItem> userState;
        public new QueueWorkerState<TQueueItem> UserState => userState;
        //public QueueProgressChangedEventArgs(QueueWorkerState<TQueueItem> userState) : this(userState.Progress, userState) { }
        public QueueProgressChangedEventArgs(int progressPercentage, QueueWorkerState<TQueueItem> userState)
            : base(progressPercentage, userState)
        {
            this.userState = userState;
        }
    }

    public class QueueProgressChangedEventArgs : QueueProgressChangedEventArgs<QueueItemBase>
    {
        private new QueueWorkerState userState;

        public new QueueWorkerState UserState => userState ?? (userState = base.userState?.Cast());
        //public QueueProgressChangedEventArgs(QueueWorkerState userState) : base(userState) { }
        public QueueProgressChangedEventArgs(int progressPercentage, QueueWorkerState userState)
            : base(progressPercentage, userState) {}
    }
}
