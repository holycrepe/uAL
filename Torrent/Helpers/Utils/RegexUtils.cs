using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Torrent.Extensions;

namespace Torrent.Helpers.Utils
{
    public static partial class RegexUtils
    {
        static string dateRegexPattern = null;

        public static string ReplaceAll(string subject, string pattern, string replacement = "")
        {
            var newSubject = subject;
            while (true) {
                newSubject = Regex.Replace(subject, pattern, replacement, RegexOptions.IgnoreCase);
                if (subject == newSubject) {
                    return subject;
                }
                subject = newSubject;
            }
        }

        public static string CreateBoundary(string pattern)
        {
            return string.Format(@"{0}(?:{2}){1}", WORD_BOUNDARY_START, WORD_BOUNDARY_END, pattern);
        }

        static string createGroup(IEnumerable<string> patterns) { return $@"(?:{string.Join("|", patterns)})"; }
        public static string CreateGroup(StringCollection patterns) { return createGroup(patterns.Cast<string>()); }
        public static string CreateGroup(IEnumerable<string> patterns) { return createGroup(patterns); }
        public static string CreateGroup(params string[] patterns) { return createGroup(patterns); }

        public static string[] GetDates(string subject)
        {
            if (dateRegexPattern == null) {
                var Formats = new Dictionary<string, string>
                              {
                                  {"Month", "(0[1-9]|1[012])"},
                                  {"Day", "(0[1-9]|[12][0-9]|3[01])"},
                                  {"Year", "(19|20)?[0-9]{2}"},
                                  {"Sep", "[- /.]"}
                              };

                dateRegexPattern =
                    @"({Month}{Sep}{Day}{Sep}{Year}|{Year}{Sep}{Month}{Sep}{Day}|\b{Year}\b)".Format(Formats);
            }
            return dateRegexPattern.Select(subject);
        }
    }
}
