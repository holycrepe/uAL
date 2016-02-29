using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Torrent.Helpers.Utils;

namespace Torrent.Extensions
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using Helpers.StringHelpers;
    using Infrastructure;

    public static class EnumerableExtensions
    {
        [DebuggerNonUserCode]
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
            => source.Shuffle(new Random());
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rnd)
        {
            var elements = source.ToArray();
            for (var i = elements.Length - 1; i >= 0; i--)
            {
                // Swap element "i" with a random earlier element it (or itself)
                // ... except we don't really need to swap it fully, as we can
                // return it immediately, and afterwards it's irrelevant.
                var swapIndex = rnd.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        public static long GetPositiveBitwiseOr(this IEnumerable<object> values)
            => values.GetBitwiseOr(true);
        public static long GetBitwiseOr(this IEnumerable<object> values, bool excludeNegativeValues=false)
            => values.Aggregate<object, long>(0, (current, flag) =>
            {
                var value = Convert.ToInt64(flag);
                if (excludeNegativeValues && value < 0)
                    return current;
                return current | value;
            });
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
        public static string ToIndentedList(this IEnumerable<string> list, string padding = "      ", string defaultValue="")
            => !list.Any() ? defaultValue : "\n" + padding + string.Join("\n" + padding, list) + "\n  ";

        public static IEnumerable<T[]> ChunkArray<T>(this IEnumerable<T> enumerable,
                                                    int chunkSize)
        {
            if (chunkSize < 1) throw new ArgumentException("chunkSize must be positive");

            using (var e = enumerable.GetEnumerator())
                while (e.MoveNext())
                {
                    var remaining = chunkSize;    // elements remaining in the current chunk
                    var innerMoveNext = new Func<bool>(() => --remaining > 0 && e.MoveNext());

                    yield return e.GetChunk(innerMoveNext).ToArray();
                    while (innerMoveNext()) {/* discard elements skipped by inner iterator */}
                }
        }
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> enumerable,
                                                    int chunkSize)
        {
            if (chunkSize < 1) throw new ArgumentException("chunkSize must be positive");

            using (var e = enumerable.GetEnumerator())
                while (e.MoveNext())
                {
                    var remaining = chunkSize;    // elements remaining in the current chunk
                    var innerMoveNext = new Func<bool>(() => --remaining > 0 && e.MoveNext());

                    yield return e.GetChunk(innerMoveNext);
                    while (innerMoveNext()) {/* discard elements skipped by inner iterator */}
                }
        }

        private static IEnumerable<T> GetChunk<T>(this IEnumerator<T> e,
                                                  Func<bool> innerMoveNext)
        {
            do yield return e.Current;
            while (innerMoveNext());
        }

        //public static void AddUnique<T>(this Collection<T> enumerable, T newItem, Func<T, T> selector = null)
        //{
        //    if (!enumerable.Contains(newItem))
        //    {
        //        enumerable.Add(newItem);
        //    }
        //}
        //public static void AddSorted<T>(this Collection<T> enumerable, T newItem, Func<T, T> selector = null)
        //{
        //    if (!enumerable.Contains(newItem))
        //    {
        //        enumerable.Add(newItem);
        //        enumerable.Sort(selector);
        //    }
        //}
        //public static void AddRangeSorted<T>(this Collection<T> enumerable, IEnumerable<T> newItems, Func<T,T> selector = null)
        //{            
        //    enumerable.AddRange(newItems);
        //    enumerable.Sort(selector);
        //}
        //public static void Sort<T>(this Collection<T> enumerable, Func<T, T> selector = null)
        //{
        //    if (selector == null)
        //    {
        //        selector = s => s;
        //    }
        //    var ordered = enumerable.ToArray().OrderBy(selector);
        //    enumerable.ClearRange(ordered);
        //}
        //public static void ClearRange<T>(this Collection<T> enumerable, IEnumerable<T> newItems)
        //{
        //    enumerable.Clear();
        //    enumerable.AddRange(newItems);
        //}

        public static bool ContainsAny<T>(this IEnumerable<T> list, IEnumerable<T> searchElements)
        {
            foreach (var searchElement in searchElements) {
                if (list.Contains(searchElement)) {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> list, IEnumerable<T> searchElements)
        {
            foreach (var searchElement in searchElements)
            {
                if (!list.Contains(searchElement))
                {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> list, Dictionary<T, T> ExtraProperties)
        {
            var q = list.AsQueryable();
            q = q.Concat(ExtraProperties.Where(p => q.Contains(p.Key)).Select(p => p.Value));
            return q;
        }

        public static MatchCollection Regex(this IEnumerable<string> patterns, string subject) => RegexUtils.CreateGroup(patterns).Regex(subject);

        public static MatchCollection Regex(this StringCollection patterns, string subject) => RegexUtils.CreateGroup(patterns).Regex(subject);

        public static T Get<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            var i = -1;
            while (++i < list.Count()) {
                var item = list.ElementAtOrDefault(i);
                if (item != null && predicate(item)) {
                    return item;
                }
            }
            return default(T);
        }

        public static string FormatList<T>(this IEnumerable<T> list,
                                           int level = 1,
                                           string sep = "\n", string delim = "",
                                           int indent = LogUtils.DEFAULT_COLLECTION_INDENT, char indentChar = ' ',
                                           bool combineDelimAndSep = true, NumberPadder padder = null,
                                           Func<T, string> formatter = null,
                                           int linePadding = LogUtils.LOG_PREFIX_TITLE_LENGTH,
                                           bool includeName=false,
                                           bool includeCount=true,
                                           bool includeIndex=true)
        {
            if (list == null) {
                return null;
            }
            padder = padder ?? new NumberPadder();
            var indentStr = indent > 0 ? new string(indentChar, level*indent) : string.Empty;
            if (formatter == null) {
                var formatterPrefix = indentStr + padder.EmptyPadding
                                      + (linePadding == 0 ? "" : new string(' ', linePadding));
                formatter = (s) => s.ToString().PrefixNewLines(formatterPrefix);
            }
            var array = list as T[] ?? list.ToArray();
            var count = array.Length;
            return includeName ? $"{list.GetType().Name}: " : "" +
                   (count == 0
                        ? "Empty"
                        : (includeCount ? $"<{count}>{sep}" : "") 
                        + string.Join(combineDelimAndSep ? delim + sep : delim,
                                      array.Select((s, i) 
                                      => indentStr 
                                      + (includeIndex ? padder.PadIndex(i) : string.Empty)
                                      + formatter(s))
                              ));
        }
    }
}
