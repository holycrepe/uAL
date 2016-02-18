using System;
using System.Diagnostics;

namespace Torrent.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string FormatFriendlyShort(this TimeSpan timeSpan)
            => FormatFriendly(timeSpan, "d", "h", "m", " ", "", "");

        public static string FormatFriendly(this TimeSpan timeSpan, string day="Day", string hour="Hour", string min="Min", string sep=", ", string prefix=" ", string pluralSuffix="s")
            => timeSpan.Days.AsCount(day, sep, prefix, pluralSuffix)
               + timeSpan.Hours.AsCount(hour, sep, prefix, pluralSuffix, timeSpan.TotalHours)
               + timeSpan.Minutes.AsCount(min, sep, prefix, pluralSuffix, timeSpan.TotalMinutes)
               + (timeSpan.Minutes == 1 ? prefix : "")
               + (timeSpan.TotalMinutes >= 1 && timeSpan.Seconds < 10 ? prefix : "")
               + timeSpan.Seconds + "s";
    }

    public static class StopwatchExtensions
    {
        public static string FormatFriendly(this Stopwatch stopwatch)
            => stopwatch.Elapsed.FormatFriendly();
    }
}
