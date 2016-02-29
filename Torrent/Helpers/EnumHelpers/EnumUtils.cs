using AddGenericConstraint;
using System;
using System.Collections;
using System.Linq;
using Torrent.Infrastructure.Enums;
using Torrent.Infrastructure.Reflection;

namespace Torrent.Helpers.Utils
{
    using Extensions;
    using System.ComponentModel;
    using System.Reflection;
    using System.Collections.Generic;
    using Puchalapalli.Extensions.Collections;
    using static DebugUtils;
    public static partial class EnumUtils
    {
        //public static T[] GetValues<[AddGenericConstraint(typeof(Enum))] T>() where T : struct
        //   => (T[])Enum.GetValues(typeof(T));
        [Obsolete("Use extension method", true)]
        public static string GetDescription<[AddGenericConstraint(typeof(Enum))] T>(T? value) where T : struct
                => value == null ? string.Empty : GetDescription(value.Value, typeof(T));

        public static string GetCombinedDescription(object value, Type t, EnumMemberDisplayFormat displayFormatIfNotDefined = EnumMemberDisplayFormat.Combined)
            => Enum.IsDefined(t, value) 
            ? value + GetDescription(value, t).Prefix(": ")
            : new EnumMember(t, value, displayFormatIfNotDefined).Display;
        public static string GetDescriptionOrValue(object value, Type t)
        {
            var description = GetDescription(value, t);
            return string.IsNullOrEmpty(description) ? value.ToString() : description;
        }
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

        public static object[] GetMatchingValues(Type t, object flags)
        {
            var flagValues = Convert.ToInt64(flags);
            return Enum.GetValues(t).Cast<object>().Where(item =>
            {
                var value = Convert.ToInt64(item);
                return value > 0 && flagValues.Has(value);
            }).ToArray();
        }
        public static Member<long>[] GetMatchingFields(Type t, object flags)
        {
            var flagValues = Convert.ToInt64(flags);
            var fields = t.GetPublicFields(Convert.ToInt64)
                .Where(fi => fi.Cast > 0)
                .OrderByDescending(fi => fi.Cast);
            var list = new List<Member<long>>();
            while (flagValues > 0)
            {
                var match = fields.FirstOrDefault(field => flagValues.Has(field.Cast));
                if ((match?.Cast ?? 0) == 0)
                    DEBUG.Break();
                var obj = Enum.ToObject(t, match);
                list.Add(match);
                flagValues -= match;
            }
            return list.OrderBy(field => field.Cast).ToArray();
        }
        public static object GetUsedBits(Type t, bool excludeNegativeValues=false)
            => GetUsedBits(t, Enum.GetValues(t).Cast<object>(), excludeNegativeValues);

        public static object GetUsedBits(Type t, IEnumerable<object> values, bool excludeNegativeValues = false)
            => Enum.ToObject(t, values.GetBitwiseOr(excludeNegativeValues));
    }
}
