using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Torrent;
using Torrent.Enums;

namespace wUAL.WPF.ValueConverters
{
    using static Torrent.Helpers.Utils.DebugUtils;
    public class DebugConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
            => DEBUG.Noop(1);
    }
}
