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
        using System.Diagnostics.CodeAnalysis;
        using System.Reflection;
    public partial class QueueBackgroundWorker<TUserState, TResult, TArgument> : Component
    {

        #region Delegates
        #region Delegates: Async Completed
        [HostProtection(SharedState = true)]
        public class AsyncCompletedTypedEventArgs : EventArgs
        {
            private readonly Exception error;
            private readonly bool cancelled;
            private readonly TUserState userState;

            [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public AsyncCompletedTypedEventArgs()
            {
                // This method was public in .NET CF, but didn't do anything.  The fact that it was public was to
                // work around a tooling issue on their side.  
            }

            public AsyncCompletedTypedEventArgs(Exception error, bool cancelled, TUserState userState)
            {
                this.error = error;
                this.cancelled = cancelled;
                this.userState = userState;
            }

            public bool Cancelled => cancelled;

            public Exception Error => error;

            public TUserState UserState => userState;

            // Call from every result 'getter'. Will throw if there's an error or operation was cancelled
            //
            [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
            protected void RaiseExceptionIfNecessary()
            {
                if (Error != null)
                {
                    throw new TargetInvocationException("Async Exception Occurred", Error);
                }
                else if (Cancelled)
                {
                    throw new InvalidOperationException("Async Operation Cancelled");
                }

            }

        }
        #endregion
        #endregion
    }
}
