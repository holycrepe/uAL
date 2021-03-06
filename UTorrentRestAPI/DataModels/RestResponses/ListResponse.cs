﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTorrentRestAPI.RestClient
{
    using Extensions;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents JSON response for ListTorrents() call
    /// </summary>
    [DataContract]
    public class ListResponse
    {
        [DataMember]
        public UTorrentLabelCollection label { get; set; }

        /// <summary>
        /// Torrent Cache ID
        /// </summary>
        [DataMember]
        public string torrentc { get; set; }

        [DataMember]
        public List<TorrentJob> torrents { get; set; }

        [DataMember]
        public List<TorrentJob> torrentp { get; set; }

        [DataMember]
        public List<string> torrentm { get; set; }

        public ListResponse SetClient(UTorrentRestClient client)
        {
            torrents?.SetClient(client);
            torrentp?.SetClient(client);
            return this;
        }
    }
}
