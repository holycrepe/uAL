using System;
using Torrent.Helpers.Utils;

namespace Torrent.Properties.Settings
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using JetBrains.Annotations;
    using System.Runtime.Serialization;
    using System.Diagnostics;
    using Extensions;
    using Serialization;

    [Serializable]
    [XmlSerializerAssembly("Torrent.XmlSerializers")]
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class BaseSettings : BaseSubSettings
    {
        string _name = null;
        Type _type = null;
        public static BaseSettingsConstructor CONSTRUCTOR_ACTION = BaseSettingsConstructor.Load;

        public sealed override string Name {
            get { return this._name ?? (this._name = BaseSettings.GetNameFromType(this.Type)); }
            protected set { this._name = value; }
        }
        private Type Type {
            get { return this._type ?? (this._type = this.GetType()); }
            set { this._type = value; }
        }

        private static string GetNameFromType(Type t)
            => t.Name.Replace("Base", "");
        public static string GetPath<T>()
            => GetPath(typeof(T));
        public static string GetPath(Type t)
        {
            var fileNamePrefix = t?.Namespace?.Split('.')?[0] + ".";
            if (fileNamePrefix == MainApp.FileName + ".")
            {
                fileNamePrefix = "";
            }
            var name = GetNameFromType(t);
            var dataContract = t.GetAttribute<DataContractAttribute>();
            if (!string.IsNullOrEmpty(dataContract?.Name))
            {
                name = dataContract.Name.Replace("Settings","");
                fileNamePrefix = "Settings.";
            }
            return MainApp.MakeRelativePath(fileNamePrefix + name + ".config");
        }

        protected BaseSettings() : base(null) { }

        #region Load/Save
        protected static T Load<T>(string name = null) where T : BaseSettings, new()
            => LoadDataContract<T>(name);
        public virtual void Save()
            => SaveDataContract();
        protected static T LoadXml<T>(string name = null) where T : BaseSettings, new()
            => XmlUtils.ReadFromFile<T>(GetPath<T>());
        public virtual void SaveXml()
            => XmlUtils.WriteToFile(GetPath(Type), this);
        protected static T LoadDataContract<T>(string name = null) where T : BaseSettings, new()
            => DataContractUtils.ReadFromFile<T>(GetPath<T>());
        public virtual void SaveDataContract()
            => DataContractUtils.WriteToFile(GetPath(Type), this);
        #endregion
    }
}
