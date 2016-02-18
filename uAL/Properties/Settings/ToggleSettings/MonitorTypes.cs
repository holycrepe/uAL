using System;
using System.ComponentModel;

namespace uAL.Properties.Settings.ToggleSettings
{
    using Torrent.Extensions;
    using static MonitorTypes;

    [Flags]
    public enum MonitorTypes
    {
        [Description("None")]
        Disabled = 0,
        [Description("All Monitors")]
        All = -1,
        [Description("Torrent File")]
        Torrent = 1,
        [Description("Metalink File")]
        Metalink = 1 << 1,
        [Description("uTorrent Job")]
        Job = 1 << 2,
        [Description("All Monitors: Torrent, Metalink, and uTorrent Job")]
        [Browsable(false)]
        AllMonitors = Torrent | Metalink | Job,
        [Description("Main Monitors: Torrent & Metalink")]
        [Browsable(false)]
        Main = Torrent | Metalink
    }

    public static class MonitorTypeExtensions
    {
        public static bool IsAll(this MonitorTypes value) 
            => value == All;
        public static bool IsTorrent(this MonitorTypes value)
            => value == MonitorTypes.Torrent;
        public static bool IsMetalink(this MonitorTypes value) 
            => value == Metalink;
        public static bool IsJob(this MonitorTypes value)
            => value == Job;

        public static bool IncludesAll(this MonitorTypes value)
            => value.IsAll() || value.Has(AllMonitors);
        public static bool Includes(this MonitorTypes value, MonitorTypes flag)
            => value.IncludesAll() || value.Has(flag);
        public static bool IncludesTorrent(this MonitorTypes value)
            => value.Includes(MonitorTypes.Torrent);
        public static bool IncludesMetalink(this MonitorTypes value)
            => value.Includes(Metalink);
        public static bool IncludesJob(this MonitorTypes value)
            => value.Includes(Job);
    }
}
