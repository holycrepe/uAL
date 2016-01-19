using System;
using System.Globalization;
using System.Windows;


namespace wUAL
{
    public class VisibilityConverter : BaseConverter
    {

        private bool? Parse(object value)
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
                return ((string)value) != "";
            }
			return value != null;
        }

        public override object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            bool? castValue = Parse(value);
            bool converted = (castValue.HasValue ? castValue.Value : (value != null));
            if (parameter != null)
            {
                converted = !converted;
            }
            return (converted ? Visibility.Visible : Visibility.Collapsed);
        }

        public override object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return ((Visibility)value == Visibility.Visible ? true : false);
            }
            return null;
        }
    }
}
