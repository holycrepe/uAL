namespace Torrent.Properties.Settings.MySettings
{
    using Properties.Settings;
    using static Properties.Settings.BaseSettings;
    using static Helpers.Utils.DebugUtils;
    public static partial class MySettings
    {
        static MySettingsBase _settings;
        public static MySettingsBase MySetting
        {
            get { return _settings ?? (_settings = MySettingsBase.Load()); }
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
        static MySettings() {
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
                MySetting = new MySettingsBase();
            }
            DEBUG.Noop();
        }

        public static void Load() { MySetting = MySettingsBase.Load(); }
        public static void Save() { MySetting.Save(); }
    }
}
