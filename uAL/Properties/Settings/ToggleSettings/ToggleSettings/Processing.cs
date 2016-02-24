using System;
using Torrent.Infrastructure;

// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using System.Xml.Serialization;
    using PostSharp.Patterns.Model;
    using System.Runtime.Serialization;
    using System.Diagnostics;
    using Serialization;
    using static MonitorTypes;
    public partial class ToggleSettingsBase
    {
        [DataContract(Namespace = Namespaces.Default)]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        [KnownType(typeof(ToggleSettingsProcessingAutomated)),
            KnownType(typeof(MonitorTypes))]
        [NotifyPropertyChanged]
        public class ToggleSettingsProcessing : ToggleSettingsBaseSubclass
        {
            [IgnoreAutoChangeNotification]
            protected override object[] DebuggerDisplayProperties => new object[]
            {
                nameof(Automated), Automated,
                nameof(Enabled), Enabled,
                nameof(PreviewMode), PreviewMode,
                nameof(CheckDupes), CheckDupes,
                nameof(PreProcess), PreProcess,
                nameof(MoveProcessedFiles), MoveProcessedFiles,
                nameof(StopCompletedTorrents), StopCompletedTorrents
            };

            [DataMember]
            public ToggleSettingsProcessingAutomated Automated { get; set; }
            [DataMember]
            public MonitorTypes Enabled { get; set; }
            [DataMember]
            public MonitorTypes PreviewMode { get; set; }
            [DataMember]
            public MonitorTypes CheckDupes { get; set; }
            [DataMember]
            public MonitorTypes PreProcess { get; set; }
            [DataMember]
            public MonitorTypes MoveProcessedFiles { get; set; }
            [DataMember]
            public MonitorTypes StopCompletedTorrents { get; set; }

            public ToggleSettingsProcessing() : this($"Initializing.{nameof(ToggleSettingsProcessing)}") { }

            public ToggleSettingsProcessing(string name) : base(name)
            {
                Automated = new ToggleSettingsProcessingAutomated();
                Enabled = All;
                PreviewMode = All;
                CheckDupes = All;
                PreProcess = Metalink;
                MoveProcessedFiles = All;
                StopCompletedTorrents = All;
            }
            public void Load(ToggleSettingsProcessing other)
            {
                if (other != null)
                {
                    Automated.Load(other.Automated);
                    Enabled = other.Enabled;
                    PreviewMode = other.PreviewMode;
                    CheckDupes = other.CheckDupes;
                    PreProcess = other.PreProcess;
                    MoveProcessedFiles = other.MoveProcessedFiles;
                    StopCompletedTorrents = other.StopCompletedTorrents;
                }
            }
        }
    }    
}
