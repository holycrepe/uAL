namespace Torrent.Properties.Settings
{
    using System;
    using System.Xml.Serialization;
    using Extensions;
    using Infrastructure;

    [Serializable]
    public class BaseSubSettings : NotifyPropertyChangedBase
    {
        public override string ToString()
            => $"[{Name} {AllSettings.ToStringKeyValue()}]";
        protected virtual object[] AllSettings => new object[] {};
        [XmlIgnore]
        public virtual string Name { get; protected set; }        				
        public BaseSubSettings() : this("BaseSubSettings") { }
        public BaseSubSettings(string name) {
            if (name != null) {
                Name = name;
            }
            // ReSharper disable once VirtualMemberCallInContructor
            Initialize();
        }		
        protected virtual void Initialize() { }						        
    }
}