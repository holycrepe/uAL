namespace Torrent.Infrastructure.ContextHandlers
{
    public class ContextHandlerStatus
    {
        public bool Enabled { get; set; }
        public ContextHandlerStatus(bool status = false) { Enabled = status; }

        public bool SetStatus(bool status)
        {
            var originalStatus = Enabled;
            Enabled = status;
            return originalStatus;
        }
        public bool Enable() => SetStatus(true);
        public bool Disable() => SetStatus(false);
        public static implicit operator bool(ContextHandlerStatus status) => status.Enabled;
        public static implicit operator ContextHandlerStatus(bool status) => new ContextHandlerStatus(status);
    }
}