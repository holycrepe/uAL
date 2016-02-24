using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DoWorkEventHandler = uAL.Queue.DoWorkEventHandler;


namespace wUAL.Queue
{
    [
        DefaultEvent("DoWork"),
        HostProtection(SharedState = true)
    ]
    public class QueueBackgroundWorker : Component
    {
        // Private statics
        private static readonly object doWorkKey = new object();
        private static readonly object runWorkerCompletedKey = new object();
        private static readonly object progressChangedKey = new object();

        // Private instance members
        private bool canCancelWorker = false;
        private bool workerReportsProgress = false;
        private bool cancellationPending = false;
        private bool isRunning = false;
        protected bool DoReportOperationCompleted = true;
        private AsyncOperation asyncOperation = null;
        private readonly WorkerThreadStartDelegate threadStart;
        private readonly SendOrPostCallback operationCompleted;
        private readonly SendOrPostCallback progressReporter;
        protected RunWorkerCompletedEventArgs RunWorkerCompletedEventArgs;

        public QueueBackgroundWorker()
        {
            this.threadStart = new WorkerThreadStartDelegate(WorkerThreadStart);
            this.operationCompleted = new SendOrPostCallback(AsyncOperationCompleted);
            this.progressReporter = new SendOrPostCallback(ProgressReporter);
        }

        private void AsyncOperationCompleted(object arg)
        {
            this.isRunning = false;
            this.cancellationPending = false;
            OnRunWorkerCompleted((RunWorkerCompletedEventArgs) arg);
        }

        [
            Browsable(false),
        ]
        public bool CancellationPending => this.cancellationPending;

        public void CancelAsync()
        {
            if (!this.WorkerSupportsCancellation) {
                throw new InvalidOperationException("Worker doesn't support cancellation");
            }

            this.cancellationPending = true;
        }

        public event DoWorkEventHandler DoWork
        {
            add { this.Events.AddHandler(doWorkKey, value); }
            remove { this.Events.RemoveHandler(doWorkKey, value); }
        }

        [
            Browsable(false),
        ]
        public bool IsBusy => this.isRunning;

        protected async virtual Task OnDoWork(DoWorkEventArgs e)
        {
            DoWorkEventHandler handler = (DoWorkEventHandler) (this.Events[doWorkKey]);
            if (handler != null) {
                await handler(this, e);
            }
        }

        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            RunWorkerCompletedEventHandler handler = (RunWorkerCompletedEventHandler) (this.Events[runWorkerCompletedKey]);
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = (ProgressChangedEventHandler) (this.Events[progressChangedKey]);
            if (handler != null) {
                if (e?.UserState == null) {
                    Debugger.Break();
                }
                handler(this, e);
            }
        }

        public event ProgressChangedEventHandler ProgressChanged
        {
            add { this.Events.AddHandler(progressChangedKey, value); }
            remove { this.Events.RemoveHandler(progressChangedKey, value); }
        }

        // Gets invoked through the AsyncOperation on the proper thread. 
        private void ProgressReporter(object arg) { OnProgressChanged((ProgressChangedEventArgs) arg); }

        // Cause progress update to be posted through current AsyncOperation.
        public void ReportProgress(int percentProgress) { ReportProgress(percentProgress, null); }

        // Cause progress update to be posted through current AsyncOperation.
        public void ReportProgress(int percentProgress, object userState)
        {
            if (!this.WorkerReportsProgress) {
                throw new InvalidOperationException("Worker doesn't report progress");
            }

            ProgressChangedEventArgs args = new ProgressChangedEventArgs(percentProgress, userState);

            if (this.asyncOperation != null) {
                this.asyncOperation.Post(this.progressReporter, args);
            } else {
                this.progressReporter(args);
            }
        }

        public void RunWorkerAsync()
            => RunWorkerAsync(null);

        public void RunWorkerAsync(object argument)
        {
            if (this.isRunning) {
                throw new InvalidOperationException("Worker already running");
            }

            this.isRunning = true;
            this.cancellationPending = false;

            this.asyncOperation = AsyncOperationManager.CreateOperation(null);
            this.threadStart.BeginInvoke(argument,
                                    null,
                                    null);
        }

        public event RunWorkerCompletedEventHandler RunWorkerCompleted
        {
            add { this.Events.AddHandler(runWorkerCompletedKey, value); }
            remove { this.Events.RemoveHandler(runWorkerCompletedKey, value); }
        }

        [
            DefaultValue(false)
        ]
        public bool WorkerReportsProgress
        {
            get { return this.workerReportsProgress; }
            set { this.workerReportsProgress = value; }
        }

        [
            DefaultValue(false)
        ]
        public bool WorkerSupportsCancellation
        {
            get { return this.canCancelWorker; }
            set { this.canCancelWorker = value; }
        }

        private delegate Task WorkerThreadStartDelegate(object argument);

        private async Task WorkerThreadStart(object argument)
        {
            object workerResult = null;
            Exception error = null;
            bool cancelled = false;

            try {
                DoWorkEventArgs doWorkArgs = new DoWorkEventArgs(argument);
                await OnDoWork(doWorkArgs);
                if (doWorkArgs.Cancel) {
                    cancelled = true;
                } else {
                    workerResult = doWorkArgs.Result;
                }
            } catch (Exception exception) {
                error = exception;
            }

            this.RunWorkerCompletedEventArgs = new RunWorkerCompletedEventArgs(workerResult, error, cancelled);

            if (this.DoReportOperationCompleted)
            {
                ReportComplete();
            }
        }

        public void ReportComplete()
        {
            if (this.RunWorkerCompletedEventArgs != null)
            {
                this.asyncOperation.PostOperationCompleted(this.operationCompleted, this.RunWorkerCompletedEventArgs);
                this.RunWorkerCompletedEventArgs = null;
            }
        }
    }
}

namespace uAL.Queue
{
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;

    public delegate Task DoWorkEventHandler(object sender, DoWorkEventArgs e);
}
