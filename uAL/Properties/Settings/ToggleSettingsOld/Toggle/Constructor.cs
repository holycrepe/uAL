// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    public partial class Toggle
    {
        public Toggle(QueueTypes type)
        {
            Type = type;
            PROCESSING = new ProcessingToggle(type);
            FILTERS = new FilterToggle(type);
        }
    }
}
