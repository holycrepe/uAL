using PostSharp.Patterns.Model;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using uAL.Serialization;

namespace uAL.Properties.Settings.ToggleSettings
{
    [DataContract(Namespace = Namespaces.Default)]
    [KnownType(typeof(QueueToggle))]
    [NotifyPropertyChanged]
    public class QueueToggles //: NotifyPropertyChangedBase
    {
        protected QueueToggle _settings, _arguments;
        
        public QueueToggle Settings
        {
            get { return (this._settings ?? (this._settings = new QueueToggle("Unknown", nameof(Settings)))); }
            set { this._settings = value; }
        }
        
        public QueueToggle Arguments
        {
            get { return (this._arguments ?? (this._arguments = new QueueToggle("Unknown", nameof(Arguments)))); }
            set { this._arguments = value; }
        }

        [DataMember(Name="All", Order = 2, EmitDefaultValue = false)]
        [IgnoreAutoChangeNotification]
        bool _settingsAll
        {
            get { return this.Settings.All;  }
            set { this.Settings.All = value; }
        }
        [DataMember(Name = "Torrents", Order = 3, EmitDefaultValue = false)]
        [IgnoreAutoChangeNotification]
        bool _settingsTorrents
        {
            get { return this.Settings.Torrents; }
            set { this.Settings.Torrents = value; }
        }
        [DataMember(Name = "Metalinks", Order = 4, EmitDefaultValue = false)]
        [IgnoreAutoChangeNotification]
        bool _settingsMetalinks
        {
            get { return this.Settings.Metalinks; }
            set { this.Settings.Metalinks = value; }
        }
        [DataMember(Name = "Jobs", Order = 5, EmitDefaultValue = false)]
        [IgnoreAutoChangeNotification]
        bool _settingsJobs
        {
            get { return this.Settings.Jobs; }
            set { this.Settings.Jobs = value; }
        }
        [XmlIgnore]
        [IgnoreAutoChangeNotification]
        public string Name
            => this.Settings.Name;
        //[DataMember(Name = "Name", Order = 1, EmitDefaultValue = false)]
        //public string Name
        //{
        //    get { return this._settings.Name; }
        //    set { this._settings.Name = value; }
        //}
        [SafeForDependencyAnalysis]
        public bool ALL => this.Settings.Status.IncludesAll() || this.Arguments.Status.IncludesAll();
        [SafeForDependencyAnalysis]
        public bool METALINKS => this.Settings.Status.IncludesMetalink() || this.Arguments.Status.IncludesMetalink();
        [SafeForDependencyAnalysis]
        public bool TORRENTS => this.Settings.Status.IncludesTorrent() || this.Arguments.Status.IncludesTorrent();
        [SafeForDependencyAnalysis]
        public bool JOBS => this.Settings.Status.IncludesJob() || this.Arguments.Status.IncludesJob();
        public string[] Values() => QueueToggle.GetValues(this.ALL, this.TORRENTS, this.METALINKS, this.JOBS);

        public bool GetValue(QueueTypes type)
        {
            switch (type) {
                case QueueTypes.Torrent:
                    return this.TORRENTS;
                case QueueTypes.Metalink:
                    return this.METALINKS;
                case QueueTypes.Job:
                    return this.JOBS;
                default:
                    return this.ALL;
            }
        }

        public void SetValue(QueueTypes type, bool value)
        {
            switch (type) {
                case QueueTypes.Torrent:
                    this.Settings.Torrents = value;
                    break;
                case QueueTypes.Metalink:
                    this.Settings.Metalinks = value;
                    break;
                case QueueTypes.Job:
                    this.Settings.Jobs = value;
                    break;
                case QueueTypes.All:
                    this.Settings.All = value;
                    break;
            }
        }

        [XmlIgnore]
        public QueueTypes Status
            =>
                this.ALL
                    ? QueueTypes.All
                    : (this.TORRENTS
                           ? QueueTypes.Torrent
                           : (this.METALINKS 
                           ? QueueTypes.Metalink 
                           : (this.JOBS ? QueueTypes.Job : QueueTypes.Disabled)));

        public override string ToString()
            =>
                string.Format((this.Arguments.Initialized ? "{0}=[{1}, " + nameof(Arguments) + "={2}]" : "{0}={1}"),
                              this.Name, this.Settings, this.Arguments);

        public string ToStringFull()
            => this.Arguments.Initialized
            ? $"{nameof(Toggles)}.{this.Name} {nameof(Settings)}={this.Settings}, {nameof(Arguments)}={this.Arguments}"
            : $"{nameof(Toggles)}.{this.Name}={this.Settings}";

        public QueueToggles() : this(null) { }
        public QueueToggles(string name) : this(name, QueueTypes.Disabled) { }

        public QueueToggles(string name, QueueTypes value)
            : this(name, value.IsAll(), value.IsTorrent(), value.IsMetalink(), value.IsJob()) {}

        public QueueToggles(string name, int value) : this(name, value == 1, value == 1, value == 1, value == 1) { }

        public QueueToggles(string name, bool all, bool torrents, bool metalinks, bool jobs)
        {            
            this.Settings = new QueueToggle(name, nameof(Settings), all, torrents, metalinks, jobs);
            this.Arguments = new QueueToggle(name, nameof(Arguments));
        }

        public void Load(QueueToggles other)
        {
            if (other != null)
            {
                Settings.Load(other.Settings);
            }
        }

    }
}
