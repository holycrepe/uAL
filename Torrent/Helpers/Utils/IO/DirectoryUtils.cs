using System.IO;

namespace Torrent.Helpers.Utils
{
    using System.Collections;
    using System.Collections.Generic;
    using Extensions;
    using static PathUtils;
    public static class DirectoryUtils
    {
        public static DirectoryInfo GetInfo(params string[] paths)
            => new DirectoryInfo(Combine(paths)).GetFinal();
        public static string Resolve(params string[] paths)
            => GetInfo(paths).FullName;

        public static bool Exists(params string[] paths)
            => Directory.Exists(Resolve(paths));

        public static IEnumerable<string> EnumerateFiles(string directory)
        {
            directory = Resolve(directory);
            return Directory.Exists(directory)
                ? Directory.EnumerateFiles(directory) 
                : new string[0];
        }

        public static IEnumerable<string> EnumerateFiles(string directory, string searchPattern,
                                                         SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            directory = Resolve(directory);
            return Directory.Exists(directory)
                ? Directory.EnumerateFiles(directory, searchPattern, searchOption)
                : new string[0];
        }
        public static IEnumerable<string> EnumerateDirectories(string directory)
        {
            directory = Resolve(directory);
            return Directory.Exists(directory)
                ? Directory.EnumerateDirectories(directory)
                : new string[0];
        }

        public static IEnumerable<string> EnumerateDirectories(string directory, string searchPattern,
                                                         SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            directory = Resolve(directory);
            return Directory.Exists(directory)
                ? Directory.EnumerateDirectories(directory, searchPattern, searchOption)
                : new string[0];
        }

        public static FileInfo[] GetFiles(string directory, string searchPattern, SearchOption searchOption)
            => GetInfo(directory).GetFilesSafe(searchPattern, searchOption);

        public static DirectoryInfo[] GetDirectories(string directory, string searchPattern, SearchOption searchOption)
            => GetInfo(directory).GetDirectoriesSafe(searchPattern, searchOption);
    }
}
