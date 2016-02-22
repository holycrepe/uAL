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
using Torrent.Helpers.Utils;

namespace wUAL
{
    public class EnumDescriptionConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
            => value == null 
            ? string.Empty 
            : EnumUtils.GetDescriptionOrValue(value, value.GetType());

        public override object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                return Enum.Parse(targetType, value.ToString());
            }
            catch (Exception ex) 
            when (ex is ArgumentException || ex is OverflowException) { }

            object converted;
            return EnumUtils.GetValueFromDescription(value as string, targetType, out converted) 
                ? converted 
                : base.ConvertBack(value, targetType, parameter, culture);
        }
    }
}
