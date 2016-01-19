using System;
using System.Globalization;
using Torrent.Extensions;


namespace wUAL
{    
    public class TimeSpanConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var cast = (TimeSpan)value;
            return cast.FormatFriendly();
        }
    }
}
