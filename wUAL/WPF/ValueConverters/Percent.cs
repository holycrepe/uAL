using System;
using System.Globalization;
using System.Windows;

namespace wUAL
{
    using System.Diagnostics;

    public class PercentConverter : BaseMultiConverter
    {
        const double MIN = 0;
        const double MAX = 100;
        const int MIN_PERCENT = 0;
        const int MAX_PERCENT = 100;
        const bool DISPLAY_IF_MIN = false;
        const bool DISPLAY_IF_MAX = true;
        const string FORMAT = "{}%{Suffix}";


        public PercentConverter() { }
        public PercentConverter(string suffix) { this.Suffix = suffix; }
        public string Suffix { get; set; }
        public string Format { get; set; } = null;
        public double Min { get; set; } = MIN;
        public double Max { get; set; } = MAX;
        public int MinPercent { get; set; } = MIN_PERCENT;
        public int MaxPercent { get; set; } = MAX_PERCENT;
        public bool DisplayIfMin { get; set; } = DISPLAY_IF_MIN;
        public bool DisplayIfMax { get; set; } = DISPLAY_IF_MAX;

        public static string Convert(double current, double min, double max, string format, string suffix,
                                     int minPercent = MIN_PERCENT, int maxPercent = MAX_PERCENT,
                                     bool displayIfMin = DISPLAY_IF_MIN, bool displayIfMax = DISPLAY_IF_MAX)
        {
            format = (format ?? FORMAT).Replace("{}", @"{0:F0}").Replace("{Suffix}", suffix ?? string.Empty);
            var percent = (max <= min ? min : current / max * 100);
            var intPercent = System.Convert.ToInt32(percent);
            return (percent < minPercent || percent > maxPercent || (intPercent == minPercent && !displayIfMin) ||
                (intPercent == maxPercent && !displayIfMax))
                ? null
                : string.Format(format, Math.Min(maxPercent, percent));
        }
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var currentValue = values.Length > Min ? values[0] : Min;
            if (currentValue == DependencyProperty.UnsetValue)            
                return null;
            
            var maxValue = values.Length > 1 ? values[1] : Max;
            double max, current;
            try {
                max = System.Convert.ToDouble(maxValue);
                current = System.Convert.ToDouble(currentValue);                
            }
            catch (Exception) {
                return null;
            }
            return Convert(current, Min, max, Format, this.Suffix, this.MinPercent, this.MaxPercent, this.DisplayIfMin,
                    this.DisplayIfMax);            
        }
    }
}