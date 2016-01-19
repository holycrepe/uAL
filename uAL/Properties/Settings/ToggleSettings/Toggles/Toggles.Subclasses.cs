using Torrent.Helpers.Utils;
using Torrent.Properties.Settings;


namespace uAL.Properties.Settings.ToggleSettings
{
    using static ToggleSettings;
    using static SerializationUtils;
    using static ToggleSettingsBase;
    public  static partial class Toggles {
    	public static class PROCESS_QUEUE {
            static ToggleProcessQueueSettings toggles => ToggleSetting.ProcessQueue;
    		public static QueueToggles All => toggles.All;
    	    public static QueueToggles Manual => toggles.Manual;
    	    public static QueueToggles Startup => toggles.Startup;
    	    public static QueueToggles OnWatcher => toggles.OnWatcher;
    	}    	
		public static class FILTERS {
            static ToggleFilterSettings toggles => ToggleSetting.Filters;
            public static QueueToggles Global => toggles.Global;
		    public static QueueToggles Include => toggles.Include;
		    public static QueueToggles Exclude => toggles.Exclude;
		}    	
    }    
}
