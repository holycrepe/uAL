using System.Linq;

namespace Torrent.Extensions.Collection
{
    public static class ArrayExtensions
    {
        public static T[] After<T>(this T[] array, params T[] elements)
        => array.Concat(elements).ToArray();
        public static T[] Before<T>(this T[] array, params T[] elements)
        => elements.Concat(array).ToArray();
    }
}