// ReSharper disable InconsistentNaming

using System.Diagnostics;

namespace uAL.Properties.Settings.ToggleSettings
{
    using PostSharp.Patterns.Model;
    using System.Runtime.Serialization;
    using Serialization;
    using static MonitorTypes;
    [DataContract(Name = "Toggles", Namespace = Namespaces.Default)]
    [KnownType(typeof(ToggleSettingsProcessing)),
        KnownType(typeof(ToggleSettingsFilters)),
        KnownType(typeof(MonitorTypes))]
    [NotifyPropertyChanged]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public partial class ToggleSettingsBase
    {

        //public override string Name => "Toggles";
        [IgnoreAutoChangeNotification]
        protected override object[] DebuggerDisplayProperties
            =>
                new object[]
                {
                    nameof(Filters),
                    Filters,
                    nameof(Processing),
                    Processing,
                    nameof(Monitor),
                    Monitor,
                    nameof(InitializeMonitor),
                    InitializeMonitor,
                    nameof(Watcher),
                    Watcher,
                    nameof(AutoExpandGroups),
                    AutoExpandGroups,
                    nameof(QueueFilesOnStartup),
                    QueueFilesOnStartup,
                };

        [DataMember]
        public ToggleSettingsProcessing Processing { get; set; }
        [DataMember]
        public ToggleSettingsFilters Filters { get; set; }
        [DataMember]
        public MonitorTypes AutoExpandGroups { get; set; }
        [DataMember]
        public MonitorTypes Monitor { get; set; }
        [DataMember]
        public MonitorTypes InitializeMonitor { get; set; }
        [DataMember]
        public MonitorTypes Watcher { get; set; }
        [DataMember]
        public MonitorTypes QueueFilesOnStartup { get; set; }
        public Toggle GetActiveToggles(MonitorTypes type) => new Toggle(this, type);
        public override string DebuggerDisplaySimple(int level = 1)
            => $"{this.Name} {base.DebuggerDisplaySimple(level)}";
        public void Load(ToggleSettingsBase other)
        {
            if (other != null)
            {
                this.Processing.Load(other.Processing);
                this.Filters.Load(other.Filters);
                this.AutoExpandGroups = other.AutoExpandGroups;
                this.Monitor = other.Monitor;
                this.InitializeMonitor = other.InitializeMonitor;
                this.Watcher = other.Watcher;
                this.QueueFilesOnStartup = other.QueueFilesOnStartup;
            }
        }
    }
}
