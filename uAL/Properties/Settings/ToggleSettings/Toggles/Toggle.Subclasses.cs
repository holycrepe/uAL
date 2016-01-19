// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using static Toggles;
    using static Toggles.PROCESS_QUEUE;
    using static Toggles.FILTERS;    
    public partial class Toggle
    {
        #region Subclasses
        #region Subclasses: ProcessQueueToggle
        public class ProcessQueueToggle
        {
            QueueToggleStatus Type;
            bool getValue(QueueToggles toggle) => toggle.GetValue(Type);
            public bool ALL => getValue(All);
            public bool MANUAL => getValue(Manual);
            public bool STARTUP => getValue(Startup);
            public bool ON_WATCHER => getValue(OnWatcher);
            public ProcessQueueToggle(QueueToggleStatus type) { Type = type; }
        }
        #endregion
        #region Subclasses: FilterToggle
        public class FilterToggle
        {
            QueueToggleStatus Type;
            bool getValue(QueueToggles toggle) => toggle.GetValue(Type);
            public bool GLOBAL => getValue(Global);
            public bool INCLUDE => getValue(Include);
            public bool EXCLUDE => getValue(Exclude);
            public FilterToggle(QueueToggleStatus type) { Type = type; }
        }
        #endregion
        #endregion
    }
}
