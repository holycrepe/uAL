using System;
using System.Collections.Generic;
using System.IO;
using Torrent.Helpers.Utils;

namespace Torrent.Extensions
{
    using Helpers.StringHelpers;

    public static class FileSystemInfoExtensions
    {
        public static bool IsReparsePoint(this FileSystemInfo fi)
            => fi.Attributes.HasFlag(FileAttributes.ReparsePoint);
        public static FileInfo GetFinal(this FileInfo fi)
            => fi.IsReparsePoint() ? new FileInfo(PathUtils.GetFinalPath(fi.FullName)) : fi;
        public static DirectoryInfo GetFinal(this DirectoryInfo di)
            => di.IsReparsePoint() ? new DirectoryInfo(PathUtils.GetFinalPath(di.FullName)) : di;
    }
}
