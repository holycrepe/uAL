// ReSharper disable InconsistentNaming

using PostSharp.Patterns.Model;

namespace uAL.Properties.Settings.ToggleSettings
{
    using static MonitorTypes;
    using static ToggleSettingsBase;

    public partial class Toggle
    {
        public partial class ProcessingToggle
        {
            public class ProcessingAutomatedToggle
            {
                ToggleSettingsProcessingAutomated Instance;
                MonitorTypes Type;
                bool getValue(MonitorTypes toggle) => toggle.Includes(Type);
                public bool Manual => getValue(this.Instance.Manual);
                public bool Startup => getValue(this.Instance.Startup);
                public bool OnWatcher => getValue(this.Instance.OnWatcher);

                public ProcessingAutomatedToggle(ToggleSettingsProcessingAutomated toggles, MonitorTypes type)
                {
                    Instance = toggles;
                    Type = type;
                }
            }
        }
    }
}
