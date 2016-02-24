using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Torrent.Extensions
{
    public static partial class RandomExtensions
    {
        public static T Pick<T>(this Random rnd, params T[] items)
            => items[rnd.Next(items.Length)];
    }
}
