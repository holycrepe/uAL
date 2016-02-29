namespace Torrent.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public static class MemberInfoExtensions
    {

        static object[] GetCustomAttributesTyped<T>(this MemberInfo member) where T : Attribute
            => member.GetCustomAttributes(typeof(T), false);
        public static T[] GetCustomAttributes<T>(this MemberInfo member) where T : Attribute
            => (T[])GetCustomAttributesTyped<T>(member);
        //public static T GetCustomAttribute<T>(this MemberInfo member) where T : Attribute
        //    => (T)GetCustomAttributesTyped<T>(member).FirstOrDefault(v => v is T);
        public static string GetMemberDescription(this MemberInfo member)
            => member.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }
}