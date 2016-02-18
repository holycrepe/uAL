using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace wUAL
{
    public class PercentConverter : DependencyObject, IValueConverter
    {
        public PercentConverter() { }
        public PercentConverter(int maximum) { Maximum = maximum; }
        public PercentConverter(string format) { Format = format; }

        public PercentConverter(int maximum, string format)
        {
            Maximum = maximum;
            Format = format;
        }

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format",
                                        typeof (string),
                                        typeof (PercentConverter),
                                        new PropertyMetadata(@"{0:F0}% Complete"));

        public string Format
        {
            get { return (string) this.GetValue(FormatProperty); }
            set { this.SetValue(FormatProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum",
                                        typeof (int),
                                        typeof (PercentConverter),
                                        new PropertyMetadata(100));

        public int Maximum
        {
            get { return (int) this.GetValue(MaximumProperty); }
            set { this.SetValue(MaximumProperty, value); }
        }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            double current = Maximum/2;
            TypeConverter converter = TypeDescriptor.GetConverter(typeof (double));
            if (converter.CanConvertFrom(value.GetType())) {
                current = (double) converter.ConvertFrom(value);
            }
            return string.Format(Format, current/Maximum*100);
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }

    public class PercentMultiConverter : BaseMultiConverter
    {
        public PercentMultiConverter() { }
        public PercentMultiConverter(string suffix) { this.Suffix = suffix; }
        public string Suffix { get; set; } = " Complete";
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var currentValue = values.Length > 0 ? values[0] : 0;
            if (currentValue == DependencyProperty.UnsetValue)
            {
                return null;
            }
            var maxValue = values.Length > 1 ? values[1] : 100.0;
            double max, current;
            try {
                max = System.Convert.ToDouble(maxValue);
                current = System.Convert.ToDouble(currentValue);                
            }
            catch (Exception) {
                return null;
            }
            var format = (parameter as string ?? @"{0:F0}%" + Suffix);
            var percent = (max <= 0 ? 0 : current / max * 100);
            return string.Format(format, Math.Min(100, percent));
        }
    }
}
