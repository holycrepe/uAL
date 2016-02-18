//-----------------------------------------------------------------------
// <copyright file="FileCollection.cs" company="Mike Davis">
//     To the extent possible under law, Mike Davis has waived all copyright and related or neighboring rights to this work.  This work is published from: United States.  See copying.txt for details.  I would appreciate credit when incorporating this work into other works.  However, you are under no legal obligation to do so.
// </copyright>
//-----------------------------------------------------------------------

namespace UTorrentRestAPI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Holds a collection of files that are contained in a torrent
    /// </summary>
    //[Serializable]
    //[XmlSerializerAssembly("UTorrentRestAPI.XmlSerializers")]
    public class FileCollection : IEnumerable<TorrentContentsFile>
    {
        private List<TorrentContentsFile> internalList = new List<TorrentContentsFile>();

        /// <summary>
        /// Initializes a new instance of the FileCollection class.  Internal constructor prevents instantiation outside of this assembly
        /// </summary>
        internal FileCollection() { }

        internal FileCollection(RestNestedList jsonList) { LoadFromResponse(jsonList); }

        /// <summary>
        /// Gets the number of files in the torrent
        /// </summary>
        public int Count
            => this.internalList.Count;

        /// <summary>
        /// Gets a File object contained in a torrent at the specified index
        /// </summary>
        /// <param name="i">index of file</param>
        /// <returns>a file in the torrent</returns>
        public TorrentContentsFile this[int i]
            => this.internalList[i];

        void LoadFromResponse(RestNestedList jsonList)
        {
            foreach (var j in jsonList) {
                this.internalList.Add(new TorrentContentsFile(j));
            }
        }

        /// <summary>
        /// Gets an enumerator for the collection
        /// </summary>
        /// <returns>An enumerator for the collection</returns>
        public IEnumerator<TorrentContentsFile> GetEnumerator()
            => internalList.GetEnumerator();

        /// <summary>
        /// Gets an enumerator for the collection
        /// </summary>
        /// <returns>An enumerator for the collection</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => internalList.GetEnumerator();
    }
}
