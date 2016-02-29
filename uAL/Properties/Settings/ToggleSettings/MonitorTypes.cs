using System;
using System.ComponentModel;

namespace uAL.Properties.Settings.ToggleSettings
{
    [Flags]
    public enum MonitorTypes
    {
        [Browsable(false)]
        Disabled = 0,
        [Description("All Monitors")]
        All = -1,
        [Description("Torrent File")]
        Torrent = 1,
        [Description("Metalink File")]
        Metalink = 1 << 1,
        [Description("uTorrent Job")]
        Job = 1 << 2,
        //[Description("Torrent, Metalink, and uTorrent Job")]
        //[Browsable(false)]
        //AllMonitors = Torrent | Metalink | Job,
        [Description("Torrent & Metalink Files")]
        [Browsable(false)]
        Main = Torrent | Metalink
    }
}
