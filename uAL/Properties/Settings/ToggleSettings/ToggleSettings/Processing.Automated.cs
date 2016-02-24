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
    using static MonitorTypes;
    public partial class ToggleSettingsBase
    {
        [DataContract(Namespace = Namespaces.Default)]
        [KnownType(typeof(MonitorTypes))]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        [NotifyPropertyChanged]
        public class ToggleSettingsProcessingAutomated : ToggleSettingsBaseSubclass
        {
            [IgnoreAutoChangeNotification]
            protected override object[] DebuggerDisplayProperties => new object[] {
                nameof(Manual), Manual,
                nameof(Startup), Startup,
                nameof(OnWatcher), OnWatcher
            };

            [DataMember]
            public MonitorTypes Manual { get; set; }
            [DataMember]
            public MonitorTypes Startup { get; set; }
            [DataMember]
            public MonitorTypes OnWatcher { get; set; }

            public ToggleSettingsProcessingAutomated() : this($"Initializing.{nameof(ToggleSettingsProcessingAutomated)}") { }

            public ToggleSettingsProcessingAutomated(string name) : base(name)
            {
                Manual = Disabled;
                Startup = Disabled;
                OnWatcher = Disabled;
            }

            public void Load(ToggleSettingsProcessingAutomated other)
            {
                if (other != null)
                {
                    Manual = other.Manual;
                    Startup = other.Startup;
                    OnWatcher = other.OnWatcher;
                }
            }
        }
    }    
}
