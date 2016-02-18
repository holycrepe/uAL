using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Torrent;

namespace wUAL
{
    public class VisibilityConverter : BaseConverter
    {
        public VisibilityConverter() : this(Visibility.Collapsed) { }
        public VisibilityConverter(Visibility hiddenType) : base() {
            this.HiddenType = hiddenType;
        }

        public Visibility HiddenType { get; set; }
        public bool Collapsed
        {
            get { return this.HiddenType == Visibility.Collapsed; }
            set { this.HiddenType = value ? Visibility.Collapsed : Visibility.Hidden; }
        }

        public override object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
        {
            bool? castValue = ParseBool(value);
            bool converted = (castValue.HasValue ? castValue.Value : (value != null));
            if (parameter != null) {
                converted = !converted;
            }
            return (converted ? Visibility.Visible : HiddenType);
        }

        public override object ConvertBack(object value, Type targetType,
                                           object parameter, CultureInfo culture)
        {
            if (value is Visibility) {
                return ((Visibility) value == Visibility.Visible ? true : false);
            }
            return null;
        }
    }
}
