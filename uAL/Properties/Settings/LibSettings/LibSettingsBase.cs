// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.LibSettings
{
    using System;
    using System.Linq;
    using System.Xml.Serialization;
    using PostSharp.Patterns.Model;
    using Torrent.Properties.Settings;
    using System.Runtime.Serialization;
    using Serialization;
    using Torrent.Serialization;

    [DataContract(Name = "Lib", Namespace = Namespaces.Default)]
    [KnownType(typeof(LibDirectorySettings)),
     KnownType(typeof(LibLabelSettings)),
     KnownType(typeof(LibConnectionSettings)),
     KnownType(typeof(LibQueueSettings)),
     KnownType(typeof(LibTorrentSettings)),
     KnownType(typeof(LibUTorrentJobSettings))]
    [NotifyPropertyChanged]
    public sealed partial class LibSettings : BaseSettings
    {
        #region Implementation
        [DataMember]
        public LibDirectorySettings Directories { get; set; }
        [DataMember]
        public LibLabelSettings Labels { get; set; }
        [DataMember]
        public LibConnectionSettings Connection { get; set; }
        [DataMember]
        public LibQueueSettings Queue { get; set; }
        [DataMember]
        public LibTorrentSettings Torrents { get; set; }
        [DataMember]
        public LibUTorrentJobSettings Jobs { get; set; }

        protected override object[] DebuggerDisplayProperties
            =>
                new object[]
                {
                    nameof(Directories),
                    Directories,
                    nameof(Labels),
                    Labels,
                    nameof(Connection),
                    Connection,
                    nameof(Queue),
                    Queue,
                    nameof(Torrents),
                    Torrents,
                    nameof(Jobs),
                    Jobs
                };

        #endregion

        #region Constructor

        public LibSettings()
        {
            Directories = new LibDirectorySettings(nameof(Directories));
            Labels = new LibLabelSettings(nameof(Labels));
            Connection = new LibConnectionSettings(nameof(Connection));
            Queue = new LibQueueSettings(nameof(Queue));
            Torrents = new LibTorrentSettings(nameof(Torrents));
            Jobs = new LibUTorrentJobSettings(nameof(Jobs));
        }

        #endregion

        #region Save/Load

        public static LibSettings Load()
            => Load<LibSettings>();
        public void Load(LibSettings other)
        {
            if (other != null)
            {
                this.Directories = other.Directories;
                this.Labels = other.Labels;
                this.Connection = other.Connection;
                this.Queue = other.Queue;
                this.Torrents = other.Torrents;
                this.Jobs = other.Jobs;
            }
        }
        #endregion
    }
}
