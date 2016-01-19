using System;
using System.Collections.Generic;
using uAL.Properties.Settings.ToggleSettings;

namespace wUAL.Queue
{
    public class QueueWorkerFactory
    {
        static Dictionary<Tuple<string, QueueToggleStatus>, QueueWorker> QueueWorkerCache = new Dictionary<Tuple<string, QueueToggleStatus>, QueueWorker>();
        public static QueueWorker Create(string Name, 
            QueueToggleStatus QueueType, 
            QueueWorker.DoQueueWorkEventHandler DoWork, 
            QueueWorkerOptions Options = null,
            QueueWorker.RunWorkerCompletedEventHandler RunWorkerCompleted = null,
            QueueWorker.ProgressChangedEventHandler ProgressChanged = null
            )
        {
            var Key = new Tuple<string, QueueToggleStatus>(Name, QueueType);
            QueueWorker bgw;
            if (QueueWorkerCache.ContainsKey(Key))
            {
                bgw = QueueWorkerCache[Key];
            }
            else
            { 
                QueueWorkerCache[Key] = bgw = new QueueWorker(App.Window, Options, QueueType, Name);
                bgw.DoWork += DoWork;
                if (ProgressChanged != null)
                {
                    bgw.ProgressChanged += ProgressChanged;
                }
                if (RunWorkerCompleted != null)
                {
                    bgw.RunWorkerCompleted += RunWorkerCompleted;
                }
            }
            return bgw;
        }

        public static int Run(string Name,
            QueueToggleStatus QueueType,
            QueueWorker.DoQueueWorkEventHandler DoWork,
            QueueWorkerOptions Options = null,
            QueueWorker.RunWorkerCompletedEventHandler RunWorkerCompleted = null,
            QueueWorker.ProgressChangedEventHandler ProgressChanged = null
            )
        {
            return Create(Name, QueueType, DoWork, Options, RunWorkerCompleted, ProgressChanged).TryRunWorkerAsync();
        }
        
    }
}
