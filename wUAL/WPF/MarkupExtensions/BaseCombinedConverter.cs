using System;
using System.Globalization;
using System.Windows.Data;

namespace wUAL
{
    public abstract class BaseCombinedConverter : BaseMarkupExtensionConverter, IValueConverter, IMultiValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object Combine(object[] values, object parameter);
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => Convert(Combine(values, parameter), targetType, parameter, culture);

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}