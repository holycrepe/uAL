
namespace uAL.Properties.Settings.ToggleSettings
{
    using static ToggleSettings;
    using static ToggleSettingsBase;

    public static partial class Toggles
    {
        public static class FILTERS
        {
            static ToggleSettingsFilters toggles => ToggleSetting.Filters;
            public static QueueToggles Global => toggles.Global;
            public static QueueToggles Include => toggles.Include;
            public static QueueToggles Exclude => toggles.Exclude;
        }
    }
}
