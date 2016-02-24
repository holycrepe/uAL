using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uAL.Properties.Settings.ToggleSettings
{
    using System.Diagnostics;
    using System.Dynamic;
    using System.Runtime.Serialization;
    using AddGenericConstraint;
    using PostSharp.Patterns.Model;
    using Serialization;
    using Torrent.Extensions;
    using Torrent.Infrastructure.Enums.Toggles;
    using static MonitorTypes;

    [DataContract(Namespace = Namespaces.Default)]
    [KnownType(typeof(MonitorTypes))]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [PostSharp.Patterns.Model.NotifyPropertyChanged]
    public class MonitorTogglesProcessing : MonitorTogglesProcessing<MonitorTypes>, IEnumToggles<MonitorTypes>
    {
        public MonitorTogglesProcessing()
        {
            Enabled = All;
            PreviewMode = All;
            CheckDupes = All;
            PreProcess = Metalink;
            MoveProcessedFiles = All;
            StopCompletedTorrents = All;
        }
    }

    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class MonitorToggleProcessing : MonitorTogglesProcessing<bool>
    {

    }

    [DataContract(Namespace = Namespaces.Default)]
    [KnownType(typeof(MonitorTogglesProcessingAutomated<MonitorTypes>)),
        KnownType(typeof(MonitorTypes))]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [NotifyPropertyChanged]
    public class MonitorTogglesProcessing<T> : MonitorToggles<T>
        where T : struct
    {
        [IgnoreAutoChangeNotification]
        protected override object[] DebuggerDisplayProperties => new object[] {
            nameof(Automated), Automated
        };

        [DataMember]
        public MonitorTogglesProcessingAutomated<T> Automated { get; set; }
            = new MonitorTogglesProcessingAutomated<T>();

        [DataMember]
        [SafeForDependencyAnalysis]
        public T Enabled { get { return Get(); } set { Set(value); } }
        [DataMember]
        [SafeForDependencyAnalysis]
        public T PreviewMode { get { return Get(); } set { Set(value); } }
        [DataMember]
        [SafeForDependencyAnalysis]
        public T CheckDupes { get { return Get(); } set { Set(value); } }
        [DataMember]
        [SafeForDependencyAnalysis]
        public T PreProcess { get { return Get(); } set { Set(value); } }
        [DataMember]
        [SafeForDependencyAnalysis]
        public T MoveProcessedFiles { get { return Get(); } set { Set(value); } }
        [DataMember]
        [SafeForDependencyAnalysis]
        public T StopCompletedTorrents { get { return Get(); } set { Set(value); } }
        public void Load(MonitorTogglesProcessing<T> other)
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
