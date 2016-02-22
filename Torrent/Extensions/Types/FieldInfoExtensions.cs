namespace Torrent.Extensions
{
    using Infrastructure.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using static Helpers.Utils.EnumUtils;
    using Exceptions;
    public static class FieldInfoExtensions
    {
        static object[] GetCustomAttributesTyped<T>(this FieldInfo field) where T : Attribute
            => field.GetCustomAttributes(typeof(T), false);
        public static T[] GetCustomAttributes<T>(this FieldInfo field) where T : Attribute
            => (T[])GetCustomAttributesTyped<T>(field);
        public static T GetCustomAttribute<T>(this FieldInfo field) where T : Attribute
            => (T)GetCustomAttributesTyped<T>(field).FirstOrDefault(v => v is T && v != null);
        public static object GetDisplay(this FieldInfo field, Type enumType)
            => field.GetDescription() ?? field.GetValue(enumType);
        public static string GetDescription(this FieldInfo field)
            => field.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }
}
