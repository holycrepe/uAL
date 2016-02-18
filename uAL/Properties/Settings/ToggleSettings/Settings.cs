// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using Torrent.Properties.Settings;
    using static Torrent.Properties.Settings.BaseSettings;
    using static Torrent.Helpers.Utils.DebugUtils;
    public static class ToggleSettings
    {
        static MonitorTogglesBase _settings;
        public static MonitorTogglesBase Toggles
        {
            get { return _settings ?? (_settings = MonitorTogglesBase.Load<MonitorTogglesBase>()); }
            set
            {
                if (_settings == null)
                {
                    _settings = value;
                }
                else
                {
                    _settings.Load(value);
                }
            }
        }
        static ToggleSettings()
        {
            if (_settings != null)
            {
                DEBUG.Break();
            }
            if (CONSTRUCTOR_ACTION == BaseSettingsConstructor.Load)
            {
                Load();
            }
            else if (CONSTRUCTOR_ACTION == BaseSettingsConstructor.Default)
            {
                Toggles = new MonitorTogglesBase();
            }
            DEBUG.Noop();
        }

        public static void Load() { Toggles = MonitorTogglesBase.Load<MonitorTogglesBase>(); }
        public static void Save() { Toggles.Save(); }
    }
}
