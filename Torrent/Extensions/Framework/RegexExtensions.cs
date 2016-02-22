using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Torrent.Extensions
{
    public static partial class Extensions
    {
        public static string[] Values(this Regex obj, string subject) => obj.Select(subject, x => x.Value);

        public static T[] Select<T>(this Regex obj, string subject, Func<Match, T> selector) => obj.Matches(subject).Select(selector);

        public static string[] Values(this MatchCollection obj) => obj.Select(x => x.Value);

        public static T[] Select<T>(this MatchCollection obj, Func<Match, T> selector) => obj.Cast<Match>().Select(selector).ToArray();
    }
}
