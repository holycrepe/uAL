using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Torrent.Infrastructure;
using Torrent.Extensions;

namespace Torrent.Helpers.Utils.IO
{
    public static partial class FileSystemUtils
    {
        public static IEnumerable<string> EnumerateFullNames(string path, string searchPattern = "*",
            SearchOption options = SearchOption.TopDirectoryOnly)
            => Directory.EnumerateFiles(path, searchPattern, options).Select(fn => Path.Combine(path, fn));
    }
}
