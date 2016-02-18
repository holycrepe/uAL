// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.LibSettings
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using PostSharp.Patterns.Model;
    using Torrent.Properties.Settings;
    using ToggleSettings;
    using Torrent.Enums;
    using Torrent.Helpers.Utils;
    using System.Runtime.Serialization;
    using System.Diagnostics;
    using Serialization;
    public sealed partial class LibSettings
    {
        [DataContract(Namespace = Namespaces.Default)]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        [NotifyPropertyChanged]
        public class LibQueueSettings : BaseSubSettings
        {
            #region Primary
            [DataMember]
            public bool UpdateLabelsOnStart { get; set; } = true;
            [DataMember]
            public bool AutoStart { get; set; } = false;
            [DataMember]
            public bool CacheDownloadedFilesOnStart { get; set; } = true;
            [DataMember]
            public List<string> TorrentFileNamesWordsToStrip { get; set; }
            public LibQueueSettings() : this($"Initializing.{nameof(LibQueueSettings)}") { }
            public LibQueueSettings(string name) : base(name) { }
            #endregion

            #region Derived
            protected override object[] DebuggerDisplayProperties => new object[]
            {
                nameof(AutoStart), AutoStart,
                nameof(UpdateLabelsOnStart), UpdateLabelsOnStart,                
                nameof(CacheDownloadedFilesOnStart), CacheDownloadedFilesOnStart,
                nameof(TorrentFileNamesWordsToStrip), TorrentFileNamesWordsToStrip == null ? "Empty" : $"{TorrentFileNamesWordsToStrip.Count} Words"
            };
            public bool HaveQueuedAllMetalinks { get; set; } = false;            
            public bool HaveQueuedAllTorrents { get; set; } = false;
            
            public bool HaveQueuedAllJobs { get; set; } = false;
            [SafeForDependencyAnalysis]
            public bool MayQueueAllMetalinks => this.MayQueueAll(MonitorTypes.Metalink);

            [SafeForDependencyAnalysis]
            public bool MayQueueAllTorrents => this.MayQueueAll(MonitorTypes.Torrent);

            //public static bool MayQueueAllMetalinks => !HaveQueuedAllMetalinks || !Monitor.METALINKS;
            //public static bool MayQueueAllTorrents => !HaveQueuedAllTorrents || !Monitor.TORRENTS;
            public bool MayQueueAll(MonitorTypes type)
            {
                var TOGGLES = ToggleSettings.Toggles.GetActiveToggles(type);
                return !(type == MonitorTypes.Torrent ? this.HaveQueuedAllTorrents : this.HaveQueuedAllMetalinks)
                       || TOGGLES.Monitor;
            }

            public void SetHasQueuedAllFiles(MonitorTypes fileType, bool value, bool force = false)
            {
                switch (fileType) {
                    case MonitorTypes.Torrent:
                        if (force) {
                            this.HaveQueuedAllTorrents = value;
                        } else {
                            this.HaveQueuedAllTorrents |= value;
                        }

                        break;
                    case MonitorTypes.Metalink:
                        if (force) {
                            this.HaveQueuedAllMetalinks = value;
                        } else {
                            this.HaveQueuedAllMetalinks |= value;
                        }
                        break;
                }
            }

            #endregion            
        }
    }
}
