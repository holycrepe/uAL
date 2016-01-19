// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using static Toggles;
    using static Toggles.PROCESS_QUEUE;
    using static Toggles.FILTERS;    
    public partial class Toggle
    {
        #region Implementation
        public QueueToggleStatus Type;
        bool getValue(QueueToggles toggle) => toggle.GetValue(Type);
        public ProcessQueueToggle PROCESS_QUEUE;
        public FilterToggle FILTERS;
        public bool AUTO_EXPAND_GROUPS => getValue(AutoExpandGroups);
        public bool PREVIEW_MODE => getValue(PreviewMode);
        public bool MONITOR => getValue(Monitor);
        public bool INITIALIZE_MONITOR => getValue(InitializeMonitor);
        public bool WATCHER => getValue(Watcher);
        public bool QUEUE_FILES_ON_STARTUP => getValue(QueueFilesOnStartup);
        public bool PRE_PROCESS => getValue(PreProcess);
        public bool MOVE_PROCESSED_FILES => getValue(MoveProcessedFiles);
        #endregion

    }
}
