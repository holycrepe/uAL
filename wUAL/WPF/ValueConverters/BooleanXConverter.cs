using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace wUAL
{
    public class BooleanXConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
            => this;
        public BooleanXConverter(object on) : this(on, 0) { }
        public BooleanXConverter(object on, object off) : base()
        {
            this.On = on;
            this.Off = off;
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
            var cast = (converted ? this.On : this.Off);
            object final;
            return BaseMarkupExtensionConverter.ConvertFrom(cast, targetType, out final) ? final : cast;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
            => this.On.Equals(value);
    }
}