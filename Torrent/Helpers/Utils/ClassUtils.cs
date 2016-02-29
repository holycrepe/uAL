using System.Reflection;

namespace Torrent.Helpers.Utils
{
    using Infrastructure;
    using System.Linq;
    using Extensions;
    using System.Collections.Generic;
    using Puchalapalli.Extensions.Collections;
    using Puchalapalli.Infrastructure.Interfaces;

    public static class ClassUtils
    {
        public static object GetProperty<T>(T source, string path)
        {
            if (path == null) {
                return source;
            }
            object current = source;
            var paths = path.Split('.');
            return (from currentPath in paths
                    let currentType = current.GetType()
                    select
                        currentType.GetProperty(currentPath,
                                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                       .Any(pi => pi == null || (current = pi.GetValue(current)) == null)
                       ? null
                       : current;
        }

        public static string GetPropertyValue<T>(T source, string path)
        {
            var value = GetProperty(source, path);
            return (value as IDebuggerDisplay)?.DebuggerDisplaySimple()
                   ?? (value as List<object>)?.FormatList()
                   ?? (value as List<string>)?.FormatList()
                   ?? value?.ToString();
        }
    }
}
