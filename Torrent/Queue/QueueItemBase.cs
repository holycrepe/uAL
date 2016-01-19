namespace Torrent.Queue
{
    using Infrastructure;

    public class QueueItemBase : NotifyPropertyChangedExtendedBase {
		public QueueItemBaseMembers QueueItem { get; internal set; }
		public QueueItemBase(string name = null, string label = null, QueueStatusMember status = null, int? number = null) {
			QueueItem = new QueueItemBaseMembers(name, label, status, number);
		}
	}
}
