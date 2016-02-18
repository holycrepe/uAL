﻿#define TRACE_FSSUx
#define TRACE_FSSU2x
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using Torrent.Helpers.Utils;

namespace Torrent.Helpers.Utils
{
    using Enums;
    using Extensions;
    using Infrastructure.FileSystem;
    using System.Diagnostics;

    public static class FileSystemSearchUtils
    {
        #region Helper Classes

        class GetFileNamesHelper
        {
            readonly string SearchPattern;
            readonly List<string> _files = new List<string>();
            readonly object lockThis = new object();

            public GetFileNamesHelper(string searchPattern) { SearchPattern = searchPattern; }

            public List<string> GetFileList(string rootFolderPath)
            {
                AddFileList(rootFolderPath);
                return _files;
            }

            public List<string> GetFileList(IEnumerable<string> rootFolderPaths)
            {
                AddFileList(rootFolderPaths);
                return _files;
            }

            void AddFileList(string directory)
            {
#if TRACE_FSSU
                Log($"Names.Plain", SearchPattern, directory);
#endif
                var dInfo = new DirectoryInfo(directory);
                var files = Directory.GetFiles(directory, SearchPattern, SearchOption.TopDirectoryOnly);
                lock (lockThis) {
                    _files.AddRange(files);
                }

                var directories = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);
                AddFileList(directories);
            }

            void AddFileList(IEnumerable<string> directories)
                => Parallel.ForEach(directories, AddFileList);
        }

        class GetFilesHelper
        {
            readonly string SearchPattern;
            readonly List<FileInfo> _files = new List<FileInfo>();
            readonly object lockThis = new object();

            public GetFilesHelper(string searchPattern) { SearchPattern = searchPattern; }

            public List<FileInfo> GetFileList(string rootFolderPath)
            {
                AddFileList(rootFolderPath);
                return _files;
            }

            public List<FileInfo> GetFileList(IEnumerable<string> rootFolderPaths)
            {
                AddFileList(rootFolderPaths);
                return _files;
            }

            void AddFileList(string directory)
            {
#if TRACE_FSSU
                Log($"Files.Plain", SearchPattern, directory);
#endif
                var dInfo = new DirectoryInfo(directory);
                var files = dInfo.GetFiles(SearchPattern, SearchOption.TopDirectoryOnly);
                lock (lockThis) {
                    _files.AddRange(files);
                }

                var directories = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);
                AddFileList(directories);
            }

            void AddFileList(IEnumerable<string> directories)
                => Parallel.ForEach(directories, AddFileList);
        }

        class GetFileNamesEnumeratorHelper
        {
            readonly string SearchPattern;
            IEnumerable<string> _files;
            readonly object lockThis = new object();

            public GetFileNamesEnumeratorHelper(string searchPattern) { SearchPattern = searchPattern; }

            public IEnumerable<string> GetFileList(string rootFolderPath)
            {
                AddFileList(rootFolderPath);
                return _files;
            }

            public IEnumerable<string> GetFileList(IEnumerable<string> rootFolderPaths)
            {
                AddFileList(rootFolderPaths);
                return _files;
            }

            void AddFileList(string directory)
            {
#if TRACE_FSSU
                Log($"Names.Enum", SearchPattern, directory);
#endif
                var files = Directory.EnumerateFiles(directory, SearchPattern, SearchOption.TopDirectoryOnly);
                lock (lockThis) {
                    _files = _files == null ? files : _files.Concat(files);
                }

                var directories = Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly);
                AddFileList(directories);
            }

            void AddFileList(IEnumerable<string> directories)
                => Parallel.ForEach(directories, AddFileList);
        }

        class GetFilesEnumeratorHelper
        {
            readonly string SearchPattern;
            IEnumerable<FileInfo> _files;
            readonly object lockThis = new object();

            public GetFilesEnumeratorHelper(string searchPattern) { SearchPattern = searchPattern; }

            public IEnumerable<FileInfo> GetFileList(string rootFolderPath)
            {
                AddFileList(rootFolderPath);
                return _files;
            }

            public IEnumerable<FileInfo> GetFileList(IEnumerable<string> rootFolderPaths)
            {
                AddFileList(rootFolderPaths);
                return _files;
            }

            void AddFileList(string folderPath) { AddFileList(new DirectoryInfo(folderPath)); }

            void AddFileList(DirectoryInfo directory)
            {
#if TRACE_FSSU
                Log($"Files.Enum", SearchPattern, directory.FullName);
#endif
                var files = directory.EnumerateFiles(SearchPattern, SearchOption.TopDirectoryOnly);
                lock (lockThis) {
                    _files = _files == null ? files : _files.Concat(files);
                }

                var directories = directory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                AddFileList(directories);
            }

            void AddFileList(IEnumerable<string> directories)
                => Parallel.ForEach(directories, AddFileList);

            void AddFileList(IEnumerable<DirectoryInfo> directories)
                => Parallel.ForEach(directories, AddFileList);
        }

        class GetFileNamesEnumeratorLinqHelper
        {
            readonly string SearchPattern;
            readonly object lockThis = new object();

            public GetFileNamesEnumeratorLinqHelper(string searchPattern) { SearchPattern = searchPattern; }

            public IEnumerable<string> GetFileList(string rootFolderPath)
            {
                var directory = new DirectoryInfo(rootFolderPath);
                return Directory.EnumerateFiles(rootFolderPath, SearchPattern)
                                .Union(Directory.EnumerateDirectories(rootFolderPath, "*", SearchOption.AllDirectories)
                                                .AsParallel()
                                           //.WithDegreeOfParallelism(Environment.ProcessorCount * 2)
                                                .SelectMany(d =>
#if TRACE_FSSU
                {
                Log($"Names.Linq", SearchPattern, d);
                    return
#endif
                                                            Directory.EnumerateFiles(d, SearchPattern,
                                                                                     SearchOption.TopDirectoryOnly)
#if TRACE_FSSU
                ;}
#endif
                                           ));
            }

            public IEnumerable<string> GetFileList(IEnumerable<string> directories)
                => directories.SelectMany(GetFileList);
        }

        class GetFilesEnumeratorLinqHelper
        {
            readonly string SearchPattern;
            readonly object lockThis = new object();

            public GetFilesEnumeratorLinqHelper(string searchPattern) { SearchPattern = searchPattern; }

            public IEnumerable<FileInfo> GetFileList(string rootFolderPath)
            {
                var directory = new DirectoryInfo(rootFolderPath);
                return directory.EnumerateFiles(SearchPattern)
                                .Union(directory.EnumerateDirectories("*", SearchOption.AllDirectories).AsParallel()
                                           //.WithDegreeOfParallelism(Environment.ProcessorCount * 2)
                                                .SelectMany(di =>
#if TRACE_FSSU
                {
                Log($"Files.Linq", SearchPattern, di.FullName);
                    return
#endif
                                                            di.EnumerateFiles(SearchPattern,
                                                                              SearchOption.TopDirectoryOnly)
#if TRACE_FSSU
                ;}
#endif
                                           ));
            }

            public IEnumerable<FileInfo> GetFileList(IEnumerable<string> directories)
                => directories.SelectMany(GetFileList);
        }

        #endregion

        #region Get Files List

        public static IEnumerable<string> GetFileNames(string rootDirectory, string searchPattern,
                                                       GetFileMethod method = GetFileMethod.Default)
        {
            switch (method.Value()) {
                case GetFileMethod.Plain:
                    return GetFileNamesPlain(rootDirectory, searchPattern);
                case GetFileMethod.Enumerator:
                    return GetFileNamesEnumerator(rootDirectory, searchPattern);
                default:
                    return GetFileNamesEnumeratorLinq(rootDirectory, searchPattern);
            }
        }

        public static IEnumerable<string> GetFileNames(IEnumerable<string> rootDirectories, string searchPattern,
                                                       GetFileMethod method = GetFileMethod.Default)
        {
            switch (method.Value()) {
                case GetFileMethod.Plain:
                    return GetFileNamesPlain(rootDirectories, searchPattern);
                case GetFileMethod.Enumerator:
                    return GetFileNamesEnumerator(rootDirectories, searchPattern);
                default:
                    return GetFileNamesEnumeratorLinq(rootDirectories, searchPattern);
            }
        }

        public static IEnumerable<FileInfo> GetFiles(string rootDirectory, string searchPattern,
                                                     GetFileMethod method = GetFileMethod.Default)
        {
            switch (method.Value()) {
                case GetFileMethod.Plain:
                    return GetFilesPlain(rootDirectory, searchPattern);
                case GetFileMethod.Enumerator:
                    return GetFilesEnumerator(rootDirectory, searchPattern);
                default:
                    return GetFilesEnumeratorLinq(rootDirectory, searchPattern);
            }
        }

        public static IEnumerable<FileInfo> GetFiles(IEnumerable<string> rootDirectories, string searchPattern,
                                                     GetFileMethod method = GetFileMethod.Default)
        {
            switch (method.Value()) {
                case GetFileMethod.Plain:
                    return GetFilesPlain(rootDirectories, searchPattern);
                case GetFileMethod.Enumerator:
                    return GetFilesEnumerator(rootDirectories, searchPattern);
                default:
                    return GetFilesEnumeratorLinq(rootDirectories, searchPattern);
            }
        }

        public static List<string> GetFileNamesPlain(string rootDirectory, string searchPattern)
        {
            var getFilesHelper = new GetFileNamesHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectory);
        }

        public static List<string> GetFileNamesPlain(IEnumerable<string> rootDirectories, string searchPattern)
        {
            var getFilesHelper = new GetFileNamesHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectories);
        }

        public static List<FileInfo> GetFilesPlain(string rootDirectory, string searchPattern)
        {
            var getFilesHelper = new GetFilesHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectory);
        }

        public static List<FileInfo> GetFilesPlain(IEnumerable<string> rootDirectories, string searchPattern)
        {
            var getFilesHelper = new GetFilesHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectories);
        }

        public static IEnumerable<string> GetFileNamesEnumerator(string rootDirectory, string searchPattern)
        {
            var getFilesHelper = new GetFileNamesEnumeratorHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectory);
        }

        public static IEnumerable<string> GetFileNamesEnumerator(IEnumerable<string> rootDirectories,
                                                                 string searchPattern)
        {
            var getFilesHelper = new GetFileNamesEnumeratorHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectories);
        }

        public static IEnumerable<FileInfo> GetFilesEnumerator(string rootDirectory, string searchPattern)
        {
            var getFilesHelper = new GetFilesEnumeratorHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectory);
        }

        public static IEnumerable<FileInfo> GetFilesEnumerator(IEnumerable<string> rootDirectories, string searchPattern)
        {
            var getFilesHelper = new GetFilesEnumeratorHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectories);
        }

        public static IEnumerable<string> GetFileNamesEnumeratorLinq(string rootDirectory, string searchPattern)
        {
            var getFilesHelper = new GetFileNamesEnumeratorLinqHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectory);
        }

        public static IEnumerable<string> GetFileNamesEnumeratorLinq(IEnumerable<string> rootDirectories,
                                                                     string searchPattern)
        {
            var getFilesHelper = new GetFileNamesEnumeratorLinqHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectories);
        }

        public static IEnumerable<FileInfo> GetFilesEnumeratorLinq(string rootDirectory, string searchPattern)
        {
            var getFilesHelper = new GetFilesEnumeratorLinqHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectory);
        }

        public static IEnumerable<FileInfo> GetFilesEnumeratorLinq(IEnumerable<string> rootDirectories,
                                                                   string searchPattern)
        {
            var getFilesHelper = new GetFilesEnumeratorLinqHelper(searchPattern);
            return getFilesHelper.GetFileList(rootDirectories);
        }

        #endregion

        #region Find File Base Methods

        public static T FindFileName<T>(IEnumerable<T> files)
        {
#if TRACE_FSSU
            var count = files.Count();    
            if (count == 0) {
            Log($"FindFileName", "<Empty>");
            }
            else {
            Log($"***FindFileName", files.FirstOrDefault().ToString(), count.ToString());
            }
#endif
            return files.FirstOrDefault();
        }

        public static FileInfo FindFile(IEnumerable<FileInfo> files, Func<FileInfo, bool> predicate1 = null,
                                        Func<FileInfo, bool> predicate2 = null)
        {
#if TRACE_FSSU
            var count = files.Count();    
            if (count == 0) {
            Log($"FindFile", "<Empty>");
            }
            else {
            Log($"***FindFile", files.FirstOrDefault().FullName, count.ToString());
            }
#endif
            if (predicate1 == null) {
                return FindFileName(files);
            }
            Func<FileInfo, bool> predicate;
            if (predicate2 == null) {
                predicate = predicate1;
            } else {
                predicate = (fi) => predicate1(fi) && predicate2(fi);
            }
            FileInfo retval = null;
            Parallel.ForEach(files, (fi, loopState) =>
                                    {
                                        if (predicate(fi)) {
                                            retval = fi;
                                            loopState.Stop();
                                        }
                                    });
            return retval;
        }

        #endregion

        #region Find File Implementations

        public static string FindFileName(DirectoryInfo rootDirectory, string searchPattern,
                                          GetFileMethod method = GetFileMethod.Default)
        {
            return FindFileName(rootDirectory.FullName, searchPattern, method);
        }

        public static string FindFileName(string rootDirectory, string searchPattern,
                                          GetFileMethod method = GetFileMethod.Default)
        {
            switch (method.Value()) {
                case GetFileMethod.Plain:
                    return FindFileNamePlain(rootDirectory, searchPattern);
                case GetFileMethod.Enumerator:
                    return FindFileNameEnumerator(rootDirectory, searchPattern);
                default:
                    return FindFileNameEnumeratorLinq(rootDirectory, searchPattern);
            }
        }

        public static string FindFileName(IEnumerable<string> rootDirectories, string searchPattern,
                                          GetFileMethod method = GetFileMethod.Default)
        {
#if TRACE_FSSU
            Log($"FindFilename.{method.Value()}", searchPattern, rootDirectories.FormatList());
#endif

            switch (method.Value()) {
                case GetFileMethod.Plain:
                    return FindFileNamePlain(rootDirectories, searchPattern);
                case GetFileMethod.Enumerator:
                    return FindFileNameEnumerator(rootDirectories, searchPattern);
                default:
                    return FindFileNameEnumeratorLinq(rootDirectories, searchPattern);
            }
        }

        public static FileInfo FindFile(DirectoryInfo rootDirectory, string searchPattern,
                                        Func<FileInfo, bool> predicate = null,
                                        GetFileMethod method = GetFileMethod.Default)
        {
            return FindFile(rootDirectory.FullName, searchPattern, predicate, method);
        }

        public static FileInfo FindFile(string rootDirectory, string searchPattern,
                                        Func<FileInfo, bool> predicate = null,
                                        GetFileMethod method = GetFileMethod.Default)
        {
            switch (method.Value()) {
                case GetFileMethod.Plain:
                    return FindFilePlain(rootDirectory, searchPattern, predicate);
                case GetFileMethod.Enumerator:
                    return FindFileEnumerator(rootDirectory, searchPattern, predicate);
                default:
                    return FindFileEnumeratorLinq(rootDirectory, searchPattern, predicate);
            }
        }

        public static FileInfo FindFile(IEnumerable<string> rootDirectories, string searchPattern,
                                        Func<FileInfo, bool> predicate = null,
                                        GetFileMethod method = GetFileMethod.Default)
        {
#if TRACE_FSSU
            Log($"FindFile.{method.Value()}", rootDirectories.FormatList());
#endif
            switch (method.Value()) {
                case GetFileMethod.Plain:
                    return FindFilePlain(rootDirectories, searchPattern, predicate);
                case GetFileMethod.Enumerator:
                    return FindFileEnumerator(rootDirectories, searchPattern, predicate);
                default:
                    return FindFileEnumeratorLinq(rootDirectories, searchPattern, predicate);
            }
        }

        public static string FindFileNamePlain(string rootDirectory, string searchPattern)
        {
            return FindFileName(GetFileNamesPlain(rootDirectory, searchPattern));
        }

        public static string FindFileNamePlain(IEnumerable<string> rootDirectory, string searchPattern)
        {
            return FindFileName(GetFileNamesPlain(rootDirectory, searchPattern));
        }

        public static FileInfo FindFilePlain(string rootDirectory, string searchPattern,
                                             Func<FileInfo, bool> predicate = null)
        {
            return FindFile(GetFilesPlain(rootDirectory, searchPattern), predicate);
        }

        public static FileInfo FindFilePlain(IEnumerable<string> rootDirectories, string searchPattern,
                                             Func<FileInfo, bool> predicate = null)
        {
            return FindFile(GetFilesPlain(rootDirectories, searchPattern), predicate);
        }

        public static string FindFileNameEnumerator(string rootDirectory, string searchPattern)
        {
            return FindFileName(GetFileNamesEnumerator(rootDirectory, searchPattern));
        }

        public static string FindFileNameEnumerator(IEnumerable<string> rootDirectories, string searchPattern)
        {
            return FindFileName(GetFileNamesEnumerator(rootDirectories, searchPattern));
        }

        public static FileInfo FindFileEnumerator(string rootDirectory, string searchPattern,
                                                  Func<FileInfo, bool> predicate = null)
        {
            return FindFile(GetFilesEnumerator(rootDirectory, searchPattern), predicate);
        }

        public static FileInfo FindFileEnumerator(IEnumerable<string> rootDirectories, string searchPattern,
                                                  Func<FileInfo, bool> predicate = null)
        {
            return FindFile(GetFilesEnumerator(rootDirectories, searchPattern), predicate);
        }

        public static string FindFileNameEnumeratorLinq(string rootDirectory, string searchPattern)
        {
            return FindFileName(GetFileNamesEnumeratorLinq(rootDirectory, searchPattern));
        }

        public static string FindFileNameEnumeratorLinq(IEnumerable<string> rootDirectories, string searchPattern)
        {
            return FindFileName(GetFileNamesEnumeratorLinq(rootDirectories, searchPattern));
        }

        public static FileInfo FindFileEnumeratorLinq(string rootDirectory, string searchPattern,
                                                      Func<FileInfo, bool> predicate = null)
        {
            return FindFile(GetFilesEnumeratorLinq(rootDirectory, searchPattern), predicate);
        }

        public static FileInfo FindFileEnumeratorLinq(IEnumerable<string> rootDirectories, string searchPattern,
                                                      Func<FileInfo, bool> predicate = null)
        {
            return FindFile(GetFilesEnumeratorLinq(rootDirectories, searchPattern), predicate);
        }

        #endregion

        #region Log

        [Conditional("DEBUG"), Conditional("TRACE_EXT")]
        static void Log(string title, string text = null, string item = null,
                        PadDirection textPadDirection = PadDirection.Default, string textSuffix = null,
                        PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            =>
                LogUtils.Log("FileSearch", title, text, item, textPadDirection, textSuffix, titlePadDirection,
                             titleSuffix, random);

        [Conditional("DEBUG"), Conditional("TRACE_EXT"), Conditional("LOG"), Conditional("TRACE_FSSU"),
         Conditional("TRACE_FSSU2")]
        public static void Test(RootDirectoryInfo roots)
        {
            //if (ActiveDownloadDirectory != "")
            //{
            //    FileSystemSearchUtils.Test(GetRootDirectoryInfo(ActiveDownloadDirectory));
            //}
#if UAL_DEV || LOG || TRACE_EXT || DEBUG || TRACE_FSSU || TRACE_FSSU2
            Test(roots, "*");
            Test(roots, "*.mkv");
            Test(roots, "*.avi");
            Test(roots, "*.wmv");
            Test(roots, "*.mpg");
            #endif
        }

        [Conditional("DEBUG"), Conditional("TRACE_EXT"), Conditional("LOG"), Conditional("TRACE_FSSU"),
         Conditional("TRACE_FSSU2")]
        public static void Test(RootDirectoryInfo roots, string extension)
        {
#if UAL_DEV || LOG || TRACE_EXT || DEBUG || TRACE_FSSU || TRACE_FSSU2
            var i1 = GC.GetTotalMemory(true);
            Log($"Test Start GetFiles {extension}");
            var stopwatch = new Stopwatch();
            var files = GetFileNames(roots.Directories, extension).ToArray();
            var timeFiles = stopwatch.Elapsed.FormatFriendly();
            stopwatch.Stop();
            Log($"Test End GetFiles {extension}");
            var i2 = GC.GetTotalMemory(true);
            var filesMemory = i2 - i1;
            Log(" ");
            Log("==================================");
            Log($"Get Files {extension}:");
            Log("==================================");
            Log("Count", files.Length.ToString());
            Log("Time", timeFiles);
            Log("Size", filesMemory.ToFileSize());
            Log(" ");
        #endif
        }

        #endregion
    }
}