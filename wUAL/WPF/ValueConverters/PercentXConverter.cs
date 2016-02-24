using System;
using System.Globalization;
using System.Windows;

namespace wUAL
{
    public class PercentXConverter : BaseConverter
    {
        public PercentXConverter() { }
        public PercentXConverter(string suffix) { this.Suffix = suffix; }
        public string Suffix { get; set; } = " Complete";
        public string Format { get; set; } = "{}%{Suffix}";
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue)
            {
                return null;
            }
            double max=100, current;
            try
            {
                current = System.Convert.ToDouble(value);
            }
            catch (Exception)
            {
                return null;
            }
            var format = (parameter as string ?? 
                this.Format.Replace("{}", @"{0:F0}").Replace("{Suffix}", this.Suffix));
            var percent = (max <= 0 ? 0 : current / max * 100);
            return string.Format(format, Math.Min(100, percent));
        }
    }
}