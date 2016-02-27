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
    public class EnumCombinedConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
            => value == null 
            ? string.Empty 
            : EnumUtils.GetCombinedDescription(value, value.GetType());
    }
}
