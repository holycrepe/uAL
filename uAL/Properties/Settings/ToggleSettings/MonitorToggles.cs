using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uAL.Properties.Settings.ToggleSettings
{
    using System.Dynamic;
    using AddGenericConstraint;
    using Torrent.Extensions;
    using Torrent.Infrastructure.Enums.Toggles;
    public class MonitorToggles<T> : EnumToggles<MonitorTypes, T>
        where T : struct
    {

    }
    public class MonitorToggles
        : EnumToggles<MonitorTypes>
    {
        
    }

    public class MonitorToggle : MonitorToggles<bool>
    {
        
    }
}
