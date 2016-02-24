using System;
using Torrent.Infrastructure;

// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using PostSharp.Patterns.Model;
    using System.Xml.Serialization;
    using Torrent.Properties.Settings;
    using static MonitorTypes;

    //[Serializable]
    //[XmlSerializerAssembly("uAL.XmlSerializers")]
    //[XmlInclude(typeof (ToggleSettingsFilters)), XmlInclude(typeof (ToggleSettingsProcessing))]    
    public partial class ToggleSettingsBase : BaseSettings
    {
        public ToggleSettingsBase()
        {
            Processing = new ToggleSettingsProcessing(nameof(Processing));
            Filters = new ToggleSettingsFilters(nameof(Filters));
            AutoExpandGroups = All;
            Monitor = Disabled;
            InitializeMonitor = All;
            Watcher = Disabled;
            QueueFilesOnStartup = All;
        }
        public static ToggleSettingsBase Load()
            => Load<ToggleSettingsBase>();
    }    
}
