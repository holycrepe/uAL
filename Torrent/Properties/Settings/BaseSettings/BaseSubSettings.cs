namespace Torrent.Properties.Settings
{
    using System;
    using System.Xml.Serialization;
    using Extensions;
    using Infrastructure;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    [Serializable]
    [XmlSerializerAssembly("Torrent.XmlSerializers")]
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class BaseSubSettings : NotifyPropertyChangedBase, IDebuggerDisplay
    {
        public override string ToString()
            => DebuggerDisplay();

        protected virtual object[] DebuggerDisplayProperties => new object[] {};

        [XmlIgnore]
        public virtual string Name { get; protected set; }

        public BaseSubSettings() : this(nameof(BaseSubSettings)) { }

        public BaseSubSettings(string name)
        {
            if (name != null) {
                Name = name;
            }
            // ReSharper disable once VirtualMemberCallInContructor
            Initialize();
        }

        protected virtual void Initialize() { }

        public virtual string DebuggerDisplay(int level = 1)
            => DebuggerDisplaySimple(level);

        public virtual string DebuggerDisplaySimple(int level = 1)
            => $"[{Name}{(DebuggerDisplayProperties.Length == 0 ? "" : " ")}{DebuggerDisplayProperties.ToStringKeyValue()}]";
    }
}
