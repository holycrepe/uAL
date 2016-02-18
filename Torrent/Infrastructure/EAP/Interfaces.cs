namespace Torrent.Infrastructure.EAP
{
    using System.ComponentModel;

    public interface IAsyncTypedOperation<TItem, TArgument>
        where TItem : class
    {
        AsyncOperation AsyncOp { get; }
        IAsyncTaskId<TItem, TArgument> Id { get; }
        TItem Item { get; }
        TArgument Arguments { get; }
    }

    public interface IAsyncTaskId<TItem, TArgument>
        where TItem : class
    {
        TItem Item { get; }
        TArgument Arguments { get; }
    }
}