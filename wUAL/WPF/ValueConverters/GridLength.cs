using System;
using System.Globalization;
using System.Windows;


namespace wUAL
{
    public class GridLengthConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (GridLength) (new System.Windows.GridLengthConverter().ConvertFrom(value));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var converted = ((GridLength) value).Value;
            int convertedInt = (int) converted;
            return convertedInt;
        }
    }
}
