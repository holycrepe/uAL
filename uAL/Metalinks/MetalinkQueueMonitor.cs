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
using Torrent.Extensions;
using NLog;
using System.Linq;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;
using System.Diagnostics;

namespace uAL.Metalinks
{
    using Torrent.Infrastructure.InfoReporters;
    using static uAL.Properties.Settings.LibSettings;
    public class MetalinkQueueMonitor : QueueMonitor<MetalinkQueueItem>
	{
        public override QueueToggleStatus QueueType => QueueToggleStatus.Metalink;
        public readonly TrulyObservableCollection<MetalinkQueueItem> Queue = new TrulyObservableCollection<MetalinkQueueItem>();
		bool doLogInfo = false;        

        static readonly Logger loggerBase = LogManager.GetLogger("FSM.Metalinks");
		static Logger logger = LogManager.GetLogger("Simple.FileSystemMonitor.Metalinks");
		static Logger errorLogger = LogManager.GetLogger("FSM.Metalinks.Error");
		static Logger successLogger = LogManager.GetLogger("Plain.FSM.Metalinks.Success");
		static Logger progressLogger = LogManager.GetLogger("Simple.FSM.Metalinks.Progress");
		static LogEventInfoCloneable logEventClassBase = new LogEventInfoCloneable(LogLevel.Info, logger.Name, "Metalink Info");
		static LogEventInfoCloneable getLogEventSubject(string subject, Dictionary<string, object> newEventDict = null)
		{
			return logEventClassBase.Clone(subject: subject, newEventDict: newEventDict);
		}
        
        
		static string lastCategory = "";
		static InfoReporter InfoReporter;
        

		public MetalinkQueueMonitor(InfoReporter infoReporter)
		{
			InfoReporter = infoReporter.SetLogger(loggerBase);
		}
        
        
        
		#pragma warning disable 1998
		public async override Task Start(bool logStartup = false)
		{        	
			doLogInfo = logStartup;
			if (TOGGLES.QUEUE_FILES_ON_STARTUP) {
				// await QueueAllFiles(true);
			}                        
			doLogInfo = true;      
            
			if (TOGGLES.WATCHER) {
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
            
			Watcher.Created += (s, e) => {            	
				if (doLogInfo) {
					loggerBase.INFO("FileSystemMonitor Found Metalink: " + e.FullPath);
				}
				var newQueueItems = QueueFile(e.FullPath);
				var fileExists = newQueueItems == null;
				if (doLogInfo && fileExists) {
					loggerBase.INFO(new string(' ', 34) + "[Metalink already exists in queue]");
				}
				if (!TOGGLES.PREVIEW_MODE && TOGGLES.PROCESS_QUEUE.ON_WATCHER) {
					ProcessQueue(newQueueItems);
				}
			};
		}
        
        internal override void NewQueueItem(object newQueueItemObject, bool addToQueue)
        {
            var newQueueItem = newQueueItemObject as MetalinkQueueItem;
            if (newQueueItem == null)
            {
                Debugger.Break();
            }
            else if (addToQueue)
            {
                Queue.Add(newQueueItem);
            }
            else {
            	Queue.ItemPropertyChanged(newQueueItem);
            }
        }

        async void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
		{
			var item = (MetalinkQueueItem)e.UserState;
			var fi = item.Metalink;
			string fileName = fi.FullName;
			string metalinkName = Path.GetFileNameWithoutExtension(fileName);
			string torrentName = Path.GetFileNameWithoutExtension(item.Torrent.FullName);
			Exception error = null;
            
			if (e.Error != null || e.Cancelled) {
				item.Status = MetalinkQueueStatus.LoadError;
				error = e.Error;
			} else {
				var torrentInfo = TorrentInfoCache.GetTorrentInfo(item.Torrent.FullName);	            
				if (torrentInfo.success) {
					item.Status = MetalinkQueueStatus.Success;
				} else {
					error = torrentInfo.error;
					item.Status = (torrentInfo.isBDecodeError ? MetalinkQueueStatus.TorrentBDecodeError : MetalinkQueueStatus.TorrentInfoError);
				}
			}
            
			var counts = new MetalinkQueueCounts(Queue, fileName);

			var eventDict = new Dictionary<string, object> {
				{ "METALINK", metalinkName }
			};

			LogEventInfoCloneable subjectEvent = getLogEventSubject(metalinkName, eventDict);

			var progressEventDict = new Dictionary<string, object> {
				{ "TOTAL", counts.Total },
				{ "READY", counts.Ready },
				{ "SUCCESS", counts.Success }
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
				LogEventInfoCloneable subjectEventSuccess = subjectEventProgress.Clone(group: "Success", newMessage: "SUCCESS: " + torrentName);

				progressEvent.Message = "SUCCESS: " + torrentName;
				successLogger.LOG(progressEvent);
				logger.LOG(subjectEventSuccess);

				if (counts.Success == counts.Total) {
					InfoReporter.ReportText("Successfully downloaded all torrents for metalink " + metalinkName, LogLevel.Info);
					var label = TorrentLabelService.CreateTorrentLabel(activeDir, fileName);
					await FileSystemUtils.MoveAddedFile(fi, addedDir, label, TOGGLES.MOVE_PROCESSED_FILES, doLogInfo);
				}
			} else {
				var errorEventDict = new Dictionary<string, object> {
					{ "TORRENT:", torrentName },
					{ "MESSAGE:", error.ToString() }
				};
				var errorInfo =
				    $"ERROR DOWNLOADING TORRENT:\n{"TORRENT:",27} {torrentName}\n{"METALINK:",27} {metalinkName}\n{"MESSAGE:",27} {error.ToString()}\n\n";
				var errorInfoConsole = $"{"TORRENT:",27} {torrentName}\n\n";

				LogEventInfoCloneable subjectEventError = subjectEvent.Clone(newEventDict: errorEventDict).SetError(e.Error);

				#if DEBUG || TRACE
				errorLogger.ERROR(error, errorInfo);
				logger.LOG(subjectEventError);
				#endif
				InfoReporter.ReportError(error, "ERROR DOWNLOADING TORRENT: ");
				InfoReporter.ReportText(errorInfoConsole);
			}

            
            
		}

		void ProcessQueue(MetalinkQueueItem item)
		{
			if (item == null || !item.Status.IsActivatable) {
				return;
			}
			if (TOGGLES.PROCESS_QUEUE.ALL) {
				Directory.CreateDirectory(item.Torrent.DirectoryName);
				var client = new WebClient();
				client.DownloadFileCompleted += DownloadFileCallback;
				client.DownloadFileAsync(item.Uri, item.Torrent.FullName, item);                
			}
			item.Status = MetalinkQueueStatus.Active;
		}
		public override Task ProcessQueue(IEnumerable<object> SelectedItems, QueueOnProgressChangedHandler<MetalinkQueueItem> OnProcessQueueComplete = null)
		{
			ProcessQueue((MetalinkQueueItem[])SelectedItems.Cast<MetalinkQueueItem>().ToArray().Clone());
			return null;
		}
		public void ProcessQueue(IEnumerable<MetalinkQueueItem> SelectedItems)
		{
			foreach (MetalinkQueueItem item in SelectedItems) {
				item.Status = item.Status.SetReady();
				ProcessQueue(item);
			}
		}
		List<MetalinkQueueItem> QueueFile(string fileName)
		{   
			if (Queue.Any(queueItem => queueItem.Metalink.FullName == fileName)) {
				return null;
			}
			var doc = new XmlDocument();
			string fileData = File.ReadAllText(fileName);
			fileData = fileData.Replace(" xmlns=\"", " whocares=\"");
			using (var sr = new StringReader(fileData)) {
				doc.Load(sr);
			}

			XmlNodeList fileNodes = doc.SelectNodes("metalink/files/file");

			int totalFiles = 0;

			var newQueueItems = new List<MetalinkQueueItem>();

			foreach (XmlNode fileNode in fileNodes) {
				XmlAttribute fileNameAttribute = fileNode.Attributes["name"];
				XmlNode urlNode = fileNode.SelectSingleNode("resources/url");
				XmlNode descriptionNode = fileNode.SelectSingleNode("description");
				XmlNode identityNode = fileNode.SelectSingleNode("identity");
				if (fileNameAttribute == null || urlNode == null) {
					InfoReporter.ReportText("Skipping Metalink Filenode: Missing File Name/URL", LogLevel.Error);
					continue;
				}
				totalFiles++;
                
				string path = PathUtils.MakeSafe(fileNameAttribute.Value.Replace('/', '\\').TrimEnd('\\')).UnescapeHTML();
				string url = urlNode.InnerText;
				string title = (identityNode == null ? "N/A" : identityNode.InnerText);
				string description = (descriptionNode == null ? "N/A" : descriptionNode.InnerText);
				string category = description.Replace(": " + title, "");
				string fullPath = Path.Combine(activeDir, path);
				var fi = new FileInfo(fullPath);
				var uri = new Uri(url);
				var queries = HttpUtility.ParseQueryString(uri.Query);
				string EMP_ID = queries["id"];
				string consoleInfo = $"NEW METALINK DOWNLOAD: [{EMP_ID,-6}] {title}";
				string logInfo = $"NEW DL: [{EMP_ID,-6}] {title}\n\n";
				if (lastCategory != category) {
					if (TOGGLES.PREVIEW_MODE) {
						InfoReporter.ReportText();
						InfoReporter.ReportBanner("CATEGORY: " + category);
						InfoReporter.ReportText();
					}
					logInfo += $"\n{"CATEGORY:",37} {category}\n\n";
					lastCategory = category;
				}

				var fileExists = File.Exists(fullPath);

				loggerBase.INFO(logInfo);
				var newQueueItem = new MetalinkQueueItem(fileName, category, uri, fi);
				Queue.Add(newQueueItem);
				newQueueItems.Add(newQueueItem);
				InfoReporter.ReportText(consoleInfo);
			}
			return newQueueItems;
		}
		public override void Clear()
		{
			Queue.Clear();
		}
		public override int Count => Queue.Count;
#pragma warning disable 1998
		public async override Task QueueAllFiles(bool isStartup, QueueOnProgressChangedHandler<MetalinkQueueItem> OnProgressChanged = null)
		{
            LibSetting.HaveQueuedAllMetalinks = true;
			var files = Directory.EnumerateFiles(activeDir, @"*.metalink", SearchOption.AllDirectories);
			int totalFiles = 0;
			var totalTorrents = 0;            
			var logString = "Adding Metalinks: ";
			var processFiles = !TOGGLES.PREVIEW_MODE && (isStartup ? TOGGLES.PROCESS_QUEUE.STARTUP : TOGGLES.PROCESS_QUEUE.MANUAL);
			foreach (var filename in files) {
				totalFiles++;
				var queueItems = QueueFile(filename);
				var torrentCount = (queueItems == null ? -1 : queueItems.Count);
				logString +=
				    $"\n    - [{(torrentCount == -1 ? "Already Exists in Queue" : torrentCount.ToString()),3}] {Path.GetFileNameWithoutExtension(filename)}";
				if (torrentCount != -1) {
					totalTorrents += torrentCount;
					if (processFiles) {
						ProcessQueue(queueItems);
					}
				}                
			}
			if (totalFiles > 0) {
				loggerBase.INFO(logString + "\n");
				InfoReporter.ReportText($"Finished Adding {totalTorrents} Torrents from {totalFiles} Metalinks(s)", LogLevel.Info);
			}
		}
		#pragma warning restore 1998
	}
}
