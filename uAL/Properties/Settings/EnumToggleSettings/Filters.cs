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
    public class MonitorTogglesFilters : MonitorTogglesFilters<MonitorTypes>, IEnumToggles<MonitorTypes>
    {
        public MonitorTogglesFilters()
        {
            Global = All;
            Include = Disabled;
            Exclude = All;
        }
    }

    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class MonitorToggleFilters : MonitorTogglesFilters<bool>
    {

    }

    [DataContract(Namespace = Namespaces.Default)]
    [KnownType(typeof(MonitorTypes))]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [NotifyPropertyChanged]
    public class MonitorTogglesFilters<T> : MonitorToggles<T>
        where T : struct
    {
        [IgnoreAutoChangeNotification]
        protected override object[] DebuggerDisplayProperties => new object[] {
            nameof(Global), Global,
            nameof(Include), Include,
            nameof(Exclude), Exclude
        };
        [DataMember]
        [SafeForDependencyAnalysis]
        public T Global { get { return Get(); } set { Set(value); } }
        [DataMember]
        [SafeForDependencyAnalysis]
        public T Include { get { return Get(); } set { Set(value); } }
        [DataMember]
        [SafeForDependencyAnalysis]
        public T Exclude { get { return Get(); } set { Set(value); } }
        public MonitorTogglesFilters() : base() { }
        public MonitorTogglesFilters(object toggles, MonitorTypes flag) : base(toggles as IEnumToggles<MonitorTypes, MonitorTypes>, flag) { }
        public void Load(MonitorTogglesFilters<T> other)
        {
            if (other != null)
            {
                Global = other.Global;
                Include = other.Include;
                Exclude = other.Exclude;
            }
        }
    }
}
