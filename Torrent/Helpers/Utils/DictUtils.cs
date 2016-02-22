using System.Collections.Generic;

namespace Torrent.Helpers.Utils
{
    static class DictUtils
    {
        public static string PadKeyValuePair(KeyValuePair<string, string> kvp, int length, string keySuffix = ":",
                                 string sep = " ") => (kvp.Key + keySuffix).PadLeft(length + keySuffix.Length) + sep + kvp.Value;

        public static string PadKeyValuePair(string key, string value, int length, string keySuffix = ":",
                                 string sep = " ") => PadKeyValuePair(new KeyValuePair<string, string>(key, value), length, keySuffix, sep);
    }
}
