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
        #region Delegates: ProgressChanged


        [HostProtection(SharedState = true)]
        public delegate void ProgressChangedTypedEventHandler(QueueBackgroundWorker<TUserState, TResult, TArgument> sender, ProgressChangedTypedEventArgs e);

        [HostProtection(SharedState = true)]
        public class ProgressChangedTypedEventArgs : EventArgs
        {
            private readonly int progressPercentage;
            private readonly TUserState userState;

            public ProgressChangedTypedEventArgs(int progressPercentage, TUserState userState)
            {
                this.progressPercentage = progressPercentage;
                this.userState = userState;
            }

            public int ProgressPercentage => progressPercentage;

            public TUserState UserState => userState;

        }

        #endregion
        #endregion
    }
}
