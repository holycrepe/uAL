using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Torrent;

namespace wUAL
{
    public class XYZR : Binding { }

    public class BooleanXConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
            => this;
        public BooleanXConverter(object on) : this(on, 0) { }
        public BooleanXConverter(object on, object off) : base()
        {
            On = on;
            Off = off;
        }

        public object On { get; set; }
        public object Off { get; set; }



        public object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
        {
            bool? castValue = BaseMarkupExtensionConverter.ParseBool(value);
            bool converted = (castValue.HasValue ? castValue.Value : (value != null));
            if (parameter != null)
            {
                converted = !converted;
            }
            var cast = (converted ? On : Off);
            object final;
            return BaseMarkupExtensionConverter.ConvertFrom(cast, targetType, out final) ? final : cast;
        }

        public object ConvertBack(object value, Type targetType,
                                           object parameter, CultureInfo culture)
            => On.Equals(value);
    }
    public class BooleanConverter : BaseConverter
    {
        public BooleanConverter() : this(1) { }
        public BooleanConverter(int trueV, int falseV) : this((object) trueV, falseV) { }
        public BooleanConverter(object trueValue) : this(trueValue, 0) { }
        public BooleanConverter(object trueValue, object falseValue) : base()
        {
            On = trueValue;
            Off = falseValue;
        }

        public object On { get; set; }
        public object Off { get; set; }

        

        public override object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
        {
            bool? castValue = ParseBool(value);
            bool converted = (castValue.HasValue ? castValue.Value : (value != null));
            if (parameter != null) {
                converted = !converted;
            }
            var cast = (converted ? On : Off);
            object final;
            return ConvertFrom(cast, targetType, out final) ? final : cast;
        }

        public override object ConvertBack(object value, Type targetType,
                                           object parameter, CultureInfo culture)
            => On.Equals(value);
    }
}
