using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace wUAL
{
    public abstract class BaseMarkupExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
            => this;
    }

    public abstract class BaseMarkupExtensionConverter : BaseMarkupExtension
    {
        public static bool? ParseBool(object value)
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
            var array = value as Array;
            if (array != null)
            {
                return array.Length > 0;
            }
            return value != null;
        }

        public static bool ConvertFrom(object value, Type targetType, out object converted)
        {
            var converter = TypeDescriptor.GetConverter(targetType);
            Type valueType = value.GetType();
            if (converter.CanConvertFrom(valueType))
            {
                converted = converter?.ConvertFrom(value);
                if (converted != null)
                {
                    return true;
                }
            }
            converted = value;
            return false;
        }

        public static T ConvertValue<T>(object[] values, T defaultValue, int index = 1)
            => values.Count() < index ? defaultValue : ConvertValue(values[index - 1], defaultValue);

        public static T ConvertValue<T>(object value, T defaultValue)
        {
            T current = defaultValue;
            Type targetType = typeof (T);
            Type valueType = value.GetType();
            if (valueType == targetType) {
                return (T) value;
            }
            object converted;
            if (ConvertFrom(value, targetType, out converted))
            {
                current = (T)converted;
            }
            //var converter = TypeDescriptor.GetConverter(targetType);
            //if (converter.CanConvertFrom(valueType)) {
            //    current = (T) converter.ConvertFrom(value);
            //}
            else {
                if (targetType == typeof(int))
                {
                    try
                    {
                        current = (T)(object)Convert.ToInt32(value);
                    }
                    catch (InvalidCastException) { }
                }

                try {                    
                    current = (T) value;
                }
                catch (InvalidCastException) {}
            }
            return current;
        }
    }


    public abstract class BaseConverter : BaseMarkupExtensionConverter, IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class BaseMultiConverter : BaseMarkupExtensionConverter, IMultiValueConverter
    {
        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
