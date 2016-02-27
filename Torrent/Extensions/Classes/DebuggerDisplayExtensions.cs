namespace Torrent.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Helpers.StringHelpers;
    using Infrastructure;
    using System;
    using Helpers.Utils;

    public static class DebuggerDisplayExtensions
    {
        public static string GetDebuggerDisplay<T>(this IEnumerable<T> list,
                                                   int level = 1,
                                                   string sep = "\n", string delim = "",
                                                   int indent = LogUtils.DEFAULT_COLLECTION_INDENT, 
                                                   char indentChar = ' ',
                                                   bool combineDelimAndSep = true, NumberPadder padder = null,
                                                   int linePadding = 0,
                                                   bool includeName=false,
                                                   bool includeCount=true,
                                                   bool includeIndex=true)
            where T : IDebuggerDisplay
            =>
                list.FormatList(level, sep, delim, indent, indentChar, combineDelimAndSep, padder,
                                (s) => s.DebuggerDisplaySimple(level + 1), linePadding, includeName, includeCount, includeIndex);

        public static string GetDebuggerDisplay<TKey, TValue>(this IDictionary<TKey, TValue> dict, int level = 1,
                                                              string sep = "\n", string delim = "",
                                                              int indent = LogUtils.DEFAULT_COLLECTION_INDENT, 
                                                              char indentChar = ' ',
                                                              bool combineDelimAndSep = true, NumberPadder padder = null,
                                                              bool includeName = true,
                                                              bool includeCount = true,
                                                              bool includeIndex = true)
            where TValue : IDebuggerDisplay
        {
            padder = padder ?? new NumberPadder();
            var titlePadder = new TitlePadder
                              {
                                  Width = dict.Keys.Max(key => key.ToString().Length) + 3
                              };
            var indentStr = indent == 0 ? string.Empty : new string(indentChar, level*indent);

            return (includeName ? $"{dict.GetType().Name}: " : "") +
                   (dict.Count == 0
                        ? "Empty"
                        : (includeCount ? $"<{dict.Count}>{sep}" : "") 
                        + string.Join(combineDelimAndSep ? delim + sep : delim,
                                      dict.Select((s, i) =>
                                                  indentStr 
                                                  + (includeIndex ? padder.PadIndex(i) : "")
                                                  + titlePadder.PadTitle($"`{s.Key}`",
                                                                         s.Value.DebuggerDisplaySimple(level + 1))
                                          )
                              ));
        }

        public static string GetDebuggerDisplaySimple<TKey, TValue>(this IDictionary<TKey, TValue> dict, int level = 1,
                                                                    string delim = ", ", string sep = "",
                                                                    int indent = LogUtils.DEFAULT_COLLECTION_INDENT,
                                                                    bool includeName = false,
                                                                    bool includeCount = true,
                                                                    bool includeIndex = true)
            where TValue : IDebuggerDisplay
            => dict.GetDebuggerDisplay(level, sep, delim, indent, includeName: includeName, includeCount: includeCount, includeIndex: includeIndex);

        public static string GetDebuggerDisplaySimple<T>(this IEnumerable<T> list, int level = 1, string delim = ", ",
                                                         string sep = "",
                                                         int indent = LogUtils.DEFAULT_COLLECTION_INDENT,
                                                         bool includeName = false,
                                                         bool includeCount = true,
                                                         bool includeIndex = true)
            where T : IDebuggerDisplay
            => list.GetDebuggerDisplay(level, sep, delim, indent, includeName: includeName, includeCount: includeCount, includeIndex: includeIndex);
    }
}
