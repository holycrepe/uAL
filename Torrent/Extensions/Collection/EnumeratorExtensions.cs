using System.Collections.Generic;
using System.Linq;

namespace Torrent.Extensions
{
    public static class EnumeratorExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator, int start=0, int end=-1)
        {
            var i = 0;
            while (enumerator.MoveNext())
            {
                if (i >= start)
                {
                    yield return enumerator.Current;
                }
                if (i >= end)
                {
                    break;
                }
                i++;
            }
        }
        public static T[] ToArray<T>(this IEnumerator<T> enumerator, int start = 0, int end = -1)
            => enumerator.ToEnumerable(start, end).ToArray();

        public static T Pop<T>(this IEnumerator<T> enumerator)
            => enumerator.MoveNext() ? enumerator.Current : default(T);
    }
}