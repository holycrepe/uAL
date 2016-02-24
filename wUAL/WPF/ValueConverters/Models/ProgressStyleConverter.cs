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
    using Selectors.Models.ProgressBar;
    using static Selectors.Models.ProgressBar.ProgressBar;
    using static Selectors.Models.ProgressBar.ProgressBarModels;
    public class ProgressStyleConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Model.GetStyle(value);
    }
    public class ProgressLabelStyleConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Model.GetStyle(value, Element.Label);
    }
    public class ProgressLabelConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Model.Get(value).Label;
    }
}
