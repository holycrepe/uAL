// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using PostSharp.Patterns.Model;
    using System.Runtime.Serialization;
    using Serialization;
    using MonitorSettings;
    [DataContract(Name = "Toggles", Namespace = Namespaces.Default)]
    [KnownType(typeof(ToggleSettingsProcessing)),
        KnownType(typeof(ToggleSettingsFilters)),
        KnownType(typeof(QueueToggles))]
    [NotifyPropertyChanged]
    public partial class ToggleSettingsBase
    {

        //public override string Name => "Toggles";
        [IgnoreAutoChangeNotification]
        protected override object[] DebuggerDisplayProperties
            =>
                new object[]
                {
                    Processing,
                    Filters,
                    Monitor,
                    InitializeMonitor,
                    Watcher,
                    QueueFilesOnStartup,
                    AutoExpandGroups
                };

        [DataMember]
        public ToggleSettingsProcessing Processing { get; set; }
        [DataMember]
        public ToggleSettingsFilters Filters { get; set; }
        [DataMember]
        public QueueToggles AutoExpandGroups { get; set; }
        [DataMember]
        public QueueToggles Monitor { get; set; }
        [DataMember]
        public QueueToggles InitializeMonitor { get; set; }
        [DataMember]
        public QueueToggles Watcher { get; set; }
        public MonitorTypes WatcherQT { get; set; } = MonitorTypes.Torrent;
        [DataMember]
        public QueueToggles QueueFilesOnStartup { get; set; }
        public static Toggle GetActive(QueueTypes type) => new Toggle(type);
        public override string DebuggerDisplaySimple(int level = 1)
            => $"[{this.Name} [{string.Join(", ", this.DebuggerDisplayProperties)}]";
        public void Load(ToggleSettingsBase other)
        {
            if (other != null)
            {
                this.Processing.Load(other.Processing);
                this.Filters.Load(other.Filters);
                this.AutoExpandGroups.Load(other.AutoExpandGroups);
                this.Monitor.Load(other.Monitor);
                this.InitializeMonitor.Load(other.InitializeMonitor);
                this.Watcher.Load(other.Watcher);
                this.QueueFilesOnStartup.Load(other.QueueFilesOnStartup);
            }
        }
    }
}
