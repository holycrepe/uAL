using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Torrent.Infrastructure
{
    public static class UI
    {
        public static Window Window;
        public static TaskScheduler TaskScheduler;
        public static SynchronizationContext SyncContext;
        static bool IsUIThread()
            => SynchronizationContext.Current?.Equals(SyncContext) == true;
        public static Task StartNewUI(Action action) => Task.Factory.StartNew(action, Task.Factory.CancellationToken, TaskCreationOptions.None, TaskScheduler);

        public async static Task StartNew(Action action)
        {
            if (IsUIThread())
                action();
            else
                await StartNewUI(action);
        }

        public static Task<T> StartNewUI<T>(Func<T> func) => Task.Factory.StartNew(func, Task.Factory.CancellationToken, TaskCreationOptions.None, TaskScheduler);

        public async static Task<T> StartNew<T>(Func<T> func)
            => IsUIThread() 
            ? func() 
            : await StartNewUI(func);
    }
}
