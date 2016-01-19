
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Windows.Controls;
using Torrent.Helpers.Utils;
using wUAL.Infrastructure;
using uAL.Queue;
using Torrent.Extensions;
using uAL.Properties.Settings.ToggleSettings;

namespace wUAL.Queue
{
    using Torrent.Enums;
    using Torrent.Helpers.StringHelpers;

    public class QueueWorker<TResult> : BackgroundWorker
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

		protected readonly MainWindow MainWindow;

		protected internal readonly WorkerStopwatch Stopwatch;
		protected internal readonly ListBoxInfoReporter InfoReporter;
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
        public string Name { get; private set; }
        public string QualifiedName => Name + "." + QueueType;

	    protected internal QueueMonitorBase Monitor => _monitor ?? (_monitor = MainWindow.GetMonitor(QueueType));

	    protected internal RadGridView GridView => _gridView ?? (_gridView = MainWindow.GetGridView(QueueType));

	    public QueueToggleStatus QueueType {
			get;
			private set;
		}
		#endregion
		
		#region Constructor
		public QueueWorker(MainWindow mw, QueueToggleStatus queueType, string name=null) : this(mw, null, queueType, name) { }
		public QueueWorker(MainWindow mw, QueueWorkerOptions options, QueueToggleStatus queueType, string name = null) : this(mw, options, name) {
			SetQueueType(queueType);
		}		
		public QueueWorker(MainWindow mw, QueueWorkerOptions options = null, string name = null)
		{
			MainWindow = mw;
			Stopwatch = mw.Stopwatch;
			InfoReporter = mw.InfoReporter;
			Options = options ?? new QueueWorkerOptions();
            Name = name;
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
		public void ReportResult(TResult result)
		{
			Result = result;
		}
		
		public void ReportProgress(QueueWorkerState state)
		{
			ReportProgress(state.Progress, state);		
			//OnProgressChanged(new QueueProgressChangedEventArgs(state));
		}
		#endregion
		
		#region Setup
		public void SetQueueType(QueueToggleStatus queueType)
		{
			QueueType = queueType;
			TOGGLES = Toggles.GetActiveToggle(QueueType);
			_monitor = null;
			_gridView = null;
		}
		#endregion

		#region Run
		public int TryRunWorkerAsync(QueueToggleStatus queueType)
		{
			SetQueueType(queueType);
			return TryRunWorkerAsync();
		}
		public int TryRunWorkerAsync()
		{
            if (Monitor == null && Options.FileSystemMonitorRequired)
            {
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
		
		public void RunWorkerAsync(QueueToggleStatus queueType)
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
//				if (GridView.SelectedItems.Count == 0) {
//					Items = new object[GridView.Items.Count];
//					GridView.ItemsSource.CopyTo(Items, 0);
//				} else {
//					Items = new object[GridView.SelectedItems.Count];
//					GridView.SelectedItems.CopyTo(Items, 0);
//				}
			}
			Stopwatch.ReportStart();
			base.RunWorkerAsync();
		}
		#endregion

		#region Events
		#region Events: Handlers
		public void Queue_RunWorkerCompleted(QueueWorker<TResult> bgw, RunWorkerCompletedEventArgs e) {
			var strTime = Stopwatch.Elapsed.FormatFriendly();
			Log("Completed in " + strTime);
			InfoReporter.ReportInfoBanner("QueueWorker " + QualifiedName + " Completed in " + strTime);
            Stopwatch.ReportComplete();
		}
		public void Queue_ProgressChanged_Old(object sender, ProgressChangedEventArgs e) {
			var state = new QueueProgressChangedEventArgs((QueueWorkerState) e.UserState);
			Queue_ProgressChanged(this, state);
		}
        public void Queue_ProgressChanged_New(QueueWorker<TResult> bgw, QueueProgressChangedEventArgs e)
        {
            Queue_ProgressChanged(bgw, e);
        }

        public void Queue_ProgressChanged(QueueWorker<TResult> bgw, QueueProgressChangedEventArgs e) {
			var state = e.UserState;
            Stopwatch.ReportProgress(state);
			if (Options.OnProgressChanged.AddQueueItem) {
				// Monitor.AddToQueue(state.QueueItem);
			} else if (Options.OnProgressChanged.UpdateQueueItem) {
				// Monitor.QueueItemChanged(state.QueueItem);
			}
		}
		#endregion
		#region Events: Do Work
		public delegate void DoQueueWorkEventHandler(QueueWorker<TResult> bgw, DoWorkEventArgs e);
		public new event DoQueueWorkEventHandler DoWork {
			add { base.Events.AddHandler(doQueueWorkKey, value); }
			remove { base.Events.RemoveHandler(doQueueWorkKey, value); }
		}
		protected override void OnDoWork(DoWorkEventArgs e)
		{
			var eventHandler = (DoQueueWorkEventHandler)base.Events[doQueueWorkKey];
			if (eventHandler != null) {
				eventHandler(this, e);
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
		public new event RunWorkerCompletedEventHandler RunWorkerCompleted {
			add { base.Events.AddHandler(runWorkerCompletedKey, value); }
			remove { base.Events.RemoveHandler(runWorkerCompletedKey, value); }
		}
		protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
		{
			var eventHandler = (RunWorkerCompletedEventHandler)base.Events[runWorkerCompletedKey];
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
		[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
        protected void Log(string title, string text = null, string item = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            => LogUtils.Log("QueueWorker", title, text, item ?? QualifiedName, textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);
        #endregion
    }
}


