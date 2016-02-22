using System.Diagnostics;

namespace Torrent.Infrastructure
{
    using System;

    public abstract class DisposableBase : IDisposable
    {
        bool _disposed = false;
        [DebuggerNonUserCode]
        public void Dispose()
        {
            Dispose(true);
            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        [DebuggerNonUserCode]
        protected abstract void DoDispose();

        [DebuggerNonUserCode]
        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing) {
                // Clear all property values that maybe have been set
                // when the class was instantiated
                DoDispose();
            }
            // Indicate that the instance has been disposed.
            _disposed = true;
        }
    }
}
