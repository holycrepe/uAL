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
    public partial class ToggleSettingsBase
    {
        [DataContract(Namespace = Namespaces.Default)]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        [KnownType(typeof(ToggleSettingsProcessingAutomated)),
            KnownType(typeof(QueueToggles))]
        [NotifyPropertyChanged]
        public class ToggleSettingsProcessing : ToggleSettingsBaseSubclass
        {
            [IgnoreAutoChangeNotification]
            protected override object[] DebuggerDisplayProperties => new object[] { Automated, Enabled, PreviewMode, CheckDupes, PreProcess, MoveProcessedFiles, StopCompletedTorrents };

            [DataMember]
            public ToggleSettingsProcessingAutomated Automated { get; set; }
            [DataMember]
            public QueueToggles Enabled { get; set; }
            [DataMember]
            public QueueToggles PreviewMode { get; set; }
            [DataMember]
            public QueueToggles CheckDupes { get; set; }
            [DataMember]
            public QueueToggles PreProcess { get; set; }
            [DataMember]
            public QueueToggles MoveProcessedFiles { get; set; }
            [DataMember]
            public QueueToggles StopCompletedTorrents { get; set; }

            public ToggleSettingsProcessing() : this($"Initializing.{nameof(ToggleSettingsProcessing)}") { }

            public ToggleSettingsProcessing(string name) : base(name)
            {
                Automated = new ToggleSettingsProcessingAutomated(nameof(Automated));
                Enabled = new QueueToggles(nameof(Enabled), QueueTypes.All);
                PreviewMode = new QueueToggles(nameof(PreviewMode), QueueTypes.All);
                CheckDupes = new QueueToggles(nameof(CheckDupes), QueueTypes.All);
                PreProcess = new QueueToggles(nameof(PreProcess), QueueTypes.Metalink);
                MoveProcessedFiles = new QueueToggles(nameof(MoveProcessedFiles), QueueTypes.All);
                StopCompletedTorrents = new QueueToggles(nameof(StopCompletedTorrents), QueueTypes.All);
            }
            public void Load(ToggleSettingsProcessing other)
            {
                if (other != null)
                {
                    Automated.Load(other.Automated);
                    Enabled.Load(other.Enabled);
                    PreviewMode.Load(other.PreviewMode);
                    CheckDupes.Load(other.CheckDupes);
                    PreProcess.Load(other.PreProcess);
                    MoveProcessedFiles.Load(other.MoveProcessedFiles);
                    StopCompletedTorrents.Load(other.StopCompletedTorrents);
                }
            }
        }
    }    
}
