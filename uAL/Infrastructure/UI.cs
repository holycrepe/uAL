using System;
using System.Threading;
using System.Threading.Tasks;

namespace uAL.Infrastructure
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
            if (SynchronizationContext.Current == null)
            {
                await StartNewUI(action);
            }
            else {
                action();
            }
        }
    }
}
