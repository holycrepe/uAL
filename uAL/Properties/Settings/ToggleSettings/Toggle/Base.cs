// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{

    public partial class Toggle
    {
        public MonitorTypes Type;
        protected ToggleSettingsBase Instance;
        bool getValue(MonitorTypes toggle) => toggle.Includes(Type);
        public ProcessingToggle Processing { get; }
        public FilterToggle Filters { get; }
        public bool AutoExpandGroups
            => getValue(this.Instance.AutoExpandGroups);        
        public bool Monitor
            => getValue(this.Instance.Monitor);
        public bool InitializeMonitor
            => getValue(this.Instance.InitializeMonitor);
        public bool Watcher
            => getValue(this.Instance.Watcher);
        public bool QueueFilesOnStartup
            => getValue(this.Instance.QueueFilesOnStartup);        
    }
}
