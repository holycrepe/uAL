namespace Torrent.Infrastructure.EAP
{
    using System.ComponentModel;
    public abstract partial class AsyncQueuedWorker<TSender, TItem, TArguments, TResult> : Component
      where TSender : class
      where TItem : class
    {
        public class Operation : IAsyncTypedOperation<TItem, TArguments>
        {
            #region Events
            private EventHandlerList _events = new EventHandlerList();
            private static readonly object progressChangedKey = new object();
            private static readonly object runWorkerCompletedKey = new object();
            protected EventHandlerList Events
                => this._events;
            public event Events.ProgressChangedHandler ProgressChanged
            {
                add { if (value != null) { Events.AddHandler(progressChangedKey, value); } }
                remove { Events.RemoveHandler(progressChangedKey, value); }
            }
            public event Events.CompletedHandler RunWorkerCompleted
            {
                add { if (value != null) { Events.AddHandler(runWorkerCompletedKey, value); } }
                remove { Events.RemoveHandler(runWorkerCompletedKey, value); }
            }
            public void OnProgressChanged(TSender sender, Events.ProgressChangedArgs e)
                => ((Events.ProgressChangedHandler)(this.Events[progressChangedKey]))?.Invoke(sender, e);
            public void OnRunWorkerCompleted(TSender sender, Events.CompletedArgs e)
                => ((Events.CompletedHandler)(this.Events[runWorkerCompletedKey]))?.Invoke(sender, e);
            #endregion
            public AsyncOperation AsyncOp { get; }
            public IAsyncTaskId<TItem, TArguments> Id { get; }
            public TItem Item
                => Id.Item;
            public TArguments Arguments
                => Id.Arguments;
            public Operation() { }
            public Operation(IAsyncTaskId<TItem, TArguments> id, Events.ProgressChangedHandler onProgressChanged=null, Events.CompletedHandler onRunWorkerCompleted=null)
            {
                AsyncOp = AsyncOperationManager.CreateOperation(id);
                Id = id;
                ProgressChanged += onProgressChanged;
                RunWorkerCompleted += onRunWorkerCompleted;
            }
            public Operation(TItem item, TArguments arguments, Events.ProgressChangedHandler onProgressChanged = null, Events.CompletedHandler onRunWorkerCompleted = null) : this(new AsyncTaskId<TItem, TArguments>(item, arguments), onProgressChanged, onRunWorkerCompleted)
            {

            }
        }
    }
}