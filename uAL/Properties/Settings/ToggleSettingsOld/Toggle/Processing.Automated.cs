// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using static Toggles.PROCESSING.ENABLED;

    public partial class Toggle
    {
        public partial class ProcessingToggle
        {
            public class ProcessingAutomatedToggle {
                QueueTypes Type;
                bool getValue(QueueToggles toggle) => toggle.GetValue(Type);
                public bool MANUAL => getValue(Manual);
                public bool STARTUP => getValue(Startup);
                public bool ON_WATCHER => getValue(OnWatcher);
                public ProcessingAutomatedToggle(QueueTypes type) { Type = type; }
            }
        }
    }
}
