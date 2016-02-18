using System;

namespace uAL.Infrastructure
{
    public static class ColoredConsole
    {
        static ConsoleColor originalForeground;
        static ConsoleColor originalBackground;
        static bool cacheForegound;
        static bool cacheBackground;

        static void CacheColor()
        {
            originalForeground = Console.ForegroundColor;
            cacheForegound = true;
            cacheBackground = false;
        }

        static void CacheBackgroundColor()
        {
            originalBackground = Console.BackgroundColor;
            cacheForegound = false;
            cacheBackground = true;
        }

        static void CacheColors()
        {
            originalForeground = Console.ForegroundColor;
            originalBackground = Console.BackgroundColor;
            cacheForegound = true;
            cacheBackground = true;
        }

        static void RestoreColors()
        {
            if (cacheForegound) {
                Console.ForegroundColor = originalForeground;
            }
            if (cacheBackground) {
                Console.BackgroundColor = originalBackground;
            }
        }

        public static string GetBanner(int width = -1, char chr = '=')
        {
            if (width < 1) {
                width = Console.WindowWidth - 2;
            }

            return new string(chr, width) + "\n";
        }

        public static string GetBannerText(string text, int width = -1, char chr = '=')
        {
            string banner = GetBanner(width, chr);

            return banner + text + "\n" + banner;
        }

        public static void WriteInfoBanner(string text, int width = -1, char chr = '=')
        {
            string banner = GetBanner(width, chr);
            WriteInfoTitle(banner);
            WriteInfoLine(text);
            WriteInfoTitle(banner);
        }

        public static void WriteInfoTitle(string text, string suffix = "", string prefix = "")
        {
            Write(text, ConsoleColor.Yellow, ConsoleColor.Blue, suffix, prefix);
        }

        public static void WriteInfoLine(string text, string suffix = "", string prefix = "")
        {
            WriteLine(text, ConsoleColor.Gray, ConsoleColor.Blue, suffix, prefix);
        }

        public static void WriteError(string title, string text, string sep = "", string suffix = "", string prefix = "")
        {
            WriteErrorTitle(title, sep, prefix);
            WriteErrorLine(text, suffix);
        }

        public static void WriteErrorTitle(string text, string suffix = "", string prefix = "")
        {
            Write(text, ConsoleColor.Gray, ConsoleColor.Red, suffix, prefix);
        }

        public static void WriteErrorLine(string text, string suffix = "", string prefix = "")
        {
            WriteLine(text, ConsoleColor.Red, suffix, prefix);
        }

        public static void WriteException(Exception ex, string title = null, string text = null, string prefix = "  ")
        {
            if (title != null && text != null) {
                WriteError(title, text, prefix: prefix);
            } else if (title != null) {
                WriteErrorTitle(title, prefix: prefix);
            } else if (text != null) {
                WriteErrorLine(text, prefix: prefix);
            }


            ConsoleColor originalForeground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;

            var exceptionStrings = ex.ToString()
                                     .Replace("\n", "\n#           ")
                                     .Split(new string[] {"\r\n"}, 2, StringSplitOptions.None);
            string exceptionTitle = exceptionStrings[0];
            string exceptionDetails = exceptionStrings[1];

            Console.WriteLine("########");
            WriteErrorTitle("Message:", suffix: "  ", prefix: "# ");
            WriteErrorLine(ex.Message);
            WriteErrorTitle("Source: ", suffix: "  ", prefix: "# ");
            WriteErrorLine(ex.Source);
            Console.Write("#-------\n# ");
            WriteErrorTitle("Details: ", suffix: "\n");
            Console.Write("#-------\n#           ");
            WriteErrorLine(exceptionTitle);
            Console.Write("#-------\n# ");
            WriteErrorTitle("Trace: ", suffix: "\n");
            Console.WriteLine("#-------");
            Console.WriteLine(exceptionDetails);
            Console.WriteLine("########");
            Console.WriteLine();

            Console.ForegroundColor = originalForeground;
        }

        public static void WriteLine(string text, ConsoleColor Foreground, string suffix = "", string prefix = "")
        {
            Write(text, Foreground, suffix + "\n", prefix);
        }

        public static void WriteLineBG(string text, ConsoleColor Background, string suffix = "", string prefix = "")
        {
            WriteBG(text, Background, suffix + "\n", prefix);
        }

        public static void WriteLine(string text, ConsoleColor Foreground, ConsoleColor Background, string suffix = "",
                                     string prefix = "")
        {
            Write(text, Foreground, Background, suffix + "\n", prefix);
        }

        public static void Write(string text, ConsoleColor Foreground, string suffix = "", string prefix = "")
        {
            CacheColor();
            if (prefix != "") {
                Console.Write(prefix);
            }
            Console.ForegroundColor = Foreground;
            Console.Write(text);
            RestoreColors();
            if (suffix != "") {
                Console.Write(suffix);
            }
        }

        public static void WriteBG(string text, ConsoleColor Foreground, string suffix = "", string prefix = "")
        {
            CacheBackgroundColor();
            if (prefix != "") {
                Console.Write(prefix);
            }
            Console.BackgroundColor = Foreground;
            Console.Write(text);
            RestoreColors();
            if (suffix != "") {
                Console.Write(suffix);
            }
        }

        public static void Write(string text, ConsoleColor Foreground, ConsoleColor Background, string suffix = "",
                                 string prefix = "")
        {
            CacheColors();
            if (prefix != "") {
                Console.Write(prefix);
            }
            Console.ForegroundColor = Foreground;
            Console.BackgroundColor = Background;
            Console.Write(text);
            RestoreColors();
            if (suffix != "") {
                Console.Write(suffix);
            }
        }
    }
}
