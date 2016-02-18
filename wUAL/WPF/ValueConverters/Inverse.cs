using System;
using System.Globalization;
using System.Windows;


namespace wUAL
{
    public class InverseConverter : BaseConverter
    {
        private bool? CastValue(object value)
        {
            if (value is bool) {
                return (bool) value;
            } else if (value is int) {
                return ((int) value) != 0;
            }
            return null;
        }

        public override object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
        {
            bool? castValue = CastValue(value);
            if (!castValue.HasValue) {
                return null;
            }
            bool converted = !(castValue.Value);

            if (targetType == typeof (Visibility)) {
                return (converted ? Visibility.Visible : Visibility.Collapsed);
            }
            return converted;
        }

        public override object ConvertBack(object value, Type targetType,
                                           object parameter, CultureInfo culture)
        {
            if (value is Visibility) {
                return ((Visibility) value != Visibility.Visible);
            }
            if (value is int) {
                return ((int) value == 0);
            }
            if (value is bool) {
                return !(bool) value;
            }
            return null;
        }
    }
}
