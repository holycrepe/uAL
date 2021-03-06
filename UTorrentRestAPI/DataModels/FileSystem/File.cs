﻿//-----------------------------------------------------------------------
// <copyright file="File.cs" company="Mike Davis">
//     To the extent possible under law, Mike Davis has waived all copyright and related or neighboring rights to this work.  This work is published from: United States.  See copying.txt for details.  I would appreciate credit when incorporating this work into other works.  However, you are under no legal obligation to do so.
// </copyright>
//-----------------------------------------------------------------------

namespace UTorrentRestAPI
{
    using System.Collections.Generic;
    using RestSharp;
    using RestSharp.Deserializers;

    public class TorrentContentsFile
    {
        internal TorrentContentsFile(RestList json)
        {
            var i = 0;
            Path = json[i++];
            SizeInBytes = json[i++];
            DownloadedBytes = json[i++];
            var priority = json[i++];
            //// 3 = priority

            // TODO Need a better way of handling versioning issues
            if (json.Count <= i) {
                return;
            }

            PieceOffset = json[i++];
            Pieces = json[i++];
            Streamable = json[i++];
            var rate = json[i++];
            //// 7 = rate
            EtaInSecs = json[i++];
            HorizontalResolution = json[i++];
            VerticalResolution = json[i++];
        }

        /// <summary>
        /// Gets the path of the file inside the torrent
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the size of the file inside the torrent
        /// </summary>
        public long SizeInBytes { get; }

        /// <summary>
        /// Gets the number of bytes downloaded for the file
        /// </summary>
        public long DownloadedBytes { get; }

        /// <summary>
        /// Gets the piece offset of the start of the file in the torrent
        /// </summary>
        public long PieceOffset { get; }

        /// <summary>
        /// Gets the number of pieces in the file
        /// </summary>
        public long Pieces { get; }

        /// <summary>
        /// Gets a value indicating whether the file is streamable or not
        /// </summary>
        public bool Streamable { get; }

        /// <summary>
        /// Gets an estimate of the number of seconds left to download the file
        /// </summary>
        public int EtaInSecs { get; }

        /// <summary>
        /// Gets the horizontal resolution of the file if available
        /// </summary>
        public int HorizontalResolution { get; }

        /// <summary>
        /// Gets the vertical resolution of the file if available
        /// </summary>
        public int VerticalResolution { get; }
    }
}
