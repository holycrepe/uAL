using System;
using System.Globalization;


namespace wUAL
{
    public class CountConverter : BaseConverter
    {
        public CountConverter() : this(null) { }
        public CountConverter(string prefix) : this(prefix, null) { }
        public CountConverter(string prefix, string suffix) : this(prefix, suffix, true) { }

        public CountConverter(string prefix, string suffix, string textIfBlank)
            : this(prefix, suffix, textIfBlank, true) {}

        public CountConverter(string prefix, string suffix, bool usePrefixIfBlank)
            : this(prefix, suffix, "", usePrefixIfBlank) {}

        public CountConverter(string prefix, string suffix, string textIfBlank, bool usePrefixIfBlank)
        {
            Prefix = prefix;
            Suffix = suffix;
            TextIfBlank = textIfBlank;
            UsePrefixIfBlank = usePrefixIfBlank;
        }

        public string Prefix { get; set; }
        public bool UsePrefixIfBlank { get; set; }
        public string Suffix { get; set; }
        public string TextIfBlank { get; set; }

        public override object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
        {
            int count = (int) value;
            string prefixSep, suffixSep;
            var strCount = (count > 0 ? count.ToString() : TextIfBlank);
            prefixSep = " ";
            suffixSep = (strCount == "" ? "" : " ");
            return (Prefix == null || (count == 0 && !UsePrefixIfBlank) ? "" : Prefix + prefixSep) + strCount
                   + suffixSep + Suffix + (count == 1 ? "" : "s");
        }
    }
}
