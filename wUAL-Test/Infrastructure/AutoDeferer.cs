using System;

namespace wUAL.Infrastructure
{
    public class AutoDeferer : IDisposable
    {
        bool _disposed = false;
        readonly Action OnStart;
        readonly Action OnDispose;
        public AutoDeferer(Action onStart, Action onDispose) : this(onDispose) { OnStart = onStart; }
        public AutoDeferer(Action onDispose) { OnDispose = onDispose; }

        public static AutoDeferer StartNew(Action onStart, Action onDispose)
        {
            var deferer = new AutoDeferer(onStart, onDispose);
            deferer.Begin();
            return deferer;
        }

        public void Begin()
            => OnStart?.Invoke();

        public void End()
            => OnDispose();

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed) {
                if (disposing) {
                    OnDispose();
                }
                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}
