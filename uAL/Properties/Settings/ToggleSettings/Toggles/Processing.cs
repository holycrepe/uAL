
namespace uAL.Properties.Settings.ToggleSettings
{
    using static ToggleSettings;
    using static ToggleSettingsBase;

    public static partial class Toggles
    {
        public static partial class PROCESSING
        {
            static ToggleSettingsProcessing toggles => ToggleSetting.Processing;
            public static ToggleSettingsProcessingAutomated Automated => toggles.Automated;
            public static QueueToggles Enabled => toggles.Enabled;
            public static QueueToggles PreviewMode => toggles.PreviewMode;
            public static QueueToggles CheckDupes => toggles.CheckDupes;
            public static QueueToggles PreProcess => toggles.PreProcess;
            public static QueueToggles MoveProcessedFiles => toggles.MoveProcessedFiles;
            public static QueueToggles StopCompletedTorrents => toggles.StopCompletedTorrents;
        }
    }
}
