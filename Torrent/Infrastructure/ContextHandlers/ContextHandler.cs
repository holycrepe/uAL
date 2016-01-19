namespace Torrent.Infrastructure.ContextHandlers
{
    using System;
    using System.ComponentModel;
    using static ContextHandlerType;
    public class ContextHandler : DisposableBase
    {
        #region Fields / Properties
        readonly ContextHandlerStatus _status;
        Func<ContextHandlerStatus> GetStatus { get; }
        public ContextHandlerStatus Status => GetStatus?.Invoke() ?? _status;
        private bool OriginalStatus { get; set; }
        public ContextHandlerType Type { get; private set; }
        #endregion
        #region Constructors
        protected ContextHandler(ContextHandler original, ContextHandlerType? type = null)
        {
            if (original.GetStatus != null) {
                GetStatus = original.GetStatus;
            } else {
                _status = original.Status;
            }
            Initialize(type);
        }

        protected ContextHandler(ContextHandlerStatus status, ContextHandlerType? type = null)
        {
            _status = status;
            Initialize(type);
        }
        protected ContextHandler(Func<ContextHandlerStatus> getStatus, ContextHandlerType? type = null)
        {
            GetStatus = getStatus;
            Initialize(type);
        }

        void Initialize(ContextHandlerType? type)
        {
            Type = type ?? ImmediateEnabler;
            if (Type.IsImmediate()) {
                Begin();
            }
        }
        #endregion
        #region Begin / End
        public bool Begin(bool? value = null) 
            => OriginalStatus = Status.SetStatus(value ?? Type.IsEnabler());
        public void End() 
            => Status.SetStatus(OriginalStatus);
        #endregion
        #region Create New Instances
        public ContextHandler New(ContextHandlerType type = ImmediateEnabler)
        {
            return new ContextHandler(this, type);
        }
        public static ContextHandler NewDisabling(ContextHandlerStatus status)
            => new ContextHandler(status, ImmediateDisabler);
        public ContextHandler NewDisabling() => New(ImmediateDisabler);
        public static ContextHandler NewDeferredDisabling(ContextHandlerStatus status)
            => new ContextHandler(status, DeferredDisabler);
        public ContextHandler NewDeferredDisabling() => New(DeferredDisabler);
        public static ContextHandler NewDeferred(ContextHandlerStatus status)
            => new ContextHandler(status, DeferredEnabler);
        public ContextHandler NewDeferred() =>  New(DeferredEnabler);

        #endregion
        #region Operators
        public static implicit operator bool(ContextHandler value) => value.Status.Enabled;
        public static implicit operator ContextHandler(ContextHandlerStatus value) => new ContextHandler(value);
        public static implicit operator ContextHandlerStatus(ContextHandler value) => value.Status;
        #endregion
        #region Interfaces
        protected override void DoDispose() => End();
        #endregion
    }
}