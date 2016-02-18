using System;

// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using System.Xml.Serialization;
    using static LibSettings.LibSettings;
    using PostSharp.Patterns.Model;
    using System.Runtime.Serialization;
    using System.Diagnostics;
    using Serialization;
    public partial class ToggleSettingsBase
    {
        [DataContract(Namespace = Namespaces.Default)]
        [KnownType(typeof(QueueToggles))]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        [NotifyPropertyChanged]
        public class ToggleSettingsFilters : ToggleSettingsBaseSubclass
        {
            [IgnoreAutoChangeNotification]
            protected override object[] DebuggerDisplayProperties => new object[] { Global, Include, Exclude };
            [DataMember]
            public QueueToggles Global { get; set; }
            [DataMember]
            public QueueToggles Include { get; set; }
            [DataMember]
            public QueueToggles Exclude { get; set; }
            public ToggleSettingsFilters() : this($"Initializing.{nameof(ToggleSettingsFilters)}") { }

            public ToggleSettingsFilters(string name) : base(name)
            {
                Global = new QueueToggles(nameof(Global), QueueTypes.All);
                Include = new QueueToggles(nameof(Include));
                Exclude = new QueueToggles(nameof(Exclude), QueueTypes.All);
                PropertyChanged += (s, e)
                    =>
                {
                    LibSetting?.Labels?.FilterResults?.Clear();
                };
            }

            public void Load(ToggleSettingsFilters other)
            {
                if (other != null)
                {
                    Global.Load(other.Global);
                    Include.Load(other.Include);
                    Exclude.Load(other.Exclude);
                }
            }
        }
    }
}
