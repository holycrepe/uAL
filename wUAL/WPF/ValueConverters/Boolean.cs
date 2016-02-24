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

namespace wUAL
{
    public class XYZR : Binding { }

    public class BooleanConverter : BaseCombinedConverter
    {
        public BooleanConverter() : this(1) { }
        public BooleanConverter(int trueV, int falseV) : this((object) trueV, falseV) { }
        public BooleanConverter(object trueValue) : this(trueValue, 0) { }
        public BooleanConverter(object trueValue, object falseValue) : base()
        {
            On = trueValue;
            Off = falseValue;
        }

        public object On { get; set; }
        public object Off { get; set; }

        public OperationType Operation { get; set; }
        public bool Inverse { get; set; } = false;

        public override object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
        {
            var converted = ConvertValue(value, Inverse, parameter);
            var cast = (converted ? On : Off);
            object final;
            return ConvertFrom(cast, targetType, out final) ? final : cast;
        }

        public override object Combine(object[] values, object parameter)
            => values.Select(a => ConvertValue(a, parameter))
                .Aggregate((a, b) => Operation.Evaluate(a, b));

        public override object ConvertBack(object value, Type targetType,
                                           object parameter, CultureInfo culture)
            => On.Equals(value);
    }
}
