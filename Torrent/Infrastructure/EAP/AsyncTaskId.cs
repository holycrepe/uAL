namespace Torrent.Infrastructure.EAP
{
    using System.ComponentModel;

    public class AsyncTaskId<TItem, TArguments> : IAsyncTaskId<TItem, TArguments>
        where TItem : class
    {
        public TItem Item { get; }
        public TArguments Arguments { get; }
        public AsyncTaskId(TItem item, TArguments arguments)
        {
            Item = item;
            Arguments = arguments;
        }
    }
}