using System;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;

// ReSharper disable InconsistentNaming

namespace Torrent.Properties.Settings.MySettings
{
    using Properties.Settings;
    using PostSharp.Patterns.Model;
    using System.Runtime.Serialization;
    using System.Diagnostics;
    using Serialization;
    using System.Xml.Serialization;
    [Serializable]
    [XmlSerializerAssembly("Torrent.Serializers")]
    [XmlInclude(typeof(MyMethodsSettings))]
    [XmlRoot("My")]
    [DataContract(Name = "My", Namespace = NamespaceAttribute.Default)]
    [KnownType(typeof(MyMethodsSettings))]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [NotifyPropertyChanged]
    public partial class MySettingsBase : BaseSettings
    {
        #region Implementation
        MyMethodsSettings _methods = null;
        [DataMember]
        [XmlElement(ElementName="Methods")]
        public MyMethodsSettings METHODS {
            get { return this._methods; }
            set
            {
                if (this._methods != value)
                {
                    this._methods = value;
                }
            }
        }

        [IgnoreAutoChangeNotification]
        protected override object[] DebuggerDisplayProperties => new object[] {nameof(METHODS), METHODS};

        #endregion

        #region Overrides

        #region Overrides: Initialization

        public MySettingsBase() { METHODS = new MyMethodsSettings(nameof(METHODS)); }

        #endregion

        #region Overrides: Save/Load

        public static MySettingsBase Load()
            => Load<MySettingsBase>();
        public void Load(MySettingsBase other)
        {
            if (other != null)
            {
                this.METHODS = other.METHODS;
            }
        }
        #endregion

        #endregion
    }
}
