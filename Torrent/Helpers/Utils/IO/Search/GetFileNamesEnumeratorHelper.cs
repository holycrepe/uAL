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
        class GetFileNamesEnumeratorHelper
        {
            readonly string SearchPattern;
            IEnumerable<string> _files;
            readonly object lockThis = new object();

            public GetFileNamesEnumeratorHelper(string searchPattern) { this.SearchPattern = searchPattern; }

            public IEnumerable<string> GetFileList(string rootFolderPath)
            {
                AddFileList(rootFolderPath);
                return this._files;
            }

            public IEnumerable<string> GetFileList(IEnumerable<string> rootFolderPaths)
            {
                AddFileList(rootFolderPaths);
                return this._files;
            }

            void AddFileList(string directory)
            {
#if TRACE_FSSU
                Log($"Names.Enum", SearchPattern, directory);
#endif
                directory = DirectoryUtils.Resolve(directory);
                if (!Directory.Exists(directory))
                    return;
                var files = Directory.EnumerateFiles(directory, this.SearchPattern, SearchOption.TopDirectoryOnly);
                lock (this.lockThis)
                {
                    this._files = this._files?.Concat(files) ?? files;
                }

                var directories = Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly);
                AddFileList(directories);
            }

            void AddFileList(IEnumerable<string> directories)
                => Parallel.ForEach(directories, AddFileList);
        }
    }
}