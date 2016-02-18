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
        #region Delegates: DoWork
        public delegate Task DoWorkTypedEventHandler(
            QueueBackgroundWorker<TUserState, TResult, TArgument> sender, 
            DoWorkTypedEventArgs e);

        [
            DefaultEvent("DoWork"),
            HostProtection(SharedState = true)
        ]

        //
        // Summary:
        //     Provides data for the System.ComponentModel.BackgroundWorker.DoWork event handler.
        public class DoWorkTypedEventArgs: CancelEventArgs
        {
            public DoWorkTypedEventArgs(TArgument argument)
            {
                Argument = argument;
            }

            public TArgument Argument { get; }
            public TResult Result { get; set; }
        }
        #endregion
        #endregion
    }
}
