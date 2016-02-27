using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wUAL.WPF.Selectors;
using wUAL.WPF.Styles;

namespace wUAL.WPF.ValueConverters
{
    using static Selectors.Selectors.Progress;
    public class ProgressStyleConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Style.SelectStyle(value);
    }
    public class ProgressLabelStyleConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Label.SelectStyle(value);
    }
    public class ProgressLabelConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Label.SelectContent(value);
    }
}
