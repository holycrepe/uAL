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
    using Torrent.Enums;
    using Torrent.Helpers.StringHelpers;
    using Torrent.Infrastructure.InfoReporters;
    using uAL.Properties.Settings.ToggleSettings;
    using static uAL.Properties.Settings.LibSettings;
    using static NLog.LogManager;
    using static uAL.Properties.Settings.ToggleSettings.Toggles;

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
        public readonly TrulyObservableCollection<TorrentQueueItem> Queue = new TrulyObservableCollection<TorrentQueueItem>();
        readonly string downloadDir;
        readonly UTorrentClient uTorrentClient;
        static readonly object _locker = new object();

        readonly bool checkDupes = true;
        readonly bool checkAlreadyAdded = true;
        readonly string[] labelsToDupeCheck;
        #pragma warning disable 0414
        string lastQueueItemChanged = null;
        #pragma warning restore 0414
        QueryDuplicateFileNames duplicateFinder;
        TorrentQueuer queuer;
        internal static InfoReporter infoReporter;
        public override QueueToggleStatus QueueType => QueueToggleStatus.Torrent;

        internal static Logger loggerBase = GetLogger("FSM.Torrents");
        static Logger dupeLogger = GetLogger("FSM.Dupes");
        TorrentLabel computedLabel;
        

        static Logger logger = GetLogger("Simple.FileSystemMonitor.Torrents");

        static LogEventInfoCloneable logEventClassBase = new LogEventInfoCloneable(LogLevel.Info, logger.Name, "Torrent Info");

        static LogEventInfoCloneable getLogEventSubject(string subject, Dictionary<string, object> newEventDict = null)
        {
            return logEventClassBase.Clone(subject: subject, newEventDict: newEventDict);
        }

        bool doLogInfo = false;

        public TorrentQueueMonitor(UTorrentClient uTorrentClient, InfoReporter InfoReporter)
        {
        	infoReporter = InfoReporter.SetLogger(loggerBase);
        	this.uTorrentClient = uTorrentClient;
            downloadDir = ActiveDownloadDirectory;
            labelsToDupeCheck = LabelsToDupeCheck;
            checkDupes = LibSetting.CheckDupes;
            checkAlreadyAdded = LibSetting.CheckAlreadyExists;
        	duplicateFinder = new QueryDuplicateFileNames(downloadDir, labelsToDupeCheck);
        	Queue.CollectionChanged += Queue_CollectionChanged;
        }
        
        #pragma warning disable 1998
        public override async Task Start(bool logStartup = false) {
        	
        	doLogInfo = logStartup;
        	if (TOGGLES.QUEUE_FILES_ON_STARTUP) {
            	//await QueueAllFiles(true);
            }                        
            doLogInfo = true;      
            
             if (TOGGLES.WATCHER) {
            	CreateWatcher();
            }
        }
        #pragma warning restore 1998
        
        public void CreateWatcher() {
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
                    if (!item.Valid)
                    {
                        Log("***Queue Item Invalidated", item.LastUpdatedProperty, item.TorrentName);
                        lock (_locker)
                        {
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
            if (doLogInfo)
            {
                loggerBase.INFO("FileSystemMonitor Found Torrent: " + fileName);
            }
            var newQueueItem = await AddFile(fileName);
            if (!TOGGLES.PREVIEW_MODE && TOGGLES.PROCESS_QUEUE.ON_WATCHER)
            {
                await ProcessQueue(newQueueItem);
            }
        }

        public async Task<ProcessQueueResult> ProcessQueue(TorrentQueueItem item, Action<TorrentQueueItem> OnProcessQueueComplete=null)
		{
            if (item == null || item.Status.IsSuccess) {
                return new ProcessQueueResult(ProcessQueueStatus.DidNotStart);

            }
            Func<ProcessQueueStatus, ProcessQueueResult> ProcessQueueComplete;
            Func<ProcessQueueStatus, ProcessQueueResult> processQueueStatusToResultFunc = (r) => new ProcessQueueResult(r);
            if (OnProcessQueueComplete == null) {
                ProcessQueueComplete = processQueueStatusToResultFunc;
            }
            else
            {
                ProcessQueueComplete = r =>
                                       {
                                           OnProcessQueueComplete(item);
                                           return processQueueStatusToResultFunc(r);
                                       };
            }
            
			if (item.Status.IsQueued) {
        		await PreProcessFile(item);
                return ProcessQueueComplete(ProcessQueueStatus.PreProcessed);
			}
			var success = false;
			if (TOGGLES.PROCESS_QUEUE.ALL) {
				Func<string, string, Task> OnTorrentAddComplete = async (f, l) => {
					var result = await FileSystemUtils.MoveAddedFile(item.File, addedDir, item.Label, TOGGLES.MOVE_PROCESSED_FILES, doLogInfo, f, l);
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
			infoReporter.ReportAndLogError("Error Adding Torrent to uTorrent",
			    $"Could not add torrent {item.Label.Base}: {item.TorrentName}");
			return ProcessQueueComplete(ProcessQueueStatus.Error);
		}

        public override Task ProcessQueue(IEnumerable<object> SelectedItems, QueueOnProgressChangedHandler<TorrentQueueItem> OnProgressChanged = null)
        {        	
        	return ProcessQueue((TorrentQueueItem[])SelectedItems.Cast<TorrentQueueItem>().ToArray().Clone(), OnProgressChanged);
        }

        public delegate Task<int> ProcessQueueItemDelegate(TorrentQueueItem item);
        public delegate void ProcessQueueItemAddTaskDelegate(Task<int> task);
        

        public async Task ProcessQueue(IEnumerable<TorrentQueueItem> QueueItems, QueueOnProgressChangedHandler<TorrentQueueItem> OnProgressChanged = null)
        {
            uTorrentClient.BeginUpdate();
            var baseState = new QueueWorkerState<TorrentQueueItem>(QueueType, QueueItems.Count());
            Action<TorrentQueueItem> OnProcessQueueComplete = (s) => OnProgressChanged(baseState.New(s));
            var tasks = new List<Task<ProcessQueueResult>>();
            Func<TorrentQueueItem, Task<ProcessQueueResult>> doProcessQueue = item => ProcessQueue(item, OnProcessQueueComplete);            
            Action<TorrentQueueItem> doProcessQueueAddTask = item => tasks.Add(doProcessQueue(item));
            //Action<TorrentQueueItem> doProcessQueueAddTaskSafe = item => Interlocked.Add(doProcessQueue(item));
            var method = ProcessQueueMethod.Default.Value();
            switch (method) {
                case ProcessQueueMethod.Default:                    
                case ProcessQueueMethod.Plain:
                    foreach (var item in QueueItems) {
                        doProcessQueueAddTask(item);
                    }
                    await Task.WhenAll(tasks);
                    break;
                case ProcessQueueMethod.ParallelForEach:
                    var loopResult = Parallel.ForEach(QueueItems, doProcessQueueAddTask);
                    await Task.WhenAll(tasks);
                    break;
                case ProcessQueueMethod.ParallelForAll:
                    QueueItems.AsParallel().ForAll(doProcessQueueAddTask);
                    await Task.WhenAll(tasks);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            uTorrentClient.EndUpdate();
        }

        TorrentQueueItem QueueFile(TorrentQueueItem newQueueItem, QueueStatusMember status = null, Action<TorrentQueueItem> OnQueueFileComplete = null)
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
#pragma warning disable 4014
            UI.StartNew(() =>
                        {
                            lock (_locker) {
                                Queue.Add(newQueueItem);
                            }
                        });
#pragma warning restore 4014
            if (TOGGLES.PRE_PROCESS) {
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

        internal Task<TorrentQueueItem> AddFile(string fileName, bool liveFile = true, int fileNumber = 0, Action<TorrentQueueItem> OnQueueFileComplete = null)
        {
            var newQueueItem = new TorrentQueueItem(activeDir, addedDir, fileName, fileNumber: fileNumber);
            return AddFile(newQueueItem, liveFile, OnQueueFileComplete);
        }

        async Task<TorrentQueueItem> AddFile(TorrentQueueItem newQueueItem, bool liveFile = true, Action<TorrentQueueItem> OnQueueFileComplete = null)
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
            if (!FilterLabel(simpleLabel.Base, TOGGLES)) {
                return null;
            }
            while (!isCreated && retries++ < MAX_RETRIES_LOCK) {
                try {
                    if (liveFile || retries > 1) {
                        await Task.Delay(500); // FSW workaround, file has not finished, let's wait a bit since they're just torrent files.
                        loggerBase.Warn("Pausing execution for 500 ms for torrent: " + fileName);
                    }
                    using (var torrentStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true)) {
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
                loggerBase.Error("Giving up tring to load torrent " + fileName + (isZeroLength ? " (Seems like it is a zero length file)" : ""));
                return QueueFile(newQueueItem, TorrentQueueStatus.LoadError, OnQueueFileComplete);
            }

            if (newQueueItem.Info == null) {
                Debugger.Break();
            } else {
                lock (_locker) {
                    existingQueueItem = Queue.Get(q => q.Info.Hash == newQueueItem.Info.Hash);
                }
                if (existingQueueItem != null) {
                    newQueueItem.Dupe = existingQueueItem.File;
                    return QueueFile(newQueueItem, TorrentQueueStatus.QueueDupe, OnQueueFileComplete);
                }
            }

            return QueueFile(newQueueItem, string.IsNullOrEmpty(computedLabel.Computed) ? TorrentQueueStatus.NoLabel : TorrentQueueStatus.Queued, OnQueueFileComplete);
        }

[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
        void Log(string title, string text = null, string item = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            => LogUtils.Log("TorrentQueue", title, text, item, textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);

        async Task<QueueStatusMember> CheckDupes(TorrentQueueItem queueItem, string label = null)
        {
            if (!FLAGS.DUPE_CHECK.GLOBAL) {
                return TorrentQueueStatus.Ready;
            }
            queueItem.UpdateTorrentInfo();
            var fileName = queueItem.FileName;
            if (!queueItem.Info.success) {
                return (queueItem.Info.isBDecodeError ? TorrentQueueStatus.TorrentBDecodeError : TorrentQueueStatus.TorrentInfoError);
            }
            // Log("Check Dupes", queueItem.Label.Base, queueItem.TorrentName);
            if (FLAGS.DUPE_CHECK.UTORRENT && checkAlreadyAdded && await uTorrentClient.Contains(queueItem)) {
                return TorrentQueueStatus.TorrentDupe;
            }
            if (!checkDupes || !FLAGS.DUPE_CHECK.FILES) {
                return TorrentQueueStatus.Ready;
            }
            var torrentFileInfo = queueItem.Info.Largest;
            var start = DateTime.Now;
            bool result = duplicateFinder.QueryDuplicates(torrentFileInfo.Name, torrentFileInfo.Size, label);
            var end = DateTime.Now;
            var length = end - start;
            // dupeLogger.Info(String.Format("{0,3}s Elapsed While Dupe Checking {3,30} {1} {2:F}", Math.Round(length.TotalSeconds), Path.GetFileNameWithoutExtension(fileName), (result ? "\n" + new String(' ', 30) + "**DUPLICATE FOUND**" : ""), label));
            if (result) {
                queueItem.Dupe = duplicateFinder.Match;
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


        public override async Task QueueAllFiles(bool isStartup, QueueOnProgressChangedHandler<TorrentQueueItem> OnProgressChanged = null)
        {
            LibSetting.HaveQueuedAllTorrents = true;
            var files = Directory.EnumerateFiles(activeDir, @"*.torrent", SearchOption.AllDirectories);
            uTorrentClient.BeginUpdate();
            var processFiles = !TOGGLES.PREVIEW_MODE && (isStartup ? TOGGLES.PROCESS_QUEUE.STARTUP : TOGGLES.PROCESS_QUEUE.MANUAL);
            queuer = new TorrentQueuer(this, files, processFiles, uTorrentClient.EndUpdate, OnProgressChanged);
            var totalFiles = await queuer.Run();
            return;
        }
    }
}
