using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Torrent.Helpers.Utils;

namespace Torrent.Extensions
{
    public static class ListExtensions
    {
        public static string ToStringKeyValue<T>(this IEnumerable<T> enumerable, string kvpDelimiter = "=", string sep = ", ")
        {
            var list = enumerable.ToArray();
            var result = "";
            var count = list.Length;            
            for (var i = 0; i < count; i += 2) {
                if (i > 0) {
                    result += sep;
                }
                result += list[i];
                if (i + 1 < count) {
                    result += kvpDelimiter + list[i + 1];
                }
            }
            return result;
        }
        public static bool ContainsAny<T>(this IEnumerable<T> list, IEnumerable<T> searchElements)
        {
            foreach (var searchElement in searchElements)
            {
                if (list.Contains(searchElement))
                {
                    return true;
                }                
            }
            return false;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> list, Dictionary<T, T> ExtraProperties)
        {
            var q = list.AsQueryable();
            q = q.Concat(ExtraProperties.Where(p => q.Contains(p.Key)).Select(p => p.Value));
            return q;
        }

		public static MatchCollection Regex(this IEnumerable<string> patterns, string subject)
        {
        	return RegexUtils.CreateGroup(patterns).Regex(subject);
        }        

		public static MatchCollection Regex(this StringCollection patterns, string subject)
        {
        	return RegexUtils.CreateGroup(patterns).Regex(subject);
        }        

        public static T Get<T>(this IEnumerable<T> list, Func<T, bool> predicate) 
        {
            var i = -1;
            while (++i < list.Count())
            {
                var item = list.ElementAtOrDefault(i);
                if (item != null && predicate(item))
                {
                    return item;
                }
            }
            return default(T);
        }
    }
}