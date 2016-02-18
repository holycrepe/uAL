﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Torrent.Extensions;

namespace Torrent.Infrastructure.Exceptions
{
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class ExceptionSummary : IDebuggerDisplay
    {
        public Exception Error { get; set; }
        public string Title { get; set; }
        string[] Details { get; set; }
        string Prefix { get; set; }
        int Padding { get; set; }
        int HeadingPadding { get; set; }
        char Separator { get; set; }
        string PrefixLine { get; set; }
        string SeparatorLine { get; set; }
        string PaddingLine { get; set; }

        public ExceptionSummary(Exception ex, string prefix = "# ", char sep = '-', int padding = 11, int headingPadding = 15)
        {
            HeadingPadding = headingPadding;
            Separator = sep;
            Error = ex;
            Prefix = prefix;
            Padding = padding;
            PaddingLine = prefix[0] + new string(' ', padding - 1);
            PrefixLine = new string(prefix[0], padding);
            SeparatorLine = prefix[0] + new string(sep, padding - 1);
            var exceptionStrings = ex.ToString().Replace("\r\n", "\n").Replace("\r", "").Split(new char[] { '\n' }, 2, StringSplitOptions.RemoveEmptyEntries);
            Title = exceptionStrings[0];
            Details = exceptionStrings[1].Split('\n');
        }
        public IEnumerable<string> GetSummary()
        {
            yield return PrefixLine;
            yield return (Prefix + "Exception:" + "  ").PadRight(HeadingPadding) + Error.GetType().Name;
            yield return (Prefix + "Message:" + "  ").PadRight(HeadingPadding) + Error.Message;
            yield return (Prefix + "Source:" + "  ").PadRight(HeadingPadding) + Error.Source;
            yield return SeparatorLine;
            yield return Prefix + "Details: ";
            yield return SeparatorLine;
            yield return PaddingLine + Title;
            yield return SeparatorLine;
            yield return PaddingLine + "Trace: ";
            foreach (var line in Details)
            {
                yield return PaddingLine + line;
            }
            var exReflection = Error as ReflectionTypeLoadException;
            if (exReflection != null)
            {
                var i = 0;
                yield return Prefix + "Loader Exceptions:";
                foreach (var ex in exReflection.LoaderExceptions)
                {
                    yield return SeparatorLine;
                    yield return PaddingLine + (Prefix + $"Loader Exception #{++i}:" + "  ").PadRight(HeadingPadding) + ex.GetType().Name;
                    foreach (var line in ex.GetSummaryText())
                    {
                        yield return PaddingLine + line;
                    }
                }
                yield return SeparatorLine;
            }
            yield return PrefixLine;
        }
        #region Interfaces
        #region Interfaces: IDebuggerDisplay
        public override string ToString()
            => DebuggerDisplay();
        public string DebuggerDisplay(int level = 1)
            => DebuggerDisplaySimple(level);
        public string DebuggerDisplaySimple(int level = 1)
            => string.Join("\n", GetSummary());
        #endregion
        #endregion
    }
}