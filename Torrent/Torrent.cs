using PostSharp.Patterns.Model;
using System;
using System.Xml.Serialization;
using Torrent.Infrastructure;

namespace Torrent
{
    [NotifyPropertyChanged]
    public class Torrent : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Gets the infohash of the torrent
        /// </summary>
        public string Hash { get; protected set; }

        /// <summary>
        /// Gets the name of the torrent
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the file name of the torrent
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the torrent's label
        /// </summary>
        public virtual string Label { get; set; }

        public int PercentageDone { get; set; }
        //public bool Running { get; set; }

        public Torrent() {
            //Running = true;
        }
    }
}
