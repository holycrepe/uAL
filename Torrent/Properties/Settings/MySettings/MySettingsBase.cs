using System;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;
// ReSharper disable InconsistentNaming
namespace Torrent.Properties.Settings.MySettings
{
    using System.Xml.Serialization;
    using Properties.Settings;

    [Serializable]
    [XmlInclude(typeof (MyMethodsSettings))]
    public partial class MySettingsBase : BaseSettings
    {			
        #region Implementation			
        public MyMethodsSettings METHODS { get; set; }
        protected override object[] AllSettings => new object[] { nameof(METHODS), METHODS };
        #endregion

        #region Overrides
        #region Overrides: Initialization        
        public MySettingsBase() 
        {
            METHODS = new MyMethodsSettings(nameof(METHODS));
            METHODS.PropertyChanged += (s, e) => OnPropertyChanged(nameof(METHODS));
        }
        #endregion
        #region Overrides: Save/Load

        public static MySettingsBase Load()
            => Load<MySettingsBase>();

        #endregion

        #endregion
    }
}

