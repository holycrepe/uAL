namespace Torrent.Infrastructure.EAP
{
    using System.ComponentModel;
    public abstract partial class AsyncQueuedWorker<TSender, TItem, TArguments, TResult> : Component
      where TSender : class
      where TItem : class
    {
        public class TaskId : IAsyncTaskId<TItem, TArguments>
        {
            public TItem Item { get; }
            public TArguments Arguments { get; }
            public TaskId(TItem item, TArguments arguments)
            {
                Item = item;
                Arguments = arguments;
            }
        }
    }
}