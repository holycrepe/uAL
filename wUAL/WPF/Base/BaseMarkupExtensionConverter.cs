using System;
using System.ComponentModel;
using System.Linq;
using Torrent.Converters;

namespace wUAL
{
    public abstract class BaseMarkupExtensionConverter : BaseMarkupExtension
    {
        public static bool? ParseBool(object value)
            => Converters.ParseBool(value);
        public bool ConvertValue(object value, bool inverse, object parameter)
        {
            var castValue = ParseBool(value);
            var converted = castValue ?? (value != null);
            return parameter == null && !inverse
                ? converted
                : !converted;
        }
        public static bool ConvertFrom(object value, Type targetType, out object converted)
        {
            if (value == null)
            {
                converted = null;
                return false;
            }
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
}