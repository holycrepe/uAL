using System;
using System.Globalization;

namespace wUAL
{
    using Torrent.Extensions;

    public class FileSizeConverter : BaseConverter
    {
        public FileSizeConverter() : this("") { }
        public FileSizeConverter(string suffix) { Suffix = suffix; }
        public string Suffix { get; set; }
        public override object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            long cast;
            if (long.TryParse(value.ToString(), out cast) && cast != 0)
            {
                return cast.ToFileSize() + Suffix;
            }
            return "";
        }
    }
}