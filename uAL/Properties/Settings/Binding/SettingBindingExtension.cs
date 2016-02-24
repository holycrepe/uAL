namespace uAL.Properties.Settings.Binding
{
    using Torrent.Properties.Settings.Binding;
    using System.Windows.Data;
    using LibSettings;
    using Settings;
    using ToggleSettings;
    using static LibSettings.LibSettings;
    using static ToggleSettings.ToggleSettings;
    using static ToggleSettings.MonitorTypes;

    #region General Settings

    public class LibExtension : SettingBindingExtensionBase<LibSettings>
    {
        public LibExtension() : base() { }
        public LibExtension(string path, string format = null) : base(path, format) { }
        protected override LibSettings Value => LibSetting;
        public override string Format { get; set; }
    }

    #endregion

    #region Toggles

    public class ToggleExtension : SettingBindingExtensionBase<ToggleSettingsBase>
    {
        public ToggleExtension() : base() { }
        public ToggleExtension(string path) : base(path) { }
        protected override ToggleSettingsBase Value 
            => Toggles;
    }

    public abstract class ToggleBindingExtensionBase : SettingBindingExtensionBase<Toggle>
    {
        protected ToggleBindingExtensionBase() : base() { }
        protected ToggleBindingExtensionBase(string path) : base(path) { }
        protected abstract MonitorTypes QueueType { get; }
        protected override Toggle Value 
            => Toggles.GetActiveToggles(QueueType);
        protected override BindingMode DefaultMode => BindingMode.OneWay;
    }

    public class TorrentExtension : ToggleBindingExtensionBase
    {
        public TorrentExtension() : base() { }
        public TorrentExtension(string path) : base(path) { }
        protected override MonitorTypes QueueType 
            => MonitorTypes.Torrent;
    }

    public class MetalinkExtension : ToggleBindingExtensionBase
    {
        public MetalinkExtension() : base() { }
        public MetalinkExtension(string path) : base(path) { }
        protected override MonitorTypes QueueType => Metalink;
    }

    public class JobExtension : ToggleBindingExtensionBase
    {
        public JobExtension() : base() { }
        public JobExtension(string path) : base(path) { }
        protected override MonitorTypes QueueType => Job;
    }

    #endregion
}
