
namespace uAL.Properties.Settings.ToggleSettings
{
    using static ToggleSettings;
    using static ToggleSettingsBase;

    public static partial class Toggles
    {
        public static partial class PROCESSING
        {
            public static class ENABLED
            {
                static ToggleSettingsProcessingAutomated toggles => ToggleSetting.Processing.Automated;
                public static QueueToggles Manual => toggles.Manual;
                public static QueueToggles Startup => toggles.Startup;
                public static QueueToggles OnWatcher => toggles.OnWatcher;
            }
        }
    }
}
