// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using static Toggles.PROCESSING;

    public partial class Toggle
    {
        public partial class ProcessingToggle
        {
            QueueTypes Type;
            bool getValue(QueueToggles toggle) => toggle.GetValue(Type);
            public ProcessingAutomatedToggle AUTOMATED { get; }
            public bool ENABLED => getValue(Enabled);
            public bool PREVIEW_MODE => getValue(PreviewMode);
            public bool CHECK_DUPES => getValue(CheckDupes);
            public bool PRE_PROCESS => getValue(PreProcess);
            public bool MOVE_PROCESSED_FILES => getValue(MoveProcessedFiles);
            public bool STOP_COMPLETED_TORRENTS => getValue(StopCompletedTorrents);
            public ProcessingToggle(QueueTypes type) {
                Type = type;
                AUTOMATED = new ProcessingAutomatedToggle(Type);
            }
        }
    }
}
