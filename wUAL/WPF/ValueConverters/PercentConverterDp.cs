using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace wUAL
{
    public class PercentConverterDp : DependencyObject, IValueConverter
    {
        public PercentConverterDp() { }
        public PercentConverterDp(int maximum) { Maximum = maximum; }
        public PercentConverterDp(string format) { Format = format; }

        public PercentConverterDp(int maximum, string format)
        {
            Maximum = maximum;
            Format = format;
        }

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format",
                                        typeof (string),
                                        typeof (PercentConverterDp),
                                        new PropertyMetadata(@"{0:F0}% Complete"));

        public string Format
        {
            get { return (string) this.GetValue(FormatProperty); }
            set { this.SetValue(FormatProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum",
                                        typeof (int),
                                        typeof (PercentConverterDp),
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
}
