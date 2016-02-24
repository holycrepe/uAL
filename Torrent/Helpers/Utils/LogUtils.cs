#define LOG_INCLUDE_TIMESTAMP_OFF

using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Torrent.Extensions;

namespace Torrent.Helpers.Utils
{
    using Enums;
    using StringHelpers;
    using static StringHelpers.PadStringOptions;

    public static class LogUtils
    {
        private const int LOG_CLASS_NAME_LENGTH = 25;
        private const int LOG_PAD_CLASS_NAME_LENGTH = 20;
#if LOG_INCLUDE_TIMESTAMP
        const int LOG_TIMESTAMP_LENGTH = 11;
#else
        private const int LOG_TIMESTAMP_LENGTH = -1;
#endif
        private const int LOG_PAD_TITLE_LENGTH = 50;
        public const int LOG_PREFIX_CLASS_NAME_LENGTH = LOG_TIMESTAMP_LENGTH + 1 + LOG_PAD_CLASS_NAME_LENGTH + 1;
        public const int LOG_PREFIX_TITLE_LENGTH = LOG_PREFIX_CLASS_NAME_LENGTH + 1 + LOG_PAD_TITLE_LENGTH;
        private static Random _rnd;

        public static bool AssertRandom(int random)
        {
            if (random == 0) {
                return true;
            }
            if (_rnd == null) {
                _rnd = new Random();
            }
            return _rnd.Next(random) + 1 == random;
        }

        public static class Writers
        {
            public static void Design(string title, string text = null)
                => WriteToFile(nameof(Design), title, text);
            public static void Types(string title, string text = null, [CallerMemberName] string source=null)
                => WriteToFile(nameof(Types), source.PadTitle(title), text);
        }

        public static void Write(string title, string text = null)
        => WriteToFile("default", title, text);
        static void WriteToFile(string file, string title, string text=null)
            => File.AppendAllText(Path.Combine(@"D:\Git\uAL\wUAL\bin\Debug\logs", file + ".log"), $"[{DateUtils.Timestamp}] {title.PadTitle(text)}\n");
        [Conditional("DEBUG"), Conditional("TRACE_EXT")]
        public static void Log(string className, string title, string text = null, string item = null,
                               PadDirection textPadDirection = PadDirection.Default, string textSuffix = null,
                               PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null,
                               int random = 0)
        {
#if DEBUG || TRACE_EXT
            if (!AssertRandom(random)) {
                return;
            }
            var finalText = title;
            if (text != null) {
                string paddedText = new TitlePadder(text, item, LOG_PAD_TITLE_LENGTH, textPadDirection, textSuffix);
                finalText = new TitlePadder(title, paddedText, TitlePadder.DEFAULT_WIDTH, titlePadDirection, titleSuffix);
            } else if (item != null) {
                finalText = new TitlePadder(title, item, LOG_PAD_TITLE_LENGTH + TitlePadder.DEFAULT_WIDTH + 1,
                                            titlePadDirection, titleSuffix);
            }
            var classNameFmt = className?.Suffix(":").PadRight(LOG_PAD_CLASS_NAME_LENGTH);
#if LOG_INCLUDE_TIMESTAMP
            Debug.WriteLine(@"{0} {1} {2}",
                            DateUtils.Timestamp,
                            classNameFmt,
                            finalText
                );
#else
            Debug.WriteLine(@"{0} {1}",
                            classNameFmt,
                            finalText
                );
#endif
#endif
        }

        public static class DEBUGS
        {
            public static readonly bool LABEL_FILTERS = true;
            internal static readonly bool STRIP_FILENAME = false;
        }

        public static bool debugStripFileName = DEBUGS.STRIP_FILENAME;
        public static readonly bool debugStripFileNameDefault = debugStripFileName;

        private const string DEBUG_STRIP_FILE_NAME_TEMPLATE = "{2}{0,19} {1}";

        [Conditional("DEBUG"), Conditional("TRACE_EXT")]
        public static void DebugLineLabel(string title = "-", string value = null, string extended = null)
        {
#if DEBUG || TRACE_EXT
            if (DEBUGS.STRIP_FILENAME && debugStripFileName) {
                if (value == null) {
                    Debug.WriteLine(new string(title[0], 80));
                } else {
                    Debug.WriteLine(DEBUG_STRIP_FILE_NAME_TEMPLATE, title + ":", value, " ");
                    if (extended != null) {
                        Debug.WriteLine(DEBUG_STRIP_FILE_NAME_TEMPLATE, title + ":", extended, "*");
                    }
                }
            }
#endif
        }

        [Conditional("DEBUG"), Conditional("TRACE_EXT")]
        public static void DebugAssemblyTypes()
        {
#if DEBUG
            var solutionAssemblies = new[] {"wUAL", "uAL", "UTorrentAPI", "Torrent"};
            Array.Reverse(solutionAssemblies);
            var assemblies =
                AppDomain.CurrentDomain.GetAssemblies()
                         .OrderByDescending(s => Array.IndexOf(solutionAssemblies, s.GetName().Name))
                         .ThenBy(s => s.FullName);
            Debug.WriteLine(string.Join("\n", assemblies
                                                  .Select(s => s.GetName())
                                                  .Select((n, i) => $"{i + 1,3}.  {n.Name + ":",-50} {n.Version}")
                                ));
            foreach (var asm in assemblies) {
                var types = asm.GetTypes();
                Debug.WriteLine("{0}\n{1}:\n{0}\n{2}\n",
                                new string('=', 80),
                                asm.GetName().Name,
                                string.Join("\n",
                                            types.OrderBy(s => s.FullName).Select((s, i) => $"{i + 1,3}. {s.FullName}"))
                    );
            }
#endif
        }
    }
}
