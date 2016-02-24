// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using Torrent.Properties.Settings;
    using static Torrent.Properties.Settings.BaseSettings;
    using static Torrent.Helpers.Utils.DebugUtils;
    public static class ToggleSettings
    {
        static ToggleSettingsBase _settings;
        public static ToggleSettingsBase Toggles
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
                Toggles = new ToggleSettingsBase();
            }
            DEBUG.Noop();
        }

        public static void Load() { Toggles = ToggleSettingsBase.Load(); }
        public static void Save() { Toggles.Save(); }
    }
}
