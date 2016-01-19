using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Timers;
using uAL.Queue;

namespace wUAL.Infrastructure
{
    using static Torrent.Infrastructure.NotifyPropertyChangedBase;
    public class WorkerStopwatch : Stopwatch, INotifyPropertyChanged, IDisposable
    {
        #region Private Variables
        int _maximum, _value;
        Timer progressUpdater;
        bool _disposed = false;
        #endregion
        #region Fields
        #region Fields: Value
        public int Maximum { get { return _maximum; } set { if (_maximum != value) { _maximum = value; OnPropertyChanged(nameof(Maximum)); }  } }
        public int Value{ get { return _value; } set { if (_value != value) { _value = value; OnPropertyChanged(nameof(Value)); } } }
        #endregion
        #region Fields: Info
        public string Status { get; internal set; }
        public string Title { get; internal set; }
        public string Text { get; internal set; }
        public bool IsActive => IsRunning && Maximum >= 1;
        #endregion
        #region Fields: Times
        int estimatedSeconds => Value == 0 ? 0 : Convert.ToInt32(Elapsed.TotalSeconds / Value * Maximum);
        int remainingSeconds => Value == 0 ? 0 : Convert.ToInt32(estimatedSeconds - Elapsed.TotalSeconds);
        public TimeSpan Estimated => TimeSpan.FromSeconds(estimatedSeconds);
        public TimeSpan Remaining => TimeSpan.FromSeconds(remainingSeconds);
        #endregion
        #endregion
        #region Initialization
        public WorkerStopwatch(long progressUpdaterFrequency = 0)
        {
            reset();
            progressUpdater = new Timer(progressUpdaterFrequency == 0  ? 1000 : progressUpdaterFrequency);
            progressUpdater.Elapsed += ProgressUpdater_Elapsed;            
        }
        public new static WorkerStopwatch StartNew() => StartNew(0);
        public static WorkerStopwatch StartNew(long progressUpdaterFrequency) => new WorkerStopwatch(progressUpdaterFrequency).Restart();
        #endregion

        #region Event Handlers
        private void ProgressUpdater_Elapsed(object sender, ElapsedEventArgs e) => OnPropertyChanged("Estimated", "Remaining", "Elapsed");
        #endregion

        #region Start / Stop Overrides
        public new WorkerStopwatch Restart()
        {
            reset();
            start();
            base.Restart();
            OnPropertyChanged(nameof(IsActive));
            return this;
        }
        void start() => progressUpdater.Start();
        public new void Start()
        {
            start();
            base.Start();
            OnPropertyChanged(nameof(IsActive));
        }
        void stop() => progressUpdater.Stop();
        public new void Stop()
        {
            stop();
            base.Stop();
            OnPropertyChanged(nameof(IsActive));
        }
        bool reset() => (((Maximum = Value = 0) == 0) && ((Status = Title = Text = "")) == "");
        
        public new void Reset()
        {
            reset();
            stop();
            base.Reset();
            OnPropertyChanged(nameof(IsActive));
        }
        #endregion

        #region Report Progress
        public void ReportStart() => Restart();
        public void ReportComplete() => Reset();
        public int ReportProgress(QueueWorkerState state) => ReportProgress(state.Total, state.Status, state.Label, state.Name);
        public int ReportProgress(int maximum, object status, string title, string text)
        {
            Status = (status as string ?? status.ToString() + ":");
            Title = title;
            Text = text;
            OnPropertyChanged("Status", "Title", "Text");
            return Tick(maximum);
        }
        public int Tick(int maximum) {
            Maximum = maximum;
            return Tick();
        }
        public int Tick() => Value++;
        #endregion

        #region Interfaces
        #region Interfaces: INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(params string[] propertyNames) => DoOnPropertyChanged(this, PropertyChanged, propertyNames);
        #endregion
        #region Interfaces: IDisposable
        public void Dispose()
        {
            Dispose(true);
            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clear all property values that maybe have been set
                    // when the class was instantiated
                    if (progressUpdater != null)
                    {
                        progressUpdater.Stop();
                        progressUpdater.Dispose();
                    }
                }
                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
        #endregion
        #endregion
    }

}
