using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Torrent.Queue;
using uAL.Properties.Settings.ToggleSettings;

namespace uAL.Queue
{
    using Properties.Settings.LibSettings;
    using Torrent.Enums;
    using Torrent.Helpers.Utils;
    using static uAL.Properties.Settings.LibSettings.LibSettings;

    public delegate void QueueOnProgressChangedHandler(QueueWorkerState state);

    public delegate void QueueOnProgressChangedHandler<TQueueItem>(QueueWorkerState<TQueueItem> state)
        where TQueueItem : QueueItemBase;

    //public delegate void QueueOnCompleteHandler<TQueueItem>(TQueueItem item);

    //public delegate void QueueOnStartHandler(int maximum);

    public abstract class QueueMonitor<TQueueItem> : QueueMonitorBase, IQueueMonitor<TQueueItem>
        where TQueueItem : QueueItemBase
    {
        public const int MAX_DEGREE_OF_PARALLELISM = 63;
        public static QueueOnProgressChangedHandler<TQueueItem> QueueOnProgressChangedHandlerPromoter(
            QueueOnProgressChangedHandler OnProgressChanged)
            => QueueOnProgressChangedHandlerPromoter<TQueueItem>(OnProgressChanged);

        public abstract MonitorTypes QueueType { get; }
        public QueueWorkerState<TQueueItem> NewQueueWorkerState(TQueueItem queueItem)
            => new QueueWorkerState<TQueueItem>(QueueType, queueItem);
        public Toggle TOGGLES
            => ToggleSettings.Toggles.GetActiveToggles(QueueType);

        protected string activeDir
            => LibSetting.Directories.ACTIVE;

        protected string addedDir
            => LibSetting.Directories.ADDED;

        #region Report Status

        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        protected void Log(string title, string text = null, string item = null,
                           PadDirection textPadDirection = PadDirection.Default, string textSuffix = null,
                           PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null,
                           int random = 0)
            =>
                LogUtils.Log($"{QueueType}Queue", title, text, item, textPadDirection, textSuffix, titlePadDirection,
                             titleSuffix, random);
#pragma warning disable 1998
        protected virtual async Task PerformUpdate(int newCount, string method, string methodVerb)
        {
            if (newCount > 0) {
                Log($"{methodVerb} {newCount} {QueueType}s");
            } else {
                Log($"Not {methodVerb} {QueueType}s: No {QueueType}s found");
            }
        }
#pragma warning restore 1998

        #endregion

        #region Process Queue		

        public override Task ProcessQueueBase(IEnumerable<object> SelectedItems, 
                                              QueueOnStartHandler OnStart = null,
                                              QueueOnProgressChangedHandler OnProcessQueueComplete = null,
                                              QueueWorkerOnCompleteHandler OnQueueWorkerComplete = null)
            => ProcessQueue(SelectedItems, OnStart, QueueOnProgressChangedHandlerPromoter(OnProcessQueueComplete), OnQueueWorkerComplete);

        public Task ProcessQueue(DataControl grid, 
                                 QueueOnStartHandler OnStart = null,
                                 QueueOnProgressChangedHandler<TQueueItem> OnProcessQueueComplete = null,
                                 QueueWorkerOnCompleteHandler OnQueueWorkerComplete = null)
            => ProcessQueue(GetDataControlItems<object>(grid), OnStart, OnProcessQueueComplete, OnQueueWorkerComplete);

        public abstract Task ProcessQueue(IEnumerable<object> SelectedItems, QueueOnStartHandler OnStart = null,
                                          QueueOnProgressChangedHandler<TQueueItem> OnProcessQueueComplete = null,
                                          QueueWorkerOnCompleteHandler OnQueueWorkerComplete = null);

        #endregion

        #region Queue All Files		

        public override Task QueueAllFilesBase(bool isStartup, QueueOnStartHandler OnStart = null,
                                               QueueOnProgressChangedHandler OnProgressChanged = null)
            => QueueAllFiles(isStartup, OnStart, QueueOnProgressChangedHandlerPromoter(OnProgressChanged));

        public Task QueueAllFiles(QueueOnStartHandler OnStart,
                                  QueueOnProgressChangedHandler<TQueueItem> OnProgressChanged)
            => QueueAllFiles(false, OnStart, OnProgressChanged);

        public abstract Task QueueAllFiles(bool isStartup, QueueOnStartHandler OnStart = null,
                                           QueueOnProgressChangedHandler<TQueueItem> OnProgressChanged = null);

        #endregion Queue All Files						
    }
}
