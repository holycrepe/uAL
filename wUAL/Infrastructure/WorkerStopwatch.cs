using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Timers;
using uAL.Queue;

namespace wUAL.Infrastructure
{
    using Torrent.Enums;
    using Torrent.Extensions;
    using Torrent.Helpers.Utils;
    using static Torrent.Infrastructure.NotifyPropertyChangedBase;

    public class WorkerStopwatch : Stopwatch, INotifyPropertyChanged, IDisposable
    {
        #region Private Variables

        int _total, _value, _number;
        string _status, _title, _text;
        TimeSpan Initialization = TimeSpan.FromSeconds(0);
        Timer progressUpdater;
        bool _disposed = false;

        #endregion

        #region Fields

        #region Fields: Value

        public int Total
        {
            get { return _total; }
            set
            {
                if (_total != value) {
                    _total = value;
                    OnPropertyChanged(nameof(Total), nameof(IsActive), nameof(IsRunning), nameof(HasReported));
                }
            }
        }

        public int Value
        {
            get { return _value; }
            set
            {
                if (_value != value) {
                    var hasReportedChanged = (value >= 1 && _value < 1) || (_value >= 1 && value < 1);
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                    if (hasReportedChanged) {
                        OnPropertyChanged(nameof(HasReported));
                    }
                }
            }
        }

        public int Number
        {
            get { return _number; }
            set
            {
                if (_number != value) {
                    _number = value;
                    OnPropertyChanged(nameof(Number));
                }
            }
        }

        public double Progress
            => GetProgress();

        #endregion

        #region Fields: Info

        public string Status
        {
            get { return _status; }
            internal protected set
            {
                if (_status != value) {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public string Title
        {
            get { return _title; }
            internal protected set
            {
                if (_title != value) {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Text
        {
            get { return _text; }
            internal protected set
            {
                if (_text != value) {
                    _text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public bool IsActive =>
            IsRunning && Total >= 1;

        public bool HasReported =>
            IsActive && Value >= 1;

        #endregion

        #region Fields: Times

        double adjustedSeconds
            => Value == 0 ? 0 : Math.Max(0, Elapsed.TotalSeconds - Initialization.TotalSeconds);

        double estimatedSeconds
            => Value == 0 ? 0 : adjustedSeconds/Value*Total;

        double remainingSeconds
            => Value == 0 ? 0 : estimatedSeconds - adjustedSeconds;

        public TimeSpan Estimated =>
            TimeSpan.FromSeconds(Value == 0 ? 0 : estimatedSeconds + Initialization.TotalSeconds);

        public TimeSpan Remaining =>
            TimeSpan.FromSeconds(Value == 0 ? 0 : remainingSeconds + Initialization.TotalSeconds);

        #endregion

        #endregion

        #region Initialization

        public WorkerStopwatch(long progressUpdaterFrequency = 0)
        {
            reset();
            progressUpdater = new Timer(progressUpdaterFrequency == 0 ? 1000 : progressUpdaterFrequency);
            progressUpdater.Elapsed += ProgressUpdater_Elapsed;
        }

        public new static WorkerStopwatch StartNew() => StartNew(0);

        public static WorkerStopwatch StartNew(long progressUpdaterFrequency)
            => new WorkerStopwatch(progressUpdaterFrequency).Restart();

        #endregion

        #region Event Handlers

        private void ProgressUpdater_Elapsed(object sender, ElapsedEventArgs e)
            => OnPropertyChanged(nameof(Estimated), nameof(Remaining), nameof(Elapsed));

        #endregion

        #region Calculations

        public int GetProgress(int offset = 0)
            => (int) Value.GetProgress(Total, offset);

        #endregion

        #region Start / Stop Overrides

        public new WorkerStopwatch Restart()
        {
            reset();
            start();
            base.Restart();
            OnPropertyChanged(nameof(IsActive), nameof(HasReported));
            return this;
        }

        void start() => progressUpdater.Start();

        public new void Start()
        {
            start();
            base.Start();
            OnPropertyChanged(nameof(IsActive), nameof(HasReported));
        }

        void stop() => progressUpdater.Stop();

        public new void Stop()
        {
            stop();
            base.Stop();
            OnPropertyChanged(nameof(IsActive), nameof(HasReported));
        }

        void reset()
        {
            Total = Value = 0;
            Status = Title = Text = "";
            Initialization = TimeSpan.FromSeconds(0);
        }

        public new void Reset()
        {
            reset();
            stop();
            base.Reset();
            OnPropertyChanged(nameof(IsActive), nameof(HasReported));
        }

        #endregion

        #region Report Progress

        public void ReportStart()
            => Restart();

        public void ReportStart(int total)
        {
            Initialization = Elapsed;
            Total = total;
            Status = Text = "";
            Title = "Preparing to Run...";
        }

        public void ReportComplete()
            => Reset();

        public int ReportProgress(QueueWorkerState state)
            => ReportProgress(state.Status, state.Label, state.Name, state.Number);

        public int ReportProgress(object status, string title, string text, int number)
        {
            Status = (status as string ?? status.ToString() + ":");
            Title = title;
            Text = text;
            Number = number;
            return Tick();
        }

        public int Tick(int step = 1) => Value += step;

        #endregion

        #region Interfaces

        #region Interfaces: INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged(string propertyName)
        //{
        //    Log(nameof(OnPropertyChanged), propertyName);
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
        protected void OnPropertyChanged(params string[] propertyNames)
            => DoOnPropertyChanged(this, PropertyChanged, propertyNames);

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
            if (!_disposed) {
                if (disposing) {
                    // Clear all property values that maybe have been set
                    // when the class was instantiated
                    if (progressUpdater != null) {
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

        #region Logging

        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        void Log(string title, string text = null, string item = null,
                 PadDirection textPadDirection = PadDirection.Default, string textSuffix = null,
                 PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            =>
                LogUtils.Log(nameof(Stopwatch), title, text, item, textPadDirection, textSuffix, titlePadDirection,
                             titleSuffix, random);

        #endregion
    }
}
