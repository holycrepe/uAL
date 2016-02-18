using System;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;

// ReSharper disable InconsistentNaming

namespace wUAL.Properties.Settings.AppSettings
{
    using System.Xml.Serialization;
    using Torrent.Properties.Settings;
    using PostSharp.Patterns.Model;
    using System.Collections.Generic;
    using System.Runtime.Serialization;    
    using System.Diagnostics;
    using Serialization;
    [Serializable]
    [XmlSerializerAssembly("wUAL.Serializers")]
    [XmlInclude(typeof(WindowPlacementEntry))]
    [XmlRoot("App")]
    [DataContract(Name = "App", Namespace = Namespaces.Default)]
    [KnownType(typeof(WINDOWPLACEMENT))]
    //[KnownType(typeof(RECT))]
    //[KnownType(typeof(POINT))]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [NotifyPropertyChanged]
    public partial class AppSettingsBase : BaseSettings
    {
        [DataMember(EmitDefaultValue = false, Order=2)]
        [XmlIgnore]        
        public Dictionary<string, WINDOWPLACEMENT?> Placements { get; set; } 
            = new Dictionary<string, WINDOWPLACEMENT?>();
        [XmlElement(ElementName="Placements", Order=2)]
        public List<WindowPlacementEntry> PlacementsList { get; set; }
            = new List<WindowPlacementEntry>();
        [DataMember(EmitDefaultValue = false, Order=1)]
        [XmlElement(Order = 1)]
        public int StatusRowHeight { get; set; } = 150;

        [IgnoreAutoChangeNotification]
        protected override object[] DebuggerDisplayProperties => new object[] {
            nameof(Placements), Placements,
            nameof(StatusRowHeight), StatusRowHeight
        };
        public AppSettingsBase() {  }
        public static AppSettingsBase Load()
            => Load<AppSettingsBase>();
        public void Load(AppSettingsBase other)
        {
            if (other != null)
            {
                this.Placements = other.Placements;
                this.PlacementsList = other.PlacementsList;
                this.StatusRowHeight = other.StatusRowHeight;
            }
        }
    }
}
