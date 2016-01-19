using System;

namespace Torrent.Extensions
{
    public static class TimeSpanExtensions
    {    	
    	public static string FormatFriendly(this TimeSpan timeSpan) {
    		return timeSpan.Days.AsCount("Day", ", ") + timeSpan.Hours.AsCount("Hour", ", ", displayIfZero: timeSpan.TotalHours) + timeSpan.Minutes.AsCount("Min", ", ", displayIfZero: timeSpan.TotalMinutes) + timeSpan.Seconds + " s";
    	}
    }
}
