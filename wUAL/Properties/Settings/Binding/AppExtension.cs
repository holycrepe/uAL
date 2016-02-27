namespace wUAL.Properties.Settings.Binding
{
    using Torrent.Properties.Settings.Binding;
    using AppSettings;
    using static AppSettings.AppSettings;
    using static AllSettings.AllSettings;
    using AllSettings;
    using System.Windows.Data;
    public class AppExtension : SettingBindingExtensionBase<AppSettingsBase>
    {
        public AppExtension() : base() { }
        public AppExtension(string path) : base(path) { }
        protected override AppSettingsBase Value => AppSetting;
    }

    public class AllExtension : SettingBindingExtensionBase<AllSettingsBase>
    {
        public AllExtension() : base() { }
        public AllExtension(string path) : base(path) { }
        protected override AllSettingsBase Value => AllSettings.Settings;
    }
}