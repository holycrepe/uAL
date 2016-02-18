using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Torrent.Extensions;
using Torrent.Helpers.Utils;

namespace Torrent.Converters
{
    using static Helpers.Utils.DebugUtils;
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type)
            : base(type)
        {
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType.IsEnum || sourceType == typeof(string);
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType.IsEnum || destinationType == typeof(string);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var sourceType = value.GetType();
            if (sourceType.IsEnum)
            {
                DEBUG.Break();
            }
            if (sourceType == typeof(string))
            {
                var source = value.ToString();
                foreach (var field in EnumType.GetPublicFields())
                {
                    if (field.Name == source)
                    {
                        return field.GetValue(EnumType);
                    }
                    if (field.GetDescription(EnumType) == source)
                    {
                        return field.GetValue(EnumType);
                    }
                }
            }
            DEBUG.Break();
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => destinationType == typeof(string) 
            ? EnumUtils.GetDescription(value, value.GetType())
            : base.ConvertTo(context, culture, value, destinationType);
    }
}