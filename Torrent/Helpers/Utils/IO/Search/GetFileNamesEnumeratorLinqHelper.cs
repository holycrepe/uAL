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
        class GetFileNamesEnumeratorLinqHelper
        {
            readonly string SearchPattern;
            readonly object lockThis = new object();

            public GetFileNamesEnumeratorLinqHelper(string searchPattern) { this.SearchPattern = searchPattern; }

            public IEnumerable<string> GetFileList(string rootFolderPath)            
                => DirectoryUtils.EnumerateFiles(rootFolderPath, this.SearchPattern)
                                .Union(DirectoryUtils.EnumerateDirectories(rootFolderPath, "*", SearchOption.AllDirectories)
                                                .AsParallel()
                                                //.WithDegreeOfParallelism(Environment.ProcessorCount * 2)
                                                .SelectMany(d =>
#if TRACE_FSSU
                {
                Log($"Names.Linq", SearchPattern, d);
                    return
#endif
                                                            DirectoryUtils.EnumerateFiles(d, this.SearchPattern)
#if TRACE_FSSU
                ;}
#endif
                                           ));
            

            public IEnumerable<string> GetFileList(IEnumerable<string> directories)
                => directories.SelectMany(GetFileList);
        }
    }
}