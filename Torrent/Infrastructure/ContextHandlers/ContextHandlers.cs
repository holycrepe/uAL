namespace Torrent.Infrastructure.ContextHandlers
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using static ContextHandlerType;

    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class ContextHandlers : IDebuggerDisplay
    {
        #region Properties

        public ContextHandler DeferredEnabler { get; }
        public ContextHandler DeferredDisabler { get; }
        public ContextHandlerStatus Status { get; }
        public ContextHandler On => DeferredEnabler.New();
        public ContextHandler Off => DeferredEnabler.NewDisabling();

        #endregion

        #region Constructor

        public ContextHandlers() : this(false) { }
        public ContextHandlers(bool startingValue) : this(new ContextHandlerStatus(startingValue)) { }

        public ContextHandlers(ContextHandlerStatus status)
        {
            Status = status;
            DeferredEnabler = ContextHandler.NewDeferred(Status);
            DeferredDisabler = DeferredEnabler.NewDeferredDisabling();
        }

        #endregion

        #region Methods: Begin / End 

        public void Begin() => DeferredEnabler.Begin();
        public void End() => DeferredEnabler.End();
        public void BeginDisable() => DeferredDisabler.Begin();
        public void EndDisable() => DeferredDisabler.End();

        #endregion

        #region Interfaces

        public string DebuggerDisplay(int level = 1)
            => $"<{nameof(ContextHandlers)}> {DebuggerDisplaySimple()}";

        public string DebuggerDisplaySimple(int level = 1)
            => Status.DebuggerDisplaySimple();

        #endregion

        #region Operators

        public static implicit operator bool(ContextHandlers value) => value.Status.Enabled;
        public static implicit operator ContextHandlers(ContextHandlerStatus value) => new ContextHandlers(value);
        public static implicit operator ContextHandlerStatus(ContextHandlers value) => value.Status;

        #endregion
    }

    //public class ContextHandlers(ContextHandlerStatus status)
    //{
    //    public ContextHandlerStatus Status { get; )
    //}
}
