using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Torrent;
using Torrent.Extensions;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;
using UTorrentRestAPI;
using uAL.Infrastructure;
using uAL.Queue;
using uAL.Services;
using Torrent.Queue;

namespace uAL.Torrents
{
    using System.Threading;
    using Properties.Settings.LibSettings;
    using Torrent.Enums;
    using Torrent.Helpers.StringHelpers;
    using Torrent.Infrastructure.InfoReporters;
    using uAL.Properties.Settings.ToggleSettings;
    using static uAL.Properties.Settings.LibSettings.LibSettings;
    using static NLog.LogManager;
    using static uAL.Properties.Settings.ToggleSettings.ToggleSettings;
    using System.Collections.Concurrent;
    public enum ProcessQueueStatus
    {
        DidNotStart,
        Error,
        PreProcessed,
        Success
    }

    public struct ProcessQueueResult
    {
        public ProcessQueueStatus Status { get; }
        public ProcessQueueResult(ProcessQueueStatus status) { Status = status; }
    }

    public class TorrentQueueMonitor : QueueMonitor<TorrentQueueItem>
    {
        const int MAX_RETRIES_LOCK = 60;

        public readonly TrulyObservableCollection<TorrentQueueItem> Queue =
            new TrulyObservableCollection<TorrentQueueItem>();

        readonly string downloadDir;
        readonly UTorrentClient uTorrentClient;
        static readonly object _locker = new object();
#pragma warning disable 0414
        string lastQueueItemChanged = null;
#pragma warning restore 0414
        QueryDuplicateFileNames duplicateFinder;
        TorrentQueuer queuer;
        internal static InfoReporter infoReporter;
        public override MonitorTypes QueueType => MonitorTypes.Torrent;

        internal static Logger loggerBase = GetLogger("FSM.Torrents");
        static Logger dupeLogger = GetLogger("FSM.Dupes");
        TorrentLabel computedLabel;


        static Logger logger = GetLogger("Simple.FileSystemMonitor.Torrents");

        static LogEventInfoCloneable logEventClassBase = new LogEventInfoCloneable(LogLevel.Info, logger.Name,
                                                                                   "Torrent Info");

        static LogEventInfoCloneable getLogEventSubject(string subject, Dictionary<string, object> newEventDict = null) => logEventClassBase.Clone(subject: subject, newEventDict: newEventDict);

        bool doLogInfo = false;

        public TorrentQueueMonitor(UTorrentClient uTorrentClient, InfoReporter InfoReporter)
        {
            infoReporter = InfoReporter.SetLogger(loggerBase);
            this.uTorrentClient = uTorrentClient;
            downloadDir = LibSetting.Directories.DOWNLOAD;
            duplicateFinder = new QueryDuplicateFileNames(downloadDir);
            Queue.CollectionChanged += Queue_CollectionChanged;
        }

#pragma warning disable 1998
        public override async Task Start(bool logStartup = false)
        {
            doLogInfo = logStartup;
            if (TOGGLES.QueueFilesOnStartup) {
                //await QueueAllFiles(true);
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
            Watcher.Filter = "*.torrent";
            Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            Watcher.EnableRaisingEvents = true;
            Watcher.IncludeSubdirectories = true;

            Watcher.Created += (s, e) => AddFileFromWatcher(e.FullPath);
        }

        void Queue_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace) {
                foreach (var item in e.NewItems.Cast<TorrentQueueItem>()) {
                    if (!item.Valid) {
                        Log("***Queue Item Invalidated", item.TorrentName);
                        lock (_locker) {
                            Queue.Remove(item);
                        }
                    }
                    //                    else if (lastQueueItemChanged != item.TorrentName) {
                    //						lastQueueItemChanged = item.TorrentName;
                    //						Log("Queue Item Changed", item.LastUpdatedProperty, item.TorrentName);
                    //					}
                    // item.UpdatePath();
                }
            }
        }

        async void AddFileFromWatcher(string fileName)
        {
            if (doLogInfo) {
                loggerBase.INFO("FileSystemMonitor Found Torrent: " + fileName);
            }
            var newQueueItem = await AddFile(fileName);
            if (!TOGGLES.Processing.PreviewMode && TOGGLES.Processing.Automated.OnWatcher) {
                await ProcessQueueItem(newQueueItem);
            }
        }

        public async Task<ProcessQueueResult> ProcessQueueItem(TorrentQueueItem item,
                                                           QueueOnCompleteHandler<TorrentQueueItem>
                                                               OnProcessQueueItemComplete = null)
        {
            if (item == null || item.Status.IsSuccess) {
                return new ProcessQueueResult(ProcessQueueStatus.DidNotStart);
            }
            Func<ProcessQueueStatus, ProcessQueueResult> ProcessQueueComplete;
            Func<ProcessQueueStatus, ProcessQueueResult> processQueueStatusToResultFunc =
                (r) => new ProcessQueueResult(r);
            if (OnProcessQueueItemComplete == null) {
                ProcessQueueComplete = processQueueStatusToResultFunc;
            } else {
                ProcessQueueComplete = r =>
                                       {
                                           OnProcessQueueItemComplete(item);
                                           return processQueueStatusToResultFunc(r);
                                       };
            }

            if (item.Status.IsQueued) {
                await PreProcessFile(item);
                return ProcessQueueComplete(ProcessQueueStatus.PreProcessed);
            }
            var success = false;
            if (TOGGLES.Processing.Enabled) {
                Func<string, string, Task> OnTorrentAddComplete = 
                    async (f, l) =>  {
                        var result =
                        await FileSystemUtils.MoveAddedFile(item.File, addedDir, item.Label,
                        TOGGLES.Processing.MoveProcessedFiles, doLogInfo, f, l);
                        if (!result.Status.IsError()) {
                            item.FileName = result.NewFileName;
                        }
                    };
                success = true;
                if (!item.Status.IsInvalid) {
                    success = await uTorrentClient.AddFile(item, doLogInfo, OnTorrentAddComplete);
                } else if (item.Status.IsDupe) {
                    await OnTorrentAddComplete(item.FileName, item.Label.Computed);
                }
            }
            if (success) {
                item.Status = QueueStatus.Success;
                return ProcessQueueComplete(ProcessQueueStatus.Success);
            }
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            UI.StartNew(() => infoReporter.ReportAndLogError("Error Adding Torrent to uTorrent",
                                           $"Could not add torrent {item.Label.Base}: {item.TorrentName}"));
#pragma warning restore CS4014            
            return ProcessQueueComplete(ProcessQueueStatus.Error);
        }

        public override Task ProcessQueue(IEnumerable<object> SelectedItems, 
                                          QueueOnStartHandler OnStart = null,
                                          QueueOnProgressChangedHandler<TorrentQueueItem> OnProgressChanged = null,
                                          QueueWorkerOnCompleteHandler OnQueueBackgroundWorkerComplete = null)
            =>
                ProcessQueue((TorrentQueueItem[]) SelectedItems.Cast<TorrentQueueItem>().ToArray().Clone(), OnStart,
                             OnProgressChanged);

        public delegate Task<int> ProcessQueueItemDelegate(TorrentQueueItem item);

        public delegate void ProcessQueueItemAddTaskDelegate(Task<int> task);

        protected override async Task PerformUpdate(int newCount, string method, string methodVerb)
        {
            //await UI.StartNew(() => {
            //    infoReporter.ReportInfoBanner($"{QueueType}.{method}: {methodVerb} {newCount} {QueueType}s");
            //    if (newCount > 0)
            //    {
            //        infoReporter.ReportText($"Updating UTorrentClient's Torrents Collection");
            //    }
            //});
#pragma warning disable 4014
            base.PerformUpdate(newCount, method, methodVerb);
#pragma warning restore 4014
            if (newCount > 0) {
                Log("Updating UTorrentClient's Torrents Collection");
                var currentCount = await uTorrentClient.Update($"TorrentQueueMonitor.{method}");
                Log("Completed Updating UTorrentClient's Torrents Collection", "", $"{currentCount} Torrents Exist");
                //infoReporter.ReportText($"Completed Updating UTorrentClient's Torrents Collection: {currentCount} Torrents Exist");
            }
        }

        public async Task ProcessQueue(IEnumerable<TorrentQueueItem> QueueItems, QueueOnStartHandler OnStart = null,
                                       QueueOnProgressChangedHandler<TorrentQueueItem> OnProgressChanged = null)
        {
            //        	uTorrentClient.BeginUpdate();
            var itemsArray = QueueItems.ToArray();
            var newCount = itemsArray.Length;
            OnStart?.Invoke(newCount);
            await PerformUpdate(newCount, nameof(ProcessQueue), "Processing");
            var tasks = new ConcurrentQueue<Task<ProcessQueueResult>>();
            QueueOnCompleteHandler<TorrentQueueItem> OnProcessQueueItemComplete =
                (s) => OnProgressChanged(NewQueueWorkerState(s));            
            Func<TorrentQueueItem, Task<ProcessQueueResult>> doProcessQueue =
                item => ProcessQueueItem(item, OnProcessQueueItemComplete);
            Action<TorrentQueueItem> doAddTask =
                item => tasks.Enqueue(doProcessQueue(item));

            using (uTorrentClient.DeferUpdates.On) {
                
                //Action<TorrentQueueItem> doProcessQueueAddTaskSafe = item => Interlocked.Add(doProcessQueue(item));
                var method = ProcessQueueMethod.Default.Value();
                switch (method) {
                    case ProcessQueueMethod.Default:
                    case ProcessQueueMethod.Plain:
                        foreach (var item in itemsArray) {
                            doAddTask(item);
                        }
                        break;
                    case ProcessQueueMethod.Parallel:
                        var opts = new ParallelOptions { MaxDegreeOfParallelism = Math.Min(MAX_DEGREE_OF_PARALLELISM, newCount) };
                        var loopResult = Parallel.ForEach(itemsArray, opts, doAddTask);
                        break;
                    case ProcessQueueMethod.PLINQ:
                        itemsArray.AsParallel()
                            .WithDegreeOfParallelism(Math.Min(MAX_DEGREE_OF_PARALLELISM, newCount))
                            .ForAll(doAddTask);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                await Task.WhenAll(tasks);
            }
            //            uTorrentClient.EndUpdate();
        }

        async Task<TorrentQueueItem> QueueFile(TorrentQueueItem newQueueItem, QueueStatusMember status = null,
                                   QueueOnCompleteHandler<TorrentQueueItem> OnQueueFileComplete = null)
        {
            if (status != null) {
                newQueueItem.Status = status;
            }
            if (OnQueueFileComplete != null) {
                OnQueueFileComplete(newQueueItem);
            } else {
                //lock (_locker)
                //{
                //    Queue.Add(newQueueItem);
                //}
            }
            await UI.StartNew(() =>
                        {
                            lock (_locker) {
                                Queue.Add(newQueueItem);
                            }
                        });
            if (TOGGLES.Processing.PreProcess) {
                //await PreProcessFile(newQueueItem);
            }
#if LOG_QUEUE_ITEM
			newQueueItem.Log();
			#endif
            return newQueueItem;
        }

        internal override void NewQueueItem(object newQueueItemObject, bool addToQueue)
        {
            var newQueueItem = newQueueItemObject as TorrentQueueItem;
            if (newQueueItem == null) {
                Debugger.Break();
            } else if (addToQueue) {
                lock (_locker) {
                    Queue.Add(newQueueItem);
                }
            } else {
                Queue.ItemPropertyChanged(newQueueItem);
            }
        }

        async Task PreProcessFile(TorrentQueueItem newQueueItem)
        {
            if (newQueueItem.Status.IsQueued) {
                newQueueItem.Status = await CheckDupes(newQueueItem, computedLabel.Base);
#if LOG_QUEUE_ITEM_DUPE
				const int logPadding = 18;
				string logString = null;
				if (newQueueItem.Status == TorrentQueueStatus.TorrentDupe) {
					logString = String.Format("{0," + logPadding + "} {1}\n", "Already in uTorrent Queue:", newQueueItem.TorrentName);
				} else if (newQueueItem.Status == TorrentQueueStatus.QueueDupe) {
					logString = String.Format("{0," + logPadding + "} {1}\n", "Already in uAL Queue:", newQueueItem.TorrentName);
				} else if (newQueueItem.Status == TorrentQueueStatus.Dupe) {
					logString = String.Format("{0," + logPadding + "} {1}\n{2," + logPadding + "} {3}\n", "Dupe Found:", newQueueItem.TorrentName, "Location:", duplicateFinder.RootLabel + duplicateFinder.MatchBase);
				}
				if (logString != null) {
					loggerBase.INFO(logString);
#if LOG_QUEUE_ITEM
					newQueueItem.Log(null);
#endif
				}
#endif
            }
        }

        internal async Task<TorrentQueueItem> AddFile(string fileName, bool liveFile = true, int fileNumber = 0,
                                                      QueueOnCompleteHandler<TorrentQueueItem> OnQueueFileComplete =
                                                          null)
        {
            var newQueueItem = new TorrentQueueItem(activeDir, addedDir, fileName, fileNumber: fileNumber);
            await newQueueItem.InitializePath();
            return await AddFile(newQueueItem, liveFile, OnQueueFileComplete);
        }

        async Task<TorrentQueueItem> AddFile(TorrentQueueItem newQueueItem, bool liveFile = true,
                                             QueueOnCompleteHandler<TorrentQueueItem> OnQueueFileComplete = null)
        {
            var isCreated = false;
            var isZeroLength = false;
            var retries = 0;
            var fileName = newQueueItem.FileName;
            TorrentQueueItem existingQueueItem;
            lock (_locker) {
                existingQueueItem = Queue.Get(q => q.FileName == fileName);
            }

            if (existingQueueItem != null) {
                return null;
            }

            var simpleLabel = TorrentLabelService.CreateSimpleTorrentLabel(activeDir, newQueueItem);
            if (!LibSetting.Labels.Filter(simpleLabel.Base, TOGGLES)) {
                return null;
            }

            while (!isCreated && retries++ < MAX_RETRIES_LOCK) {
                try {
                    if (liveFile || retries > 1) {
                        await Task.Delay(500);
                            // FSW workaround, file has not finished, let's wait a bit since they're just torrent files.
                        loggerBase.Warn("Pausing execution for 500 ms for torrent: " + fileName);
                    }
                    using (
                        var torrentStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None,
                                                           bufferSize: 4096, useAsync: true)) {
                        // Just testing
                        if (torrentStream.Length > 0) {
                            isCreated = true;
                        } else {
                            isZeroLength = true;
                        }
                    }
                } catch (FileNotFoundException ex) {
                    loggerBase.Error(ex, "Couldn't open the torrent file: " + fileName);
                } catch (UnauthorizedAccessException ex) {
                    loggerBase.Error(ex, "Couldn't access the torrent file: " + fileName);
                } catch (Exception ex) {
                    loggerBase.Error(ex, "Unknown exception occured while accessing the torrent file " + fileName);
                }
            }

            computedLabel = TorrentLabelService.CreateTorrentLabel(activeDir, newQueueItem);

            if (retries == MAX_RETRIES_LOCK) {
                loggerBase.Error("Giving up tring to load torrent " + fileName
                                 + (isZeroLength ? " (Seems like it is a zero length file)" : ""));
                return await QueueFile(newQueueItem, TorrentQueueStatus.LoadError, OnQueueFileComplete);
            }

            if (newQueueItem.Info == null) {
                Debugger.Break();
            } else {
                lock (_locker) {
                    existingQueueItem = Queue.Get(q => q.Info.Hash == newQueueItem.Info.Hash);
                }
                if (existingQueueItem != null) {
                    newQueueItem.Dupe = existingQueueItem.File;
                    return await QueueFile(newQueueItem, TorrentQueueStatus.QueueDupe, OnQueueFileComplete);
                }
            }

            return await QueueFile(newQueueItem,
                             string.IsNullOrEmpty(computedLabel.Computed)
                                 ? TorrentQueueStatus.NoLabel
                                 : TorrentQueueStatus.Queued, OnQueueFileComplete);
        }

        async Task<QueueStatusMember> CheckDupes(TorrentQueueItem queueItem, string label = null)
        {
            if (!FLAGS.DUPE_CHECK.GLOBAL) {
                return TorrentQueueStatus.Ready;
            }
            queueItem.UpdateTorrentInfo();
            var fileName = queueItem.FileName;
            if (!queueItem.Info.success) {
                return (queueItem.Info.IsBDecodeError
                            ? TorrentQueueStatus.TorrentBDecodeError
                            : TorrentQueueStatus.TorrentInfoError);
            }
            // Log("Check Dupes", queueItem.Label.Base, queueItem.TorrentName);
            if (FLAGS.DUPE_CHECK.UTORRENT 
                && LibSetting.Torrents.CheckAlreadyQueuedInUTorrent 
                && await uTorrentClient.Contains(queueItem)) {
                return TorrentQueueStatus.TorrentDupe;
            }
            if (!TOGGLES.Processing.CheckDupes || !FLAGS.DUPE_CHECK.FILES) {
                return TorrentQueueStatus.Ready;
            }
            var torrentFileInfo = queueItem.Info.Largest;
            //var start = DateTime.Now;
            var result = await Task.Run(() => duplicateFinder.QueryDuplicates(torrentFileInfo.Name, torrentFileInfo.Length, label));
            //var end = DateTime.Now;
            //var length = end - start;
            // dupeLogger.Info(String.Format("{0,3}s Elapsed While Dupe Checking {3,30} {1} {2:F}", Math.Round(length.TotalSeconds), Path.GetFileNameWithoutExtension(fileName), (result ? "\n" + new String(' ', 30) + "**DUPLICATE FOUND**" : ""), label));
            if (result) {
                queueItem.Dupe = result.Dupe;
                if (queueItem.FileName.Contains("yoko"))
                {
                    Debugger.Break();
                }
                return TorrentQueueStatus.Dupe;
            }
            return TorrentQueueStatus.Ready;
        }

        public override void Clear()
        {
            lock (_locker) {
                Queue.Clear();
            }
        }

        public override int Count => Queue.Count;


        public override async Task QueueAllFiles(bool isStartup, QueueOnStartHandler OnStart = null,
                                                 QueueOnProgressChangedHandler<TorrentQueueItem> OnProgressChanged =
                                                     null)
        {
            LibSetting.Queue.HaveQueuedAllTorrents = true;
            var files = Directory.EnumerateFiles(activeDir, @"*.torrent", SearchOption.AllDirectories).ToArray();
            var count = files.Length;
            OnStart?.Invoke(count);
            await PerformUpdate(count, nameof(QueueAllFiles), "Queueing");
            uTorrentClient.DeferUpdates.Begin();
            //uTorrentClient.BeginUpdate();
            var processFiles = !TOGGLES.Processing.PreviewMode
                               && (isStartup ? TOGGLES.Processing.Automated.Startup : TOGGLES.Processing.Automated.Manual);

            queuer = new TorrentQueuer(this, files, processFiles, uTorrentClient.DeferUpdates.End, OnProgressChanged);
            //queuer = new TorrentQueuer(this, files, processFiles, uTorrentClient.EndUpdate, OnProgressChanged);
            var totalFiles = await queuer.Run();
            return;
        }
    }
}
