﻿namespace UTorrentRestAPI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Torrent;
    using RestClient;

    /// <summary>
    /// Represents a torrent job in uTorrent
    /// </summary>
    [DataContract]
    public class TorrentJob : Torrent
    {
        private static readonly DateTime startOfEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        /// <summary>
        /// Private storage for the <c>Label</c> field
        /// </summary>
        private string label;
        
        /// <summary>
        /// A reference to the service proxy used to communicate with the
        /// uTorrent web API
        /// </summary>
        private UTorrentRestClient client;
        
        /// <summary>
        /// Initializes a new instance of the TorrentJob class
        /// </summary>
        /// <param name="json">a json array returned from the web api representing a torrent</param>
        /// <param name="client">the procol client that this torrent should use for updates, etc</param>
        internal TorrentJob(UTorrentRestClient client)
        {
            this.client = client;
        }

        public bool IsConnected
            => client?.IsConnected ?? false;

        /// <summary>
        /// Gets the current status of the torrent, which is a bitwise or of statuses
        /// </summary>
        [DataMember]
        public TorrentStatus Status { get; private set; }

        /// <summary>
        /// Gets or sets the torrent's label
        /// </summary>
        [DataMember]
        public override string Label
        {
            get
            {
                return label;
            }

            set
            {
                client?.SetTorrentProperty(Hash, "label", value);
                label = value;
            }
        }

        /// <summary>
        /// Gets the size in bytes of the torrent
        /// </summary>
        [DataMember]
        public long SizeInBytes { get; private set; }

        /// <summary>
        /// Gets the progress of a torrent download in 1/1000ths
        /// </summary>
        [DataMember]
        public int ProgressInMils { get; private set; }

        /// <summary>
        /// Gets the number of bytes currently downloaded for this torrent
        /// </summary>
        [DataMember]
        public long DownloadedBytes { get; private set; }

        /// <summary>
        /// Gets the number of bytes currently uploaded for this torrent
        /// </summary>
        [DataMember]
        public long UploadedBytes { get; private set; }

        /// <summary>
        /// Gets the upload ratio in 1/1000ths
        /// </summary>
        [DataMember]
        public int RatioInMils { get; private set; }

        /// <summary>
        /// Gets the current upload rate of the torrent
        /// </summary>
        [DataMember]
        public int UploadBytesPerSec { get; private set; }

        /// <summary>
        /// Gets the current download rate of the torrent
        /// </summary>
        [DataMember]
        public int DownloadBytesPerSec { get; private set; }

        /// <summary>
        /// Gets an estimate for how long it will take to finish downloading the torrent
        /// </summary>
        [DataMember]
        public int EtaInSecs { get; private set; }

        /// <summary>
        /// Gets the number of peers connected
        /// </summary>
        [DataMember]
        public int PeersConnected { get; private set; }

        /// <summary>
        /// Gets the number of peers in the swarm
        /// </summary>
        [DataMember]
        public int PeersInSwarm { get; private set; }

        /// <summary>
        /// Gets the numbers of seeds connected
        /// </summary>
        [DataMember]
        public int SeedsConnected { get; private set; }

        /// <summary>
        /// Gets the number of seeds in the swarm
        /// </summary>
        [DataMember]
        public int SeedsInSwarm { get; private set; }

        /// <summary>
        /// Gets the availability of the torrent in 1/65535ths
        /// </summary>
        [DataMember]
        public int Availability { get; private set; }

        /// <summary>
        /// Gets the current index in the queue
        /// </summary>
        [DataMember]
        public int QueueOrder { get; private set; }

        /// <summary>
        /// Gets the number of bytes remaining to be downloaded
        /// </summary>
        [DataMember]
        public long RemainingBytes { get; private set; }

        /// <summary>
        /// Gets the DownloadUrl if torrent was added via add-url method
        /// </summary>
        [DataMember]
        public string DownloadUrl { get; private set; }

        /// <summary>
        /// Gets the rss feed url if torrent was download via feed system
        /// </summary>
        [DataMember]
        public string RssFeedUrl { get; private set; }

        /// <summary>
        /// Gets the status text Message Ex: "Seeding", "Downloading", "Finished" of the torrent
        /// </summary>
        [DataMember]
        public string StatusMessage { get; private set; }

        /// <summary>
        /// Gets the stream id of the torrent
        /// </summary>
        [DataMember]
        public string StreamID { get; private set; }

        /// <summary>
        /// Gets the datetime on which torrent was added on
        /// </summary>
        [DataMember]
        public DateTime DateAdded { get; private set; }

        /// <summary>
        /// Gets the datetime on which torrent completed its download
        /// </summary>
        [DataMember]
        public DateTime DateCompleted { get; private set; }

        /// <summary>
        /// Gets the update url of the torrent if it exists
        /// </summary>
        [DataMember]
        public string AppUpdateUrl { get; private set; }

        /// <summary>
        /// Gets the path where the torrent is being saved
        /// </summary>
        [DataMember]
        public string SavePath { get; private set; }

        /// <summary>
        /// Gets a collection of files included in the torrent
        /// </summary>
        [DataMember]
        public FileCollection Files
            => new FileCollection(client.ListFiles(Hash));

        /// <summary>
        /// Gets or sets all the trackers for this torrent
        /// </summary>
        [DataMember]
        public string[] Trackers
        {
            get
            {
                return client.GetTrackers(Hash);
            }
            
            set
            {
                client.SetTorrentProperty(Hash, "trackers", string.Join("\r\n\r\n", value));
            }
        }

        /// <summary>
        /// Sets all of the properties in the torrent
        /// based on the supplied json
        /// </summary>
        /// <param name="json">json object that represents a torrent</param>
        void LoadFromJson(RestList json)
        {
            var i = 0;
            Hash = json[i++];
            Status = json[i++];
            Name = json[i++];
            SizeInBytes = json[i++];
            ProgressInMils = json[i++];
            DownloadedBytes = json[i++];
            UploadedBytes = json[i++];
            RatioInMils = json[i++];
            UploadBytesPerSec = json[i++];
            DownloadBytesPerSec = json[i++];
            EtaInSecs = json[i++];
            label = json[i++];
            PeersConnected = json[i++];
            PeersInSwarm = json[i++];
            this.SeedsConnected = json[i++];
            this.SeedsInSwarm = json[i++];
            this.Availability = json[i++];
            this.QueueOrder = json[i++];
            this.RemainingBytes = json[i++];
            if (json.Count > i) // i = 19
            {
                this.DownloadUrl = json[i++];
                this.RssFeedUrl = json[i++];
                this.StatusMessage = json[i++];
                this.StreamID = json[i++];
                this.DateAdded = startOfEpoch.AddSeconds(json[i++]);
                this.DateCompleted = startOfEpoch.AddSeconds(json[i++]);
                this.AppUpdateUrl = json[i++];
                this.SavePath = json[i++];
            }
        }

        #region UTorrentRestClient Shortcuts

        public string[] GetTrackers() => client.GetTrackers(Hash);
        public RestNestedList ListFiles() => client.ListFiles(Hash);
        public void Pause() => client.Pause(Hash);
        public void Recheck() => client.Recheck(Hash);
        public void Remove() => client.Remove(Hash);
        public void RemoveData() => client.RemoveData(Hash);
        public void RemoveTorrent() => client.RemoveTorrent(Hash);
        public void RemoveTorrentAndData() => client.RemoveTorrentAndData(Hash);
        public void Start(bool force = false) => client.StartTorrent(Hash, force);
        public void ForceStart() => client.ForceStartTorrent(Hash);
        public void Stop() => client.Stop(Hash);
        public void Unpause() => client.Unpause(Hash);
        #endregion
        #region Operators

        public static implicit operator string(TorrentJob value)
            => value?.Hash;

        public static implicit operator bool(TorrentJob value)
            => !string.IsNullOrEmpty(value?.Hash);

        #endregion
    }
}
