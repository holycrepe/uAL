namespace Torrent.Infrastructure
{
    using System;

    public abstract class DisposableBase : IDisposable
    {
        bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected abstract void DoDispose();

        void Dispose(bool disposing)
        {
            if (!_disposed) {
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
}
