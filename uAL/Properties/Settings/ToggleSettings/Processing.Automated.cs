using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uAL.Properties.Settings.ToggleSettings
{
    using System.Diagnostics;
    using System.Dynamic;
    using System.Runtime.Serialization;
    using AddGenericConstraint;
    using PostSharp.Patterns.Model;
    using Serialization;
    using Torrent.Extensions;
    using Torrent.Infrastructure.Enums.Toggles;
    using static MonitorTypes;

    [DataContract(Namespace = Namespaces.Default)]
    [KnownType(typeof(MonitorTypes))]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [PostSharp.Patterns.Model.NotifyPropertyChanged]
    public class MonitorTogglesProcessingAutomated : MonitorTogglesProcessingAutomated<MonitorTypes>, IEnumToggles<MonitorTypes>
    {
        public MonitorTogglesProcessingAutomated()
        {
            Manual = Disabled;
            Startup = Disabled;
            OnWatcher = Disabled;
        }
    }

    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class MonitorToggleProcessingAutomated : MonitorTogglesProcessingAutomated<bool>
    {

    }

    [DataContract(Namespace = Namespaces.Default)]
    [KnownType(typeof(MonitorTypes))]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [NotifyPropertyChanged]
    public class MonitorTogglesProcessingAutomated<T> : MonitorToggles<T>
        where T : struct
    {
        [IgnoreAutoChangeNotification]
        protected override object[] DebuggerDisplayProperties => new object[] {
            nameof(Manual), Manual,
            nameof(Startup), Startup,
            nameof(OnWatcher), OnWatcher
        };

        [DataMember]
        public T Manual { get; set; }
        [DataMember]
        public T Startup { get; set; }
        [DataMember]
        public T OnWatcher { get; set; }
        public void Load(MonitorTogglesProcessingAutomated<T> other)
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
