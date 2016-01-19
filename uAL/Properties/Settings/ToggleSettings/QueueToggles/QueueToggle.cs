using System;
using System.ComponentModel;
using System.Configuration;
using System.Xml.Serialization;

namespace uAL.Properties.Settings.ToggleSettings
{
    using Torrent.Infrastructure;
    [Serializable]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public partial class QueueToggle : NotifyPropertyChangedBase
    {
        bool _all; bool _metalinks; bool _torrents;
        [XmlIgnore]
        public bool Initialized { get; set; }
        public string Name { get; set; }
        [XmlIgnore]
        public string Type { get; set; }
        public bool All { get { return _all; } set { if (value != _all) { _all = value; OnPropertyChanged(nameof(All)); } } }
        public bool Metalinks { get { return _metalinks; } set { if (value != _metalinks) { _metalinks = value; OnPropertyChanged(nameof(Metalinks)); } } }
        public bool Torrents { get { return _torrents; } set { if (value != _torrents) { _torrents = value; OnPropertyChanged(nameof(Torrents)); } } }
        public QueueToggle() : this(null, null) { }
        public QueueToggle(string name, string type) : this(name, type, QueueToggleStatus.Disabled) { }
        public QueueToggle(string name, string type, QueueToggleStatus value) : this(name, type, value.IsAll(), value.IsTorrent(), value.IsMetalink()) { }
        public QueueToggle(string name, string type, int value) : this(name, type, value == 1, value == 1, value == 1) { }
        public QueueToggle(string name, string type, bool all, bool torrents, bool metalinks) {
        	Name = name ?? "Name";
            Type = type ?? "Settings";
        	Set(all, torrents, metalinks, false);
            Initialized = false;
        }
        public void Set(QueueToggleStatus status, bool initialized = true) {
        	Set(status.IsAll(), status.IsTorrent(), status.IsMetalink(), initialized);
        }
        public void Set(bool all, bool torrents, bool metalinks, bool initialized = true)
        {        	
        	All = all;
            Torrents = torrents;
            Metalinks = metalinks;
            Initialized = initialized;
        }
        
        public QueueToggleStatus Status => All ? QueueToggleStatus.All : (Torrents ? QueueToggleStatus.Torrent : (Metalinks ? QueueToggleStatus.Metalink : QueueToggleStatus.Disabled));
        public string[] Values => GetValues(All, Torrents, Metalinks);
        public string Value => GetValue(All, Torrents, Metalinks);
        public override string ToString() => Value;
    }
}
