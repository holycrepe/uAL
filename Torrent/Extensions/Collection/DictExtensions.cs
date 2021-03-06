namespace Torrent.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class DictExtensions
    {
        public static void Concat<TKey, TValue, TEnumerable>(this IDictionary<TKey, TValue> original,
                                                             IEnumerable<TEnumerable> newList,
                                                             Action<TEnumerable, KeyValuePair<TKey, TValue>>
                                                                 selectorFunc, bool overwrite = true)
        {
            var newDict = new Dictionary<TKey, TValue>();
            foreach (var item in newList) {
                var kvp = new KeyValuePair<TKey, TValue>();
                selectorFunc(item, kvp);
                newDict.Add(kvp.Key, kvp.Value);
            }
            original.Concat(newDict, overwrite);
        }
        public static TValue GetSelection<TKey, TValue>(this IDictionary<TKey, TValue> selections, TKey key, bool flag = true)
        {
            TValue value;
            selections.TryGetSelection(key, out value, flag);
            return value;
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            => dict.ContainsKey(key) ? dict[key] : default(TValue);
        public static bool TryGetSelection<TKey, TValue>(this IDictionary<TKey, TValue> selections, TKey key, out TValue value, bool flag=true)
        {
            if (flag && selections.ContainsKey(key))
            {
                value= selections[key];
                return true;
            }
            value = default(TValue);
            return false;
        }

        public static void Concat<TKey, TValue, TEnumerable>(this IDictionary<TKey, TValue> original,
                                                             IEnumerable<TEnumerable> newList,
                                                             Action<TEnumerable, Dictionary<TKey, TValue>> selectorFunc,
                                                             bool overwrite = true)
        {
            var newDict = new Dictionary<TKey, TValue>();
            foreach (var item in newList) {
                selectorFunc(item, newDict);
            }
            original.Concat(newDict, overwrite);
        }

        public static void Concat<TKey, TValue>(this IDictionary<TKey, TValue> original,
                                                IDictionary<TKey, TValue> newDict, bool overwrite = true)
        {
            foreach (var kvp in newDict) {
                if (original.ContainsKey(kvp.Key)) {
                    if (overwrite) {
                        original[kvp.Key] = kvp.Value;
                    }
                } else {
                    original.Add(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
