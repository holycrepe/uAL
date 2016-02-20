using AddGenericConstraint;
using System;
using System.Linq;

namespace Torrent.Helpers.Utils
{
    using Extensions;
    using System.ComponentModel;
    using System.Reflection;

    public static partial class EnumUtils
    {
        //public static T[] GetValues<[AddGenericConstraint(typeof(Enum))] T>() where T : struct
        //   => (T[])Enum.GetValues(typeof(T));
        [Obsolete("Use extension method", true)]
        public static string GetDescription<[AddGenericConstraint(typeof(Enum))] T>(T? value) where T : struct
                => value == null ? string.Empty : GetDescription(value.Value, typeof(T));
        
        public static string GetDescription(object value, Type t)
        {
            if (value != null)
            {
                var fi = t.GetField(value.ToString());
                if (fi != null)
                {
                    var attribute = fi.GetCustomAttribute<DescriptionAttribute>();
                    return string.IsNullOrEmpty(attribute?.Description) ? value.ToString() : attribute.Description;
                }
            }
            return string.Empty;
        }

        public static bool GetValueFromDescription<T>(string description, Type t, out T value)
        {
            if (description != null)
            {
                var values = Enum.GetValues(t);
                foreach (var item in values.Cast<object>()
                    .Where(item => item.ToString() == description || GetDescription(item, t) == description)) {
                    value = (T) item;
                    return true;
                }
            }
            value = default(T);
            return false;
        }
    }
}
