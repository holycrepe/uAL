using System;
using System.Globalization;
using System.IO;


namespace wUAL
{
    public class FilenameToDirectoryConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string castValue = value as string;
            return castValue != null ? Path.GetDirectoryName(castValue) : null;
        }
    }
}
