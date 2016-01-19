using System;
using Torrent.Infrastructure;
// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using Torrent.Properties.Settings;

    #region Base
    public partial class ToggleSettingsBase
    {
        #region Subclasses 
        #region Subclasses: Base
        [Serializable]
        public class ToggleSettingsBaseSubclass : BaseSubSettings
        {
        	public ToggleSettingsBaseSubclass() : this($"Initializing.{nameof(ToggleSettingsBaseSubclass)}") {}
            public ToggleSettingsBaseSubclass(string name) : base(name) { }
            public override string ToString() => $"[{Name} [{string.Join(", ", AllSettings)}]";
        }
        #endregion
        #region Subclasses: PROCESS_QUEUE        
        [Serializable]
        public class ToggleProcessQueueSettings : ToggleSettingsBaseSubclass
        {
            protected override object[] AllSettings => new object[] { All, Manual, Startup, OnWatcher };            
            public QueueToggles All { get; set; }	        
            public QueueToggles Manual { get; set; }
            public QueueToggles Startup { get; set; }
            public QueueToggles OnWatcher { get; set; }            

            public ToggleProcessQueueSettings() : this($"Initializing.{nameof(ToggleProcessQueueSettings)}") {}
            public ToggleProcessQueueSettings(string name) : base(name) {
                All = new QueueToggles(nameof(All), QueueToggleStatus.All);
                Manual = new QueueToggles(nameof(Manual));
                Startup = new QueueToggles(nameof(Startup));
                OnWatcher = new QueueToggles(nameof(OnWatcher));
                All.PropertyChanged += (s, e) => OnPropertyChanged(nameof(All), nameof(Manual), nameof(Startup), nameof(OnWatcher));
                Manual.PropertyChanged += (s, e) => OnPropertyChanged(nameof(All), nameof(Manual));
                Startup.PropertyChanged += (s, e) => OnPropertyChanged(nameof(All), nameof(Startup));
                OnWatcher.PropertyChanged += (s, e) => OnPropertyChanged(nameof(All), nameof(OnWatcher));
            }
        }
        #endregion
        #region Subclasses: FILTER
        [Serializable]
        public class ToggleFilterSettings : ToggleSettingsBaseSubclass
        {
            protected override object[] AllSettings => new object[] { Global, Include, Exclude };
            public QueueToggles Global { get; set; }	        
            public QueueToggles Include { get; set; }
            public QueueToggles Exclude { get; set; }
			public ToggleFilterSettings() : this($"Initializing.{nameof(ToggleFilterSettings)}") {}
            public ToggleFilterSettings(string name) : base(name) {
                Global = new QueueToggles(nameof(Global), QueueToggleStatus.All);
                Include = new QueueToggles(nameof(Include));
                Exclude = new QueueToggles(nameof(Exclude), QueueToggleStatus.All);
                Global.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Global), nameof(Include), nameof(Exclude));
                Include.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Global), nameof(Include));
                Exclude.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Global), nameof(Exclude));
            }
        }
        #endregion
        #endregion
    }
    #endregion	    
}
