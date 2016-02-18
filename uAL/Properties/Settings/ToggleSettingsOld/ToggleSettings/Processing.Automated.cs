using System;
using Torrent.Infrastructure;

// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using System.Xml.Serialization;
    using PostSharp.Patterns.Model;
    using System.Runtime.Serialization;
    using System.Diagnostics;
    using Serialization;
    public partial class ToggleSettingsBase
    {
        [DataContract(Namespace = Namespaces.Default)]
        [KnownType(typeof(QueueToggles))]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        [NotifyPropertyChanged]
        public class ToggleSettingsProcessingAutomated : ToggleSettingsBaseSubclass
        {
            [IgnoreAutoChangeNotification]
            protected override object[] DebuggerDisplayProperties => new object[] { Manual, Startup, OnWatcher };

            [DataMember]
            public QueueToggles Manual { get; set; }
            [DataMember]
            public QueueToggles Startup { get; set; }
            [DataMember]
            public QueueToggles OnWatcher { get; set; }

            public ToggleSettingsProcessingAutomated() : this($"Initializing.{nameof(ToggleSettingsProcessingAutomated)}") { }

            public ToggleSettingsProcessingAutomated(string name) : base(name)
            {
                Manual = new QueueToggles(nameof(Manual));
                Startup = new QueueToggles(nameof(Startup));
                OnWatcher = new QueueToggles(nameof(OnWatcher));
            }

            public void Load(ToggleSettingsProcessingAutomated other)
            {
                if (other != null)
                {
                    Manual.Load(other.Manual);
                    Startup.Load(other.Startup);
                    OnWatcher.Load(other.OnWatcher);
                }
            }
        }
    }    
}
