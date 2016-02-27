using System;
using System.Xml.Serialization;

namespace wUAL.Properties.Settings {
    
    [Serializable]
    [XmlSerializerAssembly("wUAL.XmlSerializers")]
    public sealed partial class AppSettings {
        
        public AppSettings() {

        }

        public static AppSettings AppSetting => defaultInstance;
    }
}
