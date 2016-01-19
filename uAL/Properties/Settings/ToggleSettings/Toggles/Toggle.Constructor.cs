// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using static Toggles;
    using static Toggles.PROCESS_QUEUE;
    using static Toggles.FILTERS;    
    public partial class Toggle
    {
        #region Constructor
        public Toggle(QueueToggleStatus type) { 
        	Type = type; 
        	PROCESS_QUEUE = new ProcessQueueToggle(type);
        	FILTERS = new FilterToggle(type);
        }
        #endregion
    }
}
