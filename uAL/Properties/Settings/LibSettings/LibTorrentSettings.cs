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
        public class LibTorrentSettings : BaseSubSettings
        {
            [DataMember]
            public bool CheckAlreadyQueuedInUTorrent { get; set; } = true;
            public LibTorrentSettings() : this($"Initializing.{nameof(LibTorrentSettings)}") { }
            public LibTorrentSettings(string name) : base(name) { }

            protected override object[] DebuggerDisplayProperties => new object[]
            {
                nameof(CheckAlreadyQueuedInUTorrent), CheckAlreadyQueuedInUTorrent
            };
        }
    }
}