
using System;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;

// ReSharper disable InconsistentNaming

namespace wUAL.Properties.Settings.AllSettings
{
    using System.Xml.Serialization;
    using Torrent.Properties.Settings;
    using PostSharp.Patterns.Model;
    using AppSettings;
    using uAL.Properties.Settings.LibSettings;
    using uAL.Properties.Settings.ToggleSettings;
    using Torrent.Properties.Settings.MySettings;
    using System.Runtime.Serialization;
    using System.Diagnostics;
    using Serialization;
    using Torrent.Serialization;

    [Serializable]
    [XmlSerializerAssembly("wUAL.Serializers")]
    [XmlInclude(typeof(AppSettingsBase)),
        XmlInclude(typeof(LibSettings)),
        XmlInclude(typeof(ToggleSettingsBase)),
        XmlInclude(typeof(MySettingsBase))]
    [XmlRoot(nameof(AllSettings))]
    [DataContract(Name = nameof(AllSettings), Namespace = Namespaces.Default)]
    [Namespace(Prefix = "", Uri = Namespaces.Default)]
    [Namespace(Prefix = "uAL", Uri = uAL.Serialization.Namespaces.Default)]
    [Namespace(Prefix = "Torrent", Uri = NamespaceAttribute.Default)]
    [KnownType(typeof(AppSettingsBase)),
        KnownType(typeof(LibSettings)),
        KnownType(typeof(ToggleSettingsBase)),
        KnownType(typeof(MySettingsBase)),
        //KnownType(typeof(LibSettings.LibDirectorySettings)),
        //KnownType(typeof(LibSettings.LibLabelSettings)),
        //KnownType(typeof(LibSettings.LibConnectionSettings)),
        //KnownType(typeof(LibSettings.LibQueueSettings)),
        //KnownType(typeof(LibSettings.LibTorrentSettings)),
        //KnownType(typeof(LibSettings.LibUTorrentJobSettings)),
        //KnownType(typeof(WINDOWPLACEMENT)),
        //KnownType(typeof(RECT)),
        //KnownType(typeof(POINT)),
        ]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [NotifyPropertyChanged]
    public partial class AllSettingsBase : BaseSettings
    {
#if SETTINGS_USE_ALL_SETTINGS
        [DataMember(Order = 4)]
        //[XmlElement(Order=4)]
        [XmlIgnore]
        [SafeForDependencyAnalysis]
        public AppSettingsBase App
        {
            get { return AppSettings.AppSetting; }
            set { AppSettings.AppSetting = value; }
        }
        [DataMember(Order = 3)]
        //[XmlElement(Order=3)]
        [XmlIgnore]
        [SafeForDependencyAnalysis]
        public LibSettings Lib
        {
            get { return LibSettings.LibSetting; }
            set { LibSettings.LibSetting = value; }
        }
        [DataMember(Order = 2)]
        //[XmlElement(Order=2)]
        [XmlIgnore]
        [SafeForDependencyAnalysis]
        public ToggleSettingsBase Toggle
        {
            get { return ToggleSettings.Toggles; }
            set { ToggleSettings.Toggles = value; }
        }
        [DataMember(Order = 1)]
        [XmlElement(Order = 1)]
        [SafeForDependencyAnalysis]
        public MySettingsBase My
        {
            get { return MySettings.MySetting; }
            set { MySettings.MySetting = value; }
        }
#else
        [DataMember(Order = 4)]
        //[XmlElement(Order=4)]
        [XmlIgnore]
        public AppSettingsBase App { get; set; }
        [DataMember(Order = 2)]
        //[XmlElement(Order=2)]
        [XmlIgnore]
        public LibSettings Lib { get; set; }
        [DataMember(Order = 3)]
        //[XmlElement(Order=3)]
        [XmlIgnore]
        public ToggleSettingsBase Toggle { get; set; }
        [DataMember(Order=1)]
        [XmlElement(Order=1)]        
        public MySettingsBase My { get; set; }
#endif

        [IgnoreAutoChangeNotification]
        protected override object[] DebuggerDisplayProperties => new object[] {
            nameof(App), App,
            nameof(Lib), Lib,
            nameof(Toggle), Toggle,
            nameof(My), My
        };

        public AllSettingsBase() {
            //App = new AppSettingsBase();
            //Lib = new LibSettings();
            //Toggle = new ToggleSettingsBase();
            //My = new MySettingsBase();
        }
        public static AllSettingsBase New()
            => new AllSettingsBase(AppSettings.AppSetting, LibSettings.LibSetting, ToggleSettings.Toggles, MySettings.MySetting);
        public static void SaveAllSettings()
            => New().Save();
        public AllSettingsBase(AppSettingsBase app, LibSettings lib, ToggleSettingsBase toggle, MySettingsBase my)
        {
            App = app;
            Lib = lib;
            Toggle = toggle;
            My = my;
        }
        public static AllSettingsBase Load()
            => Load<AllSettingsBase>();
    }
}
