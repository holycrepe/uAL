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
using System.Threading;
using UTorrentRestAPI;
using Torrent.Helpers.Utils;
using Torrent.Extensions;
using wUAL.Infrastructure;
using wUAL.Queue;
using wUAL.WPF.Selectors.Models.ProgressBar;
using Timer = System.Windows.Forms.Timer;

namespace wUAL
{
    using Torrent.Enums;
    using uAL.Properties.Settings.LibSettings;
    using static Torrent.Infrastructure.NotifyPropertyChangedBase;
    using static uAL.Properties.Settings.LibSettings.LibSettings;
    using static wUAL.Properties.Settings.AppSettings.AppSettings;
    using static Torrent.Helpers.Utils.DebugUtils;
    using static Properties.Settings.AllSettings.AllSettings;
    using Torrent.Properties.Settings.MySettings;
    using Torrent.Helpers.StringHelpers;
    using NLog;
    using Torrent.Infrastructure;
    using System.Net;
    using WPF;
    using Telerik.Windows.Controls.GridView;
    using Torrent.Queue;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using uAL.UTorrentJobs;
    using uAL.Services;
    using Extensions;
    using Properties.Settings.AppSettings;
    using static uAL.Properties.Settings.ToggleSettings.ToggleSettings;
    using Telerik.Windows.Controls.Data.PropertyGrid;
    using System.Collections.ObjectModel;/// <summary>
                                         /// Interaction logic for MainWindow.xaml
                                         /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Variables
        #region Public Variables
        public MetalinkQueueMonitor MetalinkMonitor = null;
        public TorrentQueueMonitor TorrentMonitor = null;
        public UTorrentJobQueueMonitor UTorrentJobMonitor = null;
        #endregion
        #region Private Variables		
        UTorrentClient uTorrentClient;
        DirectoryInfo activeDirectory = null;
        //bool _isConnected = false;
        bool IsFileSystemInitialized = false;
        //bool _hasQueuedAllTorrents = false;
        //bool _hasQueuedAllMetalinks = false;
        NotifyIcon nIcon = new NotifyIcon();
        readonly bool USE_COLLECTION_VIEW_SOURCE = false;
        readonly bool USE_DISPATCHER_FOR_ASYNC = false;
        //Dictionary<QueueToggleStatus, QueueWorker> bgwQueueAllFiles = new Dictionary<QueueToggleStatus, QueueWorker>();
        //Dictionary<QueueToggleStatus, QueueWorker> bgwProcessQueue = new Dictionary<QueueToggleStatus, QueueWorker>();
        #endregion
        #endregion
        #region Fields
        #region Private Fields
        Action<string> log = (s) => Log(s);

        int defaultTab
        {
            get
            {
                var monitor = (MonitorTypes)Toggles.Monitor;
                return monitor.IncludesTorrent() ? 1
                    : monitor.IncludesMetalink() ? 2 : 3;
            }
        }
        #endregion

        #region Public Fields
        #region Public Fields: Infrastructure
        public ListBoxInfoReporter InfoReporter { get; set; }
        public WorkerStopwatch Stopwatch { get; } = new WorkerStopwatch();

        #endregion
        #endregion
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
            //SaveSettings();
            nIcon.Dispose();
        }

        async void Window_Initialized(object sender, EventArgs e1)
        {
            Arguments = Environment.GetCommandLineArgs().ToList();
            LibSetting.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
            InfoReporter = new ListBoxInfoReporter(radListStatus);
            CreateNotifyIcon();
            if (LibSetting.Queue.AutoStart && !HasArgument("--nostart", "-s"))
            {
                await Start();
            }
            SetTitle();
            SetDefaultTab();
            Log("Window Initialized");
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
        string getNotifyIconText(MonitorTypes fileType)
        {
            var TOGGLES = Toggles.GetActiveToggles(fileType);
            var monitor = GetMonitor(fileType);
            var name = (
                fileType.IsTorrent()
                ? "Torrent"
                : fileType.IsMetalink()
                ? "Download"
                : "Job");
            if (monitor != null)
            {
                var count = monitor.Count;
                return string.Format("\n {3}{0} {1}{2}",
                                     count,
                                     name,
                                     (count == 1 ? "" : "s"),
                                     (TOGGLES.Monitor ? "+" : "-"));
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
            notifyIconText += getNotifyIconText(MonitorTypes.Torrent);
            notifyIconText += getNotifyIconText(MonitorTypes.Metalink);
            // notifyIconText += getNotifyIconText(QueueToggleStatus.UTorrentJob);

            if (notifyIconText.Length > 63)
            {
                notifyIconText = notifyIconText.Substring(0, 63);
            }
            nIcon.Text = notifyIconText;
        }
        #endregion
        #endregion
        #region Controls: RadTabControl
        void SetDefaultTab(bool force = false)
        {
            if (force || radTabControl.SelectedItem == radTabSettings || radTabControl.SelectedItem == radTabToggles)
            {
                if (defaultTab != 3)
                {
                    DEBUG.Noop();
                }
                radTabControl.SelectedIndex = defaultTab;
            }
        }

        private void RadTabControl_PreviewSelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            if (e.AddedItems.Contains(radTabStart))
            {
                e.Handled = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Start();
#pragma warning restore CS4014 
            }
            if (IsInitialized)
                return;
            if (e.RemovedItems.Contains(radTabSettings) || e.RemovedItems.Contains(radTabToggles))
            {
                SaveSettings();
            }
        }
        #endregion
        #endregion

        #region Initialization
        #region Initialization: Constructor
        public MainWindow()
        {
            UI.Window = this;
            LoadAllSettings();
            Log("Init Component");
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            if (LibSetting.Queue.UpdateLabelsOnStart)
            {
                LibSetting.Labels.GenerateLabels((count, ts) =>
                           InfoReporter.ReportTiming($"Loaded {count} File System Labels in {ts.FormatFriendly()}."));
            }
            if (LibSetting.Queue.CacheDownloadedFilesOnStart)
            {
                QueryDuplicateFileNamesCache.InitializeExtensions((count, ts) =>
                               InfoReporter.ReportTiming($"Cached {count} File Extensions in {ts.FormatFriendly()}."));
            }
#pragma warning restore CS4014
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            InitializeComponent();
            InitializeGridViewDescriptors();
            InfoReporter.ReportTiming($"Loaded UI in {stopwatch.Elapsed.FormatFriendly()}.");
            UI.TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            UI.SyncContext = SynchronizationContext.Current;
            Stopwatch.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Stopwatch));
            Log("Init Component Done");
            SetDefaultTab(true);
            WindowSettings.Instances.FirstOrDefault()?.LoadWindowState();
            var timer = new Timer
            {
                Interval = 10000
            };
            timer.Tick += (s, e) =>
            {
                WindowSettings.Instances.FirstOrDefault()?.LoadWindowState();
                timer.Stop(); timer = null;
            };
            var x = new UTorrentJobViewModel();
            var progressModel = ResourceUtils.Get<ProgressBarModels>();
            Debugger.Break();
        }

        #endregion
        #region Initialization: Start()
        async Task Start()
        {
            Log("Starting Program");
            radTabStart.Visibility = Visibility.Collapsed;
            if (!LibSetting.IS_VALID)
            {
                InfoReporter.ReportStatus("Looks like it's your first time running the program. Enter your settings above and click `Connect`.");
                radTabControl.SelectedItem = radTabSettings;
                return;
            }

            InfoReporter.ReportStatus("wUAL Launched" + PreviewModeDescription);
            SetDefaultTab();

            uTorrentClient = new UTorrentClient();

            LibSetting.Connection.IS_CONNECTED = uTorrentClient.IsConnected;
            await (LibSetting.Connection.ConnectOnStartup
                ? (Func<Task>)Connect : (Func<Task>)SetupFileSystem)();
        }
        #endregion
        #endregion

        #region Label Commands
        #region Label Commands: Main
        bool AddLabel(RadComboBox labelDropdown)
        {
            if (labelDropdown != null)
            {
                var label = labelDropdown.Text;
                LibSetting.Labels.Update(label);
                var cell = labelDropdown.ParentOfType<GridViewCell>();
                var queueItem = labelDropdown.DataContext as TorrentQueueItem;
                if (queueItem != null)
                {
                    var selectedItems = GridMultiEditBehavior.GetSelectedItems<TorrentQueueItem>(cell);
                    foreach (var item in selectedItems)
                    {
                        item.AllowUpdatePath = true;
                        item.Label.Base = label;
                    }
                }
                //GridMultiEditBehavior.CommitMultiEdit(cell, label, queueItem);
                //queueItem.Label.Base = label;
                return true;
            }
            return false;
        }
        #endregion
        #region Label Commands: Events
        private void RadGridViewCellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.EditAction == GridViewEditAction.Commit)
            {
                var selectedItems = GridMultiEditBehavior.GetSelectedItems<TorrentQueueItem>(e.Cell);
                foreach (var item in selectedItems)
                {
                    item.AllowUpdatePath = true;
                }
            }
        }
        void AddLabel_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddLabel(sender as RadComboBox);
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
            if (contentElement != null)
            {
                ExploreFile((FrameworkContentElement)control);
                return;
            }
            var element = control as FrameworkElement;
            if (element != null)
            {
                ExploreFile((FrameworkElement)element);
                return;
            }
        }

        void ExploreFile_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
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
            if (contentElement != null)
            {
                OpenURL((FrameworkContentElement)control);
                return;
            }
            var element = control as FrameworkElement;
            if (element != null)
            {
                OpenURL((FrameworkElement)element);
                return;
            }
        }
        void OpenURL_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
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
            => await Connect();

        public async Task Connect()
        {
            SetTitle("Connecting...");
            InfoReporter.ReportStatus("Connecting to " + LibSetting.Connection.HOST + ":" + LibSetting.Connection.PORT + PreviewModeDescription, log: log);
            SetDefaultTab();
            var ex = await uTorrentClient.ConnectAsync(LibSetting.Connection.HOST, LibSetting.Connection.PORT, LibSetting.Connection.USER_NAME, LibSetting.Connection.PASSWORD);
            SetDefaultTab();
            if (ex == null)
            {
                Log("Connection Successful!");
                InfoReporter.ReportStatus("Connection Successful!", writeToLog: true);
            }
            else if (ex.EndpointUnavailable)
            {
                Log("Error Connecting: UTorrent Unavailable");
                InfoReporter.ReportError("Could not Connect to Web API: uTorrent is unavailable", "Please start uTorrent, and try again.");
            }
            else {
                Log("Error Connecting");
                InfoReporter.ReportAndLogError(ex, "Could not connect to uTorrent.", "  Please ensure uTorrent is running, and try again.");
            }

            if (uTorrentClient.IsConnected)
            {
                LibSetting.Directories.uTorrent = uTorrentClient.GetAutoImportDirectory() ?? LibSetting.Directories.uTorrent;
            }

            //Logger LoggerTest = LogManager.GetLogger("Simple.TorrentLabelServiceTest");
            //var eventInfoTest = new Dictionary<string, object>
            //                {
            //                    {"Category", "Test"},
            //                    {"Torrent", "Test"},
            //                    {"Contents", "Test"},
            //                    {"TitleSuffix", "Test"}
            //                };
            //var logEventTest = new LogEventInfoCloneable(LogLevel.Info, LoggerTest.Name, "Extended Label: ", eventInfo: eventInfoTest);
            //LoggerTest.Log(logEventTest);

            Func<Task> action = SetupFileSystem;
            Action OnComplete = SetTitle;
            if (USE_DISPATCHER_FOR_ASYNC)
            {
                var op = Dispatcher.CurrentDispatcher.BeginInvoke(action);
                op.Completed += (s, ev) =>
                {
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
        #region Queue Workers
        #region Queue Workers: Run
        private int RunQueueWorker(QueueWorkerMethod name,
            MonitorTypes type,
            QueueWorker.DoQueueWorkEventHandler DoWork,
            QueueWorker.RunWorkerCompletedEventHandler RunWorkerCompleted = null,
            QueueWorker.ProgressChangedEventHandler ProgressChanged = null)
            => QueueWorkerFactory.Run(name, type,
                                      InfoReporter, Stopwatch,
                                      GetMonitor, GetGridView,
                                      DoWork, null, RunWorkerCompleted, ProgressChanged);
        private int RunQueueWorker(QueueWorkerMethod name,
            MonitorTypes type,
            QueueWorker.DoQueueWorkEventHandler DoWork,
            QueueWorkerOptions Options,
            QueueWorker.RunWorkerCompletedEventHandler RunWorkerCompleted = null,
            QueueWorker.ProgressChangedEventHandler ProgressChanged = null)
            => QueueWorkerFactory.Run(name, type,
                                      InfoReporter, Stopwatch,
                                      GetMonitor, GetGridView,
                                      DoWork, Options, RunWorkerCompleted, ProgressChanged);
        #endregion
        #endregion
        #region File System
        #region File System: Initialization

        async Task SetupFileSystem()
        {
            Log("Setting Up File System");
            if (!LibSetting.Directories.IS_VALID)
            {
                InfoReporter.ReportError("Skipping File System Setup: ", (LibSetting.Directories.ACTIVE == "" && !LibSetting.Connection.IS_CONNECTED ?
                                                                          "No Directory Specified and Unable to Connect to uTorrent to get Default Directory" :
                                                                          "Invalid Directory: " + LibSetting.Directories.ACTIVE));
            }
            else if (activeDirectory == null)
            {
                Func<Task> action = InitializeFileSystem;
                if (USE_DISPATCHER_FOR_ASYNC)
                {
                    var op = Dispatcher.CurrentDispatcher.BeginInvoke(action);
                    op.Completed += (s, ev) => Debugger.Break();
                }
                else {
                    await action();
                }
            }
            else if (!activeDirectory.PathEquals(LibSetting.Directories.ACTIVE))
            {
                InfoReporter.ReportStatus("Resetting File System: Active Directory Changed");
            }
        }

        async Task InitializeFileSystem()
        {
            SetTitle("Initializing File System...");
            InfoReporter.ReportStatus("Initializing File System...", log: log);

            LibSetting.Labels.Labels.Clear();
            activeDirectory = new DirectoryInfo(LibSetting.Directories.ACTIVE);
            var labels = activeDirectory.GetDirectories().Select(d => d.Name).ToArray();
            LibSetting.Labels.Labels.AddRange(labels);
            var inline = new Run("Active Label Folders: ");
            var textBlock = new TextBlock(inline);
            for (var i = 0; i < labels.Count(); i++)
            {
                if (i > 0)
                {
                    textBlock.Inlines.Add(new Run(", "));
                }
                var bold = new Bold(new Run($"{i + 1}. "));
                bold.Foreground = ColorUtils.HexToBrush("#66ffb2");
                textBlock.Inlines.Add(bold);
                bold = new Bold(new Run(labels[i]));
                bold.Foreground = ColorUtils.HexToBrush("#99ccff");
                textBlock.Inlines.Add(bold);
            }
            InfoReporter.ReportStatus(textBlock);
            LibSetting.Labels.UpdateRootLabels();
            Func<Task> action = StartMonitors;
            Action OnComplete = () =>
            {
                SetTitle();
                IsFileSystemInitialized = true;
            };
            if (USE_DISPATCHER_FOR_ASYNC)
            {
                var op = Dispatcher.CurrentDispatcher.BeginInvoke(action);
                op.Completed += (s, ev) => { OnComplete(); Debugger.Break(); };
            }
            else {
                var theTask = StartMonitors();
                OnComplete();
                await theTask;
            }
        }

        async Task ResetFileSystem()
            => await StartMonitors();
        #endregion
        #region File System: Monitors		
        #region File System: Monitors: Initialize
        async Task StartMonitors()
        {
            Log("Starting All Monitors");
            await StartMetalinkMonitor();
            await StartTorrentMonitor();
            await StartUTorrentJobMonitor();
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

        async Task<QueueMonitorBase> InitializeMonitor(MonitorTypes fileType) 
            => await Task.Run(() =>
            fileType.IsTorrent()
            ? uTorrentClient?.IsConnected 
            ?? false ? (QueueMonitorBase)(TorrentMonitor = new TorrentQueueMonitor(uTorrentClient, InfoReporter)) 
            : null
            : fileType.IsMetalink()
            ? (QueueMonitorBase)(MetalinkMonitor = new MetalinkQueueMonitor(InfoReporter))
            : (QueueMonitorBase)(UTorrentJobMonitor = new UTorrentJobQueueMonitor(InfoReporter))
                     );
        #endregion        
        #region File System: Monitors: Get Monitor
        public static bool IsTorrent(Type t)
            => t != typeof(MetalinkQueueItem);
        public QueueMonitorBase GetMonitor(MonitorTypes fileType)
            => fileType.IsTorrent()
            ? (QueueMonitorBase)TorrentMonitor
            : fileType.IsMetalink()
            ? (QueueMonitorBase)MetalinkMonitor
            : (QueueMonitorBase)UTorrentJobMonitor;
        //		public QueueMonitor<TQueueItem> GetMonitor<TQueueItem>() where TQueueItem : QueueItemBase
        //		{
        //			//return IsTorrent(typeof(TQueueItem)) ? (QueueMonitor<TorrentQueueItem>)TorrentMonitor : (QueueMonitor<MetalinkQueueItem>)MetalinkMonitor;
        //		}
        #endregion
        #region File System: Monitors: Start Monitor
        async Task StartMonitor(MonitorTypes fileType)
        {
            var toggles = Toggles.GetActiveToggles(fileType);
            if (toggles.Monitor && (IsFileSystemInitialized || toggles.InitializeMonitor))
            {
                Log(fileType + " Monitor", "Starting");
                var monitor = GetMonitor(fileType);
                monitor?.Dispose();
                UnbindGridView(fileType);
                monitor = await InitializeMonitor(fileType);
                if (monitor == null)
                {
                    Log(fileType + " Monitor", "*UNABLE TO START*");
                    return;
                }
                await monitor.Start();
                InitializeGridView(fileType);
                if (toggles.QueueFilesOnStartup)
                {
                    QueueAllFiles(fileType);
                }
                LibSetting.Queue.SetHasQueuedAllFiles(fileType, toggles.QueueFilesOnStartup);
                Log(fileType + " Monitor", "Started");
            }
            else {
                Log(fileType + " Monitor", "*NOT* Starting");
            }
        }
        async void StartTorrentMonitor(object sender, RoutedEventArgs e)
            => await StartTorrentMonitor();
        async Task StartTorrentMonitor()
            => await StartMonitor(MonitorTypes.Torrent);
        async void StartMetalinkMonitor(object sender, RoutedEventArgs e)
            => await StartMetalinkMonitor();
        async Task StartMetalinkMonitor()
            => await StartMonitor(MonitorTypes.Metalink);
        async void StartUTorrentJobMonitor(object sender, RoutedEventArgs e)
            => await StartUTorrentJobMonitor();
        async Task StartUTorrentJobMonitor()
            => await StartMonitor(MonitorTypes.Job);
        #endregion
        #region File System: Monitors: Queue Actions
        #region File System: Monitors: Queue Actions: Queue All Files
        void QueueAllFiles(MonitorTypes fileType, bool clearFileSearchCache = false)
            => RunQueueWorker(QueueWorkerMethod.Queue, fileType,
                                        bgwQueueAllFiles_DoWork,
                                        new QueueWorkerOptions(clearFileSearchCache: clearFileSearchCache),
                                        bgwQueueAllFiles_RunWorkerCompleted
                                        );

        async Task bgwQueueAllFiles_DoWork(QueueWorker bgw, DoWorkEventArgs e)
            => await bgw.Monitor.QueueAllFilesBase(bgw.ReportStart, bgw.ReportProgress);
        void bgwQueueAllFiles_RunWorkerCompleted(QueueWorker bgw, RunWorkerCompletedEventArgs e)
        {
            LibSetting.Queue.SetHasQueuedAllFiles(bgw.Type, true);
            if (bgw.Type.IsTorrent())
            {
                var queue = TorrentMonitor.Queue;
                var count = LibSetting.Labels.Collection.Count;
                var queueLabelsCount = LibSetting.Labels.Queue.Count;
                foreach (var queueItem in queue)
                {
                    LibSetting.Labels.Add(queueItem.Label.Base);
                }
                var newQueueLabelsCount = LibSetting.Labels.Queue.Count;
                if (newQueueLabelsCount > queueLabelsCount)
                {
                    LibSetting.Labels.Update();
                    var newCount = LibSetting.Labels.Collection.Count;
                    InfoReporter.ReportTiming($"Added {newCount - count} Queue Labels, for a total of {newCount} Generated Labels.");
                }
            }
            if (bgw.TOGGLES.Processing.PreProcess)
            {
                ProcessQueue(bgw.Type);
            }
        }
        void QueueAllTorrents(object sender = null, RoutedEventArgs e = null)
            => QueueAllFiles(MonitorTypes.Torrent, false);

        void QueueAllMetalinks(object sender = null, RoutedEventArgs e = null)
            => QueueAllFiles(MonitorTypes.Metalink, true);

        void QueueAllUTorrentJobs(object sender = null, RoutedEventArgs e = null)
            => QueueAllFiles(MonitorTypes.Job, false);

        #endregion
        #region File System: Monitors: Queue Actions: Process Queue
        void ProcessQueue(MonitorTypes fileType, bool clearFileSearchCache = false)
            => RunQueueWorker(QueueWorkerMethod.Process, fileType,
                                        bgwProcessQueue_DoWork,
                                        new QueueWorkerOptions(
                                            clearFileSearchCache: clearFileSearchCache,
                                            gridViewPrepareItems: true,
                                            doReportOperationComplete: !fileType.IsJob())
                                        );

        async Task bgwProcessQueue_DoWork(QueueWorker bgw, DoWorkEventArgs e)
            => await bgw.Monitor.ProcessQueueBase(bgw.Items, bgw.ReportStart, bgw.ReportProgress, bgw.ReportComplete);

        void ProcessTorrentQueue(object sender = null, RoutedEventArgs e = null)
            => ProcessQueue(MonitorTypes.Torrent, false);
        void ProcessMetalinkQueue(object sender = null, RoutedEventArgs e = null)
            => ProcessQueue(MonitorTypes.Metalink, true);
        void ProcessUTorrentJobQueue(object sender = null, RoutedEventArgs e = null)
            => ProcessQueue(MonitorTypes.Job, false);

        #endregion
        #region File System: Monitors: Queue Actions: Reset Queue
        void ResetQueue(MonitorTypes fileType)
        {
            var monitor = GetMonitor(fileType);
            if (monitor != null)
            {
                monitor.Clear();
                LibSetting.Queue.SetHasQueuedAllFiles(fileType, false, true);
            }
        }
        void ResetTorrentQueue(object sender = null, RoutedEventArgs e = null)
            => ResetQueue(MonitorTypes.Torrent);

        void ResetMetalinkQueue(object sender = null, RoutedEventArgs e = null)
            => ResetQueue(MonitorTypes.Metalink);
        void ResetUTorrentJobQueue(object sender = null, RoutedEventArgs e = null)
            => ResetQueue(MonitorTypes.Job);

        #endregion
        #endregion
        #endregion
        #endregion

        #region GridView
        AutoDeferer DeferGridRefresh(MonitorTypes fileType)
            => AutoDeferer.StartNew(() => UnbindGridView(fileType),
                                    () => InitializeGridView(fileType));
        void UnbindGridView(MonitorTypes fileType)
        {
            if (USE_COLLECTION_VIEW_SOURCE)
            {
                CollectionViewSource queueItemViewSource = ((CollectionViewSource)(this.FindResource(fileType.ToString().ToLower() + "QueueItemViewSource")));
                queueItemViewSource.Source = null;
            }
            else {
                var gridView = GetGridView(fileType);
                gridView.ItemsSource = null;
            }
        }

        void InitializeGridViewDescriptors()
        {
            grdTorrents
                .AddColumnGroupDescriptor("Status")
                .AddColumnGroupDescriptor("Label")
                .AddColumnGroupDescriptor("ExtendedLabel", "Category")
                .AddColumnSortDescriptors("Status", "Label", "ExtendedLabel", "Torrent");

            grdMetalinks
                .AddColumnGroupDescriptor("Status")
                .AddColumnGroupDescriptor("Category")
                .AddColumnSortDescriptors("Status", "Category",
                                         // "Metalink",
                                         "Torrent"
                                        );

            grdUTorrent
                .AddColumnGroupDescriptor("Progress")
                .AddColumnGroupDescriptor("Changed", "IsChanged");
            //.AddColumnGroupDescriptor("Status")
            //.AddColumnGroupDescriptor("Started", "Status")
            //.AddColumnGroupDescriptor("Label")
            //.AddColumnSortDescriptors("Progress", "Changed", "Status", "Started", "Label", "Caption");
        }


        RadGridView InitializeGridView(MonitorTypes fileType)
        {
            object source;

            switch (fileType)
            {
                case MonitorTypes.Torrent:
                    source = TorrentMonitor.Queue;
                    break;
                case MonitorTypes.Metalink:
                    source = MetalinkMonitor.Queue;
                    break;
                case MonitorTypes.Job:
                    source = UTorrentJobMonitor.Queue;
                    break;
                default:
                    return null;
            }
            var gridView = GetGridView(fileType);
            if (USE_COLLECTION_VIEW_SOURCE)
            {
                CollectionViewSource queueItemViewSource = ((CollectionViewSource)(this.FindResource(fileType.ToString().ToLower() + "QueueItemViewSource")));
                queueItemViewSource.Source = source;
            }
            else {
                gridView.ItemsSource = source;
            }
            return gridView;
        }
        public RadGridView GetGridView(MonitorTypes fileType)
            => fileType.IsTorrent()
            ? grdTorrents
            : fileType.IsMetalink()
            ? grdMetalinks
            : grdUTorrent;

        #region Queue Item Events
        void TorrentQueueItem_SetAllowUpdatePath(object sender, RoutedEventArgs e)
        {
            var queueItem = ((TorrentQueueItem)grdTorrents.SelectedItem);
            queueItem.AllowUpdatePath = true;
        }
        #endregion
        #endregion

        #region Settings
        void SaveSettings()
        {
            SaveAllSettings();
            //AppSetting.Save();
            //LibSetting.Save();
            //Toggles.Save();
            //MySettings.Save();
        }
        #endregion

        #region Log
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        static void Log(string title, string text = null, string item = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            => LogUtils.Log(nameof(MainWindow), title, text, item, textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);
        #endregion


        #region Interfaces: INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(params string[] propertyNames)
            => DoOnPropertyChanged(this, PropertyChanged, propertyNames);
        #endregion

    }
}
