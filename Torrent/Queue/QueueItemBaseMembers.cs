namespace Torrent.Queue
{
    using System;
    using PostSharp.Patterns.Model;
    using Infrastructure;

    [NotifyPropertyChanged]
    public class QueueItemBaseMembers : NotifyPropertyChangedBase
    {
        string name, label;
        Func<string> _name, _label;
        QueueStatusMember status;
        Func<QueueStatusMember> _status;
        int? number;
        Func<int> _number;

        [SafeForDependencyAnalysis]
        public string Name
        {
            get { return this._name == null ? this.name : this._name(); }
            set { this.name = value; }
        }

        [SafeForDependencyAnalysis]
        public string Label
        {
            get { return this._label == null ? this.label : this._label(); }
            set { this.label = value; }
        }

        [SafeForDependencyAnalysis]
        public QueueStatusMember Status
        {
            get { return (this._status == null ? this.status : this._status()) ?? QueueStatus.Uninitialized; }
            set { this.status = value; }
        }

        [SafeForDependencyAnalysis]
        public int Number
        {
            get { return this._number == null ? (this.number.HasValue ? this.number.Value : 0) : this._number(); }
            set { this.number = value; }
        }

        public void SetName(Func<string> func) { _name = func; }
        public void SetLabel(Func<string> func) { _label = func; }
        public void SetStatus(Func<QueueStatusMember> func) { _status = func; }
        public void SetNumber(Func<int> func) { _number = func; }

        public QueueItemBaseMembers(string name = null, string label = null, QueueStatusMember status = null,
                                    int? number = null)
        {
            Name = name;
            Label = label;
            Status = status;
            if (number.HasValue) {
                Number = number.Value;
            }
        }
    }
}
