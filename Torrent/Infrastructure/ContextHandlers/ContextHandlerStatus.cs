using System;
using System.Diagnostics;

namespace Torrent.Infrastructure.ContextHandlers
{
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class ContextHandlerStatus : IDebuggerDisplay
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

        public string DebuggerDisplay(int level = 1)
            => $"<{nameof(ContextHandlerStatus)}> {DebuggerDisplaySimple()}";

        public string DebuggerDisplaySimple(int level = 1)
            => Enabled ? "Enabled" : "Disabled";

        public static implicit operator bool(ContextHandlerStatus status) => status.Enabled;
        public static implicit operator ContextHandlerStatus(bool status) => new ContextHandlerStatus(status);
    }
}
