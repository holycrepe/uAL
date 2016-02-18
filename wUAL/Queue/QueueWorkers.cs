using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Torrent.Infrastructure.InfoReporters;
using uAL.Properties.Settings.ToggleSettings;
using uAL.Queue;
using wUAL.Infrastructure;

namespace wUAL.Queue
{
    public class QueueWorker<TResult, TArgument> : QueueWorker<TResult> where TArgument : class
    {
        #region Fields: Protected: Event Keys

        // disable once StaticFieldInGenericType
        static readonly object doQueueWorkKey = new object();

        #endregion

        #region Constructors

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

        #endregion

        #region Events: Do Work

        public new delegate Task DoQueueWorkEventHandler(
            QueueWorker<TResult, TArgument> bgw, DoQueueWorkEventArgs<TArgument> e);

        public new event DoQueueWorkEventHandler DoWork
        {
            add { Events.AddHandler(doQueueWorkKey, value); }
            remove { Events.RemoveHandler(doQueueWorkKey, value); }
        }

        protected async Task OnDoWork(DoQueueWorkEventArgs<TArgument> e)
        {
            var eventHandler = (DoQueueWorkEventHandler) base.Events[doQueueWorkKey];
            if (eventHandler != null) {
                await eventHandler(this, e);
            }
        }

        protected override async Task OnDoWork(DoWorkEventArgs e)
        {
            await OnDoWork(new DoQueueWorkEventArgs<TArgument>(e.Argument as TArgument));
        }

        #endregion
    }
}
