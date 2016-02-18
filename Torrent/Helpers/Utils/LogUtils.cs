#define LOG_INCLUDE_TIMESTAMP

using System;
using System.Linq;
using System.Diagnostics;
using Torrent.Extensions;

namespace Torrent.Helpers.Utils
{
    using Enums;
    using StringHelpers;
    using static StringHelpers.PadStringOptions;

    public static class LogUtils
    {
        const int LOG_CLASS_NAME_LENGTH = 25;
#if LOG_INCLUDE_TIMESTAMP
        const int LOG_TIMESTAMP_LENGTH = 11;
#else
        const int LOG_TIMESTAMP_LENGTH = -1;
#endif
        const int LOG_PAD_TITLE_LENGTH = 50;
        public const int LOG_PREFIX_CLASS_NAME_LENGTH = LOG_TIMESTAMP_LENGTH + 1 + LOG_CLASS_NAME_LENGTH + 1;
        public const int LOG_PREFIX_TITLE_LENGTH = LOG_PREFIX_CLASS_NAME_LENGTH + 1 + LOG_PAD_TITLE_LENGTH;
        static Random rnd;

        public static bool AssertRandom(int random)
        {
            if (random == 0) {
                return true;
            }
            if (rnd == null) {
                rnd = new Random();
            }
            return rnd.Next(random) + 1 == random;
        }

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
#if LOG_INCLUDE_TIMESTAMP
            Debug.WriteLine(@"{0} {1,15} {2}",
                            DateUtils.Timestamp,
                            className + ":",
                            finalText
                );
#else
            Debug.WriteLine(@"{0,15} {1}",
                            className + ":",
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

        const string debugStripFileNameTemplate = "{2}{0,19} {1}";

        [Conditional("DEBUG"), Conditional("TRACE_EXT")]
        public static void DebugLineLabel(string title = "-", string value = null, string extended = null)
        {
#if DEBUG || TRACE_EXT
            if (DEBUGS.STRIP_FILENAME && debugStripFileName) {
                if (value == null) {
                    Debug.WriteLine(new string(title[0], 80));
                } else {
                    Debug.WriteLine(debugStripFileNameTemplate, title + ":", value, " ");
                    if (extended != null) {
                        Debug.WriteLine(debugStripFileNameTemplate, title + ":", extended, "*");
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
