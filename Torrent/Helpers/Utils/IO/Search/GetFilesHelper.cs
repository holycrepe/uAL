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
        class GetFilesHelper
        {
            readonly string SearchPattern;
            readonly List<FileInfo> _files = new List<FileInfo>();
            readonly object lockThis = new object();

            public GetFilesHelper(string searchPattern) { this.SearchPattern = searchPattern; }

            public List<FileInfo> GetFileList(string rootFolderPath)
            {
                AddFileList(rootFolderPath);
                return this._files;
            }

            public List<FileInfo> GetFileList(IEnumerable<string> rootFolderPaths)
            {
                AddFileList(rootFolderPaths);
                return this._files;
            }

            void AddFileList(string directory)
            {
#if TRACE_FSSU
                Log($"Files.Plain", SearchPattern, directory);
#endif
                var dInfo = DirectoryUtils.GetInfo(directory);
                if (!dInfo.Exists)
                    return;
                var files = dInfo.GetFiles(this.SearchPattern, SearchOption.TopDirectoryOnly);
                lock (this.lockThis)                
                    this._files.AddRange(files);                
                var directories = Directory.GetDirectories(dInfo.FullName, "*", SearchOption.TopDirectoryOnly);
                AddFileList(directories);
            }

            void AddFileList(IEnumerable<string> directories)
                => Parallel.ForEach(directories, AddFileList);
        }
    }
}