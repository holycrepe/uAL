#define TRACE_FSSUx
#define TRACE_FSSU2x
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using Torrent.Helpers.Utils;

namespace Torrent.Helpers.Utils.IO
{
    using Enums;
    using Extensions;
    using Infrastructure.FileSystem;
    using System.Diagnostics;

    public static partial class FileSystemSearchUtils
    {
        class GetFileNamesHelper
        {
            readonly string SearchPattern;
            readonly List<string> _files = new List<string>();
            readonly object lockThis = new object();

            public GetFileNamesHelper(string searchPattern) { this.SearchPattern = searchPattern; }

            public List<string> GetFileList(string rootFolderPath)
            {
                AddFileList(rootFolderPath);
                return this._files;
            }

            public List<string> GetFileList(IEnumerable<string> rootFolderPaths)
            {
                AddFileList(rootFolderPaths);
                return this._files;
            }

            void AddFileList(string directory)
            {
#if TRACE_FSSU
                Log($"Names.Plain", SearchPattern, directory);
#endif
                directory = DirectoryUtils.Resolve(directory);
                if (!Directory.Exists(directory))
                    return;
                var files = Directory.GetFiles(directory, this.SearchPattern, SearchOption.TopDirectoryOnly);
                lock (this.lockThis)
                {
                    this._files.AddRange(files);
                }

                var directories = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);
                AddFileList(directories);
            }

            void AddFileList(IEnumerable<string> directories)
                => Parallel.ForEach(directories, AddFileList);
        }
    }
}