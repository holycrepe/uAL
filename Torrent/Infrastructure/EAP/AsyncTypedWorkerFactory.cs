using System;

namespace Torrent.Infrastructure.EAP
{
    using System.Collections.Concurrent;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Threading;
    
    public abstract partial class AsyncTypedWorker<TSender, TOperation, TItem, TArguments, TResult> 
        where TOperation : class, IAsyncTypedOperation<TItem, TArguments>, new()
        where TSender : class
        where TItem : class
    {
        
    }
}
