using System;
using Torrent.Helpers.Utils;

namespace Torrent.Properties.Settings
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using JetBrains.Annotations;

    [Serializable]
    public class BaseSettings : BaseSubSettings
    { 
        [XmlIgnore]
        public sealed override string Name { get; protected set; } 
        //[XmlIgnore]
        //public string InstanceName {
        //    get { return _instanceName; }
        //    protected set { _instanceName = value;
        //        SetPath();
        //    }
        //}
        private Type Type { get; }
        private static string GetNameFromType(Type t)
            => t.Name.Replace("Base", "");
        private static string GetPath(Type t)
            => SettingsManager.MakeRelativePath(t?.Namespace?.Split('.')[0] + "." + GetNameFromType(t) + ".xml");
        protected BaseSettings()  : base(null)
        {
            Type = GetType();
            Name = GetNameFromType(Type);
        }

        protected static T Load<T>(string name = null) where T : BaseSettings, new()
            => SerializationUtils.ReadFromXmlFile<T>(GetPath(typeof(T)));

        public void Save()
            => SerializationUtils.WriteToXmlFile(GetPath(Type), this);
    }


}