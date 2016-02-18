namespace Torrent.Queue
{
    using Infrastructure;

    public delegate void QueueOnStartHandler(int maximum);
    public delegate void QueueOnCompleteHandler<TQueueItem>(TQueueItem item);
    public delegate void QueueWorkerOnCompleteHandler();    
    public delegate void ProcessQueueItemCompleteHandler<TQueueItem, TResult>(TQueueItem item, TResult result);

    public class QueueItemBase : NotifyPropertyChangedBase
    {
        protected QueueItemBaseMembers _queueItem;

        public QueueItemBaseMembers QueueItem
        {
            get { return _queueItem; }
            internal set { _queueItem = value; }
        }

        public QueueItemBase(string name = null, string label = null, QueueStatusMember status = null,
                             int? number = null)
        {
            this._queueItem = new QueueItemBaseMembers(name, label, status, number);
        }
    }
}
