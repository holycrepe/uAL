namespace uAL.Properties.Settings.ToggleSettings
{
    using Torrent.Extensions;

    public static class MonitorTypeExtensions
    {
        public static bool IsAll(this MonitorTypes value) 
            => value == MonitorTypes.All;
        public static bool IsTorrent(this MonitorTypes value)
            => value == MonitorTypes.Torrent;
        public static bool IsMetalink(this MonitorTypes value) 
            => value == MonitorTypes.Metalink;
        public static bool IsJob(this MonitorTypes value)
            => value == MonitorTypes.Job;

        public static bool IncludesAll(this MonitorTypes value)
            => value.IsAll(); // || value.Has(AllMonitors);
        public static bool Includes(this MonitorTypes value, MonitorTypes flag)
            => value.IncludesAll() || value.Has(flag);
        public static bool IncludesTorrent(this MonitorTypes value)
            => value.Includes(MonitorTypes.Torrent);
        public static bool IncludesMetalink(this MonitorTypes value)
            => value.Includes(MonitorTypes.Metalink);
        public static bool IncludesJob(this MonitorTypes value)
            => value.Includes(MonitorTypes.Job);
    }
}