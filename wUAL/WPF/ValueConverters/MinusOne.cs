using System;
using System.Globalization;


namespace wUAL
{
    public class MinusOneConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (int)value - 1;
        }

        public override object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (int)value + 1;
        }
    }
}
