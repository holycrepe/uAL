using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Windows.Controls;
using Torrent.Helpers.Utils;
using wUAL.Infrastructure;
using uAL.Queue;
using Torrent.Extensions;
using uAL.Properties.Settings.ToggleSettings;
using uAL.Services;

namespace wUAL.Queue
{
    using System.Threading.Tasks;
    using Torrent.Enums;
    using Torrent.Helpers.StringHelpers;
    using static Torrent.Helpers.Utils.DebugUtils;
    using static uAL.Properties.Settings.ToggleSettings.ToggleSettings;

    public class QueueWorker<TResult> : QueueBackgroundWorker
    {
        #region Fields

        #region Fields: Protected

        #region Fields: Protected: Event Keys

        // disable once StaticFieldInGenericType
        static readonly object doQueueWorkKey = new object();

        // disable once StaticFieldInGenericType
        static readonly object progressChangedKey = new object();

        // disable once StaticFieldInGenericType
        static readonly object runWorkerCompletedKey = new object();

        #endregion

        #region Fields: Protected: Components

        protected RadGridView _gridView;

        protected internal readonly WorkerStopwatch Stopwatch;
        protected internal readonly ListBoxInfoReporter InfoReporter;

        private Func<MonitorTypes, QueueMonitorBase> GetMonitor;
        private Func<MonitorTypes, RadGridView> GetGridView;

        #endregion

        #region Fields: Protected: Data

        protected QueueMonitorBase _monitor;
        protected internal IEnumerable<object> Items;
        protected internal DateTime StartTime;
        public TResult Result;

        #endregion

        #region Fields: Protected: Options

        protected internal Toggle TOGGLES;
        protected readonly QueueWorkerOptions Options;

        #endregion

        #endregion

        #endregion

        #region Properties

        public QueueWorkerMethod Method { get; }
        public string QualifiedName => $"{Type}.{Method}";

        protected internal QueueMonitorBase Monitor => _monitor ?? (_monitor = GetMonitor(Type));
        protected internal RadGridView GridView => _gridView ?? (_gridView = GetGridView(Type));

        public MonitorTypes Type { get; private set; }

        #endregion

        #region Constructor

        public QueueWorker(ListBoxInfoReporter infoReporter, WorkerStopwatch stopwatch,
                           Func<MonitorTypes, QueueMonitorBase> getMonitor,
                           Func<MonitorTypes, RadGridView> getGridView,
                           MonitorTypes type,
                           QueueWorkerMethod name = QueueWorkerMethod.Unknown) :
                               this(infoReporter, stopwatch, getMonitor, getGridView, null, type, name) {}

        public QueueWorker(ListBoxInfoReporter infoReporter, WorkerStopwatch stopwatch,
                           Func<MonitorTypes, QueueMonitorBase> getMonitor,
                           Func<MonitorTypes, RadGridView> getGridView,
                           QueueWorkerOptions options, MonitorTypes type,
                           QueueWorkerMethod name = QueueWorkerMethod.Unknown) :
                               this(infoReporter, stopwatch, getMonitor, getGridView, options, name)
        {
            SetQueueType(type);
        }

        public QueueWorker(ListBoxInfoReporter infoReporter, WorkerStopwatch stopwatch,
                           Func<MonitorTypes, QueueMonitorBase> getMonitor,
                           Func<MonitorTypes, RadGridView> getGridView,
                           QueueWorkerOptions options = null, QueueWorkerMethod name = QueueWorkerMethod.Unknown)
        {
            Stopwatch = stopwatch;
            InfoReporter = infoReporter;
            GetMonitor = getMonitor;
            GetGridView = getGridView;
            Options = options ?? new QueueWorkerOptions();
            this.DoReportOperationCompleted = options.DoReportOperationComplete;
            Method = name;
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
            Initialize();
        }

        protected virtual void Initialize()
        {
            ProgressChanged += Queue_ProgressChanged_Old;
            //ProgressChanged += Queue_ProgressChanged;
            RunWorkerCompleted += Queue_RunWorkerCompleted;
        }

        #endregion

        #region Reporting

        public void ReportResult(TResult result) { Result = result; }

        public void ReportStart(int maximum)
            => Stopwatch.ReportStart(maximum);

        public void ReportProgress(QueueWorkerState state)
            => ReportProgress((int) Stopwatch.GetProgress(1), state);

        #endregion

        #region Setup

        public void SetQueueType(MonitorTypes queueType)
        {
            Type = queueType;
            TOGGLES = Toggles.GetActiveToggles(Type);
            _monitor = null;
            _gridView = null;
        }

        #endregion

        #region Run

        public virtual void OnRunWorkerAsync()
        {
            if (Options.ClearFileSearchCache) {
                QueryDuplicateFileNamesCache.Clear();
            }
            Stopwatch.ReportStart();
        }

        public int TryRunWorkerAsync(MonitorTypes queueType)
        {
            SetQueueType(queueType);
            return TryRunWorkerAsync();
        }

        public int TryRunWorkerAsync()
        {
            if (Monitor == null && Options.FileSystemMonitorRequired) {
                Log("Not Starting: Monitor Unavailable");
                return -1;
            }
            if (IsBusy) {
                Log("Not Starting: Busy");
                return 0;
            }
            RunWorkerAsync();
            return 1;
        }

        public void RunWorkerAsync(MonitorTypes queueType)
        {
            SetQueueType(queueType);
            RunWorkerAsync();
        }

        public new void RunWorkerAsync()
        {
            Log("Starting");
            StartTime = DateTime.Now;
            if (Options.GridView.PrepareItems) {
                Items = QueueMonitorBase.GetDataControlItems<object>(GridView);
            }
            OnRunWorkerAsync();
            base.RunWorkerAsync();
        }

        #endregion

        #region Events

        #region Events: Handlers

        public void Queue_RunWorkerCompleted(QueueWorker<TResult> bgw, RunWorkerCompletedEventArgs e)
        {
            var strTime = Stopwatch.Elapsed.FormatFriendly();
            Log("Completed in " + strTime);
            InfoReporter.ReportInfoTitle("QueueWorker " + QualifiedName + " Completed in " + strTime);
            Stopwatch.ReportComplete();
        }

        //public void Queue_ProgressChanged_Old(object sender, ProgressChangedEventArgs e) 
        //          => Queue_ProgressChanged(this, new QueueProgressChangedEventArgs((int) Stopwatch.GetProgress(1), (QueueWorkerState)e.UserState));
        //      public void Queue_ProgressChanged_New(QueueWorker<TResult> bgw, QueueProgressChangedEventArgs e)
        //          => Queue_ProgressChanged(bgw, e);

        //public void Queue_ProgressChanged(QueueWorker<TResult> bgw, QueueProgressChangedEventArgs e) 
        //    => Stopwatch.ReportProgress(e.UserState);

        public void Queue_ProgressChanged_Old(object sender, ProgressChangedEventArgs e)
        {
            if (e?.UserState == null) {
                DEBUG.Break();
            }
            Queue_ProgressChanged(this,
                                  new QueueProgressChangedEventArgs((int) Stopwatch.GetProgress(1),
                                                                    (QueueWorkerState) e.UserState));
        }

        public void Queue_ProgressChanged_New(QueueWorker<TResult> bgw, QueueProgressChangedEventArgs e)
        {
            if (e?.UserState == null) {
                DEBUG.Break();
            }
            Queue_ProgressChanged(bgw, e);
        }

        public void Queue_ProgressChanged(QueueWorker<TResult> bgw, QueueProgressChangedEventArgs e)
        {
            if (e?.UserState == null) {
                DEBUG.Break();
            }
            Stopwatch.ReportProgress(e.UserState);
        }

        public virtual void Queue_DoWork() { }

        #endregion

        #region Events: Do Work

        public delegate Task DoQueueWorkEventHandler(QueueWorker<TResult> bgw, DoWorkEventArgs e);

        public new event DoQueueWorkEventHandler DoWork
        {
            add { base.Events.AddHandler(doQueueWorkKey, value); }
            remove { base.Events.RemoveHandler(doQueueWorkKey, value); }
        }

        protected async override Task OnDoWork(DoWorkEventArgs e)
        {
            var eventHandler = (DoQueueWorkEventHandler) base.Events[doQueueWorkKey];
            if (eventHandler != null) {
                Queue_DoWork();
                await eventHandler(this, e);
            }
        }

        #endregion

        #region Events: Progress Changed

        //public delegate void ProgressChangedEventHandler(QueueWorker<TResult> bgw, QueueProgressChangedEventArgs e);
        //public new event ProgressChangedEventHandler ProgressChanged {
        //	add { base.Events.AddHandler(progressChangedKey, value); }
        //	remove { base.Events.RemoveHandler(progressChangedKey, value); }
        //}
        //protected void OnProgressChanged(QueueProgressChangedEventArgs e) {
        //	var eventHandler = (ProgressChangedEventHandler)base.Events[progressChangedKey];
        //	if (eventHandler != null) {
        //		eventHandler(this, e);
        //	}
        //}
        //protected override void OnProgressChanged(ProgressChangedEventArgs e)
        //{
        //	OnProgressChanged(new QueueProgressChangedEventArgs(e.ProgressPercentage, e.UserState as QueueWorkerState));
        //}

        #endregion

        #region Events: Run Worker Completed

        public delegate void RunWorkerCompletedEventHandler(QueueWorker<TResult> bgw, RunWorkerCompletedEventArgs e);

        public new event RunWorkerCompletedEventHandler RunWorkerCompleted
        {
            add { base.Events.AddHandler(runWorkerCompletedKey, value); }
            remove { base.Events.RemoveHandler(runWorkerCompletedKey, value); }
        }

        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            var eventHandler = (RunWorkerCompletedEventHandler) base.Events[runWorkerCompletedKey];
            if (eventHandler != null) {
                eventHandler(this, e);
            }
        }

        #endregion

        #endregion

        #region Casting

        //public static implicit operator QueueWorker(QueueWorker<TResult> self)
        //{
        //	return self;
        //}

        #endregion

        #region Log        

        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        protected void Log(string title, string text = null, string item = null,
                           PadDirection textPadDirection = PadDirection.Default, string textSuffix = null,
                           PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null,
                           int random = 0)
            =>
                LogUtils.Log("QueueWorker", title, text, item ?? QualifiedName, textPadDirection, textSuffix,
                             titlePadDirection, titleSuffix, random);

        #endregion
    }
}
