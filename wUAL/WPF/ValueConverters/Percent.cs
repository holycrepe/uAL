using System;
using System.Globalization;
using System.Windows;

namespace wUAL
{
    public class PercentConverter : BaseMultiConverter
    {
        public PercentConverter() { }
        public PercentConverter(string suffix) { this.Suffix = suffix; }
        public string Suffix { get; set; } = "";
        public string Format { get; set; } = "{}%{Suffix}";
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var currentValue = values.Length > 0 ? values[0] : 0;
            if (currentValue == DependencyProperty.UnsetValue)            
                return null;
            
            var maxValue = values.Length > 1 ? values[1] : 100.0;
            double max, current;
            try {
                max = System.Convert.ToDouble(maxValue);
                current = System.Convert.ToDouble(currentValue);                
            }
            catch (Exception) {
                return null;
            }
            var format = (parameter as string ?? @"{0:F0}%" + this.Suffix);
            var percent = (max <= 0 ? 0 : current / max * 100);
            return percent <= 0 
                ? null 
                : string.Format(format, Math.Min(100, percent));
        }
    }
}