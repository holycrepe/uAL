// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using static Toggles;

    public partial class Toggle
    {
        public QueueTypes Type;
        bool getValue(QueueToggles toggle) => toggle.GetValue(Type);
        public ProcessingToggle PROCESSING { get; }
        public FilterToggle FILTERS { get; }
        public bool AUTO_EXPAND_GROUPS => getValue(AutoExpandGroups);        
        public bool MONITOR => getValue(Monitor);
        public bool INITIALIZE_MONITOR => getValue(InitializeMonitor);
        public bool WATCHER => getValue(Watcher);
        public bool QUEUE_FILES_ON_STARTUP => getValue(QueueFilesOnStartup);        
    }
}
