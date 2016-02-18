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
        #region Delegates: Worker Completed


        [HostProtection(SharedState = true)]
        public delegate void RunWorkerCompletedTypedEventHandler(QueueBackgroundWorker<TUserState, TResult, TArgument> sender,
                                                            RunWorkerCompletedTypedEventArgs e);

        [HostProtection(SharedState = true)]
        public class RunWorkerCompletedTypedEventArgs: AsyncCompletedTypedEventArgs<TUserState>
        {
            private TResult result;
            public RunWorkerCompletedTypedEventArgs(TResult result, Exception error, bool cancelled) : base(error, cancelled, null) { this.result = result; }
            public TResult Result { get { base.RaiseExceptionIfNecessary(); return result; } }

            // Hide from editor, since never used. 
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
            public new TUserState UserState { get { return base.UserState; } }
        }
        #endregion
        #endregion
    }
}
