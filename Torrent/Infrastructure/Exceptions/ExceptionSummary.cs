using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Torrent.Extensions;

namespace Torrent.Infrastructure.Exceptions
{
    using Puchalapalli.Infrastructure.Interfaces;

    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class ExceptionSummary : IDebuggerDisplay
    {
        public Exception Error { get; set; }
        public string Title { get; set; }
        string[] Details { get; }
        string Prefix { get; }
        int Padding { get; }
        int HeadingPadding { get; }
        char Separator { get; }
        string PrefixLine { get; }
        string SeparatorLine { get; }
        string PaddingLine { get; }

        public ExceptionSummary(Exception ex, string prefix = "# ", char sep = '-', int padding = -1, int headingPadding = -1)
        {
            if (padding == -1)
                padding = 11;
            if (headingPadding == -1)
                headingPadding = 15;
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
            Details = exceptionStrings.Length > 1 
                ? exceptionStrings[1].Split('\n') 
                : new string[] {};
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
