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
    using static MonitorTypes;
    public partial class ToggleSettingsBase
    {
        [DataContract(Namespace = Namespaces.Default)]
        [KnownType(typeof(MonitorTypes))]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        [NotifyPropertyChanged]
        public class ToggleSettingsFilters : ToggleSettingsBaseSubclass
        {
            [IgnoreAutoChangeNotification]
            protected override object[] DebuggerDisplayProperties => new object[] {
                nameof(Global), Global,
                nameof(Include), Include,
                nameof(Exclude), Exclude
            };
            [DataMember]
            public MonitorTypes Global { get; set; }
            [DataMember]
            public MonitorTypes Include { get; set; }
            [DataMember]
            public MonitorTypes Exclude { get; set; }
            public ToggleSettingsFilters() : this($"Initializing.{nameof(ToggleSettingsFilters)}") { }

            public ToggleSettingsFilters(string name) : base(name)
            {
                Global = All;
                Include = Disabled;
                Exclude = All;
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
                    Global = other.Global;
                    Include = other.Include;
                    Exclude = other.Exclude;
                }
            }
        }
    }
}
