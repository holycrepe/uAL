using System.Collections.Generic;

namespace uAL.Properties.Settings.ToggleSettings
{
    public partial class QueueToggle
    {
        public static string[] GetValues(bool all, bool torrents, bool metalinks)
        {
            var values = new List<string>();
            if (all)
            {
                values.Add("All");
            }
            if (torrents)
            {
                values.Add("Torrents");
            }
            if (metalinks)
            {
                values.Add("Metalinks");
            }
            if (values.Count == 0)
            {
                values.Add("Off");
            }
            return values.ToArray();
        }
        
        public static string GetValue(bool all, bool torrents, bool metalinks) 
        {
        	if (all && torrents && metalinks) {
    			return "*All";
    		}
    		if (torrents && metalinks) {
    			return "+All";
    		}
        	return (all ? "All" : 
        	        (torrents ? "Torrent" : 
        	         (metalinks ? "Metalink" : "Off")
        	        ));
        }

}
}
