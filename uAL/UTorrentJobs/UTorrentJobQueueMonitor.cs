#define DEBUG_UTORRENT_JOB_LABELS

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
using Torrent.Infrastructure;
using uAL.Queue;
using uAL.Services;
using Torrent.Queue;

namespace uAL.UTorrentJobs
{
    using static Torrent.Async.Workers;
    using Properties.Settings.ToggleSettings;
    using static Properties.Settings.LibSettings.LibSettings;
    using static Properties.Settings.LibSettings.LibSettings.LibUTorrentJobSettings;
    using Torrent.Enums;
    using Torrent.Infrastructure.InfoReporters;
    using static NLog.LogManager;
    using static Torrent.Helpers.Utils.DebugUtils;
    using System.Collections.Concurrent;
    using Torrent.Helpers.Utils;

    public class UTorrentJobQueueMonitor : QueueMonitor<UTorrentJob>
    {
        const int MAX_RETRIES_LOCK = 60;
        public delegate void ProcessQueueItemCompleteHandler(UTorrentJob item, bool result);
        public readonly TrulyObservableCollection<UTorrentJob> Queue =
            new TrulyObservableCollection<UTorrentJob>();        
        UTorrentJobCollection Jobs = null;
        readonly string downloadDir;
        static readonly object _locker = new object();
#pragma warning disable 0414
        string lastQueueItemChanged = null;
#pragma warning restore 0414
        QueryDuplicateFileNames duplicateFinder;
        internal static InfoReporter infoReporter;
        public override MonitorTypes QueueType => MonitorTypes.Torrent;

        //TorrentLabel computedLabel;
        internal static Logger loggerBase = GetLogger("FSM.UTorrentJob");
        //static Logger dupeLogger = GetLogger("FSM.Dupes");
        //static Logger logger = GetLogger("Simple.FileSystemMonitor.UTorrentJobs");
        //static LogEventInfoCloneable logEventClassBase = new LogEventInfoCloneable(LogLevel.Info, logger.Name,
        //                                                                           "uTorrent Job Info");
        //static LogEventInfoCloneable getLogEventSubject(string subject, Dictionary<string, object> newEventDict = null)
        //    => logEventClassBase.Clone(subject: subject, newEventDict: newEventDict);

        bool doLogInfo = false;

        public UTorrentJobQueueMonitor(InfoReporter InfoReporter)
        {
            infoReporter = InfoReporter.SetLogger(loggerBase);
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
            
        }

        void Queue_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }
        public async Task<bool> DoMoveFile(UTorrentJob item, bool hasReportedText)
        {
            if (item.Path == item.OriginalPath)
            {
                return hasReportedText;
            }

            var result = item.Move();
            if (result.Status.IsSuccess())
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                hasReportedText = await UI.StartNew(() => ReportText(item, hasReportedText, $"Successfully Moved Download: \r\nFrom: {item.OriginalPath}\r\n  To: {item.NewPath}"));
#pragma warning restore CS4014 
                hasReportedText = true;
            }
            else if (!result.Status.IsSuccessful())
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                hasReportedText = await UI.StartNew(() => ReportText(item, hasReportedText, $"Error Moving Download: {result.Status}: {result.Error}"));
#pragma warning restore CS4014
            }
            return hasReportedText;
        }
        bool CheckDupePath(UTorrentJob item, TorrentFileInfo largestFile, string path)
        {
            var fi = FileUtils.GetInfo(path);
            if (fi.Exists && fi.Length == largestFile.Length)
            {
                return true;
            }
            if (Directory.Exists(path))
            {
                fi = FileUtils.GetInfo(Path.Combine(path, largestFile.FullName));
                if (fi.Exists && fi.Length == largestFile.Length)
                {
                    return true;
                }
            }
            return false;
        }
        protected void CheckDupesProgressChanged(UTorrentJob item, double progress)
        {

        }
        void CheckDupesAsync(UTorrentJob item)
        {

        }
        void CheckDupes_Step3(UTorrentJob item, QueryDuplicateFileNamesResult result, double percentComplete)
        {
            var largestFile = item.Torrent.Largest;
            var newPath = result.Dupe.FullName.TrimEnd(".!ut").Replace(largestFile.FullName, "").TrimEnd(Path.DirectorySeparatorChar);
            var newPathParent = Directory.GetParent(newPath).FullName;
            var newLabel = newPath.TrimStart(downloadDir).TrimStart(Path.DirectorySeparatorChar);
            var newLabelParent = newPathParent.TrimStart(downloadDir).TrimStart(Path.DirectorySeparatorChar);
            var isLabel = LibSetting.Labels.Collection.Contains(newLabel);
            var isLabelParent = LibSetting.Labels.Collection.Contains(newLabelParent);
            item.Path = newPath;
            if (percentComplete == 100)
            {
                if (isLabel)
                {
                    item.PrimaryLabel = newLabel;
                }
                else if (isLabelParent)
                {
                    item.SecondaryLabel = newLabelParent;
                }
                else
                {
                    DEBUG.Noop();
                }
            }
            else
            {
                Debugger.Break();
                item.PrimaryLabel = newLabel;
                if (isLabelParent)
                {
                    item.SecondaryLabel = newLabelParent;
                }
            }
        }
        bool CheckDupes_Step2(UTorrentJob item, QueryDuplicateFileNamesResult result, double percentComplete)
        {
            if (result)
            {
                if (percentComplete == -1)
                {
                    DownloadedFileChecker.Check(item, onRunWorkerCompleted: (s, e) => {
                        CheckDupes_Step3(item, result, e.Result);
                    });
                }
                else {
                    CheckDupes_Step3(item, result, percentComplete);
                }
                return true;
            }
            return false;
        }
        void CheckDupes(UTorrentJob item, Action<bool> callback)
        {
            if (item.DownloadedFileExists)
            {
                callback(false);
            }
            var label = item.SecondaryLabel;
            var largestFile = item.Torrent.Largest;
            var result = duplicateFinder.QueryDuplicates(largestFile.Name, largestFile.Length, label);
            double percentComplete = -1;

            Action ProcessMatch = () =>
            {
                var originalPath = item.Path.TrimEnd(".!ut").Replace(largestFile.FullName, "").TrimEnd(Path.DirectorySeparatorChar);
                var newPath = result.Dupe.FullName.TrimEnd(".!ut").Replace(largestFile.FullName, "").TrimEnd(Path.DirectorySeparatorChar);
                var newPathParent = Directory.GetParent(newPath).FullName;
                var newLabel = newPath.TrimStart(downloadDir).TrimStart(Path.DirectorySeparatorChar);
                var newLabelParent = newPathParent.TrimStart(downloadDir).TrimStart(Path.DirectorySeparatorChar);
                var isLabel = LibSetting.Labels.Collection.Contains(newLabel);
                var isLabelParent = LibSetting.Labels.Collection.Contains(newLabelParent);
                var finalPath = item.Path.Replace(originalPath, newPath);
                if (finalPath != item.Path)
                {
                    item.Path = finalPath;
                }
                if (percentComplete == 100)
                {
                    if (isLabel)
                    {
                        item.PrimaryLabel = newLabel;
                    }
                    else if (isLabelParent)
                    {
                        item.SecondaryLabel = newLabelParent;
                    }
                    else
                    {
                        DEBUG.Noop();
                    }
                }
                else
                {
                    Debugger.Break();
                    item.PrimaryLabel = newLabel;
                    if (isLabelParent)
                    {
                        item.SecondaryLabel = newLabelParent;
                    }
                }
            };

            Func<bool> ProcessResult = () =>
            {
                if (result)
                {
                    if (percentComplete == -1)
                    {
                        DownloadedFileChecker.Check(item, onRunWorkerCompleted: (s, e) => {
                            percentComplete = e.Result;
                            ProcessMatch();
                        });
                    }
                    else {
                        ProcessMatch();
                    }
                    return true;
                }
                return false;
            };

            if (!result)
            {
                result = duplicateFinder.QueryDuplicates(largestFile.Name + ".!ut", largestFile.Length, label);
                if (result && !result.Dupe.FullName.Contains("Incomplete"))
                {
                    DownloadedFileChecker.Check(item, onRunWorkerCompleted: (s, e) => {
                        percentComplete = e.Result;
                        if (percentComplete == 100)
                        {
                            try
                            {
                                result.Dupe.MoveTo(result.Dupe.FullName.TrimEnd("!.ut"));
                            }
                            catch (Exception ex)
                            {
                                DEBUG.Noop(ex);
                                throw;
                            }
                        }
                        callback(ProcessResult());
                    });
                    return;
                }
            }
            callback(ProcessResult());
        }
        #pragma warning disable 1998
        public void ProcessQueueItem(UTorrentJob item, ProcessQueueItemCompleteHandler OnProcessQueueItemComplete = null)
        {
            if (!TOGGLES.Processing.Enabled || !item.Name.Contains("25841_03"))
            {
                OnProcessQueueItemComplete?.Invoke(item, false);
                return;
            }
            var changed = item.Changed;
            var hasReportedText = false;

            Action<bool> onCheckDupes = async (isDupe) =>
            {
                if (isDupe)
                {
                    changed = true;
                    hasReportedText = await UI.StartNew(() => ReportText(item, hasReportedText, $"Duplicate Files Found: \nOld Path: {item.OriginalPath}\nNew path: {item.Path}\n"));
                    DEBUG.Noop();
                }

                if ((item.Name.Contains("Nuru Massage") && item.Label.Contains("Anissa Kate")) || item.Name.Contains("25841"))
                {
                    //var start = DateTime.Now;
                    DEBUG.Noop();
                }

                if (LibSetting.Jobs.FixLabels)
                {
                    if (LibSetting.Jobs.FixOriginalLabel && item.OriginalLabel != item.Labels[0])
                    {
                        item.Changed = true;
                        changed = true;
                    }
                    if (LibSetting.Jobs.FixSecondaryLabel && item.FixSecondaryLabel())
                    {
                        changed = true;
                        hasReportedText = await DoMoveFile(item, hasReportedText);
                    }
                    if (LibSetting.Jobs.RemoveExtraPrimaryLabel && item.RemoveExtraPrimaryLabel())
                    {
                        changed = true;
                    }
                }


                if (changed != item.Changed)
                {
                    DEBUG.Noop();
                }

                if (changed && LibSetting.Jobs.MoveDownloadedFiles)
                {
                    hasReportedText = await DoMoveFile(item, hasReportedText);
                    item.OriginalPath = item.Path;
                }

                OnProcessQueueItemComplete?.Invoke(item, changed);
            };

            if (TOGGLES.Processing.CheckDupes)
            {
                CheckDupes(item, onCheckDupes);
            }
            else
            {
                onCheckDupes(false);
            }
        }
#pragma warning restore 1998

        public override Task ProcessQueue(IEnumerable<object> SelectedItems, 
                                          QueueOnStartHandler OnStart = null,
                                          QueueOnProgressChangedHandler<UTorrentJob> OnProgressChanged = null,
                                          QueueWorkerOnCompleteHandler OnQueueWorkerComplete = null)
            =>
                ProcessQueue((UTorrentJob[]) SelectedItems.Cast<UTorrentJob>().ToArray().Clone(), OnStart,
                             OnProgressChanged, OnQueueWorkerComplete);

        public delegate Task<int> ProcessQueueItemDelegate(UTorrentJob item);

        public delegate void ProcessQueueItemAddTaskDelegate(Task<int> task);

        #pragma warning disable 1998
        protected override async Task PerformUpdate(int newCount, string method, string methodVerb)
        {
#pragma warning disable 4014
            base.PerformUpdate(newCount, method, methodVerb);
#pragma warning restore 4014
        }
#pragma warning restore 1998

        public async Task ProcessQueue(IEnumerable<UTorrentJob> QueueItems, 
                                       QueueOnStartHandler OnStart = null,
                                       QueueOnProgressChangedHandler<UTorrentJob> OnProgressChanged = null,
                                       QueueWorkerOnCompleteHandler OnQueueWorkerComplete = null)
        {
            //        	uTorrentClient.BeginUpdate();            
            var itemsArray = QueueItems.ToArray();
            var newCount = itemsArray.Length;
            var remaining = newCount;
            var changed = 0;
            OnStart?.Invoke(newCount);
            await PerformUpdate(newCount, nameof(ProcessQueue), "Processing");            
            var tasks = new ConcurrentQueue<Task>();
            ProcessQueueItemCompleteHandler OnProcessQueueItemComplete =
                (s, r) =>
                {
                    OnProgressChanged?.Invoke(NewQueueWorkerState(s));
                    lock (_locker)
                    {
                        remaining--;
                        if (r)
                        {
                            changed++;
                        }
                        if (remaining == 0)
                        {
                            OnQueueWorkerComplete?.Invoke();
                            if (changed > 0)
                            {
                                Jobs.Save(LibSetting.Jobs.OverwriteUTorrentResumeFile);
                            }
                        }
                    }
                };
            //Func<UTorrentJob, Task> doProcessQueue =
            //    item => ProcessQueue(item, OnProcessQueueItemComplete);
            Action<UTorrentJob> doAddTask =
                item => ProcessQueueItem(item, OnProcessQueueItemComplete);

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
            //await Task.WhenAll(tasks);
            //if (results.Any(s => s))
            //{
            //    Jobs.Save();
            //}
            //            uTorrentClient.EndUpdate();
        }

        async Task<UTorrentJob> QueueFile(UTorrentJob newQueueItem, QueueStatusMember status = null,
                                   QueueOnCompleteHandler<UTorrentJob> OnQueueFileComplete = null)
        {
            //if (status != null) {
            //    newQueueItem.Status = status;
            //}
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
            var newQueueItem = newQueueItemObject as UTorrentJob;
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

        void PreProcessFile(UTorrentJob newQueueItem)
        {
            if (newQueueItem.Status.IsQueued) {
                /// TODO: Fix this
                //newQueueItem.Status = CheckDupes(newQueueItem, null);
                //newQueueItem.Status = await CheckDupes(newQueueItem, computedLabel.Base);
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
#pragma warning disable 1998
        internal async Task<UTorrentJob> AddFile(string fileName, bool liveFile = true, int fileNumber = 0,
                                          QueueOnCompleteHandler<UTorrentJob> OnQueueFileComplete =
                                              null) => null;
#pragma warning restore 1998
#pragma warning disable 1998
        async Task<UTorrentJob> AddFile(UTorrentJob newQueueItem, bool liveFile = true,
                                 QueueOnCompleteHandler<UTorrentJob> OnQueueFileComplete = null) => null;
#pragma warning restore 1998
#pragma warning disable 1998
        //async Task<QueueStatusMember> CheckDupes(UTorrentJob queueItem, string label = null)
        //{
        //    return UTorrentJobQueueStatus.Ready;
        //}
#pragma warning restore 1998
        public override void Clear()
        {
            lock (_locker) {
                Queue.Clear();
            }
        }

        public override int Count => Queue.Count;
        bool ReportText(UTorrentJob item, bool hasReportedItem, string text)
        {
            if (!hasReportedItem)
            {
                infoReporter.ReportBanner(item.SecondaryLabel.PadTitle(item.SubLabel.PadTitle(item.Caption, 80), 60));
            }
            infoReporter.ReportText(text);
            return true;
        }
        public void QueueFile(UTorrentJob item)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed            
            if (!LibSetting.Labels.Filter(item.PrimaryLabel, TOGGLES))
            {
                return;
            }

            UI.StartNew(() => {
                var hasReportedItem = false;
                if (item.Targets.Length > 1)
                {
                    hasReportedItem = ReportText(item, hasReportedItem, $"Multiple Targets: {item.Targets.GetDebuggerDisplay(sep: "; ")}");
                }
                if (item.OriginalLabel != item.Labels[0])
                {
                    hasReportedItem = ReportText(item, hasReportedItem, $"Inconsistent Labels: {nameof(item.Label)} should equal {nameof(item.Labels)}[0]: {item.OriginalLabel} != {item.Labels[0]}");
                }
                if (item.HasSecondaryLabel)
                {
                    if (!item.LabelsAreValid)
                    {
                        hasReportedItem = ReportText(item, hasReportedItem, 
                            $"{(item.IsComplete ? "*" : " ")}Invalid Labels: {nameof(item.Label)} should equal {nameof(item.PrimaryLabel)}: \r\n{item.Label} != {item.PrimaryLabel}. \r\nNew Path: {item.NewPath}");
                    }
                }
                if (hasReportedItem)
                {
                    // Debugger.Break();
                }
                Queue.Add(item);
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed      
        }
#pragma warning disable 1998
        public override async Task QueueAllFiles(bool isStartup, QueueOnStartHandler OnStart = null,
                                                 QueueOnProgressChangedHandler<UTorrentJob> OnProgressChanged =
                                                     null)
        {

            LibSetting.Queue.HaveQueuedAllJobs = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            UI.StartNew(() => Queue.Clear());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Jobs = new UTorrentJobCollection(LibSetting.Directories.ResumeDat, OnStart, item => {
                QueueFile(item);
                OnProgressChanged?.Invoke(NewQueueWorkerState(item));
            });

#if DEBUG_UTORRENT_JOB_LABELS
            var labels = Jobs.Where(j => !j.LabelsAreValid).Select(j => j.PrimaryLabel).OrderBy(s => s);
            Log("Job Labels: ", "\n" + new string('-', 80) + "\n" + string.Join("\n", labels));
#endif
        }

    }
}
