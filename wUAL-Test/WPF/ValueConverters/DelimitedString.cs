using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;


namespace wUAL
{
    public class DelimitedStringConverter : BaseConverter
    {
        public DelimitedStringConverter() : this("\n") { }
        public DelimitedStringConverter(string delimiter) { Delimiter = delimiter; }
        public string Delimiter { get; set; }

        public override object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
        {
            var items = value as IEnumerable<string>;
            if (items != null) {
                return string.Join(Delimiter, items);
            }
            var itemsByte = value as IEnumerable<byte>;
            if (itemsByte != null)
            {
                return string.Join(Delimiter, itemsByte.Select(b => b.ToString()));
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType,
                                           object parameter, CultureInfo culture)
        {
            var items = new string[0];
            var strItems = value as string;
            if (strItems != null) {
                if (Delimiter == "\n") {
                    strItems = strItems.Replace('\r', '\n');
                }
                items = strItems.Split(new[] {Delimiter}, StringSplitOptions.RemoveEmptyEntries);                
            }
            return items;
        }
    }
}
