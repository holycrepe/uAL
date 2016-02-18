using System;

namespace Torrent.Infrastructure.EAP
{
    using System.ComponentModel;

    //public class AsyncTypedUserState<TItem, TArguments>
    //    where TItem : class
    //{
    //    public TItem Item;
    //    public TArguments Argument;
    //    public AsyncTypedUserState(TItem item, TArguments argument) : this(item)
    //    {
    //        this.Argument = argument;
    //    }
    //    public AsyncTypedUserState(TItem item)
    //    {
    //        this.Item = item;
    //    }
    //}
    

    public partial class AsyncQueuedWorker<TSender, TItem, TArguments, TResult> 
        
    {

        public new class Events
        {
            
            public delegate void CompletedHandler(TSender sender, CompletedArgs e);
            public class CompletedArgs : AsyncCompletedEventArgs
            {
                public TResult Result { get; }
                public new Operation UserState { get; }
                public AsyncOperation Operation
                    => UserState.AsyncOp;
                public TItem Item
                    => UserState.Item;
                public CompletedArgs(TResult result, Exception error, bool cancelled, object userSuppliedState)
                    : this(result, userSuppliedState as Operation, error, cancelled)
                {

                }
                public CompletedArgs(TResult result, Operation operation, Exception error = null, bool cancelled = false)
                    : base(error, cancelled, operation)
                {
                    Result = result;
                    UserState = operation;
                }
            }
            public delegate void ProgressChangedHandler(TSender sender, ProgressChangedArgs e);
            public class ProgressChangedArgs
            {
                public double Progress { get; }
                public Operation UserState { get; }
                public AsyncOperation Operation
                    => UserState.AsyncOp;
                public TItem Item
                    => UserState.Item;
                public ProgressChangedArgs(double progress, AsyncOperation asyncOp)
                {
                    this.Progress = progress;
                    this.UserState = asyncOp.UserSuppliedState as Operation;
                }
                public ProgressChangedArgs(double progress, Operation userState)
                {
                    this.Progress = progress;
                    this.UserState = userState;
                }
            }
        }
    }
}
