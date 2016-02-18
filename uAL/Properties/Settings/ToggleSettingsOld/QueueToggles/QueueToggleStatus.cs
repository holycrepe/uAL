using System;
using System.ComponentModel;

namespace uAL.Properties.Settings.ToggleSettings
{
    [Flags]
    public enum QueueTypes
    {
        [Description("No Queue Types are Enabled")]
        Disabled = 0,
        [Description("Downloaded Torrent File")]
        Torrent = 1,
        [Description("Downloaded Metalink File")]
        Metalink = 1 << 1,
        [Description("uTorrent Job")]
        Job = 1 << 2,
        [Description("All Queue Types: Torrent, Metalink, and uTorrent Job")]
        All = -1, //Torrent | Metalink | Job,
        [Description("Main Queue Types: Torrents & Metalinks")]
        Main = Torrent | Metalink
    }

    public static class Extensions
    {
        public static bool IsAll(this QueueTypes value) 
            => value == QueueTypes.All;
        public static bool IsTorrent(this QueueTypes value)
            => value == QueueTypes.Torrent;
        public static bool IsMetalink(this QueueTypes value) 
            => value == QueueTypes.Metalink;
        public static bool IsJob(this QueueTypes value)
            => value == QueueTypes.Job;

        public static bool IncludesAll(this QueueTypes value)
            => value.IsAll() || (value.IsTorrent() && value.IsMetalink() && value.IsJob());

        public static bool IncludesTorrent(this QueueTypes value) 
            => value.IsAll() || value.IsTorrent();

        public static bool IncludesMetalink(this QueueTypes value)
            => value.IsAll() || value.IsMetalink();

        public static bool IncludesJob(this QueueTypes value)
            => value.IsAll() || value.IsJob();
    }
}
