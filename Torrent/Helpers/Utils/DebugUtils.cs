namespace Torrent.Helpers.Utils
{
    using Infrastructure.InfoReporters;
    using System.Diagnostics;
    using System;
    using System.Collections;
    using Infrastructure;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;

    public static class DebugUtils
    {
        public static readonly DebuggerInfoReporter DEBUG = new DebuggerInfoReporter();

        public static string GetDebuggerDisplay(object item, int level = 1, string delim = "\n",
                                                string sep = "",
                                                int indent = LogUtils.DEFAULT_COLLECTION_INDENT,
                                                bool includeName = false,
                                                bool includeCount = true,
                                                bool includeIndex = true)
        {
            var debugger = item as IDebuggerDisplay;
            if (debugger != null)
                return debugger.DebuggerDisplaySimple(level);
            var dictionary = item as IDictionary;
            if (dictionary != null)
            {
                var dictTypes = item.GetType().GetGenericArguments();
                if (dictTypes.Length == 2 && dictTypes[1].IsInstanceOfType(typeof(IDebuggerDisplay)))
                {
                    return dictionary
                            .Cast<DictionaryEntry>()
                            .ToDictionary(entry => entry.Key,
                                          entry => entry.Value as IDebuggerDisplay)
                                          .GetDebuggerDisplaySimple(level, delim, sep, indent, includeName, includeCount, includeIndex);
                }
            }
            return (item as IEnumerable<IDebuggerDisplay>)?
                .GetDebuggerDisplaySimple(level, delim, sep, indent, includeName, includeCount, includeIndex)
                ?? item?.ToString();
        }

    }
}
