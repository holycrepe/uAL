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

        public static IEnumerable<FileInfo> EnumerateFilesSafe(this DirectoryInfo di, string searchPattern,
                                                           SearchOption searchOption = SearchOption.TopDirectoryOnly)
            => di.IsReparsePoint()
                ? di.GetFinal().EnumerateFilesSafe(searchPattern, searchOption)
                : di.Exists
                    ? di.EnumerateFiles(searchPattern, searchOption)
                    : new FileInfo[0];

        public static IEnumerable<DirectoryInfo> EnumerateDirectoriesSafe(this DirectoryInfo di, string searchPattern,
                                                           SearchOption searchOption = SearchOption.TopDirectoryOnly)
            => di.IsReparsePoint()
                ? di.GetFinal().EnumerateDirectoriesSafe(searchPattern, searchOption)
                : di.Exists
                    ? di.EnumerateDirectories(searchPattern, searchOption)
                    : new DirectoryInfo[0];

        public static FileInfo[] GetFilesSafe(this DirectoryInfo di, string searchPattern,
                                                           SearchOption searchOption = SearchOption.TopDirectoryOnly)
            => di.IsReparsePoint()
                ? di.GetFinal().GetFilesSafe(searchPattern, searchOption)
                : di.Exists
                    ? di.GetFiles(searchPattern, searchOption)
                    : new FileInfo[0];

        public static DirectoryInfo[] GetDirectoriesSafe(this DirectoryInfo di, string searchPattern,
                                                           SearchOption searchOption = SearchOption.TopDirectoryOnly)
            => di.IsReparsePoint()
                ? di.GetFinal().GetDirectoriesSafe(searchPattern, searchOption)
                : di.Exists
                    ? di.GetDirectories(searchPattern, searchOption)
                    : new DirectoryInfo[0];
    }
}
