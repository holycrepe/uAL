using System;
using System.Diagnostics;

namespace Torrent.Extensions
{
    public static class DateTimeExtensions
    {
        public static string FormatFriendly(this DateTime date, bool pad = true, string padChar = " ")
            => date.ToShortDateString() + " " + date.FormatTimestamp(pad, padChar);
        public static string FormatTimestamp(this DateTime date, bool pad = true, string padChar ="0")
            => (date.Hour % 12 < 10 && pad ? padChar : " ") + date.ToLongTimeString();
    }
}