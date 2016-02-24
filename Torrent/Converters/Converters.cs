using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torrent.Converters
{
    public static class Converters
    {
        public static bool? ParseBool(object value)
        {
            if (value is bool)
            {
                return (bool)value;
            }
            if (value is int)
            {
                return ((int)value) > 0;
            }
            if (value is double)
            {
                return ((double)value) > 0;
            }
            // disable once CanBeReplacedWithTryCastAndCheckForNull
            if (value is string)
            {
                return !string.IsNullOrEmpty((string)value);
            }
            var array = value as Array;
            if (array != null)
            {
                return array.Length > 0;
            }
            return value != null;
        }
    }
}
