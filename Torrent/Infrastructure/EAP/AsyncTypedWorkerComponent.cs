using System;

namespace Torrent.Infrastructure.EAP
{
    using System.Collections.Concurrent;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    //public abstract class AsyncTypedWorker<TSender, TItem, TArguments, TResult>
    //    : AsyncTypedWorker<TSender, AsyncTypedOperation<TSender, TArguments>, TSender, TArguments, TResult>
    //    where TSender : class
    //{

    //}

    public abstract partial class AsyncQueuedWorker<TSender, TItem, TArguments, TResult> : Component        
        where TSender : class
        where TItem : class
    {
        #region Delegates
        //public Events.CompletedHandler Completed1;
        //public Events.ProgressChangedHandler ProgressChanged1;
        private delegate void WorkerEventHandler(Operation asyncTypedOp);
        protected SendOrPostCallback onProgressReportDelegate;
        protected SendOrPostCallback onRunWorkerCompletedDelegate;
        #endregion
        #region Events
        public event Events.ProgressChangedHandler ProgressChanged
        {
            add { base.Events.AddHandler(progressChangedKey, value); }
            remove { base.Events.RemoveHandler(progressChangedKey, value); }
        }
        public event Events.CompletedHandler RunWorkerCompleted
        {
            add { base.Events.AddHandler(runWorkerCompletedKey, value); }
            remove { base.Events.RemoveHandler(runWorkerCompletedKey, value); }
        }
        #endregion
        #region Fields
        #region Fields: Private, Static
        //private static readonly object doWorkKey = new object();
        private static readonly object runWorkerCompletedKey = new object();
        private static readonly object progressChangedKey = new object();
        #endregion
        #region Fields: Public
        protected OperationQueue Queue { get; }
        #endregion
        #endregion
        #region Properties
        public int MaxConcurrency
        {
            get { return this.Queue.MaxConcurrency; }
            set { if (this.MaxConcurrency != value) { this.Queue.MaxConcurrency = value; } }
        }
        protected virtual int MAX_CONCURRENCY_DEFAULT { get; } = 1;
        #endregion

        #region Constructor
        public AsyncQueuedWorker()
        {
            Queue = new OperationQueue(MAX_CONCURRENCY_DEFAULT);
            Initialize();
        }
        public AsyncQueuedWorker(int maxConcurrency)
        {
            Queue = new OperationQueue(maxConcurrency);
            Initialize();
        }
        protected virtual void Initialize()
        {
            InitializeComponent();
            InitializeDelegates();
        }
        protected virtual void InitializeComponent() { }
        protected virtual void InitializeDelegates()
        {
            onProgressReportDelegate =
                new SendOrPostCallback(ReportProgress);
            onRunWorkerCompletedDelegate =
                new SendOrPostCallback(ReportCompleted);
        }
        #endregion



        #region Implementing Completed Method
        /// <summary>
        /// This is the method that the underlying, free-threaded asynchronous behavior will invoke.
        /// This will happen on an arbitrary thread.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="exception"></param>
        /// <param name="canceled"></param>
        /// <param name="asyncOp"></param>
        private void DoWorkCompleted(TResult result, Exception exception, bool canceled, Operation asyncOp)

        {
            // If the task was not previously canceled,
            // remove the task from the lifetime collection.
            if (!canceled && !Queue.Remove(asyncOp.Id))
            {
                throw new ThreadStateException($"Unable to remove running task {asyncOp.Id}");
            }

            // Package the results of the operation in a AsyncEvents.CompletedArgs.
            var e = new Events.CompletedArgs(result, exception, canceled, asyncOp);

            // End the task. The asyncOp object is responsible 
            // for marshaling the call.
            asyncOp.AsyncOp.PostOperationCompleted(onRunWorkerCompletedDelegate, e);

            // Note that after the call to OperationCompleted, 
            // asyncOp is no longer usable, and any attempt to use it
            // will cause an exception to be thrown.

            RunQueue();
        }
        #endregion
        #region Implement Cancelled
        /// <summary>
        /// Utility method for determining if a task has been canceled.
        /// </summary>
        /// <param name="asyncOp"></param>
        /// <returns></returns>
        private bool TaskCancelled(Operation asyncOp)
            => TaskCancelled(asyncOp.Id);

        /// <summary>
        /// Utility method for determining if a task has been canceled.
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private bool TaskCancelled(IAsyncTaskId<TItem, TArguments> taskId)
            => Queue.TaskCancelled(taskId);

        /// <summary>
        /// This method performs the actual work. It must be overriden in implemented classes
        /// It is executed on the worker thread.
        /// </summary>
        /// <param name="userState"></param>
        /// <param name="asyncOp"></param>
        /// <returns></returns>
        protected abstract TResult DoWork(Operation asyncOp);

        /// <summary>
        /// This method is a wrapper for DoWork(), which performs the actual work
        /// It is executed on the worker thread.
        /// </summary>
        /// <param name="userState"></param>
        /// <param name="asyncOp"></param>
        private void OnDoWork(Operation asyncOp)
        {
            var result = default(TResult);
            Exception e = null;

            // Check that the task is still active.
            // The operation may have been canceled before
            // the thread was scheduled.
            if (!TaskCancelled(asyncOp))
            {
                try
                {
                    // DoWork...

                    result = DoWork(asyncOp);
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            this.DoWorkCompleted(result, e, TaskCancelled(asyncOp), asyncOp);
        }

        #endregion
        #region Events
        #region Events: Completed
        // This method is invoked via the AsyncOperation object,
        // so it is guaranteed to be executed on the correct thread.
        private void ReportCompleted(object state)
            => OnRunWorkerCompleted(state as Events.CompletedArgs);
        protected void OnRunWorkerCompleted(Events.CompletedArgs e)
        {
            var sender = this as TSender;
            ((Events.CompletedHandler)(base.Events[runWorkerCompletedKey]))?.Invoke(sender, e);
            e.UserState.OnRunWorkerCompleted(sender, e);
        }
        protected virtual void ReportCompleted(Events.CompletedArgs e)
            => e.Operation.Post(this.onRunWorkerCompletedDelegate, e);
        protected virtual void ReportCompleted(TResult result, Operation asyncOp)
            => ReportCompleted(new Events.CompletedArgs(result, asyncOp));
        #endregion
        #region Events: Report Progress
        // This method is invoked via the AsyncOperation object,
        // so it is guaranteed to be executed on the correct thread.
        private void ReportProgress(object state)
            => OnProgressChanged(state as Events.ProgressChangedArgs);
        protected void OnProgressChanged(Events.ProgressChangedArgs e)
        {
            var sender = this as TSender;
            ((Events.ProgressChangedHandler)(base.Events[progressChangedKey]))?.Invoke(sender, e);
            e.UserState.OnProgressChanged(sender, e);            
        }
        protected virtual void ReportProgress(Events.ProgressChangedArgs e)
            => e.Operation.Post(this.onProgressReportDelegate, e);
        protected virtual void ReportProgress(double progress, Operation asyncOp)
            => ReportProgress(new Events.ProgressChangedArgs(progress, asyncOp));
        protected virtual void ReportProgress(long current, long total, Operation asyncOp)
            => ReportProgress(Convert.ToDouble(current) / Convert.ToDouble(total) * 100, asyncOp);
        #endregion
        #endregion
        #region Queueing
        Operation Enqueue(Operation asyncOp)
        {
            if (Queue.Enqueue(asyncOp))
            {
                StartOperation(asyncOp);
            }
            else {
                RunQueue();
            }
            return asyncOp;
        }
        void StartOperation(Operation asyncOp, WorkerEventHandler workerDelegate = null)
            => (workerDelegate ?? new WorkerEventHandler(OnDoWork))
                .BeginInvoke(asyncOp, null, null);
        protected virtual void RunQueue()
        {
            WorkerEventHandler workerDelegate = null;
            while (Queue.Active < MaxConcurrency && Queue.Queued > 0)
            {
                // Start the asynchronous operation.
                StartOperation(Queue.Dequeue(),
                    (workerDelegate ?? (workerDelegate = new WorkerEventHandler(OnDoWork))));
            }
        }
        #endregion
        #region Implementation of Start/Cancel Methods
        public virtual void OnQueueWorkerAsync(Operation asyncOp)
        {

        }
        /// <summary>
        /// This method starts an asynchronous calculation.
        /// First, it checks the supplied task ID for uniqueness.
        /// If taskId is unique, it creates a new WorkerEventHandler
        /// and calls its BeginInvoke method to start the work implemented via DoWork().
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="taskId"></param>
        public virtual Operation QueueWorkerAsync(TItem item, TArguments argument, Events.ProgressChangedHandler onProgressChanged = null, Events.CompletedHandler onRunWorkerCompleted = null)
        {

            // Create an AsyncOperation for taskId.
            var asyncOp = new Operation(item, argument, onProgressChanged, onRunWorkerCompleted);
            OnQueueWorkerAsync(asyncOp);
            // Multiple threads will access the task dictionary,
            // so it must be locked to serialize access.

            if (Queue.Exists(asyncOp))
            {
                throw new ArgumentException("Task ID parameter must be unique",
                    nameof(asyncOp) + "." + nameof(asyncOp.Id));
            }
            return Enqueue(asyncOp);
        }
        /// <summary>
        /// This method cancels a pending asynchronous operation.
        /// </summary>
        /// <param name="asyncOp"></param>
        public void CancelAsync(Operation asyncOp)
        {
            if (asyncOp != null)
            {
                if (Queue.Remove(asyncOp))
                {
                    RunQueue();
                }
                else {
                    throw new ThreadStateException($"Unable to remove running task {asyncOp.Id}");
                }
            }
        }
        /// <summary>
        /// This method cancels a pending asynchronous operation.
        /// </summary>
        /// <param name="taskId"></param>
        public void CancelAsync(IAsyncTaskId<TItem, TArguments> taskId)
        {
            if (!Queue.Exists(taskId))
            {
                throw new ArgumentException($"Unable to Cancel Operation #{taskId}: Task ID not found", nameof(taskId));
            }
            CancelAsync(Queue.Get(taskId));
        }
        #endregion
    }    
}
