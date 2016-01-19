using System;
using System.ComponentModel;
using System.Configuration;
using System.Xml.Serialization;

namespace uAL.Properties.Settings.ToggleSettings
{
    using Torrent.Infrastructure;

    [Serializable]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class QueueToggles : NotifyPropertyChangedBase
    {
        public QueueToggle Settings { get; set; }        
        [XmlIgnore]
        public QueueToggle Arguments { get; set; }
        
        public string Name => Settings.Name;
        public bool ALL => Settings.Status.IncludesAll() || Arguments.Status.IncludesAll();
        public bool METALINKS => Settings.Status.IncludesMetalink() || Arguments.Status.IncludesMetalink();
        public bool TORRENTS => Settings.Status.IncludesTorrent() || Arguments.Status.IncludesTorrent();

        public string[] Values() => QueueToggle.GetValues(ALL, TORRENTS, METALINKS);
        public bool GetValue(QueueToggleStatus type)
        {
            switch (type)
            {
                case QueueToggleStatus.Torrent:
                    return TORRENTS;
                case QueueToggleStatus.Metalink:
                    return METALINKS;
                default:
                    return ALL;
            }
        }
        public void SetValue(QueueToggleStatus type, bool value)
        {
            switch (type)
            {
                case QueueToggleStatus.Torrent:
                    Settings.Torrents = value;
                    break;
                case QueueToggleStatus.Metalink:
                    Settings.Metalinks = value;
                    break;
                case QueueToggleStatus.All:
                    Settings.All = value;
                    break;
            }
        }
        [XmlIgnore]
        public QueueToggleStatus Status => ALL ? QueueToggleStatus.All : (TORRENTS ? QueueToggleStatus.Torrent : (METALINKS ? QueueToggleStatus.Metalink : QueueToggleStatus.Disabled));
        public override string ToString() => string.Format((Arguments.Initialized ? "{0}=[{1}, " + nameof(Arguments) + "={2}]" : "{0}={1}"), Name, Settings, Arguments);

        public string ToStringFull() 
            => string.Format(Arguments.Initialized ? 
                "Toggles.{0} " + nameof(Settings) + "={1}, " + nameof(Arguments) + "={2}" : 
                "Toggles.{0}={1}", Name, Settings, Arguments);

        public QueueToggles() : this(null) { }
        public QueueToggles(string name) : this(name, QueueToggleStatus.Disabled) { }
        public QueueToggles(string name, QueueToggleStatus value) : this(name, value.IsAll(), value.IsTorrent(), value.IsMetalink()) { }
        public QueueToggles(string name, int value) : this(name, value == 1, value == 1, value == 1) { }
        public QueueToggles(string name, bool all, bool torrents, bool metalinks)
        {        	
        	Settings = new QueueToggle(name, nameof(Settings), all, torrents, metalinks);
            Arguments = new QueueToggle(name, nameof(Arguments));
            Settings.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Settings), e.PropertyName == "All" ? "" : "ALL");
            Arguments.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Arguments), e.PropertyName == "All" ? "" : "ALL");
        }
    }
}
