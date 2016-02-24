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

        public static string GetNameWithoutExtension(this FileInfo fi)
            => fi.Name.TrimEnd("." + fi.Extension);

        public static string GetFullNameWithoutExtension(this FileInfo fi)
            => fi.FullName.TrimEnd("." + fi.Extension);
        public static FileInfo GetFinal(this FileInfo fi)
            => fi.IsReparsePoint() ? new FileInfo(PathUtils.GetFinalPath(fi.FullName)) : fi;
        public static DirectoryInfo GetFinal(this DirectoryInfo di)
            => di.IsReparsePoint() ? new DirectoryInfo(PathUtils.GetFinalPath(di.FullName)) : di;
    }
}
