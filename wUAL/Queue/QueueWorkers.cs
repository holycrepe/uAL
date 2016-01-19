using System.ComponentModel;
using uAL.Properties.Settings.ToggleSettings;

namespace wUAL.Queue
{

    public class QueueWorker<TResult, TArgument> : QueueWorker<TResult> where TArgument : class
	{
		#region Fields: Protected: Event Keys
		// disable once StaticFieldInGenericType
		static readonly object doQueueWorkKey = new object();
        #endregion

        public QueueWorker(MainWindow mw, QueueToggleStatus queueType, string name) : base(mw, queueType, name) { }

        public QueueWorker(MainWindow mw, QueueWorkerOptions options, QueueToggleStatus queueType, string name) : base(mw, options, queueType, name) { }

        public QueueWorker(MainWindow mw, QueueWorkerOptions options = null, string name = null) : base(mw, options, name) { }


        #region Events: Do Work
        public new delegate void DoQueueWorkEventHandler(QueueWorker<TResult, TArgument> bgw, DoQueueWorkEventArgs<TArgument> e);
		public new event DoQueueWorkEventHandler DoWork {
			add { Events.AddHandler(doQueueWorkKey, value); }
			remove { Events.RemoveHandler(doQueueWorkKey, value); }
		}

		protected void OnDoWork(DoQueueWorkEventArgs<TArgument> e)
		{
			var eventHandler = (DoQueueWorkEventHandler)base.Events[doQueueWorkKey];
			if (eventHandler != null) {
				eventHandler(this, e);
			}
		}		
		
		protected override void OnDoWork(DoWorkEventArgs e)
		{
			OnDoWork(new DoQueueWorkEventArgs<TArgument>(e.Argument as TArgument));
		}
		#endregion
	}
	
	
	
}
