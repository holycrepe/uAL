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
    //using PostSharp.Patterns.Model;
    using Serialization;
    using Torrent.Extensions;
    using Torrent.Infrastructure.Enums.Toggles;
    using static MonitorTypes;

    [DataContract(Name = "Toggles", Namespace = Namespaces.Default)]
    [KnownType(typeof(MonitorTogglesFilters<MonitorTypes>)),
        KnownType(typeof(MonitorTogglesProcessing<MonitorTypes>)),
        KnownType(typeof(MonitorTypes))]
    [PostSharp.Patterns.Model.NotifyPropertyChanged]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class MonitorTogglesBase : MonitorTogglesBase<MonitorTypes>, IEnumToggles<MonitorTypes>
    {
        public MonitorTogglesBase()
        {
            AutoExpandGroups = All;
            Monitor = Disabled;
            InitializeMonitor = All;
            Watcher = Disabled;
            QueueFilesOnStartup = All;
        }
        public Toggle GetActiveToggles(MonitorTypes monitorType)
            => this.GetActiveToggles<Toggle, MonitorTypes>(monitorType);
    }
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class Toggle : MonitorTogglesBase<bool>
    {

    }
    [DataContract(Name = "Toggles", Namespace = Namespaces.Default)]
    [KnownType(typeof(MonitorTogglesFilters<MonitorTypes>)),
        KnownType(typeof(MonitorTogglesProcessing<MonitorTypes>)),
        KnownType(typeof(MonitorTypes))]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [PostSharp.Patterns.Model.NotifyPropertyChanged]
    public class MonitorTogglesBase<T> : EnumToggleSettings<MonitorTypes, T>
        where T : struct
    {
        [PostSharp.Patterns.Model.IgnoreAutoChangeNotification]
        protected override object[] DebuggerDisplayProperties
            =>
                new object[]
                {
                    nameof(Filters),
                    Filters,
                    nameof(Processing),
                    Processing,
                    nameof(Monitor),
                    Monitor,
                    nameof(InitializeMonitor),
                    InitializeMonitor,
                    nameof(Watcher),
                    Watcher,
                    nameof(AutoExpandGroups),
                    AutoExpandGroups,
                    nameof(QueueFilesOnStartup),
                    QueueFilesOnStartup,                    
                };

        [DataMember]
        public MonitorTogglesFilters<T> Filters { get; set; }
            = new MonitorTogglesFilters<T>();
        [DataMember]
        public MonitorTogglesProcessing<T> Processing { get; set; }
            = new MonitorTogglesProcessing<T>();

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
        public void Load(MonitorTogglesBase<T> other)
        {
            if (other != null)
            {
                this.Processing.Load(other.Processing);
                this.Filters.Load(other.Filters);
                this.AutoExpandGroups = other.AutoExpandGroups;
                this.Monitor = other.Monitor;
                this.InitializeMonitor = other.InitializeMonitor;
                this.Watcher = other.Watcher;
                this.QueueFilesOnStartup = other.QueueFilesOnStartup;
            }
        }
    }

    public class x
    {
        public void y()
        {
            var x = new MonitorTogglesBase();
            x.AutoExpandGroups = Main;
            var xx = x.GetActiveToggles(MonitorTypes.Torrent);
            var filters = xx.Filters;
            var xfilters = x.Filters;
        }
    } 
}

