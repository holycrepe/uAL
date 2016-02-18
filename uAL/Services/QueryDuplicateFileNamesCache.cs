#define TRACE_DUPE

using System;
using System.IO;
using Torrent.Helpers.Utils;
using Torrent.Extensions;
using Torrent.Infrastructure.FileSystem;
using System.Linq;
using static Torrent.Helpers.Utils.DebugUtils;
using System.Diagnostics;
using Torrent.Enums;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace uAL.Services
{
    using static Properties.Settings.LibSettings.LibSettings;
    public static class QueryDuplicateFileNamesCache
    {
        private const int MAX_DEGREE_OF_PARALLELISM = 63;
        private const bool INITIALIZE_FILES = true;
        private const bool INITIALIZE_FILE_NAMES = false;
        private static ConcurrentDictionary<FileSearchOptions, FileSearchResult<string>> FileNameCache =
            new ConcurrentDictionary<FileSearchOptions, FileSearchResult<string>>();
        private static ConcurrentDictionary<FileSearchOptions, bool> FileNameCacheInProgress = 
            new ConcurrentDictionary<FileSearchOptions, bool>();


        private static ConcurrentDictionary<FileSearchOptions, FileSearchResult<FileInfo>> FileCache =
            new ConcurrentDictionary<FileSearchOptions, FileSearchResult<FileInfo>>();
        private static ConcurrentDictionary<FileSearchOptions, bool> FileCacheInProgress =
            new ConcurrentDictionary<FileSearchOptions, bool>();

        public static void Clear()
        {
            FileNameCache.Clear();
            FileCache.Clear();
        }
        #region Initialize        
        public static Task InitializeExtensions(Action<int, TimeSpan> callback=null, params string[] extensions)
            => InitializeExtensions(LibSetting.Labels.GetRootDirectoryInfo(LibSetting.Directories.DOWNLOAD), callback, extensions);
        public static async Task InitializeExtensions(RootDirectoryInfo roots, Action<int, TimeSpan> callback=null, params string[] extensions)
        {
            if (!roots.Valid || (!INITIALIZE_FILE_NAMES && !INITIALIZE_FILES))
            {
                return;
            }
            var stopwatch = Stopwatch.StartNew();
            if (extensions.Length == 0)
            {
                extensions = new string[] { ".mkv", ".mpg", ".mp4", ".avi", ".wmv", ".flv", ".!ut" };
            }
            var tasks = new ConcurrentQueue<Task>();
            Action<string> doAddTask =
                extension => {
#pragma warning disable 0162
                    if (INITIALIZE_FILE_NAMES)
                    {
                        tasks.Enqueue(Task.Run(() => GetFileNames(roots, extension, false)));
                    }
                    if (INITIALIZE_FILES)
                    {
                        tasks.Enqueue(Task.Run(() => GetFiles(roots, extension, false)));
                    }
#pragma warning restore 0162
                };
            var newCount = extensions.Length;
            var method = ProcessQueueMethod.Default.Value();
            switch (method)
            {
                case ProcessQueueMethod.Default:
                case ProcessQueueMethod.Plain:
                    foreach (var extension in extensions)
                    {
                        doAddTask(extension);
                    }
                    break;
                case ProcessQueueMethod.Parallel:
                    var opts = new ParallelOptions { MaxDegreeOfParallelism = Math.Min(MAX_DEGREE_OF_PARALLELISM, newCount) };
                    var loopResult = Parallel.ForEach(extensions, opts, doAddTask);
                    break;
                case ProcessQueueMethod.PLINQ:
                    extensions.AsParallel()
                        .WithDegreeOfParallelism(Math.Min(MAX_DEGREE_OF_PARALLELISM, newCount))
                        .ForAll(doAddTask);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            await Task.WhenAll(tasks);
            callback?.Invoke(extensions.Length, stopwatch.Elapsed);
        }
        #endregion
        #region Get Files

        public static string[] GetFileNames(RootDirectoryInfo roots, string extension, bool debug=true)
        {
            var key = new FileSearchOptions(roots, "*" + extension);
            string method = nameof(GetFileNames);
            var inProgress = FileNameCacheInProgress.ContainsKey(key);
            Stopwatch stopwatch;

            if (inProgress) {
                stopwatch = Stopwatch.StartNew();
                //Log(method, key.SearchPattern, "Waiting for Cache");
                while (FileNameCacheInProgress.ContainsKey(key))
                {
                    Thread.Sleep(5000);
                }
                //Log(method, key.SearchPattern, "Waiting Completed in " + stopwatch.FormatFriendly());
                stopwatch = null;
            }
            if (FileNameCache.ContainsKey(key))
            {
                var result = FileNameCache[key];
                var resultHash = result.Roots.GetHashCode();
                var keyHash = key.Roots.GetHashCode();
                if (resultHash == keyHash)
                {
                    return result.Files;
                }
                DEBUG.Break();
            }

            FileNameCacheInProgress[key] = true;
            // if (debug) { Log(method, key.SearchPattern, "Start Cache"); }
            stopwatch = Stopwatch.StartNew();
            var files = FileSystemSearchUtils.GetFileNames(key.Roots.Directories, key.SearchPattern)?.ToArray();
            if (debug) { Log(method, key.SearchPattern, "Caching Completed in " + stopwatch.FormatFriendly()); }
            stopwatch = null;
            if (files == null) {
                DEBUG.Break();
                return new string[] {};
            }
            FileNameCache[key] = new FileSearchResult<string>(key.Roots, files);
            bool b;
            if (!FileNameCacheInProgress.TryRemove(key, out b))
            {
                // DEBUG.Break();
            }
            return files;
        }
        public static FileInfo[] GetFiles(RootDirectoryInfo roots, string extension, bool debug = true)
        {
            var key = new FileSearchOptions(roots, "*" + extension);
            string method = nameof(GetFiles);
            var inProgress = FileCacheInProgress.ContainsKey(key);
            Stopwatch stopwatch;

            if (inProgress)
            {
                stopwatch = Stopwatch.StartNew();
                if (debug) { Log(method, key.SearchPattern, "Waiting for Cache"); }
                while (FileCacheInProgress.ContainsKey(key))
                {
                    Thread.Sleep(5000);
                }
                //Log(method, key.SearchPattern, "Waiting Completed in " + stopwatch.FormatFriendly());
                stopwatch = null;
            }
            if (FileCache.ContainsKey(key))
            {
                var result = FileCache[key];
                if (result.Roots.Equals(key.Roots))
                {
                    return result.Files;
                }
                DEBUG.Break();
            }

            FileCacheInProgress[key] = true;
            if (debug) { Log(method, key.SearchPattern, "Start Cache"); }
            stopwatch = Stopwatch.StartNew();
            var files = FileSystemSearchUtils.GetFiles(key.Roots.Directories, key.SearchPattern)?.ToArray();
            if (debug) { Log(method, key.SearchPattern, "Caching Completed in " + stopwatch.FormatFriendly()); }
            stopwatch = null;
            if (files == null)
            {
                DEBUG.Break();
                return new FileInfo[] { };
            }
            FileCache[key] = new FileSearchResult<FileInfo>(key.Roots, files);
            bool b;
            if (!FileCacheInProgress.TryRemove(key, out b))
            {
                DEBUG.Break();
            }
            return files;
        }

        #endregion

        #region Find Files

        public static string FindFileName(RootDirectoryInfo roots, string fileName, string extension = null)
            => FileSystemSearchUtils.FindFileName(
                                                  GetFileNames(roots, extension ?? Path.GetExtension(fileName))
                                                      .Where(s => s.EndsWith(fileName))
                );

        public static FileInfo FindFile(RootDirectoryInfo roots, string fileName, Func<FileInfo, bool> predicate = null,
                                        string extension = null)
            => FileSystemSearchUtils.FindFile(
                                              GetFiles(roots, extension ?? Path.GetExtension(fileName)),
                                              fi => fi.FullName.EndsWith(fileName),
                                              predicate
                );

        #endregion

        #region Log

        [Conditional("DEBUG"), Conditional("TRACE_EXT"), Conditional("TRACE_DUPE")]
        static void Log(string title, string text = null, string item = null,
                        PadDirection textPadDirection = PadDirection.Default, string textSuffix = null,
                        PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            =>
                LogUtils.Log("QueryDupes", title, text, item, textPadDirection, textSuffix, titlePadDirection,
                             titleSuffix, random);

        #endregion
    }
}
