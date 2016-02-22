using System;
using System.Collections.Generic;
using Telerik.Windows.Controls;
using Torrent.Infrastructure.InfoReporters;
using uAL.Properties.Settings.ToggleSettings;
using uAL.Queue;
using wUAL.Infrastructure;

namespace wUAL.Queue
{
    public class QueueWorkerFactory
    {
        static Dictionary<string, QueueWorker> QueueWorkerCache =
            new Dictionary<string, QueueWorker>();

        public static QueueWorker Create(QueueWorkerMethod name,
                                         MonitorTypes type,
                                         ListBoxInfoReporter infoReporter,
                                         WorkerStopwatch stopwatch,
                                         Func<MonitorTypes, QueueMonitorBase> getMonitor,
                                         Func<MonitorTypes, RadGridView> getGridView,
                                         QueueWorker.DoQueueWorkEventHandler DoWork,
                                         QueueWorkerOptions Options = null,
                                         QueueWorker.RunWorkerCompletedEventHandler RunWorkerCompleted = null,
                                         QueueWorker.ProgressChangedEventHandler ProgressChanged = null
            )
        {
            var Key = name + "." + type;
            QueueWorker bgw;
            if (QueueWorkerCache.ContainsKey(Key)) {
                bgw = QueueWorkerCache[Key];
            } else {
                QueueWorkerCache[Key] =
                    bgw = new QueueWorker(infoReporter, stopwatch, getMonitor, getGridView, Options, type, name);
                bgw.DoWork += DoWork;
                if (ProgressChanged != null) {
                    bgw.ProgressChanged += ProgressChanged;
                }
                if (RunWorkerCompleted != null) {
                    bgw.RunWorkerCompleted += RunWorkerCompleted;
                }
            }
            return bgw;
        }

        public static int Run(QueueWorkerMethod name,
                  MonitorTypes type,
                  ListBoxInfoReporter infoReporter,
                  WorkerStopwatch stopwatch,
                  Func<MonitorTypes, QueueMonitorBase> getMonitor,
                  Func<MonitorTypes, RadGridView> getGridView,
                  QueueWorker.DoQueueWorkEventHandler DoWork,
                  QueueWorkerOptions Options = null,
                  QueueWorker.RunWorkerCompletedEventHandler RunWorkerCompleted = null,
                  QueueWorker.ProgressChangedEventHandler ProgressChanged = null
) => Create(name, type,
              infoReporter, stopwatch,
              getMonitor, getGridView,
              DoWork, Options, RunWorkerCompleted, ProgressChanged).TryRunWorkerAsync();
    }
}
