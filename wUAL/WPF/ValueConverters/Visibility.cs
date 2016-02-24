using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Expr = System.Linq.Expressions;
using Torrent.Enums;

namespace wUAL
{
    public class VisibilityConverter : BaseCombinedConverter
    {
        public VisibilityConverter() : this(OperationType.And) { }
        public VisibilityConverter(Visibility hiddenType)
            : this(OperationType.And, hiddenType)
        { }

        public VisibilityConverter(OperationType operation, Visibility hiddenType = Visibility.Collapsed) : base()
        {
            this.HiddenType = hiddenType;
            this.Operation = operation;
        }
        public OperationType Operation { get; set; } 
        public Visibility HiddenType { get; set; }
        public bool Collapsed
        {
            get { return this.HiddenType == Visibility.Collapsed; }
            set { this.HiddenType = value ? Visibility.Collapsed : Visibility.Hidden; }
        }

        public bool Inverse { get; set; } = false;        

        public override object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
            => ConvertValue(value, Inverse, parameter) ? Visibility.Visible : HiddenType;

        public override object Combine(object[] values, object parameter)
            => values.Select(a => ConvertValue(a, parameter))
                .Aggregate((a, b) => Operation.Evaluate(a, b));

        public override object ConvertBack(object value, Type targetType,
                                           object parameter, CultureInfo culture) 
            => value is Visibility 
            ? ((Visibility) value == Visibility.Visible) 
            : base.ConvertBack(value, targetType, parameter, culture);
    }
}
