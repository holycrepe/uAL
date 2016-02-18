using System;
using System.Collections.Generic;
using System.IO;
using Torrent.Helpers.Utils;

namespace Torrent.Extensions
{
    using Helpers.StringHelpers;

    public static partial class Extensions
    {
        static bool debugStripFileNameHasLoggedPattern;

        public static string StripFilename(this string filename, bool extended = true,
                                           Func<bool, IEnumerable<string>> getWordsToStrip = null)
        {
            LogUtils.debugStripFileName = LogUtils.debugStripFileNameDefault;
            var stripWords = RegexUtils.CreateGroup((getWordsToStrip ?? RegexUtils.GetLabelsWordsToStrip)(false));
            if (LogUtils.debugStripFileName) {
                var Regular = new RegexStripper(filename);
                var Extended = new RegexStripper(filename, true);
                if (!debugStripFileNameHasLoggedPattern) {
                    debugStripFileNameHasLoggedPattern = true;
                    LogUtils.DebugLineLabel("=");
                    LogUtils.DebugLineLabel("Regex Tokens", RegexUtils.TokenGroup);
                    LogUtils.DebugLineLabel();
                    LogUtils.DebugLineLabel("Regex Pattern", Regular.Pattern, Extended.Pattern);
                    LogUtils.DebugLineLabel("Words Pattern", Regular.WordsPattern, Extended.WordsPattern);
                    LogUtils.DebugLineLabel("Strip Words", Regular.StripWords, Extended.StripWords);
                }
                LogUtils.DebugLineLabel("=");
                LogUtils.DebugLineLabel("Original Filename", filename);
                LogUtils.DebugLineLabel();
                LogUtils.DebugLineLabel("Stripped Filename", Regular.Stripped, Extended.Stripped);
                LogUtils.DebugLineLabel("Final Filename", Regular.Final, Extended.Final);
                LogUtils.DebugLineLabel("=");
                return (extended ? Extended.Final : Regular.Final);
            }
            return new RegexStripper(filename, extended).Final;
        }

        public static string TrimPath(this string fileName) { return fileName.TrimEnd(new char[] {'\\', '/'}); }

        public static bool PathEquals(this string fileName, string otherFileName)
        {
            return fileName.TrimPath() == otherFileName.TrimPath();
        }

        public static bool PathEquals(this FileSystemInfo fileInfo, string otherFileName)
        {
            return fileInfo.FullName.TrimPath() == otherFileName.TrimPath();
        }
    }
}
