using System.ComponentModel;
using uAL.Properties.Settings.ToggleSettings;
using uAL.Queue;
namespace wUAL.Queue
{

    public class QueueWorker : QueueWorker<object>
	{
		
		#region Fields: Protected: Event Keys
		// disable once StaticFieldInGenericType
		static readonly object doQueueWorkKey = new object();

		// disable once StaticFieldInGenericType
		static readonly object progressChangedKey = new object();

		// disable once StaticFieldInGenericType
		static readonly object runWorkerCompletedKey = new object();
        #endregion

        public QueueWorker(MainWindow mw, QueueToggleStatus queueType, string name) : base(mw, queueType, name) { }

        public QueueWorker(MainWindow mw, QueueWorkerOptions options, QueueToggleStatus queueType, string name) : base(mw, options, queueType, name) { }

        public QueueWorker(MainWindow mw, QueueWorkerOptions options = null, string name = null) : base(mw, options, name) { }

        protected override void Initialize()
        {
            ProgressChanged += Queue_ProgressChanged_New;
            RunWorkerCompleted += Queue_RunWorkerCompleted;
        }

		
		#region Events
		#region Events: Do Work
		public new delegate void DoQueueWorkEventHandler(QueueWorker bgw, DoWorkEventArgs e);

		public new event DoQueueWorkEventHandler DoWork {
			add {
				base.Events.AddHandler(doQueueWorkKey, value);
			}
			remove {
				base.Events.RemoveHandler(doQueueWorkKey, value);
			}
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
		public delegate void ProgressChangedEventHandler(QueueWorker bgw, QueueProgressChangedEventArgs e);

		public new event ProgressChangedEventHandler ProgressChanged {
			add {
				base.Events.AddHandler(progressChangedKey, value);
			}
			remove {
				base.Events.RemoveHandler(progressChangedKey, value);
			}
		}

		protected override void OnProgressChanged(ProgressChangedEventArgs e)
		{
			var eventHandler = (ProgressChangedEventHandler)base.Events[progressChangedKey];
			if (eventHandler != null) {
				eventHandler(this, new QueueProgressChangedEventArgs(e.ProgressPercentage, e.UserState as QueueWorkerState));
			}
		}

		#endregion
		#region Events: Run Worker Completed
		public new delegate void RunWorkerCompletedEventHandler(QueueWorker bgw, RunWorkerCompletedEventArgs e);

		public new event RunWorkerCompletedEventHandler RunWorkerCompleted {
			add {
				base.Events.AddHandler(runWorkerCompletedKey, value);
			}
			remove {
				base.Events.RemoveHandler(runWorkerCompletedKey, value);
			}
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
	}
}


