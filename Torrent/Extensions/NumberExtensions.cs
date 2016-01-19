using System;

namespace Torrent.Extensions
{
    using Helpers.StringHelpers;

    public static class NumberExtensions
    {

        public static string PadIndex(this int index, bool useValue = true, int width = 0)
            => new NumberPadder(index, useValue, width);

        public static string AsCount(this double number, string description, string suffix = "", string pluralSuffix = "s", object displayIfZero = null)
        {
            return Convert.ToInt32(number).AsCount(description, suffix, pluralSuffix, displayIfZero);
        }
        public static string AsCount(this int number, string description, string suffix = "", string pluralSuffix = "s", object displayIfZero = null)
        {
            var doDisplayIfZero = false;
            if (displayIfZero is bool)
            {
                doDisplayIfZero = (bool) displayIfZero;
            }
            else if (displayIfZero is int || displayIfZero is double)
            {
                doDisplayIfZero = Convert.ToInt32(displayIfZero) > 0;
            }
            if (number == 0 && !doDisplayIfZero)
            {
                return "";
            }
            return number + " " + description + (number == 1 ? "" : pluralSuffix) + suffix;
        }
    }
}
