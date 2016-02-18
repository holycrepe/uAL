using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using uAL.Queue;
using uAL.Properties.Settings.ToggleSettings;
using Torrent.Extensions;
using Torrent.Enums;
using Torrent.Infrastructure;

namespace uAL.Torrents
{
    using System.Collections.Concurrent;
    using static Torrent.Helpers.Utils.DebugUtils;
    class TorrentQueuer
    {
        readonly Dictionary<string, List<string>> LoggedLabels;
        int TotalFiles;
        readonly bool ProcessFiles;
        readonly ConcurrentQueue<TorrentQueueItem> newQueueItems;
        readonly string[] Queue;
        readonly Action OnComplete;
        readonly QueueOnProgressChangedHandler<TorrentQueueItem> OnProgressChanged;
        readonly TorrentQueueMonitor Monitor;
        readonly QueueWorkerState<TorrentQueueItem> state;

        public TorrentQueuer(TorrentQueueMonitor monitor, string[] files, bool processFiles, Action onComplete = null,
                             QueueOnProgressChangedHandler<TorrentQueueItem> onProgressChanged = null)
        {
            Queue = files;
            newQueueItems = new ConcurrentQueue<TorrentQueueItem>();
            TotalFiles = 0;
            LoggedLabels = new Dictionary<string, List<string>>();
            Monitor = monitor;
            OnComplete = onComplete;
            ProcessFiles = processFiles;
            OnProgressChanged = onProgressChanged;
            state = new QueueWorkerState<TorrentQueueItem>(MonitorTypes.Torrent);
        }

        public async Task<int> Run()
        {
            var tasks = new ConcurrentQueue<Task>();       
            var i = 0;
            Action<string> doAddTask =
                item =>
                {
                    var task = QueueFile(item, ++i);
                    if (task == null)
                    {
                        DEBUG.Break();
                    }
                    tasks.Enqueue(task);
                };
            var method = ProcessQueueMethod.Default.Value();
            switch (method)
            {
                case ProcessQueueMethod.Default:
                case ProcessQueueMethod.Plain:
                    foreach (var item in Queue)
                    {
                        doAddTask(item);
                    }
                    break;
                case ProcessQueueMethod.Parallel:
                    var opts = new ParallelOptions { MaxDegreeOfParallelism = TorrentQueueMonitor.MAX_DEGREE_OF_PARALLELISM };
                    var loopResult = Parallel.ForEach(Queue, opts, doAddTask);
                    break;
                case ProcessQueueMethod.PLINQ:
                    Queue.AsParallel()
                        .WithDegreeOfParallelism(TorrentQueueMonitor.MAX_DEGREE_OF_PARALLELISM)
                        .ForAll(doAddTask);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            await Task.WhenAll(tasks);
            return await Completed();
        }

        async Task QueueFile(string filename, int fileNumber)
        {
            var newQueueItem = await Monitor.AddFile(filename, false, fileNumber, ReportProgress);
            string label;
            if (newQueueItem == null) {
                // Queue.Remove(filename);
                return;
            }
            if (newQueueItem.Label == null) {
                Debugger.Break();
                label = "";
            } else {
                label = newQueueItem.Label.Base;
            }
            var depth = label.Split('\\').Length;
            if (!LoggedLabels.ContainsKey(label)) {
                LoggedLabels[label] = new List<string>();
                if (label != "") {
                    LoggedLabels[label].Add((TotalFiles > 0 ? "\n" : "") + new string('-', depth*4) + "[" + label + "]");
                }
            }

            var prefix = new string('\t', depth);
            if (newQueueItem == null) {
                prefix += "\t xxxx ";
            } else if (newQueueItem.Status.IsError) {
                prefix += " * ERROR  ";
            } else if (newQueueItem.Status == TorrentQueueStatus.NoLabel) {
                prefix += "\t  [X] ";
            } else if (newQueueItem.Status == TorrentQueueStatus.TorrentDupe) {
                prefix += " * EXISTS ";
            } else if (newQueueItem.Status.IsDupe) {
                prefix += " ** DUPE  ";
            } else {
                prefix += "\t      ";
            }

            LoggedLabels[label].Add(prefix + " - " + Path.GetFileNameWithoutExtension(filename));

            if (newQueueItem != null) {
                TotalFiles++;
                if (ProcessFiles) {
                    newQueueItems.Enqueue(newQueueItem);
                }
            }

            // Queue.Remove(filename);
        }

        void ReportProgress(TorrentQueueItem newQueueItem)
        {
            if (OnProgressChanged != null) {
                var newState = state.New(newQueueItem);
                if (newState == null) {
                    Debugger.Break();
                }
                OnProgressChanged(newState);
            }
        }

        async Task<int> Completed()
        {
            if (OnComplete != null) {
                OnComplete();
            }
            var logString = "Queueing Torrents: ";
            if (TotalFiles > 0) {
                foreach (var kvp in LoggedLabels) {
                    logString += "\n" + string.Join("\n", kvp.Value);
                }
                TorrentQueueMonitor.loggerBase.INFO(logString);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                UI.StartNew(() =>
                TorrentQueueMonitor.infoReporter.ReportAndLogText("Finished Queueing " + TotalFiles + " Torrent(s)"));
#pragma warning restore CS4014                
            }
            if (ProcessFiles) {
                await Monitor.ProcessQueue(newQueueItems);
            }
            return TotalFiles;
        }
    }
}
