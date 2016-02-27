namespace wUAL.Properties.Settings.AppSettings
{
    using Torrent.Properties.Settings;
    using static Torrent.Properties.Settings.BaseSettings;
    using static Torrent.Helpers.Utils.DebugUtils;
    public static partial class AppSettings
    {
        static AppSettingsBase _settings;
        public static AppSettingsBase AppSetting
        {
            get { return _settings ?? (_settings = AppSettingsBase.Load()); }
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
        static AppSettings() {
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
                AppSetting = new AppSettingsBase();
            }
            DEBUG.Noop();
        }

        public static void Load() { AppSetting = AppSettingsBase.Load(); }
        public static void Save() {
            AppSetting.Save();
        }
    }
}
