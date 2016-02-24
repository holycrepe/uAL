using System.IO;

namespace Torrent.Helpers.Utils
{
    using Extensions;
    using static PathUtils;
    public static class DirectoryUtils
    {
        public static DirectoryInfo GetInfo(params string[] paths)
            => new DirectoryInfo(Combine(paths)).GetFinal();
        public static string Resolve(params string[] paths)
            => GetInfo(paths).FullName;
    }
}
