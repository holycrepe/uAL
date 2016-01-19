namespace Torrent.Queue
{
    using System;

    public class QueueItemBaseMembers {	
        string name, label;
        Func<string> _name, _label;
        QueueStatusMember status;
        Func<QueueStatusMember> _status;
        int? number;
        Func<int> _number;
		
        public string Name { get { return _name == null ? name : _name(); } set { name = value; } }
        public string Label { get { return _label == null ? label : _label(); } set { label = value; } }
        public QueueStatusMember Status { get { return (_status == null ? status : _status()) ?? QueueStatus.Uninitialized; } set { status = value; } }
        public int Number { get { return _number == null ? (number.HasValue ? number.Value : 0) : _number(); } set { number = value; } }
        public void SetName(Func<string> func) { _name = func; }
        public void SetLabel(Func<string> func) { _label = func; }
        public void SetStatus(Func<QueueStatusMember> func) { _status = func; }
        public void SetNumber(Func<int> func) { _number = func; }
			
        public QueueItemBaseMembers(string name = null, string label = null, QueueStatusMember status = null, int? number = null) {
            Name = name;
            Label = label;
            Status = status;
            if (number.HasValue) {
                Number = number.Value;
            }
        }
    }
}