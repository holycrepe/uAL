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
        {
            return this;
        }        
    }

    public abstract class BaseMarkupExtensionConverter : BaseMarkupExtension
    {
        public static T ConvertValue<T>(object[] values, T defaultValue, int index = 1)
        {
            return values.Count() < index ? defaultValue : ConvertValue(values[index - 1], defaultValue);
        }
        public static T ConvertValue<T>(object value, T defaultValue)
        {
            T current = defaultValue;
            Type currentType = typeof(T);
            Type valueType = value.GetType();
            if (valueType == currentType)
            {
                return (T)value;
            }
            TypeConverter converter = TypeDescriptor.GetConverter(currentType);
            if (converter.CanConvertFrom(valueType))
            {
                current = (T)converter.ConvertFrom(value);
            }
            else {
                try
                {
                    current = (T)value;
                }
                catch (InvalidCastException) { }
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
