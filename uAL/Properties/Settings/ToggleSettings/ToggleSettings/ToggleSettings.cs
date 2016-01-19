// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    public static class ToggleSettings {
	    #region Constructor
	    public static ToggleSettingsBase ToggleSetting;
        static ToggleSettings() { Load(); }
        public static void Load() { ToggleSetting = ToggleSettingsBase.Load(); }
        public static void Save() { ToggleSetting.Save(); }
        #endregion
    }
}
