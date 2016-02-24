using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using Torrent.Extensions;
using Torrent.Helpers.Utils;

namespace Torrent.Converters
{
    using static Helpers.Utils.DebugUtils;
    public class ColorCollectionTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType.IsAssignableFrom(typeof(string));
        //public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        //    => destinationType.IsEnum || destinationType == typeof(string);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = value as string;
            return strValue != null 
                ? new Collection<Color>(strValue.Replace(",", " ").Split()
                .Select(ColorUtils.HexToColor).ToList()) 
                : base.ConvertFrom(context, culture, value);
        }
    }
}