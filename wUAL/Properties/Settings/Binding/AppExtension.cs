namespace wUAL.Properties.Settings.Binding
{
    using Torrent.Properties.Settings.Binding;
    using static AppSettings;

    public class AppExtension : SettingBindingExtensionBase<AppSettings>
    {
        public AppExtension(string path) : base(path) { }
        protected override AppSettings Value => AppSetting;
    }                
}