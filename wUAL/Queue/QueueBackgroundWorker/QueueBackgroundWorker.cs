using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace uAL.Queue.QueueBackgroundWorker
    {
        using System.ComponentModel;
        using System.ComponentModel.Design.Serialization;
        using System.Diagnostics;
        using System.Security.Permissions;
        using System.Threading;
    

        public partial class QueueBackgroundWorker<TUserState, TResult, TArgument> : Component where TUserState : class where TResult : class where TArgument : class
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
            private AsyncOperation asyncOperation = null;
            private readonly WorkerThreadStartDelegate threadStart;
            private readonly SendOrPostCallback operationCompleted;
            private readonly SendOrPostCallback progressReporter;

            public QueueBackgroundWorker()
            {
                threadStart = new WorkerThreadStartDelegate(WorkerThreadStart);
                operationCompleted = new SendOrPostCallback(AsyncOperationCompleted);
                progressReporter = new SendOrPostCallback(ProgressReporter);
            }

            private void AsyncOperationCompleted(TArgument arg)
            {
                isRunning = false;
                cancellationPending = false;
                OnRunWorkerCompleted((RunWorkerCompletedTypedEventArgs)arg);
            }

            [
              Browsable(false),
            ]
            public bool CancellationPending
            {
                get { return cancellationPending; }
            }

            public void CancelAsync()
            {
                if (!WorkerSupportsCancellation)
                {
                    throw new InvalidOperationException("Worker doesn't support cancellation");
                }

                cancellationPending = true;
            }
            
            public event DoWorkEventHandler DoWork
            {
                add
                {
                    this.Events.AddHandler(doWorkKey, value);
                }
                remove
                {
                    this.Events.RemoveHandler(doWorkKey, value);
                }
            }
            
            [
              Browsable(false),
            ]
            public bool IsBusy
            {
                get
                {
                    return isRunning;
                }
            }
            
            protected async virtual Task OnDoWork(DoWorkTypedEventArgs e)
            {
                DoWorkTypedEventHandler handler = (DoWorkTypedEventHandler)(Events[doWorkKey]);
                if (handler != null)
                {
                    await handler(this, e);
                }
            }
            
            protected virtual void OnRunWorkerCompleted(RunWorkerCompletedTypedEventArgs e)
            {
            RunWorkerCompletedTypedEventHandler handler = (RunWorkerCompletedTypedEventHandler)(Events[runWorkerCompletedKey]);
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            protected virtual void OnProgressChanged(ProgressChangedTypedEventArgs e)
            {
            ProgressChangedTypedEventHandler handler = (ProgressChangedTypedEventHandler)(Events[progressChangedKey]);
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            
            public event ProgressChangedTypedEventHandler ProgressChanged
            {
                add
                {
                    this.Events.AddHandler(progressChangedKey, value);
                }
                remove
                {
                    this.Events.RemoveHandler(progressChangedKey, value);
                }
            }

            // Gets invoked through the AsyncOperation on the proper thread. 
            private void ProgressReporter(object arg)
                => OnProgressChanged((ProgressChangedTypedEventArgs)arg);

        // Cause progress update to be posted through current AsyncOperation.
            public void ReportProgress(int percentProgress)
            => ReportProgress(percentProgress, null);

        // Cause progress update to be posted through current AsyncOperation.
        public void ReportProgress(int percentProgress, TUserState userState)
            {
                if (!WorkerReportsProgress)
                {
                    throw new InvalidOperationException("Worker doesn't report progress");
                }

                ProgressChangedTypedEventArgs args = new ProgressChangedTypedEventArgs(percentProgress, userState);

                if (asyncOperation != null)
                {
                    asyncOperation.Post(progressReporter, args);
                }
                else
                {
                    progressReporter(args);
                }
            }

            public void RunWorkerAsync()
                => RunWorkerAsync(null);

            public void RunWorkerAsync(object argument)
            {
                if (isRunning)
                {
                    throw new InvalidOperationException("Worker already running");
                }

                isRunning = true;
                cancellationPending = false;

                asyncOperation = AsyncOperationManager.CreateOperation(null);
                threadStart.BeginInvoke(argument,
                                        null,
                                        null);
            }
            
            public event RunWorkerCompletedEventHandler RunWorkerCompleted
            {
                add
                {
                    this.Events.AddHandler(runWorkerCompletedKey, value);
                }
                remove
                {
                    this.Events.RemoveHandler(runWorkerCompletedKey, value);
                }
            }

            [
                DefaultValue(false)
            ]
            public bool WorkerReportsProgress
            {
                get { return workerReportsProgress; }
                set { workerReportsProgress = value; }
            }

            [
              DefaultValue(false)
            ]
            public bool WorkerSupportsCancellation
            {
                get { return canCancelWorker; }
                set { canCancelWorker = value; }
            }

            private delegate Task WorkerThreadStartDelegate(object argument);

            private async Task WorkerThreadStart(object argument)
            {
                object workerResult = null;
                Exception error = null;
                bool cancelled = false;

                try
                {
                    DoWorkEventArgs doWorkArgs = new DoWorkEventArgs(argument);
                    await OnDoWork(doWorkArgs);
                    if (doWorkArgs.Cancel)
                    {
                        cancelled = true;
                    }
                    else
                    {
                        workerResult = doWorkArgs.Result;
                    }
                }
                catch (Exception exception)
                {
                    error = exception;
                }

                RunWorkerCompletedEventArgs e =
                    new RunWorkerCompletedEventArgs(workerResult, error, cancelled);

                asyncOperation.PostOperationCompleted(operationCompleted, e);
            }

        }
    }
