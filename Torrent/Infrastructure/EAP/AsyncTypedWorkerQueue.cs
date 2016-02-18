using System;

namespace Torrent.Infrastructure.EAP
{
    using System.Collections.Concurrent;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Threading;
    
    public abstract partial class AsyncQueuedWorker<TSender, TItem, TArguments, TResult> 
        where TSender : class
        where TItem : class
    {        
        protected class OperationQueue
        {
            internal int MaxConcurrency { get; set; }
            ConcurrentDictionary<IAsyncTaskId<TItem, TArguments>, Operation> queue
                = new ConcurrentDictionary<IAsyncTaskId<TItem, TArguments>, Operation>();
            ConcurrentDictionary<IAsyncTaskId<TItem, TArguments>, Operation> active
                = new ConcurrentDictionary<IAsyncTaskId<TItem, TArguments>, Operation>();
            protected ConcurrentQueue<IAsyncTaskId<TItem, TArguments>> queuedTasks
                = new ConcurrentQueue<IAsyncTaskId<TItem, TArguments>>();
            internal OperationQueue() : this(1) { }
            internal OperationQueue(int maxConcurrency)
            {
                this.MaxConcurrency = maxConcurrency;
            }
            internal bool Remove(Operation asyncOp)
                => Remove(asyncOp.Id);
            internal bool Remove(IAsyncTaskId<TItem, TArguments> taskId)
            {
                Operation cancelled;
                if (this.queue.ContainsKey(taskId))
                {
                    return this.queue.TryRemove(taskId, out cancelled);
                }
                else if (this.active.ContainsKey(taskId))
                {
                    return this.active.TryRemove(taskId, out cancelled);
                }
                return false;
            }
            internal bool TaskCancelled(IAsyncTaskId<TItem, TArguments> taskId)
                => (!queue.ContainsKey(taskId) || queue[taskId] == null) && (!active.ContainsKey(taskId) || active[taskId] == null);
            internal bool Exists(Operation asyncOp)
                => Exists(asyncOp.Id);
            internal bool Exists(IAsyncTaskId<TItem, TArguments> taskId)
                => this.queue.ContainsKey(taskId) || this.active.ContainsKey(taskId);
            bool IsActiveQueueFull
                => this.Active >= this.MaxConcurrency;
            internal Operation Get(IAsyncTaskId<TItem, TArguments> taskId)
            {
                if (this.queue.ContainsKey(taskId))
                {
                    return this.queue[taskId];
                }
                if (this.active.ContainsKey(taskId))
                {
                    return this.active[taskId];
                }
                throw new ArgumentException($"Could not find task id {taskId}");
            }
            internal bool Enqueue(Operation asyncOp)
            {                
                if (this.IsActiveQueueFull)
                {
                    queue[asyncOp.Id] = asyncOp;
                    queuedTasks.Enqueue(asyncOp.Id);
                    return false;
                }
                active[asyncOp.Id] = asyncOp;
                return true;
            }
            internal Operation Dequeue()
            {
                IAsyncTaskId<TItem, TArguments> taskId;
                if (!queuedTasks.TryDequeue(out taskId))
                {
                    throw new ThreadStateException("Error Dequeuing pending task");
                }
                Operation asyncOp;
                if (!queue.TryRemove(taskId, out asyncOp))
                {
                    throw new ThreadStateException("Error removing pending task");
                }

                active[taskId] = asyncOp;
                return asyncOp;
            }
            internal int Active
                => active.Count;
            internal int Total
                => Active + Queued;
            internal int Queued
                => queue.Count;
        }        
        
    }
}
