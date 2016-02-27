namespace wUAL.Properties.Settings.Binding
{
    using uAL.Properties.Settings.LibSettings;
    using Torrent.Properties.Settings.Binding;
    using Torrent.Properties.Settings.MySettings;
    using uAL.Properties.Settings.ToggleSettings;
    using static AllSettings.AllSettings;
    using static uAL.Properties.Settings.LibSettings.LibSettings;
    using static uAL.Properties.Settings.ToggleSettings.ToggleSettings;
    using static uAL.Properties.Settings.ToggleSettings.Toggles;
    using static uAL.Properties.Settings.ToggleSettings.QueueTypes;
    using System.Windows.Data;
    public class MyExtension : SettingBindingExtensionBase<MySettingsBase>
    {
        public MyExtension(string path) : base(path) { }
        protected override MySettingsBase Value => Settings.My;
    }
    public class LibExtension : SettingBindingExtensionBase<LibSettings>
    {
        public LibExtension(string path) : base(path) { }
        protected override LibSettings Value => Settings.Lib;
    }
    public class ToggleExtension : SettingBindingExtensionBase<ToggleSettingsBase>
    {
        public ToggleExtension(string path) : base(path) { }
        protected override ToggleSettingsBase Value => Settings.Toggle;
    }

    public abstract class ToggleBindingExtensionBase : SettingBindingExtensionBase<Toggle>
    {
        protected ToggleBindingExtensionBase(string path) : base(path) { }
        protected abstract QueueToggleStatus QueueType { get; }
        protected override Toggle Value => GetActiveToggle(this.QueueType);
        protected override BindingMode DefaultMode => BindingMode.OneWay;
    }

    public class TorrentExtension : ToggleBindingExtensionBase
    {
        public TorrentExtension(string path) : base(path) { }
        protected override QueueToggleStatus QueueType => Torrent;
    }

    public class MetalinkExtension : ToggleBindingExtensionBase
    {
        public MetalinkExtension(string path) : base(path) { }
        protected override QueueToggleStatus QueueType => Metalink;
    }

    public class JobExtension : ToggleBindingExtensionBase
    {
        public JobExtension(string path) : base(path) { }
        protected override QueueToggleStatus QueueType => Job;
    }
}