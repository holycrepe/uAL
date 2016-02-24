// ReSharper disable InconsistentNaming

using PostSharp.Patterns.Model;

namespace uAL.Properties.Settings.ToggleSettings
{
    using static ToggleSettingsBase;
    public partial class Toggle
    {
        public class FilterToggle
        {
            ToggleSettingsFilters Instance;
            MonitorTypes Type;
            [Pure]
            bool getValue(MonitorTypes toggle) => toggle.Includes(this.Type);
            public bool Global => getValue(this.Instance.Global);
            public bool Include => getValue(this.Instance.Include);
            public bool Exclude => getValue(this.Instance.Exclude);

            public FilterToggle(ToggleSettingsFilters toggles, MonitorTypes type)
            {
                Instance = toggles;
                Type = type;
            }
        }
    }
}
