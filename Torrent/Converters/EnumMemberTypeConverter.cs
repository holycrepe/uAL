using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Torrent.Extensions;
using Torrent.Helpers.Utils;

namespace Torrent.Converters
{
    using Infrastructure.Enums;
    using static EnumUtils;
    using static Helpers.Utils.DebugUtils;
    public class EnumMemberTypeConverter : EnumConverter
    {
        public EnumMemberTypeConverter(Type type)
            : base(type)
        {
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType.IsEnum || sourceType == typeof(string) || sourceType == typeof(EnumMember);
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType.IsEnum || destinationType == typeof(string) || destinationType == typeof(EnumMember);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var sourceType = value.GetType();
            var sourceMember = value as EnumMember;
            if (sourceMember != null)
            {
                return sourceMember.Value;
            }
            if (sourceType.IsEnum)
            {
                DEBUG.Break();
            }
            if (sourceType == typeof(string))
            {
                var source = value.ToString();
                foreach (var field in EnumType.GetPublicFieldInfo())
                {
                    if (field.Name == source)
                    {
                        return field.GetValue(EnumType);
                    }
                    if (field.GetDescription() == source)
                    {
                        return field.GetValue(EnumType);
                    }
                }
            }
            DEBUG.Break();
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => destinationType == typeof(EnumMember)
            ? new EnumMember(EnumType, value)
            : destinationType == typeof(string) 
            ? GetDescription(value, value.GetType())
            : base.ConvertTo(context, culture, value, destinationType);
    }
}