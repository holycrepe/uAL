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
        public class LibUTorrentJobSettings : BaseSubSettings
        {
            [DataMember]
            public bool OverwriteUTorrentResumeFile { get; set; } = false;
            [DataMember]
            public bool MoveDownloadedFiles { get; set; } = true;
            [DataMember]
            public bool FixLabels { get; set; } = true;
            [DataMember]
            public bool FixOriginalLabel { get; set; } = true;
            [DataMember]
            public bool FixSecondaryLabel { get; set; } = true;
            [DataMember]
            public bool RemoveExtraPrimaryLabel { get; set; } = true;
            public LibUTorrentJobSettings() : this($"Initializing.{nameof(LibUTorrentJobSettings)}") { }
            public LibUTorrentJobSettings(string name) : base(name) { }
            protected override object[] DebuggerDisplayProperties => new object[]
            {
                nameof(OverwriteUTorrentResumeFile), OverwriteUTorrentResumeFile,
                nameof(MoveDownloadedFiles), MoveDownloadedFiles,
                nameof(FixLabels), FixLabels,
                nameof(FixOriginalLabel), FixOriginalLabel,
                nameof(FixSecondaryLabel), FixSecondaryLabel,
                nameof(RemoveExtraPrimaryLabel), RemoveExtraPrimaryLabel,
            };
        }
    }
}