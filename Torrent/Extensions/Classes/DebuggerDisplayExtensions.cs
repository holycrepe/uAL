namespace Torrent.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Helpers.StringHelpers;
    using Infrastructure;
    using System;

    public static class DebuggerDisplayExtensions
    {
        public static string GetDebuggerDisplay<T>(this IEnumerable<T> list,
                                                   int level = 1,
                                                   string sep = "\n", string delim = "",
                                                   int indent = 4, char indentChar = ' ',
                                                   bool combineDelimAndSep = true, NumberPadder padder = null,
                                                   int linePadding = 0)
            where T : IDebuggerDisplay
            =>
                list.FormatList(level, sep, delim, indent, indentChar, combineDelimAndSep, padder,
                                (s) => s.DebuggerDisplaySimple(level + 1), linePadding);

        public static string GetDebuggerDisplay<TKey, TValue>(this IDictionary<TKey, TValue> dict, int level = 1,
                                                              string sep = "\n", string delim = "",
                                                              int indent = 4, char indentChar = ' ',
                                                              bool combineDelimAndSep = true, NumberPadder padder = null,
                                                              bool includeTypeName = true)
            where TValue : IDebuggerDisplay
        {
            padder = padder ?? new NumberPadder();
            var titlePadder = new TitlePadder
                              {
                                  Width = dict.Keys.Max(key => key.ToString().Length) + 3
                              };
            var indentStr = new string(indentChar, level*indent);

            return (includeTypeName ? $"{dict.GetType().Name}: " : "") +
                   (dict.Count == 0
                        ? "Empty"
                        : $"<{dict.Count}>{sep}" +
                          string.Join(combineDelimAndSep ? delim + sep : delim,
                                      dict.Select((s, i) =>
                                                  indentStr + padder.PadIndex(i)
                                                  + titlePadder.PadTitle($"`{s.Key}`",
                                                                         s.Value.DebuggerDisplaySimple(level + 1))
                                          )
                              ));
        }

        public static string GetDebuggerDisplaySimple<TKey, TValue>(this IDictionary<TKey, TValue> dict, int level = 1,
                                                                    string delim = ", ", string sep = "")
            where TValue : IDebuggerDisplay
            => dict.GetDebuggerDisplay(level, sep, delim);

        public static string GetDebuggerDisplaySimple<T>(this IEnumerable<T> list, int level = 1, string delim = ", ",
                                                         string sep = "")
            where T : IDebuggerDisplay
            => list.GetDebuggerDisplay(level, sep, delim);
    }
}
