using System.Reflection;

namespace Torrent.Helpers.Utils
{
    using System.Linq;

    public static class ClassUtils
    {
        public static object GetProperty<T>(T source, string path)
        {
            if (path == null)
            {
                return source;
            }
            object current = source;
            var paths = path.Split('.');
            return (from currentPath in paths let currentType = current.GetType() select currentType.GetProperty(currentPath, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)).Any(pi => pi == null || (current = pi.GetValue(current)) == null) ? null : current;
        }
            
    }
}
