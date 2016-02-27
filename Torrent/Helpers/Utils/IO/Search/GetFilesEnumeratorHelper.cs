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
        class GetFilesEnumeratorLinqHelper
        {
            readonly string SearchPattern;
            readonly object lockThis = new object();

            public GetFilesEnumeratorLinqHelper(string searchPattern) { this.SearchPattern = searchPattern; }

            public IEnumerable<FileInfo> GetFileList(string rootFolderPath)
            {
                var directory = DirectoryUtils.GetInfo(rootFolderPath);
                return !directory.Exists
                    ? new FileInfo[0]
                    : directory.EnumerateFiles(this.SearchPattern)
                               .Union(directory.EnumerateDirectories("*", SearchOption.AllDirectories)
                                               .AsParallel()
                                          //.WithDegreeOfParallelism(Environment.ProcessorCount * 2)
                                               .SelectMany(di =>
#if TRACE_FSSU
                {
                Log($"Files.Linq", SearchPattern, di.FullName);
                    return
#endif
                                                               di.EnumerateFilesSafe(this.SearchPattern)
#if TRACE_FSSU
                ;}
#endif
                                          ));
            }

            public IEnumerable<FileInfo> GetFileList(IEnumerable<string> directories)
                => directories.SelectMany(GetFileList);
        }
    }
}