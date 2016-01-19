using System;
using Torrent.Infrastructure;
// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using Torrent.Properties.Settings;

    #region Base
    public partial class ToggleSettingsBase 
    {
        #region Implementation	
        //public override string Name => "Toggles";
        protected override object[] AllSettings => new object[] {  ProcessQueue ,  Filters ,  PreviewMode ,  Monitor ,  InitializeMonitor ,  Watcher ,  QueueFilesOnStartup ,  PreProcess ,  MoveProcessedFiles ,  AutoExpandGroups  };
        public ToggleProcessQueueSettings ProcessQueue { get; set; }	    	
        public ToggleFilterSettings Filters { get; set; }	    	
        public QueueToggles AutoExpandGroups { get; set; }
        public QueueToggles PreviewMode { get; set; }
        public QueueToggles Monitor { get; set; }
        public QueueToggles InitializeMonitor { get; set; }
        public QueueToggles Watcher { get; set; }
        public QueueToggles QueueFilesOnStartup { get; set; }
        public QueueToggles PreProcess { get; set; }	        
        public QueueToggles MoveProcessedFiles { get; set; }
        public static Toggle GetActive(QueueToggleStatus type) => new Toggle(type);        
        public override string ToString() => $"[{Name} [{string.Join(", ", AllSettings)}]";
        #endregion        
    }
    #endregion	    
}
