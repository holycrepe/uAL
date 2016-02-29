using Torrent.Infrastructure.Reflection;

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
    public static partial class TypeExtensions
    {
        #region Generic Types
        static Type GetBaseGenericType(this Type type, bool getTypeDefinition, params Type[] typeParameters)
        {
            var numArguments = typeParameters.Length;
            var baseType = type;
            while (true)
            {                
                if (baseType.IsGenericType)
                {
                    var generic = baseType.GetGenericTypeDefinition();
                    if (numArguments == 0 || numArguments == generic.GetGenericArguments().Length)
                    {
                        return getTypeDefinition ? generic : baseType;
                    }
                }
                if (baseType.BaseType == null || baseType.BaseType == typeof(object))
                {
                    throw new ArgumentTypeException($"Could not find generic type definition in {type.GetFriendlyFullName()} or its base types", nameof(type));
                }
                baseType = baseType.BaseType;
            }
        }
        public static Type GetBaseGenericType(this Type type, params Type[] typeParameters)
            => type.GetBaseGenericType(false, typeParameters);
        public static Type GetBaseGenericTypeDefinition(this Type type, params Type[] typeParameters)
            => type.GetBaseGenericType(true, typeParameters);
        public static Type MakeGenericTypeFromBase(this Type type, params Type[] typeParameters) 
            => type.GetBaseGenericTypeDefinition(typeParameters)
            .MakeGenericType(typeParameters);
        #endregion
        #region Property Info

        public static IEnumerable<PropertyInfo> GetPublicPropertyInfo(this Type type)
            => type?.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        public static IEnumerable<Member<object>> GetPublicProperties(this Type type)
            => type.GetPublicProperties<object>();
        public static IEnumerable<Member<T>> GetPublicProperties<T>(this Type type, Func<object, T> valueSelector = null)
            => type?.GetPublicPropertyInfo().Select(pi => new Member<T>(pi, type, valueSelector));
        #endregion
        #region Field Info
        public static IEnumerable<FieldInfo> GetPublicFieldInfo(this Type type)
            => type?.GetFields(BindingFlags.Public | BindingFlags.Static);

        public static IEnumerable<Member<object>> GetPublicFields(this Type type)
            => type.GetPublicFields<object>();
        public static IEnumerable<Member<T>> GetPublicFields<T>(this Type type, Func<object, T> valueSelector = null)
            => type?.GetPublicFieldInfo().Select(fi => new Member<T>(fi, type, valueSelector));
        public static string[] GetNames(this Type type)
            => type?.GetPublicFieldInfo().Select(x => x.Name).ToArray();
        public static object[] GetValues(this Type type)
            => type?.GetPublicFieldInfo().Select(x => x.GetValue(type)).ToArray();
        public static string[] GetDescriptions(this Type type)
            => type?.GetPublicFieldInfo().Select(x => x.GetMemberDescription()).ToArray();
        public static object[] GetDisplays(this Type type)
            => type?.GetPublicFieldInfo().Select(x => x.GetDisplay(type)).ToArray();
        #endregion
        #region Attributes
        static object[] GetAttributesTyped<T>(this Type type) where T : Attribute
            => type?.GetCustomAttributes(typeof(T), true);
        public static T[] GetAttributes<T>(this Type type) where T : Attribute
            => (T[]) GetAttributesTyped<T>(type);
        public static T GetAttribute<T>(this Type type) where T : Attribute
            => (T) GetAttributesTyped<T>(type).FirstOrDefault(v => v is T && v != null);
        #endregion
        #region Friendly Name
        private static readonly Dictionary<Type, string> _typeToFriendlyName = new Dictionary<Type, string>
                                                                               {
                                                                                   {typeof (string), "string"},
                                                                                   {typeof (object), "object"},
                                                                                   {typeof (bool), "bool"},
                                                                                   {typeof (byte), "byte"},
                                                                                   {typeof (char), "char"},
                                                                                   {typeof (decimal), "decimal"},
                                                                                   {typeof (double), "double"},
                                                                                   {typeof (short), "short"},
                                                                                   {typeof (int), "int"},
                                                                                   {typeof (long), "long"},
                                                                                   {typeof (sbyte), "sbyte"},
                                                                                   {typeof (float), "float"},
                                                                                   {typeof (ushort), "ushort"},
                                                                                   {typeof (uint), "uint"},
                                                                                   {typeof (ulong), "ulong"},
                                                                                   {typeof (void), "void"}
                                                                               };

        public static string GetFriendlyFullName(this Type type)
            => $"{type.Namespace}.{type.GetFriendlyName()}";

        public static string GetFriendlyName(this Type type)
        {
            string friendlyName;
            if (_typeToFriendlyName.TryGetValue(type, out friendlyName)) {
                return friendlyName;
            }

            friendlyName = type.Name;
            if (type.IsGenericType) {
                int backtick = friendlyName.IndexOf('`');
                if (backtick > 0) {
                    friendlyName = friendlyName.Remove(backtick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; i++) {
                    string typeParamName = typeParameters[i].GetFriendlyName();
                    friendlyName += (i == 0 ? typeParamName : ", " + typeParamName);
                }
                friendlyName += ">";
            }

            if (type.IsArray) {
                return type.GetElementType().GetFriendlyName() + "[]";
            }

            return friendlyName;
        }
        #endregion
    }
}
