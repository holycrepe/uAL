using System;
using Torrent.Infrastructure;

// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using PostSharp.Patterns.Model;
    using System.Xml.Serialization;
    using Torrent.Properties.Settings;    

    //[Serializable]
    //[XmlSerializerAssembly("uAL.XmlSerializers")]
    //[XmlInclude(typeof (ToggleSettingsFilters)), XmlInclude(typeof (ToggleSettingsProcessing))]    
    public partial class ToggleSettingsBase : BaseSettings
    {
        public ToggleSettingsBase()
        {
            Processing = new ToggleSettingsProcessing(nameof(Processing));
            Filters = new ToggleSettingsFilters(nameof(Filters));
            AutoExpandGroups = new QueueToggles(nameof(AutoExpandGroups), 1);
            Monitor = new QueueToggles(nameof(Monitor));
            InitializeMonitor = new QueueToggles(nameof(InitializeMonitor), QueueTypes.All);
            Watcher = new QueueToggles(nameof(Watcher));
            QueueFilesOnStartup = new QueueToggles(nameof(QueueFilesOnStartup), 1);
        }
        public static ToggleSettingsBase Load()
            => Load<ToggleSettingsBase>();
    }    
}
