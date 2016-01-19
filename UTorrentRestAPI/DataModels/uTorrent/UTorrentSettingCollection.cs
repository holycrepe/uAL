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
    using RestSharp;
    using Torrent.Infrastructure;
    using Newtonsoft.Json.Linq;
    /// <summary>
    /// Holds a collection of uTorrent Settings
    /// </summary>
    [Serializable]
    public class UTorrentSettingCollection : IEnumerable<UTorrentSetting>
    {
        private readonly InternalUTorrentSettingCollection internalCollection = new InternalUTorrentSettingCollection();

        /// <summary>
        /// Initializes a new instance of the UTorrentSettingCollection class.  Internal constructor prevents instantiation outside of this assembly
        /// </summary>
        internal UTorrentSettingCollection()
        {
        }
        internal UTorrentSettingCollection(JArray jsonList) { LoadFromResponse(jsonList); }

        /// <summary>
        /// Gets the number of settings
        /// </summary>
        public int Count
            => this.internalCollection.Count;

        internal void InitializeFromResponse(JArray jsonList)
        {
            if (jsonList != null) {
                Clear();
                LoadFromResponse(jsonList);
            }
        }
        internal void LoadFromResponse(JArray jsonList)
        {
            if (jsonList != null) {
	            foreach (JArray j in jsonList)
	            {
	                AddOrReplace(new UTorrentSetting(j));
	            }
        	}
        }

        #region InternalCollection Shortcuts
        #region InternalCollection Shortcuts: Actions
        public void Add(UTorrentSetting item)
            => internalCollection.Add(item);
        public void AddOrReplace(UTorrentSetting item)
            => internalCollection.AddOrReplace(item);
        public void AddOrReplace(IEnumerable<UTorrentSetting> items)
            => internalCollection.AddOrReplace(items);
        public void Clear(IEnumerable<UTorrentSetting> newItems = null)
            => internalCollection.Clear(newItems);
        public bool ContainsKey(string name)
            => internalCollection.ContainsKey(name);
        public void Remove(string name)
            => internalCollection.Remove(name);
        public void Remove(IEnumerable<string> names)
            => internalCollection.Remove(names);
        #endregion
        #region InternalCollection Shortcuts: Indexers
        /// <summary>
        /// Gets the torrent job object at the specified index
        /// </summary>
        /// <param name="i">index of the torrent</param>
        /// <returns>torrent at the specified index</returns>
        public UTorrentSetting this[int i]
            => this.internalCollection[i];

        /// <summary>
        /// Gets the torrent job object with the specified hash
        /// </summary>
        /// <param name="hash">hash of the torrent</param>
        /// <returns>torrent with the specified hash</returns>
        public UTorrentSetting this[string hash]
            => this.internalCollection[hash];


        /// <summary>
        /// Gets the torrent job object with the specified torrent object's hash
        /// </summary>
        /// <param name="item">Torrent Object</param>
        /// <returns>torrent with the specified hash</returns>
        public UTorrentSetting this[UTorrentSetting item]
            => this.internalCollection[item];
        #endregion
        #endregion
        #region Interfaces: IEnumerable
        /// <summary>
        /// Gets an enumerator for the collection
        /// </summary>
        /// <returns>An enumerator for the collection</returns>
        public IEnumerator<UTorrentSetting> GetEnumerator()
            => internalCollection.GetEnumerator();

        /// <summary>
        /// Gets an enumerator for the collection
        /// </summary>
        /// <returns>An enumerator for the collection</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => internalCollection.GetEnumerator();
        #endregion
        #region Private Class: InternalUTorrentSettingCollection 

        public class InternalUTorrentSettingCollection : MyKeyedCollection<string, UTorrentSetting>
        {
            protected override string GetKeyForItem(UTorrentSetting item)
                => item.Name;
        }
        #endregion
    }
}
