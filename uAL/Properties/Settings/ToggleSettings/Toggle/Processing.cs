// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using static ToggleSettingsBase;

    public partial class Toggle
    {
        public partial class ProcessingToggle
        {
            private ToggleSettingsProcessing Instance;
            MonitorTypes Type;
            bool getValue(MonitorTypes toggle) => toggle.Includes(Type);
            public ProcessingAutomatedToggle Automated { get; }
            public bool Enabled
                => getValue(this.Instance.Enabled);
            public bool PreviewMode
                => getValue(this.Instance.PreviewMode);
            public bool CheckDupes
                => getValue(this.Instance.CheckDupes);
            public bool PreProcess
                => getValue(this.Instance.PreProcess);
            public bool MoveProcessedFiles
                => getValue(this.Instance.MoveProcessedFiles);
            public bool StopCompletedTorrents
                => getValue(this.Instance.StopCompletedTorrents);
            public ProcessingToggle(ToggleSettingsProcessing toggles, MonitorTypes type)
            {
                Instance = toggles;
                Type = type;
                this.Automated = new ProcessingAutomatedToggle(toggles.Automated, Type);
            }
        }
    }
}
