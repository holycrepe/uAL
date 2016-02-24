using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Torrent.Extensions.Framework
{
    public static class ResourceDictionaryExtensions
    {
        public static bool TryFindResource<T>(this ResourceDictionary resources, string key, out T value)
            where T : class
        {
            if (resources.Contains(key))
            {
                value = resources[key] as T;
                return true;
            }
            foreach (var mergedDictionary in resources.MergedDictionaries)
                if (mergedDictionary.TryFindResource(key, out value))
                    return true;
            value = default(T);
            return false;
        }

        public static T TryFindResource<T>(this ResourceDictionary resources, string key)
            where T : class
        {
            T value;
            return resources.TryFindResource(key, out value) 
                ? value 
                : default(T);
        }
    }
}
