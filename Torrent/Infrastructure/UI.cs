using System;
using System.Threading;
using System.Threading.Tasks;

namespace Torrent.Infrastructure
{
    public static class UI
    {
        public static TaskScheduler TaskScheduler;

        public static Task StartNewUI(Action action)
        {
            return Task.Factory.StartNew(action, Task.Factory.CancellationToken, TaskCreationOptions.None, TaskScheduler);
        }

        public async static Task StartNew(Action action)
        {
            if (SynchronizationContext.Current == null) {
                await StartNewUI(action);
            } else {
                action();
            }
        }

        public static Task<T> StartNewUI<T>(Func<T> func)
        {
            return Task.Factory.StartNew(func, Task.Factory.CancellationToken, TaskCreationOptions.None, TaskScheduler);
        }

        public async static Task<T> StartNew<T>(Func<T> func)
        {
            if (SynchronizationContext.Current == null)
            {
                return await StartNewUI(func);
            }
            else {
                return func();
            }
        }
    }
}
