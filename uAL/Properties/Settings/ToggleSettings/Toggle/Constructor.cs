// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    public partial class Toggle
    {
        public Toggle(ToggleSettingsBase toggles, MonitorTypes type)
        {
            Instance = toggles;
            Type = type;
            this.Processing = new ProcessingToggle(toggles.Processing, type);
            this.Filters = new FilterToggle(toggles.Filters, type);
        }
    }
}
