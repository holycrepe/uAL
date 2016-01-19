using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Torrent.Extensions
{
    using System.Diagnostics.CodeAnalysis;
    using Helpers.StringHelpers;
    
    public static partial class Extensions
    {
        static readonly char[] trimChars = new char[] { ' ', '_', '-' };
        public static string UnescapeHTML(this string subject)
            => subject.Replace("&#39;", "'").Replace("&quot;", "'").Replace("&rsquo;", "'").Replace("&amp;", "&").Replace("&ndash;", "-");
        public static string Capitalize(this string subject)
            => subject.Substring(0, 1).ToUpper() + subject.Substring(1);
        public static string ToSnakeCase(this string camelCaseStr) => string.Concat(camelCaseStr.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();

        public static string Pad(this string subject, PadStringOptions options)
            =>
                options.DoPadRight
                    ? subject.PadCenter(options.Width, options.PaddingChar)
                : options.DoPadRight
                    ? subject.PadRight(options.Width, options.PaddingChar)
                    : subject.PadLeft(options.Width, options.PaddingChar);

        public static string PadTitle(this string title, object text)
            => new TitlePadder(title, text);

        public static string PadTitle(this string title, object text, TitlePadder options)
            => options.PadTitle(title, text);
        //public static string PadTitle(this string title, object text = null, int length = DEFAULT_PAD_TITLE_LENGTH, string pad = DEFAULT_PAD_TITLE_TITLE_SEPARATOR, string sep = DEFAULT_PAD_TITLE_TEXT_SEPARATOR, bool padRight = true)
        //    => text == null ? title : (title + (title == "" ? " " : pad)).Pad(length, padRight) + sep + text;
        //public static string PadTitle(this string title, object text, bool padRight, int length = DEFAULT_PAD_TITLE_LENGTH, string pad = DEFAULT_PAD_TITLE_TITLE_SEPARATOR, string sep = DEFAULT_PAD_TITLE_TEXT_SEPARATOR)
        //    => PadTitle(title, text, length, pad, sep, padRight);
        //public static string PadTitle(this string title, object text, int length, bool padRight, string pad = DEFAULT_PAD_TITLE_TITLE_SEPARATOR, string sep = DEFAULT_PAD_TITLE_TEXT_SEPARATOR)
        //    => PadTitle(title, text, length, pad, sep, padRight);
        //public static string PadTitleRight(this string title, object text = null, int length = DEFAULT_PAD_TITLE_LENGTH, string pad = DEFAULT_PAD_TITLE_TITLE_SEPARATOR, string sep = DEFAULT_PAD_TITLE_TEXT_SEPARATOR) 
        //    => PadTitle(title, text, length, pad, sep);

        //public static string PadTitleLeft(this string title, object text = null, int length = DEFAULT_PAD_TITLE_LENGTH, string pad = DEFAULT_PAD_TITLE_TITLE_SEPARATOR, string sep = DEFAULT_PAD_TITLE_TEXT_SEPARATOR) 
        //    => PadTitle(title, text, length, pad, sep, false);

        public static string PadCenter(this string text, int length, char chr = ' ', bool padLeftIfUneven = true)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));
            var diff = Math.Max(length - text.Length, 0);
            var left = diff / 2;
            var right = left;
            if (left + right < diff)
            {
                if (padLeftIfUneven)
                {
                    left++;
                }
                else
                {
                    right++;
                }
            }
            return new string(chr, left) + text + new string(chr, right);
        }

        public static string TrimAll(this string subject)
            => subject.Trim(trimChars);

        public static string Format(this string template, Dictionary<string, string> formats)
        {
            var values = new List<string>();
            foreach (var kvp in formats)
            {
                var i = values.Count;
                template = template.Replace("{" + kvp.Key + "}", "{" + i + "}").Replace("{" + kvp.Key + ",", "{" + i + ",");
                values.Add(kvp.Value);
            }
            return string.Format(template, args: values.ToArray());
        }
        public static T[] Select<T>(this string pattern, string subject, Func<Match, T> selector)
            => pattern.Regex(subject).Select(selector);

        public static string[] Select(this string pattern, string subject)
            => pattern.Select(subject, x => x.Value);

        public static Regex Regex(this string pattern)
            => new Regex(pattern, RegexOptions.IgnoreCase);

        public static MatchCollection Regex(this string pattern, string subject)
            => pattern.Regex().Matches(subject);

        public static bool ContainedIn(this string searchElement, IEnumerable<string> list)
            => list.Any(searchElement.Contains);

        public static bool StartsWith(this string searchElement, IEnumerable<string> list)
            => list.Any(item => searchElement.StartsWith(item, StringComparison.CurrentCulture));
    }
}
