namespace Torrent.Infrastructure.EAP
{
    using System.ComponentModel;

    public class AsyncTypedOperation<TItem, TArguments> : IAsyncTypedOperation<TItem, TArguments>
        where TItem : class
    {
        private AsyncOperation _operation;
        private IAsyncTaskId<TItem, TArguments> _id;
        public AsyncOperation AsyncOp { get { return _operation; } }        
        public IAsyncTaskId<TItem, TArguments> Id { get { return _id; } }
        public TItem Item
            => Id.Item;
        public TArguments Arguments
            => Id.Arguments;
        public AsyncTypedOperation() { }
        public AsyncTypedOperation(IAsyncTaskId<TItem, TArguments> id)
        {
            Set(id);
        }
        public AsyncTypedOperation(TItem item, TArguments argument)
        {
            Set(item, argument);
        }

        protected void Set(IAsyncTaskId<TItem, TArguments> id)
        {
            _operation = AsyncOperationManager.CreateOperation(id);
            _id = id;
        }

        public void Set(TItem item, TArguments arguments)
            => Set(new AsyncTaskId<TItem, TArguments>(item, arguments));
    }
}