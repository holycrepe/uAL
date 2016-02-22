using System;
using System.ComponentModel;

namespace Torrent.Converters
{
    public class TypeTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType.IsAssignableFrom(typeof(string));

        // Try to load the type from the current assembly (wUAL.exe), Torrent.dll, or uAL.dll
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
                                       object value) => ConvertFromName(value?.ToString());

        public static Type ConvertFromName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) {
                return null;
            }
            var t = GetTypeFromAssembly(typeName, "Torrent") ??
                    GetType(typeName) ??
                    GetTypeFromAssembly(typeName, "uAL");
            return t;
        }

        static Type GetTypeFromAssembly(string typeName, Type knownType)
        {
            var assemblyName = knownType.AssemblyQualifiedName;
            return GetTypeFromAssembly(typeName, assemblyName.Replace(knownType.FullName + ", ", ""));
        }

        static Type GetType(string typeName) 
            => Type.GetType(typeName, false);

        static Type GetTypeFromAssembly(string typeName, string assemblyName)
            => GetType($"{typeName}, {assemblyName}");
    }
}
