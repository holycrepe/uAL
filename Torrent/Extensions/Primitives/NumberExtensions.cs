using System;

namespace Torrent.Extensions
{
    using Formatters;
    using Helpers.StringHelpers;
    using PostSharp.Patterns.Model;

    public static class NumberExtensions
    {
        public static string PadIndex(this int index, bool useValue = true, int width = 0)
            => new NumberPadder(index, useValue, width);

        public static string AsCount(this double number, string description, string suffix = "", string prefix = " ",
                                     string pluralSuffix = "s", object displayIfZero = null)
            => Convert.ToInt32(number).AsCount(description, suffix, prefix, pluralSuffix, displayIfZero);

        public static string AsCount(this int number, string description, string suffix = "", string prefix = " ", string pluralSuffix = "s",
                                     object displayIfZero = null)
        {
            var doDisplayIfZero = false;
            if (displayIfZero is bool) {
                doDisplayIfZero = (bool) displayIfZero;
            } else if (displayIfZero is int || displayIfZero is double) {
                doDisplayIfZero = Math.Floor(Convert.ToDouble(displayIfZero)) > 0;
            }
            if (number == 0 && !doDisplayIfZero) {
                return "";
            }
            return number + prefix + description + (number == 1 ? "" : pluralSuffix) + suffix;
        }

        public static string ToFileSize(this long number)
            => string.Format(new FileSizeFormatProvider(), "{0:fs}", number);

        [Pure]
        public static double GetProgress(this int value, int maximum, int offset = 0)
            => maximum == 0 ? 0 : Convert.ToDouble(value + offset)/maximum*100;
    }
}
