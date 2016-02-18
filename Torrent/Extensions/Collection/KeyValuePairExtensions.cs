namespace Torrent.Extensions
{
    using System.Collections.Generic;

    public static class KeyValuePairExtensions
    {
        public static string PadKeyValuePair(this KeyValuePair<string, object> kvp, int length, string keySuffix = ":",
                                             string sep = " ")
            => kvp.Key.PadKeyValuePair(kvp.Value, length, keySuffix, sep);

        public static string PadKeyValuePair(this string key, object value, int length, string keySuffix = ":",
                                             string sep = " ")
            => (key + keySuffix).PadLeft(length + keySuffix.Length) + sep + value;
    }
}
