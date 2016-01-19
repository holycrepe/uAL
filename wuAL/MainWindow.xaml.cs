using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using uAL.Infrastructure;
using uAL.Properties.Settings.ToggleSettings;
using uAL.Queue;
using uAL.Torrents;
using uAL.Metalinks;
using Telerik.Windows.Controls;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using UTorrentRestAPI;
using Torrent.Helpers.Utils;
using Torrent.Extensions;
using wUAL.Infrastructure;
using wUAL.Queue;

namespace wUAL
{
    using Torrent.Enums;
    using static Torrent.Infrastructure.NotifyPropertyChangedBase;
    using static uAL.Properties.Settings.LibSettings;
    using static wUAL.Properties.Settings.AppSettings;
    using Torrent.Properties.Settings.MySettings;
    using Torrent.Helpers.StringHelpers;    /// <summary>
                                            /// Interaction logic for MainWindow.xaml
                                            /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
	{        
		#region Variables
		#region Public Variables
		public MetalinkQueueMonitor MetalinkMonitor = null;
		public TorrentQueueMonitor TorrentMonitor = null;		
		#endregion
		#region Private Variables		
		UTorrentClient uTorrentClient;
		DirectoryInfo activeDirectory = null;
		bool _isConnected = false;
		bool IsFileSystemInitialized = false;
		bool _hasQueuedAllTorrents = false;
		bool _hasQueuedAllMetalinks = false;
		NotifyIcon nIcon = new NotifyIcon();
		readonly bool USE_COLLECTION_VIEW_SOURCE = false;
		readonly bool USE_DISPATCHER_FOR_ASYNC = false;
        readonly WorkerStopwatch stopwatch = new WorkerStopwatch();
		Dictionary<QueueToggleStatus, QueueWorker> bgwQueueAllFiles = new Dictionary<QueueToggleStatus, QueueWorker>();
        Dictionary<QueueToggleStatus, QueueWorker> bgwProcessQueue = new Dictionary<QueueToggleStatus, QueueWorker>();
        #endregion
        #endregion
        #region Fields
        #region Private Fields
        Action<string> log = (s) => Log(s);
		#endregion
		
		#region Public Fields
		#region Public Fields: `Queue All` Status
		#region Public Fields: `Queue All` Status: Has Queued All
		void SetHasQueuedAllFiles(QueueToggleStatus fileType, bool value, bool force = false)
		{
			switch (fileType) {
				case QueueToggleStatus.Torrent:
					if (force) {
						HasQueuedAllTorrents = value;
					} else {
						HasQueuedAllTorrents |= value;
					}
					
					break;
				case QueueToggleStatus.Metalink:
					if (force) {
						HasQueuedAllMetalinks = value;
					} else {
						HasQueuedAllMetalinks |= value;
					}
					break;
			}
		}
		public bool HasQueuedAllTorrents {
			get { return _hasQueuedAllTorrents; }
			set {
				_hasQueuedAllTorrents = value;
				OnPropertyChanged(nameof(HasQueuedAllTorrents));
				OnPropertyChanged(nameof(MayQueueAllTorrents));
			}
		}
		public bool HasQueuedAllMetalinks {
			get { return _hasQueuedAllMetalinks; }
			set {
				_hasQueuedAllMetalinks = value;
				OnPropertyChanged(nameof(HasQueuedAllMetalinks));
				OnPropertyChanged(nameof(MayQueueAllMetalinks));
			}
		}
		#endregion		
		#region Public Fields: `Queue All` Status: May Queue All
		bool mayQueueAll(QueueToggleStatus type)
		{
			var TOGGLES = Toggles.GetActiveToggle(type);
			return !(type == QueueToggleStatus.Torrent ? HasQueuedAllTorrents : HasQueuedAllMetalinks) || TOGGLES.MONITOR;
		}
		public bool MayQueueAllTorrents => mayQueueAll(QueueToggleStatus.Torrent);

        public bool MayQueueAllMetalinks => mayQueueAll(QueueToggleStatus.Metalink);

        #endregion
		#endregion
		#region Public Fields: General Statuses
		public bool IsConnected {
			get { return _isConnected; }
			set {
				_isConnected = value;
				OnPropertyChanged(nameof(IsConnected));
			}
		}
		public static bool IsValidSettings => IS_VALID;

        #endregion
        #region Public Fields: Infrastructure
        public ListBoxInfoReporter InfoReporter { get; set; }
        public WorkerStopwatch Stopwatch => stopwatch;

        #endregion
        #endregion
        #endregion

        #region Interfaces: INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(params string[] propertyNames)
		{
            DoOnPropertyChanged(this, PropertyChanged, propertyNames);

			if (ValidityDeterminingSettings.ContainsAny(propertyNames)) {
				OnPropertyChanged(nameof(IsValidSettings));
			}
			
			if (propertyNames.Contains("IsValidSettings")) {
				btnSettingsConnect.IsEnabled = IsValidSettings;
				btnTorrentsConnect.IsEnabled = IsValidSettings;
				btnMetalinksConnect.IsEnabled = IsValidSettings;
			}
		}
        #endregion

        #region Log

        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
        static void Log(string title, string text = null, string item = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            => LogUtils.Log("MainWindow", title, text, item, textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);
		#endregion

		#region Controls
		#region Controls: Window
		void SetTitle() { SetTitle(null); }
		void SetTitle(string title)
		{
			Title = "wUAL: " + (title ?? StatusDescription);
		}
		void Window_StateChanged(object sender, EventArgs e)
		{
			ShowInTaskbar &= WindowState != WindowState.Minimized;
		}		
		void Window_Closing(object sender, CancelEventArgs e)
		{
			SaveSettings();
			nIcon.Dispose();
		}
		
		async void Window_Initialized(object sender, EventArgs e1)
		{
			Arguments = Environment.GetCommandLineArgs().ToList();
			LibSetting.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
			InfoReporter = new ListBoxInfoReporter(radListStatus);
			CreateNotifyIcon();						
			if (LibSetting.AutoStart && !HasArgument("--nostart", "-s")) {
				await Start();
			}
			SetTitle();
		}
		#endregion
		#region Controls: Notify Icon
		void CreateNotifyIcon()
		{
			StateChanged += Window_StateChanged;
			var icon = (BitmapImage)(this.FindResource("DefaultIcon"));
			Stream iconStream = System.Windows.Application.GetResourceStream(icon.UriSource).Stream;
			nIcon.BalloonTipText = "Click to open wUAL";
			nIcon.BalloonTipTitle = "wUAL";
			nIcon.BalloonTipIcon = ToolTipIcon.Info;
			nIcon.Text = "wUAL: Initializing";
			nIcon.Icon = new System.Drawing.Icon(iconStream);
			nIcon.Visible = true;
			nIcon.MouseUp += nIcon_MouseUp;
			nIcon.MouseMove += nIcon_MouseMove;
		}
		string getNotifyIconText(QueueToggleStatus fileType)
		{
			var TOGGLES = Toggles.GetActiveToggle(fileType);
			var monitor = GetMonitor(fileType);
			var name = (fileType == QueueToggleStatus.Torrent ? "Torrent" : "Download");
			if (monitor != null) {
				var count = monitor.Count;
				return string.Format("\n {3}{0} {1}{2}",
				                     count,
				                     name,
				                     (count == 1 ? "" : "s"),
				                     (TOGGLES.MONITOR ? "+" : "-"));
			}
			return "";
		}
		#region Controls: Notify Icon: Events
		void nIcon_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.ShowInTaskbar = true;
			BringIntoView();
		}
		
		void nIcon_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			var notifyIconText = Title;
			notifyIconText += getNotifyIconText(QueueToggleStatus.Torrent);
			notifyIconText += getNotifyIconText(QueueToggleStatus.Metalink);

			if (notifyIconText.Length > 63) {
				notifyIconText = notifyIconText.Substring(0, 63);
			}
			nIcon.Text = notifyIconText;			
		}
		#endregion
		#endregion		
		#endregion
		
		#region Initialization
		#region Initialization: Constructor
		public MainWindow()
		{
            Log("Init Component");			
			InitializeComponent();
            App.Window = this;
            UI.TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Log("Init Component Done");			
		}
		#endregion		
		#region Initialization: Start()
		async void Start(object sender, RoutedEventArgs e)
		{
			Func<Task> action = Start;
			if (USE_DISPATCHER_FOR_ASYNC) {
				var op =  Dispatcher.CurrentDispatcher.BeginInvoke(action);
				op.Completed += (s, ev) => Debugger.Break();				
			}
			else {
				await action();
			}
		}
		async Task Start()
		{
			Log("Starting Program");
			radTabStart.Visibility = Visibility.Collapsed;
			if (!IS_VALID) {
				InfoReporter.ReportStatus("Looks like it's your first time running the program. Enter your settings above and click `Connect`.");
				radTabControl.SelectedItem = radTabSettings;
				return;
			}
			
			InfoReporter.ReportStatus("wUAL Launched" + PreviewModeDescription);

			uTorrentClient = new UTorrentClient();
			
			IsConnectedLambda = () => uTorrentClient.IsConnected;
			Func<Task> action = (LibSetting.ConnectOnStartup ? (Func<Task>)Connect : (Func<Task>)SetupFileSystem);
			if (USE_DISPATCHER_FOR_ASYNC) {
				var op =  Dispatcher.CurrentDispatcher.BeginInvoke(action);
				op.Completed += (sender, e) => Debugger.Break();				
			}
			else {
				await action();
			}
		}
		#endregion
		#endregion
				
		#region Hyperlink Commands
		#region Hyperlink Commands: Main
		void ExploreFile(string fileName)
		{
			Process.Start(new ProcessStartInfo("explorer.exe", @"/select," + '"' + fileName + '"'));
		}
		void OpenURL(string URL)
		{
			Process.Start(new ProcessStartInfo(@"E:\$\Chrome.lnk", '"' + URL + '"'));
		}
		#endregion
		#region Hyperlink Commands: Overloads
		#region Hyperlink Commands: Overloads: Explore File
		void ExploreFile(FileSystemInfo fi)
		{
			ExploreFile(fi.FullName);
		}
		void ExploreFile(Uri uri)
		{
			ExploreFile(uri.AbsolutePath);
		}
		void ExploreFile(FrameworkElement control)
		{
			var fi = (FileInfo)control.DataContext;
			ExploreFile(fi);
		}
		void ExploreFile(FrameworkContentElement control)
		{
			var fi = (FileInfo)control.DataContext;
			ExploreFile(fi);
		}
		void ExploreFile(object control)
		{
			var contentElement = control as FrameworkContentElement;
			if (contentElement != null) {
				ExploreFile((FrameworkContentElement)control);
				return;
			}
			var element = control as FrameworkElement;
			if (element != null) {
				ExploreFile((FrameworkElement)element);
				return;
			}
		}
		
		void ExploreFile_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released) {
				ExploreFile(e.Source);
			}
			e.Handled = true;
		}
		
		void ExploreFile(object sender, RoutedEventArgs e)
		{
            ExploreFile(e.Source);
			e.Handled = true;
		}
		#endregion		
		#region Hyperlink Commands: Overloads: Open URL
		void OpenURL(Uri uri)
		{
			OpenURL(uri.AbsoluteUri);
		}
		void OpenURL(FrameworkElement control)
		{
			var fi = (Uri)control.DataContext;
			OpenURL(fi);
		}
		void OpenURL(FrameworkContentElement control)
		{
			var fi = (Uri)control.DataContext;
			OpenURL(fi);
		}
		void OpenURL(object control)
		{
			var contentElement = control as FrameworkContentElement;
			if (contentElement != null) {
				OpenURL((FrameworkContentElement)control);
				return;
			}
			var element = control as FrameworkElement;
			if (element != null) {
				OpenURL((FrameworkElement)element);
				return;
			}
		}
		
		void OpenURL_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released) {
				OpenURL(e.Source);
			}
			e.Handled = true;
		}
		
		void OpenURL(object sender, RoutedEventArgs e)
		{
			OpenURL(e.Source);
			e.Handled = true;
		}
		#endregion
		#endregion
		#endregion
		
		#region UTorrent Actions
		#region UTorrent Actions: Connect
		public async void Connect(object sender, RoutedEventArgs e)
		{
			await Connect();
		}

		public async Task Connect()
		{
			SetTitle("Connecting...");
			InfoReporter.ReportStatus("Connecting to " + ActiveHost + ":" + LibSetting.Port + PreviewModeDescription, log: log);
			var ex = await uTorrentClient.ConnectAsync(ActiveHost, LibSetting.Port, LibSetting.UserName, LibSetting.Password);
			if (ex == null) {
				Log("Connection Successful!");
				InfoReporter.ReportStatus("Connection Successful!", writeToLog: true);
			} else {
				Log("Error Connecting");
				InfoReporter.ReportAndLogError(ex, "Could not connect to uTorrent.", "  Please ensure uTorrent is running, and try again.");
			}
			
			if (uTorrentClient.IsConnected) {
				ActiveDirectory = uTorrentClient.GetAutoImportDirectory();
			}
			
			Func<Task> action = SetupFileSystem;
			Action OnComplete = SetTitle;
			if (USE_DISPATCHER_FOR_ASYNC) {
				var op =  Dispatcher.CurrentDispatcher.BeginInvoke(action);
				op.Completed += (s, ev) => { 
					OnComplete();
					Debugger.Break();
				};
			}
			else {				
				await action();
				OnComplete();
			}
		}
		#endregion
		#endregion
		
		#region File System
		#region File System: Initialization
		
		async Task SetupFileSystem()
		{
			Log("Setting Up File System");
			if (!IS_VALID_DIRECTORY) {
				InfoReporter.ReportError("Skipping File System Setup: ", (ActiveDirectory == "" && !IS_CONNECTED ?
				                                                          "No Directory Specified and Unable to Connect to uTorrent to get Default Directory" :
				                                                          "Invalid Directory: " + ActiveDirectory));
			} else if (activeDirectory == null) {				
				Func<Task> action = InitializeFileSystem;
				if (USE_DISPATCHER_FOR_ASYNC) {
					var op =  Dispatcher.CurrentDispatcher.BeginInvoke(action);
					op.Completed += (s, ev) => Debugger.Break();
				}
				else {					
					await action();
				}
			} else if (!activeDirectory.PathEquals(ActiveDirectory)) {
				InfoReporter.ReportStatus("Resetting File System: Active Directory Changed");
			}
		}
		
		async Task InitializeFileSystem()
		{
			SetTitle("Initializing File System...");
			InfoReporter.ReportStatus("Initializing File System...", log: log);
			InfoReporter.ReportStatus("Active Label Folders: ");
			Labels = new List<string>();
			activeDirectory = new DirectoryInfo(ActiveDirectory);
			foreach (DirectoryInfo label in activeDirectory.GetDirectories()) {
				InfoReporter.ReportStatus(label.Name);
				Labels.Add(label.Name);
			}
			Func<Task> action = StartMonitors;
			Action OnComplete = () => {
				SetTitle();
				IsFileSystemInitialized = true;
			};
			if (USE_DISPATCHER_FOR_ASYNC) {
				var op =  Dispatcher.CurrentDispatcher.BeginInvoke(action);
				op.Completed += (s, ev) => { OnComplete(); Debugger.Break(); };
			}
			else {				
				var theTask = StartMonitors();
				OnComplete();
				await theTask;
			}
			
		}		
		
		
		async Task ResetFileSystem()
		{
			await StartMonitors();
		}
		#endregion
		#region File System: Monitors		
		#region File System: Monitors: Initialize
		async Task StartMonitors()
		{
			Log("Starting All Monitors");
			await StartMetalinkMonitor();
			await StartTorrentMonitor();
		}
		
//		public class QueueMethods<TQueueItem> where TQueueItem : QueueItemBase {
//			static bool IsTorrent { get { return TQueueItem != typeof(MetalinkQueueItem); } }
//			static QueueToggleStatus QueueType { get { return IsTorrent ? QueueToggleStatus.Torrent : QueueToggleStatus.Metalink; } }
//			
//			async Task<QueueMonitor<TQueueItem>> InitializeMonitor(QueueToggleStatus fileType)
//		{
//			return await Task.Run(() =>			                      
//			                      IsTorrent ?
//			                      (QueueMonitor<TQueueItem>)(TorrentMonitor = new TorrentQueueMonitor(uTorrentClient, infoReporter)) :
//			                      (QueueMonitor<TQueueItem>)(MetalinkMonitor = new MetalinkQueueMonitor(infoReporter))
//			                     );
//			}
//		}
		
		async Task<QueueMonitorBase> InitializeMonitor(QueueToggleStatus fileType)
		{
			return await Task.Run(() =>
			                      fileType == QueueToggleStatus.Torrent ?
			                      (QueueMonitorBase)(TorrentMonitor = new TorrentQueueMonitor(uTorrentClient, InfoReporter)) :
			                      (QueueMonitorBase)(MetalinkMonitor = new MetalinkQueueMonitor(InfoReporter))
			                     );
		}
		#endregion
		#region File System: Monitors: Get Monitor
		public static bool IsTorrent(Type t) {
			return t != typeof(MetalinkQueueItem);
		}
		public QueueMonitorBase GetMonitor(QueueToggleStatus fileType)
		{
			return (fileType == QueueToggleStatus.Torrent ? (QueueMonitorBase)TorrentMonitor : (QueueMonitorBase)MetalinkMonitor);
		}
//		public QueueMonitor<TQueueItem> GetMonitor<TQueueItem>() where TQueueItem : QueueItemBase
//		{
//			//return IsTorrent(typeof(TQueueItem)) ? (QueueMonitor<TorrentQueueItem>)TorrentMonitor : (QueueMonitor<MetalinkQueueItem>)MetalinkMonitor;
//		}
		#endregion
		#region File System: Monitors: Start Monitor
		async Task StartMonitor(QueueToggleStatus fileType)
		{
			var TOGGLES = Toggles.GetActiveToggle(fileType);
			var debug = fileType.IsTorrent();			
			DispatcherOperation dispatcherOperation = null;
			if (TOGGLES.MONITOR && (IsFileSystemInitialized || TOGGLES.INITIALIZE_MONITOR)) {
				Log(fileType + " Monitor", "Starting");
				var monitor = GetMonitor(fileType);
				if (monitor != null) {
					monitor.Dispose();
				}
				UnbindGridView(fileType);
				monitor = await InitializeMonitor(fileType);				
				if (debug) {
					//Debugger.Break();
				}
				Action<Toggle> OnStartComplete = (Toggle TOGGLE) => {
					if (debug) {
						// Debugger.Break();
					}
					InitializeGridView(fileType);
					if (TOGGLE.QUEUE_FILES_ON_STARTUP) {
		            	QueueAllFiles(fileType);
		            }
					SetHasQueuedAllFiles(fileType, TOGGLE.QUEUE_FILES_ON_STARTUP);
					Log(fileType + " Monitor", "Started");
				};
				if (USE_DISPATCHER_FOR_ASYNC) {
					Delegate del = new Action(() => monitor.Start());
					dispatcherOperation = Dispatcher.CurrentDispatcher.BeginInvoke(del);					
					dispatcherOperation.Completed += (object sender, EventArgs e) => OnStartComplete(TOGGLES);					
				}
				else {
					await monitor.Start();
					OnStartComplete(TOGGLES);
				}
			} else {
				Log(fileType + " Monitor", "*NOT* Starting");
			}
		}
		async void StartTorrentMonitor(object sender, RoutedEventArgs e)
		{
			await StartTorrentMonitor();
		}
		async Task StartTorrentMonitor()
		{
			await StartMonitor(QueueToggleStatus.Torrent);
		}

		async void StartMetalinkMonitor(object sender, RoutedEventArgs e)
		{
			await StartMetalinkMonitor();
		}
		async Task StartMetalinkMonitor()
		{
			await StartMonitor(QueueToggleStatus.Metalink);
		}		
		#endregion		
		#region File System: Monitors: Queue Actions
		#region File System: Monitors: Queue Actions: Queue All Files
		void QueueAllFiles(QueueToggleStatus fileType)
		{
			var TOGGLES = Toggles.GetActiveToggle(fileType);
			var monitor = GetMonitor(fileType);
            var options = new QueueWorkerOptions(
                    onProgressChangedAddQueueItem: true
                    );
            var result = QueueWorkerFactory.Run("QueueAllFiles", fileType, bgwQueueAllFiles_DoWork, options, bgwQueueAllFiles_RunWorkerCompleted);			
		}

        void bgwQueueAllFiles_DoWork(QueueWorker bgw, DoWorkEventArgs e) {
			bgw.Monitor.QueueAllFilesBase(bgw.ReportProgress);
		}
		void bgwQueueAllFiles_RunWorkerCompleted(QueueWorker bgw, RunWorkerCompletedEventArgs e) {
			SetHasQueuedAllFiles(bgw.QueueType, true);
			if (bgw.TOGGLES.PRE_PROCESS) {				
				#if BGW_PROCESS_QUEUE || DEBUG
				ProcessQueue(bgw.QueueType);
				#endif
			}
		}
		void QueueAllTorrents(object sender = null, RoutedEventArgs e = null)
		{
			QueueAllFiles(QueueToggleStatus.Torrent);
		}

		void QueueAllMetalinks(object sender = null, RoutedEventArgs e = null)
		{
			QueueAllFiles(QueueToggleStatus.Metalink);
		}
		#endregion
		#region File System: Monitors: Queue Actions: Process Queue
		void ProcessQueue(QueueToggleStatus fileType) 
		{
            var options = new QueueWorkerOptions(
                    gridViewPrepareItems: true,
                    onProgressChangedUpdateQueueItem: true
                    );
            QueueWorkerFactory.Run("ProcessQueue", fileType, bgwProcessQueue_DoWork, options);
		}

		void bgwProcessQueue_DoWork(QueueWorker bgw, DoWorkEventArgs e) {
			bgw.Monitor.ProcessQueueBase(bgw.Items, bgw.ReportProgress);
		}

        void ProcessTorrentQueue(object sender = null, RoutedEventArgs e = null)
		{
			ProcessQueue(QueueToggleStatus.Torrent);
		}

		void ProcessMetalinkQueue(object sender = null, RoutedEventArgs e = null)
		{
			ProcessQueue(QueueToggleStatus.Metalink);
		}
		#endregion		
		#region File System: Monitors: Queue Actions: Reset Queue
		void ResetQueue(QueueToggleStatus fileType)
		{
			var TOGGLES = Toggles.GetActiveToggle(fileType);
			var monitor = GetMonitor(fileType);
			if (monitor != null) {
				monitor.Clear();
				SetHasQueuedAllFiles(fileType, false, true);
			}
		}
		void ResetTorrentQueue(object sender = null, RoutedEventArgs e = null)
		{
			ResetQueue(QueueToggleStatus.Torrent);
			
		}
		void ResetMetalinkQueue(object sender = null, RoutedEventArgs e = null)
		{
			ResetQueue(QueueToggleStatus.Metalink);
		}
		#endregion				
		#endregion
		#endregion
		#endregion
		
		#region GridView
		AutoDeferer DeferGridRefresh(QueueToggleStatus fileType)
            => AutoDeferer.StartNew(() => UnbindGridView(fileType), 
                                    () => InitializeGridView(fileType));
        void UnbindGridView(QueueToggleStatus fileType)
		{
			if (USE_COLLECTION_VIEW_SOURCE) {
				CollectionViewSource queueItemViewSource = ((CollectionViewSource)(this.FindResource(fileType.ToString().ToLower()+"QueueItemViewSource")));
				queueItemViewSource.Source = null;
			}
			else {				
				var gridView = GetGridView(fileType);
				gridView.ItemsSource = null;
			}
		}
		RadGridView InitializeGridView(QueueToggleStatus fileType)
		{
			object source;
			switch (fileType) {
				case QueueToggleStatus.Torrent:
					source = TorrentMonitor.Queue;
					break;
				case QueueToggleStatus.Metalink:
					source = MetalinkMonitor.Queue;
					break;
				default:
					return null;
			}
			var gridView = GetGridView(fileType);
			if (USE_COLLECTION_VIEW_SOURCE) {
				CollectionViewSource queueItemViewSource = ((CollectionViewSource)(this.FindResource(fileType.ToString().ToLower()+"QueueItemViewSource")));
				queueItemViewSource.Source = source;
			}
			else {				
				gridView.ItemsSource = source;
			}			
			return gridView;
		}
		public RadGridView GetGridView(QueueToggleStatus fileType)
		{
			return (fileType == QueueToggleStatus.Torrent ? grdTorrents : grdMetalinks);
		}		
		#region Queue Item Events
		void TorrentQueueItem_SetAllowUpdatePath(object sender, RoutedEventArgs e)
		{
			var queueItem = (TorrentQueueItem)grdTorrents.SelectedItem;
			queueItem.AllowUpdatePath = true;
		}
		#endregion
		#endregion
								
		#region Settings
		void SaveSettings()
		{
			AppSetting.Save();
			LibSetting.Save();
			Toggles.Save();
            MySettings.Save();
		}
		#endregion
		
		
		
	}
}
