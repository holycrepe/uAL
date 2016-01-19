namespace Torrent.Properties.Settings.MySettings
{

    public static partial class MySettings
	{
        #region Constructor
        public static MySettingsBase MySetting;
        static MySettings() { Load(); }
        #endregion
        public static void Load() { MySetting = MySettingsBase.Load(); }
        public static void Save() { MySetting.Save(); }
	}		 
}

