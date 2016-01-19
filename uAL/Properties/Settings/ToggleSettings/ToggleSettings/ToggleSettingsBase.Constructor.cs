using System;
using Torrent.Infrastructure;
// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using System.Xml.Serialization;
    using Torrent.Properties.Settings;

    #region Base
    [Serializable]
    [XmlInclude(typeof(ToggleFilterSettings)), XmlInclude(typeof(ToggleProcessQueueSettings))]
    public partial class ToggleSettingsBase : BaseSettings
    {
        #region Overrides
        #region Overrides: Constructor
        public ToggleSettingsBase() {
            ProcessQueue = new ToggleProcessQueueSettings(nameof(ProcessQueue));				
            ProcessQueue.PropertyChanged += (s, e) => OnPropertyChanged(nameof(ProcessQueue));
            Filters = new ToggleFilterSettings(nameof(Filters));
            Filters.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Filters));
            AutoExpandGroups = new QueueToggles(nameof(AutoExpandGroups), 1);
            AutoExpandGroups.PropertyChanged += (s, e) => OnPropertyChanged(nameof(AutoExpandGroups));
            PreviewMode = new QueueToggles(nameof(PreviewMode), QueueToggleStatus.All);
            PreviewMode.PropertyChanged += (s, e) => OnPropertyChanged(nameof(PreviewMode));
            Monitor = new QueueToggles(nameof(Monitor));
            Monitor.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Monitor));
            InitializeMonitor = new QueueToggles(nameof(InitializeMonitor));
            InitializeMonitor.PropertyChanged += (s, e) => OnPropertyChanged(nameof(InitializeMonitor));
            Watcher = new QueueToggles(nameof(Watcher), 1);
            Watcher.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Watcher));
            QueueFilesOnStartup = new QueueToggles(nameof(QueueFilesOnStartup), 1);
            QueueFilesOnStartup.PropertyChanged += (s, e) => OnPropertyChanged(nameof(QueueFilesOnStartup));
            PreProcess = new QueueToggles(nameof(PreProcess), QueueToggleStatus.Metalink);
            PreProcess.PropertyChanged += (s, e) => OnPropertyChanged(nameof(PreProcess));
            MoveProcessedFiles = new QueueToggles(nameof(MoveProcessedFiles), 1);
            MoveProcessedFiles.PropertyChanged += (s, e) => OnPropertyChanged(nameof(MoveProcessedFiles));
        }
        #endregion
        #region Overrides: Save/Load
        public static ToggleSettingsBase Load()
            => Load<ToggleSettingsBase>();
        #endregion
        #endregion  
    }
    #endregion	    
}
