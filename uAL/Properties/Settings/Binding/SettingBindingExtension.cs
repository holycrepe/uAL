namespace uAL.Properties.Settings.Binding
{
    using Torrent.Properties.Settings.Binding;
    using System.Windows.Data;
    using Settings;
    using ToggleSettings;
    using static Settings.LibSettings;
    using static ToggleSettings.ToggleSettings;
    using static ToggleSettings.Toggles;
    using static ToggleSettings.QueueToggleStatus;

    #region General Settings
    public class LibExtension : SettingBindingExtensionBase<LibSettings>
    {
        public LibExtension(string path) : base(path) { }
        protected override LibSettings Value => LibSetting;
    }
    #endregion
    #region Toggles
    public class ToggleExtension : SettingBindingExtensionBase<ToggleSettingsBase>
    {
        public ToggleExtension(string path) : base(path) { }
        protected override ToggleSettingsBase Value => ToggleSetting;
    }

    public abstract class ToggleBindingExtensionBase : SettingBindingExtensionBase<Toggle>
    {
        protected ToggleBindingExtensionBase(string path) : base(path) { }
        protected abstract QueueToggleStatus QueueType { get; }
        protected override Toggle Value => GetActiveToggle(QueueType);
        protected override BindingMode DefaultMode => BindingMode.OneWay;
    }
    public class TorrentExtension : ToggleBindingExtensionBase
    {
        public TorrentExtension(string path) : base(path) { }
        protected override QueueToggleStatus QueueType => QueueToggleStatus.Torrent;
    }
    public class MetalinkExtension : ToggleBindingExtensionBase
    {
        public MetalinkExtension(string path) : base(path) { }
        protected override QueueToggleStatus QueueType => Metalink;
    }
    #endregion
}