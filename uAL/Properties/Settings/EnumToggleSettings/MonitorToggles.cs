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
    using Serialization;
    using Torrent.Extensions;
    using Torrent.Infrastructure.Enums.Toggles;
    [DataContract(Namespace = Namespaces.Default)]
    [PostSharp.Patterns.Model.NotifyPropertyChanged]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class MonitorToggles<T> : EnumToggles<MonitorTypes, T>
        where T : struct
    {
        public MonitorToggles() : base() { }
        public MonitorToggles(object toggles, MonitorTypes flag) : base(toggles as IEnumToggles<MonitorTypes, MonitorTypes>, flag) { }
    }
    public class MonitorToggles
        : EnumToggles<MonitorTypes>
    {
        
    }

    public class MonitorToggle : MonitorToggles<bool>
    {
        
    }
}
