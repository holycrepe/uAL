using System;
using System.Globalization;
using Torrent.Extensions;
using Torrent.Helpers.Utils;

namespace wUAL
{
    public class DateTimeConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) {
                return null;
            }
            if (value is TimeSpan)
            {
                return ((TimeSpan)value).FormatFriendlyShort();
            }
            if (value is DateTime)
            {
                var cast = (DateTime)value;
                return (cast == DateUtils.StartOfEpoch ? "" : cast.FormatFriendly());
            }
            return null;
        }
    }
}
