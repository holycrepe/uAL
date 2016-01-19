﻿using System;
using System.ComponentModel;
namespace Torrent.Converters
{
    public class TypeTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType.IsAssignableFrom(typeof(string));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
            // Try to load the type from the current assembly (wUAL.exe), Torrent.dll, or uAL.dll
            return ConvertFromName(value?.ToString());
		}

        public static Type ConvertFromName(string typeName)
        {            
            if (string.IsNullOrEmpty(typeName))
            {
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
        {
            return Type.GetType(typeName, false);
        }
        static Type GetTypeFromAssembly(string typeName, string assemblyName)
        {
            return GetType($"{typeName}, {assemblyName}");
        }
	}
}


