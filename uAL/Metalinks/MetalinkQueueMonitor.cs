using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using uAL.Infrastructure;
using uAL.Queue;
using uAL.Services;
using uAL.Properties;
using uAL.Properties.Settings.ToggleSettings;
using Torrent;
using Torrent.Queue;
using Torrent.Extensions;
using NLog;
using System.Linq;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;
using System.Diagnostics;

namespace uAL.Metalinks
{
    using Properties.Settings.LibSettings;
    using System.Collections.Concurrent;
    using Torrent.Enums;
    using Torrent.Infrastructure.InfoReporters;
    using static uAL.Properties.Settings.LibSettings.LibSettings;
    public class QueueFileResult
    {
        public List<MetalinkQueueItem> QueueItems = new List<MetalinkQueueItem>();
        public string FileName;
        public void Add(MetalinkQueueItem queueItem)
            => QueueItems.Add(queueItem);
        public QueueFileResult(string fileName)
        {
            FileName = fileName;
        }
    }
    public class MetalinkQueueMonitor : QueueMonitor<MetalinkQueueItem>
    {
        new const int MAX_DEGREE_OF_PARALLELISM = 50;
        public override MonitorTypes QueueType => MonitorTypes.Metalink;

        public readonly TrulyObservableCollection<MetalinkQueueItem> Queue =
            new TrulyObservableCollection<MetalinkQueueItem>();

        bool doLogInfo = false;

        readonly QueueWorkerState<MetalinkQueueItem> workerState =
            new QueueWorkerState<MetalinkQueueItem>(MonitorTypes.Metalink);

        static readonly object _locker = new object();

        static readonly Logger loggerBase = LogManager.GetLogger("FSM.Metalinks");
        static Logger logger = LogManager.GetLogger("Simple.FileSystemMonitor.Metalinks");
        static Logger errorLogger = LogManager.GetLogger("FSM.Metalinks.Error");
        static Logger successLogger = LogManager.GetLogger("Plain.FSM.Metalinks.Success");
        static Logger progressLogger = LogManager.GetLogger("Simple.FSM.Metalinks.Progress");

        static LogEventInfoCloneable logEventClassBase = new LogEventInfoCloneable(LogLevel.Info, logger.Name,
                                                                                   "Metalink Info");

        static LogEventInfoCloneable getLogEventSubject(string subject, Dictionary<string, object> newEventDict = null)
        {
            return logEventClassBase.Clone(subject: subject, newEventDict: newEventDict);
        }


        static string lastCategory = "";
        static InfoReporter InfoReporter;


        public MetalinkQueueMonitor(InfoReporter infoReporter) { InfoReporter = infoReporter.SetLogger(loggerBase); }

        #region Initialization / Startup

#pragma warning disable 1998
        public async override Task Start(bool logStartup = false)
        {
            doLogInfo = logStartup;
            if (TOGGLES.QueueFilesOnStartup) {
                // await QueueAllFiles(true);
            }
            doLogInfo = true;

            if (TOGGLES.Watcher) {
                CreateWatcher();
            }
        }
#pragma warning restore 1998

        public void CreateWatcher()
        {
            Watcher = new FileSystemWatcher(activeDir);
            Watcher.Filter = "*.metalink";
            Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            Watcher.EnableRaisingEvents = true;
            Watcher.IncludeSubdirectories = true;

            Watcher.Created += async (s, e) =>
                                     {
                                         if (doLogInfo) {
                                             loggerBase.INFO("FileSystemMonitor Found Metalink: " + e.FullPath);
                                         }
                                         var result = await QueueFile(e.FullPath);
                                         var fileExists = result.QueueItems.Count == 0;
                                         if (doLogInfo && fileExists) {
                                             loggerBase.INFO(new string(' ', 34) + "[Metalink already exists in queue]");
                                         }
                                         if (!fileExists && !TOGGLES.Processing.PreviewMode && TOGGLES.Processing.Automated.OnWatcher) {
                                             await ProcessQueue(result.QueueItems);
                                         }
                                     };
        }

        #endregion

        internal override void NewQueueItem(object newQueueItemObject, bool addToQueue)
        {
            var newQueueItem = newQueueItemObject as MetalinkQueueItem;
            if (newQueueItem == null) {
                Debugger.Break();
            } else if (addToQueue) {
                Queue.Add(newQueueItem);
            } else {
                Queue.ItemPropertyChanged(newQueueItem);
            }
        }
        async void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
            => await DownloadFileCallback((MetalinkQueueItem)e.UserState, e.Error);

        async Task DownloadFileCallback(MetalinkQueueItem item, Exception error)
        {            
            var fi = item.Metalink;
            string fileName = fi.FullName;
            string metalinkName = Path.GetFileNameWithoutExtension(fileName);
            string torrentName = Path.GetFileNameWithoutExtension(item.Torrent.FullName);

            if (error != null) { //|| e.Cancelled) {
                item.Status = MetalinkQueueStatus.LoadError;
                //error = e.Error;
            } else {
                var torrentInfo = TorrentInfoCache.GetTorrentInfo(item.Torrent.FullName);
                if (torrentInfo.success) {
                    item.Status = MetalinkQueueStatus.Success;
                } else {
                    error = torrentInfo.error;
                    item.Status = (torrentInfo.IsBDecodeError
                                       ? MetalinkQueueStatus.TorrentBDecodeError
                                       : MetalinkQueueStatus.TorrentInfoError);
                }
            }

            var counts = new MetalinkQueueCounts(Queue, fileName);

            var eventDict = new Dictionary<string, object>
                            {
                                {"METALINK", metalinkName}
                            };

            LogEventInfoCloneable subjectEvent = getLogEventSubject(metalinkName, eventDict);

            var progressEventDict = new Dictionary<string, object>
                                    {
                                        {"TOTAL", counts.Total},
                                        {"READY", counts.Ready},
                                        {"SUCCESS", counts.Success}
                                    };

            var progressInfo =
                $"{"METALINK:",15} {metalinkName}\n{"TOTAL:",15} {counts.Total,3}\n{"READY:",15} {counts.Ready,3}  {"SUCCESS:",15} {counts.Success,3}";

            if (counts.Error > 0) {
                progressInfo += $"  {"ERRORS:",15} {counts.Error,3}";
                progressEventDict["ERRORS"] = counts.Error;
            }

            var subjectEventProgress = subjectEvent.Clone(group: "Progress", newEventDict: progressEventDict);

            var progressEvent = new LogEventInfo(LogLevel.Info, "", progressInfo);

            progressEvent.Properties["Subdirectory"] = metalinkName;


            progressLogger.LOG(progressEvent);
            logger.LOG(subjectEventProgress);

            if (item.Status.IsSuccess) {
                LogEventInfoCloneable subjectEventSuccess = subjectEventProgress.Clone(group: "Success",
                                                                                       newMessage:
                                                                                           "SUCCESS: " + torrentName);

                progressEvent.Message = "SUCCESS: " + torrentName;
                successLogger.LOG(progressEvent);
                logger.LOG(subjectEventSuccess);

                if (counts.Success == counts.Total) {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    UI.StartNew(() => InfoReporter.ReportText("Successfully downloaded all torrents for metalink " + metalinkName + ", ending with " + torrentName,
                                            LogLevel.Info));
#pragma warning restore CS4014 
                    var label = TorrentLabelService.CreateTorrentLabel(activeDir, fileName);
                    await FileSystemUtils.MoveAddedFile(fi, addedDir, label, TOGGLES.Processing.MoveProcessedFiles, doLogInfo);
                }
            } else {
                var errorEventDict = new Dictionary<string, object>
                                     {
                                         {"TORRENT:", torrentName},
                                         {"MESSAGE:", error.ToString()}
                                     };
                var errorInfo =
                    $"ERROR DOWNLOADING TORRENT:\n{"TORRENT:",27} {torrentName}\n{"METALINK:",27} {metalinkName}\n{"MESSAGE:",27} {error.ToString()}\n\n";
                var errorInfoConsole = $"{"TORRENT:",27} {torrentName}\n\n";

                LogEventInfoCloneable subjectEventError =
                    subjectEvent.Clone(newEventDict: errorEventDict).SetError(error);

#if DEBUG || TRACE_EXT
                errorLogger.ERROR(error, errorInfo);
                logger.LOG(subjectEventError);
#endif
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                UI.StartNew(() => {
                    InfoReporter.ReportError(error, "ERROR DOWNLOADING TORRENT: ");
                    InfoReporter.ReportText(errorInfoConsole);
                });
#pragma warning restore CS4014                 
            }
        }

        async Task<bool> ProcessQueue(MetalinkQueueItem item, QueueOnCompleteHandler<MetalinkQueueItem> OnProcessQueueItemComplete)
        {
            if (item == null || !item.Status.IsActivatable) {
                return false;
            }
            item.Status = MetalinkQueueStatus.Active;
            if (TOGGLES.Processing.Enabled) {
                Directory.CreateDirectory(item.Torrent.DirectoryName);                
                item.Client = new WebClient();
                // client.DownloadDataCompleted += DownloadFileCallback;
                Exception error = null;
                try {
                    await item.Client.DownloadFileTaskAsync(item.Uri, item.Torrent.FullName);
                }
                catch (Exception ex)
                {
                    error = ex;
                }
                await DownloadFileCallback(item, error);
                //client.DownloadFileCompleted += DownloadFileCallback;
                //client.DownloadFileAsync(item.Uri, item.Torrent.FullName, item);
            }
            OnProcessQueueItemComplete(item);
            return true; 
        }

        public override Task ProcessQueue(IEnumerable<object> SelectedItems, QueueOnStartHandler OnStart = null,
                                          QueueOnProgressChangedHandler<MetalinkQueueItem> OnProcessQueueComplete = null,
                                          QueueWorkerOnCompleteHandler OnQueueBackgroundWorkerComplete = null)
            =>
                ProcessQueue((MetalinkQueueItem[]) SelectedItems.Cast<MetalinkQueueItem>().ToArray().Clone(), OnStart,
                             OnProcessQueueComplete);

        public async Task ProcessQueue(IEnumerable<MetalinkQueueItem> QueueItems, QueueOnStartHandler OnStart = null,
                                          QueueOnProgressChangedHandler<MetalinkQueueItem> OnProgressChanged = null)
        {
            var itemsArray = QueueItems.ToArray();
            var newCount = itemsArray.Length;
            OnStart?.Invoke(newCount);
            await PerformUpdate(newCount, nameof(ProcessQueue), "Processing");

            var baseState = new QueueWorkerState<MetalinkQueueItem>(QueueType);
            QueueOnCompleteHandler<MetalinkQueueItem> OnProcessQueueItemComplete =
                (s) => OnProgressChanged(baseState.New(s));
            var tasks = new ConcurrentQueue<Task<bool>>();
            Func<MetalinkQueueItem, Task<bool>> doProcessQueue =
                item => ProcessQueue(item, OnProcessQueueItemComplete);
            Action<MetalinkQueueItem> doAddTask =
                item => tasks.Enqueue(doProcessQueue(item));
            //Action<TorrentQueueItem> doProcessQueueAddTaskSafe = item => Interlocked.Add(doProcessQueue(item));
            var method = ProcessQueueMethod.Default.Value();
            switch (method)
            {
                case ProcessQueueMethod.Default:
                case ProcessQueueMethod.Plain:
                    foreach (var item in itemsArray)
                    {
                        doAddTask(item);
                    }
                    break;
                case ProcessQueueMethod.Parallel:
                    var opts = new ParallelOptions { MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM };
                    var loopResult = Parallel.ForEach(itemsArray, opts, doAddTask);
                    break;
                case ProcessQueueMethod.PLINQ:
                    itemsArray.AsParallel()
                        .WithDegreeOfParallelism(MAX_DEGREE_OF_PARALLELISM)
                        .ForAll(doAddTask);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            await Task.WhenAll(tasks);
        }

        async Task<QueueFileResult> QueueFile(string fileName,
                                                      QueueOnProgressChangedHandler<MetalinkQueueItem> OnProgressChanged
                                                          = null, int number = 0, int totalTorrents = 0)
        {
            var metalink = FileUtils.GetInfo(fileName);
            var result = new QueueFileResult(fileName);

            MetalinkQueueItem existingQueueItem;
            lock (_locker)
            {
                existingQueueItem = Queue.Get(q => q.Metalink.FullName == metalink.FullName);
            }

            if (existingQueueItem != null)
            {
                return result;
            }

            var baseLabels = Path.GetFileNameWithoutExtension(fileName).Split('-').ToList();
            var lastLabelPart = baseLabels.Last();
            int num;
            if (int.TryParse(baseLabels.Last(), out num))
            {
                baseLabels.RemoveAt(baseLabels.Count - 1);
            }
            var baseLabel = string.Join("\\", baseLabels);
            if (!LibSetting.Labels.Filter(baseLabel, TOGGLES))
            {
                return result;
            }
            
            var doc = new XmlDocument();
            string fileData = File.ReadAllText(fileName);
            fileData = fileData.Replace(" xmlns=\"", " whocares=\"");
            using (var sr = new StringReader(fileData)) {
                doc.Load(sr);
            }

            XmlNodeList fileNodes = doc.SelectNodes("metalink/files/file");

            int totalFiles = 0;

            foreach (XmlNode fileNode in fileNodes) {
                XmlAttribute fileNameAttribute = fileNode.Attributes["name"];
                XmlNode urlNode = fileNode.SelectSingleNode("resources/url");
                XmlNode descriptionNode = fileNode.SelectSingleNode("description");
                XmlNode identityNode = fileNode.SelectSingleNode("identity");
                if (fileNameAttribute == null || urlNode == null) {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    UI.StartNew(() => InfoReporter.ReportText("Skipping Metalink Filenode: Missing File Name/URL", LogLevel.Error));
#pragma warning restore CS4014
                    continue;
                }
                totalFiles++;

                string path =
                    PathUtils.MakeSafe(fileNameAttribute.Value.Replace('/', '\\').TrimEnd('\\')).UnescapeHTML();
                string url = urlNode.InnerText;
                string title = (identityNode == null ? "N/A" : identityNode.InnerText);
                string description = (descriptionNode == null ? "N/A" : descriptionNode.InnerText);
                string category = description.Replace(": " + title, "");
                string fullPath = PathUtils.Shorten(activeDir, path);
                var fi = FileUtils.GetInfo(fullPath);
                var uri = new Uri(url);
                var queries = HttpUtility.ParseQueryString(uri.Query);
                string EMP_ID = queries["id"];
                //string consoleInfo = $"NEW METALINK DOWNLOAD: [{EMP_ID,-6}] {title}";
                string logInfo = $"NEW DL: [{EMP_ID,-6}] {title}";
                if (lastCategory != category) {
                    if (TOGGLES.Processing.PreviewMode) {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        UI.StartNew(() =>
                        {
                            InfoReporter.ReportText();
                            InfoReporter.ReportBanner("CATEGORY: " + category);
                            InfoReporter.ReportText();
                        });
#pragma warning restore CS4014
                    }
                    logInfo += $"\n{"CATEGORY:",37} {category}\n";
                    lastCategory = category;
                }

                var fileExists = File.Exists(fullPath);

                loggerBase.INFO(logInfo);
                var newQueueItem = new MetalinkQueueItem(metalink, category, uri, fi, totalTorrents + totalFiles);
                await UI.StartNew(() =>
                            {
                                lock (_locker) {
                                    Queue.Add(newQueueItem);
                                }
                                //InfoReporter.ReportText(consoleInfo);
                            });
                result.Add(newQueueItem);
            }
            if (OnProgressChanged != null) {
                var firstQueueItem = result.QueueItems.Count == 0 ? null : result.QueueItems[0];
                var newQueueItem = new MetalinkQueueItem(metalink,
                                                         Path.GetFileNameWithoutExtension(fileName).Replace("-", ": "),
                                                         firstQueueItem?.Uri, firstQueueItem?.Torrent ?? metalink, number);
                var newState = workerState.New(newQueueItem);
                if (newState == null) {
                    Debugger.Break();
                }
                OnProgressChanged(newState);
            }
            return result;
        }

        public override void Clear() { Queue.Clear(); }
        public override int Count => Queue.Count;
#pragma warning disable 1998
        public async override Task QueueAllFiles(bool isStartup, QueueOnStartHandler OnStart = null,
                                                 QueueOnProgressChangedHandler<MetalinkQueueItem> OnProgressChanged =
                                                     null)
        {
            LibSetting.Queue.HaveQueuedAllMetalinks = true;
            var files = Directory.EnumerateFiles(activeDir, @"*.metalink", SearchOption.AllDirectories).ToArray();
            var count = files.Length;
            OnStart?.Invoke(count);
#pragma warning disable 4014
            PerformUpdate(count, nameof(QueueAllFiles), "Queueing");
#pragma warning restore 4014

            int totalFiles = 0;
            var totalTorrents = 0;
            var logString = "Adding Metalinks: ";
            var processFiles = !TOGGLES.Processing.PreviewMode
                               && (isStartup ? TOGGLES.Processing.Automated.Startup : TOGGLES.Processing.Automated.Manual);
            var tasks = new ConcurrentQueue<Task<QueueFileResult>>();
            Action<string> doAddTask =
                filename => tasks.Enqueue(QueueFile(filename, OnProgressChanged, ++totalFiles, totalTorrents));
            

            var tasksProcessResult = new ConcurrentQueue<Task>();
            Action<QueueFileResult> doAddTaskProcessResult =
                result =>
                {
                    var torrentCount = (result == null ? -1 : result.QueueItems.Count);
                    logString +=
                        $"\n    - [{(torrentCount == -1 ? "Already Exists in Queue" : torrentCount.ToString()),3}] {Path.GetFileNameWithoutExtension(result.FileName)}";
                    if (torrentCount != -1)
                    {
                        totalTorrents += torrentCount;
                        if (processFiles)
                        {
                            tasksProcessResult.Enqueue(ProcessQueue(result.QueueItems));
                        }
                    }
                };
            var method = ProcessQueueMethod.Default.Value();
            switch (method)
            {
                case ProcessQueueMethod.Default:
                case ProcessQueueMethod.Plain:
                    foreach (var item in files)
                    {
                        doAddTask(item);
                    }
                    break;
                case ProcessQueueMethod.Parallel:
                    var opts = new ParallelOptions { MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM };
                    var loopResult = Parallel.ForEach(files, opts, doAddTask);
                    break;
                case ProcessQueueMethod.PLINQ:
                    files.AsParallel()
                        .WithDegreeOfParallelism(MAX_DEGREE_OF_PARALLELISM)
                        .ForAll(doAddTask);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var results = await Task.WhenAll(tasks);
            switch (method)
            {
                case ProcessQueueMethod.Default:
                case ProcessQueueMethod.Plain:
                    foreach (var item in results)
                    {
                        doAddTaskProcessResult(item);
                    }
                    break;
                case ProcessQueueMethod.Parallel:
                    var opts = new ParallelOptions { MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM };
                    var loopResult = Parallel.ForEach(results, opts, doAddTaskProcessResult);
                    break;
                case ProcessQueueMethod.PLINQ:
                    results.AsParallel()
                        .WithDegreeOfParallelism(MAX_DEGREE_OF_PARALLELISM)
                        .ForAll(doAddTaskProcessResult);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            await Task.WhenAll(tasksProcessResult);

            if (totalFiles > 0)
            {
                loggerBase.INFO(logString + "\n");
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                UI.StartNew(() =>
                InfoReporter.ReportText($"Finished Adding {totalTorrents} Torrents from {totalFiles} Metalinks(s)",
                                        LogLevel.Info));
#pragma warning restore CS4014
            }
            

            //foreach (var filename in files) {
            //    totalFiles++;
            //    var queueItems = await QueueFile(filename, OnProgressChanged, totalFiles, totalTorrents);
            //    var torrentCount = (queueItems == null ? -1 : queueItems.Count);
            //    logString +=
            //        $"\n    - [{(torrentCount == -1 ? "Already Exists in Queue" : torrentCount.ToString()),3}] {Path.GetFileNameWithoutExtension(filename)}";
            //    if (torrentCount != -1) {
            //        totalTorrents += torrentCount;
            //        if (processFiles) {
            //            tasks.Add(ProcessQueue(queueItems));
            //        }
            //    }
            //}
            //if (totalFiles > 0) {
            //    loggerBase.INFO(logString + "\n");
            //    await UI.StartNew(() =>
            //    InfoReporter.ReportText($"Finished Adding {totalTorrents} Torrents from {totalFiles} Metalinks(s)",
            //                            LogLevel.Info));
            //}
            //await Task.WhenAll(tasks);
        }
#pragma warning restore 1998
    }
}
