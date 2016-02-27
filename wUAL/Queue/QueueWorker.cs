using System;
using System.ComponentModel;
using Telerik.Windows.Controls;
using Torrent.Infrastructure.InfoReporters;
using uAL.Properties.Settings.ToggleSettings;
using wUAL.Infrastructure;
using uAL.Queue;
using System.Threading.Tasks;

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

        public QueueWorker(ListBoxInfoReporter infoReporter, WorkerStopwatch stopwatch,
                           Func<MonitorTypes, QueueMonitorBase> getMonitor,
                           Func<MonitorTypes, RadGridView> getGridView,
                           MonitorTypes type,
                           QueueWorkerMethod name) :
                               base(infoReporter, stopwatch, getMonitor, getGridView, type, name) {}

        public QueueWorker(ListBoxInfoReporter infoReporter, WorkerStopwatch stopwatch,
                           Func<MonitorTypes, QueueMonitorBase> getMonitor,
                           Func<MonitorTypes, RadGridView> getGridView,
                           QueueWorkerOptions options, MonitorTypes type,
                           QueueWorkerMethod name) :
                               base(infoReporter, stopwatch, getMonitor, getGridView, options, type, name) {}

        public QueueWorker(ListBoxInfoReporter infoReporter, WorkerStopwatch stopwatch,
                           Func<MonitorTypes, QueueMonitorBase> getMonitor,
                           Func<MonitorTypes, RadGridView> getGridView,
                           QueueWorkerOptions options = null,
                           QueueWorkerMethod name = QueueWorkerMethod.Unknown) :
                               base(infoReporter, stopwatch, getMonitor, getGridView, options, name) {}

        protected override void Initialize()
        {
            ProgressChanged += Queue_ProgressChanged_New;
            RunWorkerCompleted += Queue_RunWorkerCompleted;
        }

        #region Events

        #region Events: Do Work

        public new delegate Task DoQueueWorkEventHandler(QueueWorker bgw, DoWorkEventArgs e);

        public new event DoQueueWorkEventHandler DoWork
        {
            add { base.Events.AddHandler(doQueueWorkKey, value); }
            remove { base.Events.RemoveHandler(doQueueWorkKey, value); }
        }

        protected async override Task OnDoWork(DoWorkEventArgs e)
        {
            var eventHandler = (DoQueueWorkEventHandler) base.Events[doQueueWorkKey];
            if (eventHandler != null) {
                await eventHandler(this, e);
            }
        }

        #endregion

        #region Events: Progress Changed

        public delegate void ProgressChangedEventHandler(QueueWorker bgw, QueueProgressChangedEventArgs e);

        public new event ProgressChangedEventHandler ProgressChanged
        {
            add { base.Events.AddHandler(progressChangedKey, value); }
            remove { base.Events.RemoveHandler(progressChangedKey, value); }
        }

        protected override void OnProgressChanged(ProgressChangedEventArgs e)
        {
            var eventHandler = (ProgressChangedEventHandler) base.Events[progressChangedKey];
            if (eventHandler != null) {
                eventHandler(this,
                             new QueueProgressChangedEventArgs(e.ProgressPercentage, e.UserState as QueueWorkerState));
            }
        }

        #endregion

        #region Events: Run Worker Completed

        public new delegate void RunWorkerCompletedEventHandler(QueueWorker bgw, RunWorkerCompletedEventArgs e);

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
    }
}
