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
        const int LOG_PAD_TITLE_LENGTH = 50;
        static Random rnd;
        public static bool AssertRandom(int random) {
        	if (random == 0) {
        		return true;
        	}
        	if (rnd == null) {
        		rnd = new Random();
        	}
        	return rnd.Next(random) + 1 == random;
        }        

        [Conditional("DEBUG"), Conditional("TRACE")]
        public static void Log(string className, string title, string text = null, string item = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
        {
#if DEBUG || TRACE
            if (!AssertRandom(random))
            {
                return;
            }
            var finalText = title;
            if (text != null)
            {
                string paddedText = new TitlePadder(text, item, LOG_PAD_TITLE_LENGTH, textPadDirection, textSuffix);
                finalText = new TitlePadder(title, paddedText, 0, titlePadDirection, titleSuffix);
            }
            else if (item != null)
            {
                finalText = new TitlePadder(title, item, LOG_PAD_TITLE_LENGTH + TitlePadder.DEFAULT_WIDTH, titlePadDirection, titleSuffix);
            }
            Debug.WriteLine(@"[{1:hh\:mm\:ss}] {0,15} {2}",
                            className + ":",
                            DateTime.Now,
                            finalText
                );
#endif
        }

        public static class DEBUG {
			public static readonly bool LABEL_FILTERS = true;
			internal static readonly bool STRIP_FILENAME = false;			
		}        
    			
		public static bool debugStripFileName = DEBUG.STRIP_FILENAME;
		public static readonly bool debugStripFileNameDefault = debugStripFileName;
		
		const string debugStripFileNameTemplate = "{2}{0,19} {1}";
	        
        [Conditional("DEBUG"), Conditional("TRACE")]
		public static void DebugLineLabel(string title = "-", string value = null, string extended = null)
		{
			#if DEBUG || TRACE        	
			if (DEBUG.STRIP_FILENAME && debugStripFileName) {
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

        [Conditional("DEBUG"), Conditional("TRACE")]
        public static void DebugAssemblyTypes()
        {
#if DEBUG
            var solutionAssemblies = new[] { "wUAL", "uAL", "UTorrentAPI", "Torrent" };
            Array.Reverse(solutionAssemblies);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().OrderByDescending(s => Array.IndexOf(solutionAssemblies, s.GetName().Name)).ThenBy(s => s.FullName);
            Debug.WriteLine(string.Join("\n", assemblies
                .Select(s => s.GetName())
                .Select((n, i) => $"{i + 1,3}.  {n.Name + ":",-50} {n.Version}")
                ));
            foreach (var asm in assemblies)
            {
                var types = asm.GetTypes();
                Debug.WriteLine("{0}\n{1}:\n{0}\n{2}\n",
                    new string('=', 80),
                    asm.GetName().Name,
                    string.Join("\n", types.OrderBy(s => s.FullName).Select((s, i) => $"{i + 1,3}. {s.FullName}"))
                    );
            }
#endif
        }
    }
}
