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
        class GetFilesEnumeratorHelper
        {
            readonly string SearchPattern;
            IEnumerable<FileInfo> _files;
            readonly object lockThis = new object();

            public GetFilesEnumeratorHelper(string searchPattern) { this.SearchPattern = searchPattern; }

            public IEnumerable<FileInfo> GetFileList(string rootFolderPath)
            {
                AddFileList(rootFolderPath);
                return this._files;
            }

            public IEnumerable<FileInfo> GetFileList(IEnumerable<string> rootFolderPaths)
            {
                AddFileList(rootFolderPaths);
                return this._files;
            }

            void AddFileList(string folderPath) { AddFileList(new DirectoryInfo(folderPath)); }

            void AddFileList(DirectoryInfo directory)
            {
#if TRACE_FSSU
                Log($"Files.Enum", SearchPattern, directory.FullName);
#endif
                directory = directory.GetFinal();
                if (!directory.Exists)
                    return;
                var files = directory.EnumerateFiles(this.SearchPattern, SearchOption.TopDirectoryOnly);
                lock (this.lockThis)                
                    this._files = this._files?.Concat(files) ?? files;
                var directories = directory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                AddFileList(directories);
            }

            void AddFileList(IEnumerable<string> directories)
                => Parallel.ForEach(directories, AddFileList);

            void AddFileList(IEnumerable<DirectoryInfo> directories)
                => Parallel.ForEach(directories, AddFileList);
        }
    }
}