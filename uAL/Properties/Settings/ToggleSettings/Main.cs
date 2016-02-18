using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uAL.Properties.Settings.ToggleSettings
{
    using System.Dynamic;
    using System.Runtime.Serialization;
    using AddGenericConstraint;
    using PostSharp.Patterns.Model;
    using Serialization;
    using Torrent.Extensions;
    using Torrent.Infrastructure.Enums.Toggles;

    [KnownType(typeof(MonitorTypes))]
    [NotifyPropertyChanged]
    public class MonitorTogglesMain<T> : MonitorToggles<T>
        where T : struct
    {
        [DataMember]
        public T AutoExpandGroups { get; set; }
        [DataMember]
        public T Monitor { get; set; }
        [DataMember]
        public T InitializeMonitor { get; set; }
        [DataMember]
        public T Watcher { get; set; }
        [DataMember]
        public T QueueFilesOnStartup { get; set; }
    }
}
