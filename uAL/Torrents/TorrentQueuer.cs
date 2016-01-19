using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using uAL.Queue;
using uAL.Properties.Settings.ToggleSettings;
using Torrent.Extensions;

namespace uAL.Torrents
{
	struct TorrentQueuer
	{
		readonly Dictionary<string, List<string>> LoggedLabels;
		int TotalFiles;
		readonly bool ProcessFiles;
		readonly List<TorrentQueueItem> newQueueItems;
		readonly string[] Queue;
		readonly Action OnComplete;
		readonly QueueOnProgressChangedHandler<TorrentQueueItem> OnProgressChanged;
		readonly TorrentQueueMonitor Monitor;
		readonly QueueWorkerState<TorrentQueueItem> state;
		public TorrentQueuer(TorrentQueueMonitor monitor, IEnumerable<string> files, bool processFiles, Action onComplete = null, QueueOnProgressChangedHandler<TorrentQueueItem> onProgressChanged = null)
		{
			Queue = files.ToArray();
			newQueueItems = new List<TorrentQueueItem>();
			TotalFiles = 0;
			LoggedLabels = new Dictionary<string, List<string>>();
			Monitor = monitor;
			OnComplete = onComplete;
			ProcessFiles = processFiles;
			OnProgressChanged = onProgressChanged;
			state = new QueueWorkerState<TorrentQueueItem>(QueueToggleStatus.Torrent, Queue.Count());
		}
        	
		public async Task<int> Run()
		{
			var tasks = new List<Task>();
			for (int i=0, j=Queue.Count(); i < j; i++) {
				tasks.Add(QueueFile(Queue[i], i+1));
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
					LoggedLabels[label].Add((TotalFiles > 0 ? "\n" : "") + new string('-', depth * 4) + "[" + label + "]");
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
					newQueueItems.Add(newQueueItem);
				}        			
			}       
        		
			// Queue.Remove(filename);
		}
		void ReportProgress(TorrentQueueItem newQueueItem) {
			if (OnProgressChanged != null) {
				OnProgressChanged(state.New(newQueueItem));
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
				TorrentQueueMonitor.infoReporter.ReportAndLogText("Finished Queueing " + TotalFiles + " Torrent(s)");
			}
			if (ProcessFiles) {
				await Monitor.ProcessQueue(newQueueItems);
			}
			return TotalFiles;
		}
	}
}