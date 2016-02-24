using System.IO;

namespace Torrent.Helpers.Utils
{
    using Extensions;
    using static PathUtils;
    public static class FileUtils
    {
        public static FileInfo GetInfo(params string[] paths)
            => new FileInfo(Combine(paths)).GetFinal();
        public static string Resolve(params string[] paths)
            => GetInfo(paths).FullName;
    }
}
