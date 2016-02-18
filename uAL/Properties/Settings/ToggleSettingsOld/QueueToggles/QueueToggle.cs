
namespace uAL.Properties.Settings.ToggleSettings
{
    using PostSharp.Patterns.Model;
    using System.Runtime.Serialization;
    using Serialization;

    [DataContract(Namespace = Namespaces.Default)]
    [NotifyPropertyChanged]
    public partial class QueueToggle // : NotifyPropertyChangedBase
    {
        public bool Initialized { get; set; }
        [DataMember]
        public string Name { get; set; }        
        public string Type { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public bool All { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public bool Metalinks { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public bool Torrents { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public bool Jobs { get; set; }

        public QueueToggle() : this(null, null) { }
        public QueueToggle(string name, string type) : this(name, type, QueueTypes.Disabled) { }
        public QueueToggle(string name, string type, QueueTypes value)
            : this(name, type, value.IsAll(), value.IsTorrent(), value.IsMetalink(), value.IsJob()) {}
        public QueueToggle(string name, string type, int value) : this(name, type, value == 1, value == 1, value == 1, value == 1) {}
        public QueueToggle(string name, string type, bool all, bool torrents, bool metalinks, bool jobs)
        {
            Name = name ?? nameof(Name);
            Type = type ?? nameof(Settings);
            Set(all, torrents, metalinks, jobs, false);
            Initialized = false;
        }
        public void Load(QueueToggle other)
        {
            if (other != null)
            {
                Set(other.All, other.Torrents, other.Metalinks, other.Jobs, other.Initialized);
                if (this.Name == null)
                {
                    Name = other.Name;
                }
            }
        }
        public void Set(QueueTypes status, bool initialized = true)
            => Set(status.IsAll(), status.IsTorrent(), status.IsMetalink(), status.IsJob(), initialized);

        public void Set(bool all, bool torrents, bool metalinks, bool jobs, bool initialized = true)
        {
            All = all;
            Torrents = torrents;
            Metalinks = metalinks;
            Jobs = jobs;
            Initialized = initialized;
        }

        public QueueTypes Status
            =>
                All
                    ? QueueTypes.All
                    : (this.Torrents
                           ? QueueTypes.Torrent
                           : (this.Metalinks ? QueueTypes.Metalink 
                           : this.Jobs ? QueueTypes.Job : QueueTypes.Disabled));

        public string[] Values
            => GetValues(this.All, this.Torrents, this.Metalinks, this.Jobs);

        public string Value
            => GetValue(this.All, this.Torrents, this.Metalinks, this.Jobs);

        public override string ToString()
            => this.Value;
    }
}
