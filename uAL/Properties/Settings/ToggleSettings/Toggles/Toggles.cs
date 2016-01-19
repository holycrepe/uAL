


namespace uAL.Properties.Settings.ToggleSettings
{
    using static ToggleSettings;
    public static partial class Toggles {
		public static QueueToggles AutoExpandGroups => ToggleSetting.AutoExpandGroups;
        public static QueueToggles PreviewMode => ToggleSetting.PreviewMode;
        public static QueueToggles Monitor => ToggleSetting.Monitor;
        public static QueueToggles InitializeMonitor => ToggleSetting.InitializeMonitor;
        public static QueueToggles Watcher => ToggleSetting.Watcher;
        public static QueueToggles QueueFilesOnStartup => ToggleSetting.QueueFilesOnStartup;
        public static QueueToggles PreProcess => ToggleSetting.PreProcess;
        public static QueueToggles MoveProcessedFiles => ToggleSetting.MoveProcessedFiles;

        public static Toggle GetActiveToggle(QueueToggleStatus type)
            => new Toggle(type);
        public static void Save() => ToggleSetting.Save();
    }    
}