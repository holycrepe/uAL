using System;
using System.Globalization;
using System.Windows.Data;

namespace wUAL
{
    public abstract class BaseMultiConverter : BaseMarkupExtensionConverter, IMultiValueConverter, IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => Convert(new [] {value}, targetType, parameter, culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}