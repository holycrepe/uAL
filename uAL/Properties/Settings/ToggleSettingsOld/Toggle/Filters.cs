// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using static Toggles.FILTERS;

    public partial class Toggle
    {public class FilterToggle
        {
            QueueTypes Type;
            bool getValue(QueueToggles toggle) => toggle.GetValue(Type);
            public bool GLOBAL => getValue(Global);
            public bool INCLUDE => getValue(Include);
            public bool EXCLUDE => getValue(Exclude);
            public FilterToggle(QueueTypes type) { Type = type; }
        }
    }
}
