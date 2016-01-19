//-----------------------------------------------------------------------
// <copyright file="DirectoryCollection.cs" company="Mike Davis">
//     To the extent possible under law, Mike Davis has waived all copyright and related or neighboring rights to this work.  This work is published from: United States.  See copying.txt for details.  I would appreciate credit when incorporating this work into other works.  However, you are under no legal obligation to do so.
// </copyright>
//-----------------------------------------------------------------------

namespace UTorrentRestAPI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A collection of directories that uTorrent can save to.
    /// </summary>
    [Serializable]
    public class DirectoryCollection : IEnumerable<uTorrentDirectory>
    {
        private readonly List<uTorrentDirectory> _internalList = new List<uTorrentDirectory>();

        internal DirectoryCollection() 
        {
        }

        /// <summary>
        /// Gets the number of directories that uTorrent can store to
        /// </summary>
        public int Count => _internalList.Count;

        /// <summary>
        /// Gets the Directory object stored at the specified index
        /// </summary>
        /// <param name="i">index of the directory</param>
        /// <returns>a directory where uTorrent can store downloads</returns>
        public uTorrentDirectory this[int i]
            => this._internalList[i];

        /// <summary>
        /// Sets the state of the object based
        /// on the supplied json
        /// </summary>
        /// <param name="directories">Directories list that will be used</param>
        void LoadFromResponse(IEnumerable<uTorrentDirectory> directories)
            => _internalList.AddRange(directories);

        /// <summary>
        /// Gets an enumerator for the collection
        /// </summary>
        /// <returns>An enumerator for the collection</returns>
        public IEnumerator<uTorrentDirectory> GetEnumerator()
            => _internalList.GetEnumerator();

        /// <summary>
        /// Gets an enumerator for the collection
        /// </summary>
        /// <returns>An enumerator for the collection</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => _internalList.GetEnumerator();
    }
}
