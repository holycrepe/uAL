using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;


namespace wUAL
{
    public class StringCollectionConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
        {
            var items = value as StringCollection;
            if (items != null) {
                var list = items.Cast<string>().ToArray();
                return string.Join("\n", list);
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType,
                                           object parameter, CultureInfo culture)
        {
            var strItems = value as string;
            var list = new StringCollection();
            if (strItems != null) {
                list.AddRange(strItems.Replace('\r', '\n').Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries));
            }
            return list;
        }
    }
}
