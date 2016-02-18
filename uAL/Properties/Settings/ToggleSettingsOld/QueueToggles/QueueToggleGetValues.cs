using PostSharp.Patterns.Model;
using System.Collections.Generic;

namespace uAL.Properties.Settings.ToggleSettings
{
    public partial class QueueToggle
    {
        [Pure]
        public static string[] GetValues(bool all, bool torrents, bool metalinks, bool jobs)
        {
            var values = new List<string>();
            if (all) {
                values.Add(nameof(All));
            }
            if (torrents) {
                values.Add(nameof(Torrents));
            }
            if (metalinks)
            {
                values.Add(nameof(Metalinks));
            }
            if (jobs)
            {
                values.Add(nameof(Jobs));
            }
            if (values.Count == 0) {
                values.Add("Off");
            }
            return values.ToArray();
        }

        [Pure]
        public static string GetValue(bool all, bool torrents, bool metalinks, bool jobs)
        {
            if (all && torrents && metalinks && jobs) {
                return "*All";
            }
            if (torrents && metalinks && jobs) {
                return "+All";
            }
            return all
                ? "All"
                : torrents & metalinks 
                ? "T+M" 
                : torrents
                ? "Torrent"
                : metalinks 
                ? "Metalink" 
                : jobs
                ? "Job"
                : "Off";
        }
    }
}
