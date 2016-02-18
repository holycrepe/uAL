using System;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;

// ReSharper disable InconsistentNaming

namespace Torrent.Properties.Settings.MySettings
{
    using Enums;
    using PostSharp.Patterns.Model;
    using Properties.Settings;
    using Serialization;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    public partial class MySettingsBase
    {

        [Serializable]
        [XmlSerializerAssembly("Torrent.Serializers")]
        [XmlInclude(typeof(GetFileMethod)),
            XmlInclude(typeof(ProcessQueueMethod))]
        [XmlRoot("Methods")]
        [DataContract(Namespace = NamespaceAttribute.Default)]
        [KnownType(typeof(GetFileMethod)),
            KnownType(typeof(ProcessQueueMethod))]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        [NotifyPropertyChanged]
        public class MyMethodsSettings : BaseSubSettings
        {
            [IgnoreAutoChangeNotification]
            protected override object[] DebuggerDisplayProperties
                => new object[] {nameof(GET_FILES), GET_FILES, nameof(PROCESS_QUEUE), PROCESS_QUEUE};
            GetFileMethod _getFiles = GetFileMethod.Plain;
            ProcessQueueMethod  _processQueue = ProcessQueueMethod.Plain;

            [DataMember]
            [XmlElement(ElementName="GetFiles")]
            public GetFileMethod GET_FILES
            {
                get { return this._getFiles; }
                set
                {
                    if (value != this._getFiles) {
                        this._getFiles = value;
                    }
                }
            }
            [DataMember]
            [XmlElement(ElementName = "ProcessQueue")]
            public ProcessQueueMethod PROCESS_QUEUE
            {
                get { return this._processQueue; }
                set
                {
                    if (value != this._processQueue)
                    {
                        this._processQueue = value;
                    }
                }
            }

            public MyMethodsSettings() : this($"Initializing.{nameof(MyMethodsSettings)}") { }

            public MyMethodsSettings(string name) : base(name)
            {
                GET_FILES = GetFileMethod.Plain;
                PROCESS_QUEUE = ProcessQueueMethod.Plain;
            }
            #region Operators
            public override int GetHashCode()
                => GET_FILES.GetHashCode() ^ PROCESS_QUEUE.GetHashCode();
            public override bool Equals(object o)
            {
                if (o == null)
                {
                    return false;
                }
                var member = o as MyMethodsSettings;
                if (member != null)
                {
                    return this.Equals(member);
                }
                return false;
            }
            public bool Equals(MyMethodsSettings other)
                => other?.GET_FILES == this.GET_FILES && other?.PROCESS_QUEUE == this.PROCESS_QUEUE;

            public static bool operator ==(MyMethodsSettings a, MyMethodsSettings b)
            {
                // If both are null, or both are same instance, return true.
                if (System.Object.ReferenceEquals(a, b))
                {
                    return true;
                }

                // If one is null, but not both, return false.
                if (((object)a == null) || ((object)b == null))
                {
                    return false;
                }

                // Return true if the fields match:
                return a.Equals(b);
            }
            public static bool operator !=(MyMethodsSettings a, MyMethodsSettings b) 
                => !(a == b);
            #endregion
        }
    }
}
