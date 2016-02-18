// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using Torrent.Properties.Settings;
    using static Torrent.Properties.Settings.BaseSettings;
    using static Torrent.Helpers.Utils.DebugUtils;
    public static class ToggleSettings
    {
        static ToggleSettingsBase _settings;
        public static ToggleSettingsBase ToggleSetting
        {
            get { return _settings ?? (_settings = ToggleSettingsBase.Load()); }
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
                ToggleSetting = new ToggleSettingsBase();
            }
            DEBUG.Noop();
        }

        public static void Load() { ToggleSetting = ToggleSettingsBase.Load(); }
        public static void Save() { ToggleSetting.Save(); }
    }
}
