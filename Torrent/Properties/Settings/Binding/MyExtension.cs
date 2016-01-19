namespace Torrent.Properties.Settings.Binding
{
    using Settings.MySettings;
    using static Settings.MySettings.MySettings;
    public class MyExtension : SettingBindingExtensionBase<MySettingsBase>
    {
        public MyExtension(string path) : base(path) { }
        protected override MySettingsBase Value => MySetting;
    }
}