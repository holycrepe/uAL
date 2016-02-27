#define LOG_PROGRESS_MODEL

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wUAL.WPF.Selectors;
using wUAL.WPF.Styles;

namespace wUAL.WPF.Models.ProgressBar
{
    using ValueConverters;
    
    //public class ProgressStyleConverter : BaseConverter
    //{
    //    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //        => Model.GetStyle(value);
    //}
    //public class ProgressTemplateConverter : BaseConverter
    //{
    //    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //        => Model.GetTemplate(value);
    //}
    //public class ProgressLabelStyleConverter : BaseConverter
    //{
    //    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //        => Model.GetStyle(value, Element.Label);
    //}
    //public class ProgressLabelConverter : BaseConverter
    //{
    //    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //        => Model.Get(value).Label;
    //}

    public class ProgressStyleConverter : ProgressBarValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = ProgressBarModels.Current.GetStyleModel(value);
            var result = ProgressBarModels.Current.GetStyle(value);
            return LogResult(value, model, result, parameter);
        }
    }

    public class ProgressTemplateConverter : ProgressBarValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = ProgressBarModels.Current.GetTemplateModel(value);
            var result = ProgressBarModels.Current.GetTemplate(value);
            return LogResult(value, model, result, parameter);
        }
    }

    public class ProgressLabelStyleConverter : ProgressBarValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = ProgressBarModels.Current.GetStyleModel(value, ProgressBar.Element.Label);
            var result = ProgressBarModels.Current.GetStyle(value, ProgressBar.Element.Label);
            return LogResult(value, model, result, parameter);
        }
    }

    public class ProgressLabelConverter : ProgressBarValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = ProgressBarModels.Current.Get(value);
            var result = ProgressBarModels.Current.Get(value).Label;
            return LogResult(value, model, result, parameter);
        }
    }
}