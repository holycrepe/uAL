using System;

namespace Torrent.Helpers.Utils
{
    using Extensions;
    public static class DateUtils
    {
        public static readonly DateTime StartOfEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime FromUnix(long secsFromEpoch)
            => StartOfEpoch.AddSeconds(secsFromEpoch);
        public static long ToUnix(DateTime date)
            => (long)StartOfEpoch.Subtract(date).TotalSeconds;
        public static string Timestamp
            => GetTimestamp(" ");
        public static string GetTimestamp(string padChar="0", bool pad=true)
            => DateTime.Now.FormatTimestamp(pad, padChar);
    }
}
