#define DEBUG_UTORRENT_LABELS

namespace UTorrentRestAPI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Torrent;
    using Torrent.Infrastructure;
    using Torrent.Helpers.Utils;
    using IO = System.IO;
    using RestSharp;
    using RestClient;
    using Puchalapalli.Infrastructure.ContextManagers;
    using Torrent.Extensions;
    using System.Diagnostics;    /// <summary>
                                 /// Contains all the current torrent jobs
                                 /// </summary>
    public class TorrentCollection : IEnumerable<TorrentJob>
    {
        #region Private Variables        

        /// <summary>
        /// Provides actual storage for the torrent objects
        /// </summary>
        private InternalTorrentCollection internalCollection = new InternalTorrentCollection();

        

        /// <summary>
        /// A reference to the service proxy used to communicate with the
        /// uTorrent web API
        /// </summary>
        private UTorrentRestClient client;

        /// <summary>
        /// Keeps track of the cache id of the torrent collection so we can
        /// do incremental updates
        /// </summary>
        private string cid = "0";

        bool updating = false;
        bool updateInProgress = false;
        bool updated = false;

        #endregion

        #region Constructor        

        /// <summary>
        /// Initializes a new instance of the TorrentCollection class
        /// </summary>
        /// <param name="client">the protocol client that this torrent collection should use for updates, etc</param>
        internal TorrentCollection(UTorrentRestClient client)
        {
            this.client = client;
        }

        #endregion

        #region Public Fields

        /// <summary>
        /// uTorrent Client is Connected to Web API
        /// </summary>
        public bool IsConnected => client?.IsConnected ?? false;

        public UTorrentLabelCollection Labels { get; set; } = new UTorrentLabelCollection();

        /// <summary>
        /// Returns true if BeginUpdate() has been called but Update() has not
        /// </summary>
        bool ShouldUpdate => !updating || !updated;

        public ContextManagers DeferUpdates { get; } = new ContextManagers();

        /// <summary>
        /// Gets the number of torrents currently tracked by uTorrent, without updating the collection
        /// </summary>
        public int Count => internalCollection.Count;

        #endregion

        #region Get Count

        /// <summary>
        /// Gets the number of torrents currently tracked by uTorrent, after updating the collection
        /// </summary>
        public async Task<int> GetCount(bool force = false)
        {
            await this.Update("CountNew", null, force);
            return Count;
        }

        #endregion

        #region Actions

        #region uTorrent Actions

        #region Add Torrent by Url or File

        #region Add Url

        /// <summary>
        /// Adds and starts a torrent job based off the supplied url and saves data in the supplied path
        /// </summary>
        /// <param name="url">A url to the torrent file</param>
        /// <param name="savePath">The directory where torrent data should be saved</param>
        /// <exception cref="IO.DirectoryNotFoundException">throws if uTorrent is not configured to be able to save to the supplied path</exception>
        public void AddUrl(string url, string savePath = null)
        {
            if (string.IsNullOrEmpty(savePath)) {
                client.AddTorrentFromUrl(url);
            } else {
                int storageDirectory;
                string subDirectory;
                ResolveStorageDirectory(savePath, out storageDirectory, out subDirectory);
                client.AddTorrentFromUrl(url, storageDirectory, subDirectory);
            }
        }

        #endregion

        #region Add File		        

        /// <summary>
        /// Adds the specified torrent file to uTorrent
        /// </summary>
        /// <param name="path">path to the torrent file to add</param>
        /// <param name="savePath">path to save the downloaded torrent to</param>
        public bool AddFile(string path, string savePath = null)
        {
            if (!IO.File.Exists(path)) {
                Log("AddFile -> Path DOES NOT EXIST", path);
                return false;
            }
            Log("AddFile -> Path", path);

            if (string.IsNullOrEmpty(savePath)) {
                client.AddTorrentFromFile(path);
            } else {
                int storageDir;
                string subDir;
                ResolveStorageDirectory(savePath, out storageDir, out subDir);
                client.AddTorrentFromFile(path, storageDir, subDir);
            }
            return true;
        }

        /// <summary>
        /// Adds the specified torrent file to uTorrent
        /// </summary>
        /// <param name="fileContents">a stream containing the contents of a torrent file</param>
        /// <param name="savePath">path to save the downloaded torrent to</param>
        public bool AddFile(IO.Stream fileContents, string savePath = null)
        {
            if (string.IsNullOrEmpty(savePath)) {
                client.AddTorrentFromFile(fileContents);
            } else {
                int storageDir;
                string subDir;
                ResolveStorageDirectory(savePath, out storageDir, out subDir);
                client.AddTorrentFromFile(fileContents, storageDir, subDir);
            }
            return true;
        }

        /// <summary>
        /// Add Torrent Item, and update torrent label 
        /// </summary>
        /// <param name="torrent"></param>
        /// <param name="logInfo"></param>
        /// <param name="OnAddTorrentComplete"></param>
        /// <returns></returns>
        public async Task<bool> AddFile(TorrentItem torrent, bool logInfo = true,
                                        Func<string, string, Task> OnAddTorrentComplete = null)
        {
            var labels = new List<string>() { torrent.Label.Base };
            string label;
            if (torrent.Label.IsComputed)
            {
                labels.Add(torrent.Label.Computed);
                label = torrent.Label.Base + ": " + torrent.Label.Extended;
            }
            else
            {
                label = torrent.Label.Base;
            }

            var baseLabel = torrent.Label.Base;
            if (!IsConnected) {
                return false;
            }
            Log("AddFile -> Torrent", label, torrent.TorrentName);
            if (!await Contains(torrent, ShouldUpdate)) {
                AddFile(torrent.FileName);
            }
            var success = false;
            var addedTorrent = await Get(torrent, force: true);
            if (addedTorrent == null) {
                return false;
            }
            if (addedTorrent.Label == "") {
                Log("AddFile -> Label", label);
                addedTorrent.Labels = labels.ToArray();
                success = true;
            } else if (!addedTorrent.Labels.ContainsAll(labels)) {
                Log("AddFile -> Already Has Label", string.Join("; ", addedTorrent.Labels), " vs " + label);
                success = false;
            }
            if (OnAddTorrentComplete != null) {
                await OnAddTorrentComplete(torrent.FileName, label);
            }
            return success;
        }

        #endregion

        #endregion

        #region Remove Torrents

        /// <summary>
        /// Removes all of the finished torrents from uTorrent
        /// </summary>
        /// <param name="removalOptions">the removal options to use</param>
        public void RemoveFinished(TorrentRemovalOptions removalOptions = TorrentRemovalOptions.TorrentFile)
        {
            for (int x = 0; x < this.internalCollection.Count;) {
                TorrentJob t = this.internalCollection[x];
                if (t.Status == TorrentStatus.FinishedOrStopped && t.ProgressInMils == 1000) {
                    this.Remove(t, removalOptions);
                } else {
                    x++;
                }
            }
        }

        /// <summary>
        /// Removes the specified torrent from uTorrent
        /// </summary>
        /// <param name="torrent">the torrent to remove</param>
        /// <param name="removalOptions">the removal options to use</param>
        /// <returns>value is unused--it is always true</returns>
        public bool Remove(TorrentJob torrent, TorrentRemovalOptions removalOptions = TorrentRemovalOptions.TorrentFile)
        {
            this.Remove(torrent.Hash, removalOptions);
            return true;
        }

        /// <summary>
        /// Removes the specified torrent from uTorrent
        /// </summary>
        /// <param name="torrentHash">the torrent to remove</param>
        /// <param name="removalOptions">the removal options to use</param>
        public void Remove(string torrentHash, TorrentRemovalOptions removalOptions = TorrentRemovalOptions.TorrentFile)
        {
            switch (removalOptions) {
                case TorrentRemovalOptions.Job:
                    client.Remove(torrentHash);
                    break;
                case TorrentRemovalOptions.Data:
                    client.RemoveData(torrentHash);
                    break;
                case TorrentRemovalOptions.TorrentFile:
                    client.RemoveTorrent(torrentHash);
                    break;
                case TorrentRemovalOptions.TorrentFileAndData:
                    client.RemoveTorrentAndData(torrentHash);
                    break;
                default:
                    throw new InvalidOperationException("Invalid removalOptions supplied.");
            }
        }

        /// <summary>
        /// Removes the torrent at the specified index from uTorrent (torrent file only).
        /// </summary>
        /// <param name="index">the index of the torrent to remove</param>
        /// <param name="removalOptions">the removal options to use</param>
        public void RemoveAt(int index, TorrentRemovalOptions removalOptions = TorrentRemovalOptions.TorrentFile)
        {
            this.Remove(this.internalCollection[index].Hash, removalOptions);
        }

        #endregion

        #endregion

        #region TorrentCollection Actions

        #region Update Torrents        

        #region BeginUpdate/EndUpdate        

        /// <summary>
        /// Start Update
        /// </summary>
        public void BeginUpdate()
        {
            updating = true;
            updated = false;
        }

        /// <summary>
        /// End Update
        /// </summary>
        public void EndUpdate()
        {
            updating = false;
            updated = false;
        }

        #endregion

        #region Update Torrent Collection

        #region InternalCollection Shortcuts

        public void Add(TorrentJob torrent)
            => internalCollection.Add(torrent);

        public void Add(IEnumerable<TorrentJob> torrents)
            => internalCollection.Add(torrents);

        public void AddOrReplace(TorrentJob torrent)
            => internalCollection.AddOrReplace(torrent);

        public void AddOrReplace(IEnumerable<TorrentJob> torrents)
            => internalCollection.AddOrReplace(torrents);

        public void Clear(IEnumerable<TorrentJob> newTorrents = null)
            => internalCollection.Clear(newTorrents);

        public void Remove(string hash)
            => internalCollection.Remove(hash);

        public void Remove(IEnumerable<string> hashes)
            => internalCollection.Remove(hashes);

        #endregion

        /// <summary>
        /// Causes the in-memory collection to be updated from uTorrent
        /// </summary>
        public async Task<int> Update(string source = null, string logText = null, bool force = false, int run = 0)
        {
            if (DeferUpdates && !force) {
                return Count;
            }
            if (updateInProgress) {
                while (updateInProgress) await Task.Delay(1000);
                return Count;
            }
            updateInProgress = true;
            UTorrentRestClientException ex;
            var response = client.ListTorrents(cid, out ex);

            if (ex) {
                if (run > 10) {
                    throw ex;
                }
            }
            if (ex || response == null) {
                await Update(source, logText, force, run + 1);
                return Count;
            }

            Log("Update" + (source == null ? "" : " Via " + source + (run > 0 ? new string('*', run) : "")), logText);

            LoadFromResponse(response);

            // Save the newest cacheId
            cid = response.torrentc;
            updated = true;
            updateInProgress = false;
            return Count;
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region Individual Torrents: Indexers, Get(), and Contains()

        #region Individual Torrents: Indexers

        /// <summary>
        /// Gets the torrent job object at the specified index
        /// </summary>
        /// <param name="i">index of the torrent</param>
        /// <returns>torrent at the specified index</returns>
        public TorrentJob this[int i]
            => this.internalCollection[i];

        /// <summary>
        /// Gets the torrent job object with the specified hash
        /// </summary>
        /// <param name="hash">hash of the torrent</param>
        /// <returns>torrent with the specified hash</returns>
        public TorrentJob this[string hash]
            => this.internalCollection[hash];


        /// <summary>
        /// Gets the torrent job object with the specified torrent object's hash
        /// </summary>
        /// <param name="item">Torrent Object</param>
        /// <returns>torrent with the specified hash</returns>
        public TorrentJob this[Torrent item]
            => this.internalCollection[item];

        /// <summary>
        /// Gets the torrent job object with the specified torrent item object's hash
        /// </summary>
        /// <param name="item">Torrent Item Object</param>
        /// <returns>torrent with the specified hash</returns>
        public TorrentJob this[TorrentItem item]
            => this.internalCollection[item];

        #endregion

        #region Individual Torrents: Get

        /// <summary>
        /// Gets Torrent after updating collection if needed
        /// </summary>
        /// <param name="item">A torrent item object</param>
        /// <param name="update">Update Collection before getting torrent</param>
        /// <returns>Torrent Job Object</returns>
        public async Task<TorrentJob> Get(TorrentItem item, bool update = true, bool force = false)
        {
            if (update) {
                await Update("Get", item.TorrentName, force);
            }
            return this[item];
        }

        /// <summary>
        /// Gets Torrent after updating collection if needed
        /// </summary>
        /// <param name="hash">The infohash of a torrent</param>
        /// <param name="update">Update Collection before getting torrent</param>
        /// <returns>Torrent Job Object</returns>
        public async Task<TorrentJob> Get(string hash, bool update = true, bool force = false)
        {
            if (update) {
                await Update("Get", hash, force);
            }
            return this[hash];
        }

        /// <summary>
        /// Gets Torrent after updating collection if needed
        /// </summary>
        /// <param name="item">A torrent object</param>
        /// <param name="update">Update Collection before getting torrent</param>
        /// <returns>Torrent Job Object</returns>
        public async Task<TorrentJob> Get(Torrent item, bool update = true, bool force = false)
        {
            if (update) {
                await Update("Get", item.Name, force);
            }
            return this[item];
        }

        #endregion

        #region Individual Torrents: Contains

        #region Contains: Auto

        /// <summary>
        /// Determines whether the collection contains a torrent with the given hash, only updating the collection before checking if needed 
        /// </summary>
        /// <param name="hash">The infohash of a torrent</param>
        /// <returns>True if the torrent is loaded in uTorrent</returns>
        public async Task<bool> ContainsAuto(string hash) => await Contains(hash, ShouldUpdate);

        /// <summary>
        /// Determines whether the collection contains a given torrent, only updating the collection before checking if needed
        /// </summary>
        /// <param name="item">A torrent object</param>
        /// <returns>True if the torrent is loaded in uTorrent</returns>
        public async Task<bool> ContainsAuto(Torrent item) => await Contains(item, ShouldUpdate);

        /// <summary>
        /// Determines whether the collection contains a given torrent, only updating the collection before checking if needed
        /// </summary>
        /// <param name="item">A torrent item object</param>
        /// <returns>True if the torrent is loaded in uTorrent</returns>
        public async Task<bool> ContainsAuto(TorrentItem item) => await Contains(item.Info, ShouldUpdate);

        #endregion

        #region Contains: New

        /// <summary>
        /// Determines whether the collection contains a torrent with the given hash, automatically updating the collection before checking
        /// </summary>
        /// <param name="hash">The infohash of a torrent</param>
        /// <returns>True if the torrent is loaded in uTorrent</returns>
        public async Task<bool> ContainsNew(string hash) => await Contains(hash, true);

        /// <summary>
        /// Determines whether the collection contains a given torrent, automatically updating the collection before checking
        /// </summary>
        /// <param name="item">A torrent object</param>
        /// <returns>True if the torrent is loaded in uTorrent</returns>
        public async Task<bool> ContainsNew(Torrent item) => await Contains(item, true);

        /// <summary>
        /// Determines whether the collection contains a given torrent, automatically updating the collection before checking
        /// </summary>
        /// <param name="item">A torrent item object</param>
        /// <returns>True if the torrent is loaded in uTorrent</returns>
        public async Task<bool> ContainsNew(TorrentItem item) => await Contains(item.Info, true);

        #endregion

        #region Contains: Base

        /// <summary>
        /// Determines whether the collection contains a torrent with the given hash
        /// </summary>
        /// <param name="hash">The infohash of a torrent</param>
        /// <param name = "update">Update Collection Before Checking</param>
        /// <returns>True if the torrent is loaded in uTorrent</returns>        
        public async Task<bool> Contains(string hash, bool update = false, bool force = false)
        {
            if (update) {
                if (this.internalCollection.Contains(hash)) {
                    return true;
                }
                await Update("Contains", hash, force);
            }
            return this.internalCollection.Contains(hash);
        }

        /// <summary>
        /// Determines whether the collection contains a given torrent
        /// </summary>
        /// <param name="item">A torrent object</param>
        /// <param name = "update">Update Collection Before Checking</param>
        /// <returns>True if the torrent is loaded in uTorrent</returns>
        public async Task<bool> Contains(Torrent item, bool update = false, bool force = false)
        {
            if (update) {
                if (this.internalCollection.Contains(item.Hash)) {
                    return true;
                }
                await Update("Contains", item.Name, force);
            }
            return this.internalCollection.Contains(item.Hash);
        }

        /// <summary>
        /// Determines whether the collection contains a given torrent
        /// </summary>
        /// <param name="item">A torrent item object</param>
        /// <param name = "update">Update Collection Before Checking</param>
        /// <returns>True if the torrent is loaded in uTorrent</returns>
        public async Task<bool> Contains(TorrentItem item, bool update = false, bool force = false)
        {
            if (update) {
                if (this.internalCollection.Contains(item.Info.Hash)) {
                    return true;
                }
                await Update("Contains", item.TorrentName, force);
            }
            return this.internalCollection.Contains(item.Info.Hash);
        }

        #endregion

        #endregion

        #endregion

        #region Interfaces        

        #region IEnumerable Interface: Get Enumerator

#pragma warning disable 4014

        /// <summary>
        /// Returns an enumerator of torrents loaded in uTorrent
        /// </summary>
        /// <returns>An enumerator of torrents</returns>
        public IEnumerator<TorrentJob> GetEnumerator()
        {
            Update("IEnumerator.GetEnumerator");
            return this.internalCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator of torrents loaded in uTorrent
        /// </summary>
        /// <returns>An enumerator of torrents</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            Update("IEnumerable.GetEnumerator");
            return this.internalCollection.GetEnumerator();
        }
#pragma warning restore 4014

        #endregion

        #region Load From Response

        /// <summary>
        /// Updates the in-memory collection from its json representation
        /// </summary>
        /// <param name="response">the response to load</param>
        void LoadFromResponse(ListResponse response)
        {
            var torrents = response.torrents;
            if (response.label != null)
            {
                Labels = response.label;
#if DEBUG_UTORRENT_LABELS
                if (response.torrents != null)
                {
                    this.Log("UTorrent Labels: \r\n", string.Join("\n", Labels.Select(l => l.Name)));
                }
#endif
            }
            if (response.torrents != null) {
                Clear(response.torrents);
            } else {
                AddOrReplace(response.torrentp);
                Remove(response.torrentm);
            }
        }

#endregion

#endregion

#region Directories

        private void ResolveStorageDirectory(string savePath, out int storageDirectory, out string subDirectory)
        {
            var directories = client.ListDirectories();

            storageDirectory = -1;
            for (int x = 0; x < directories.Count(); x++) {
                var directory = directories[x];
                if (savePath.StartsWith(directory.Path, StringComparison.OrdinalIgnoreCase)) {
                    storageDirectory = x;
                    break;
                }
            }

            if (storageDirectory < 0) {
                throw new IO.DirectoryNotFoundException(
                    $"uTorrent is not configured to allow saving to the supplied directory: {savePath}");
            }

            subDirectory = savePath.Substring(directories[storageDirectory].Path.Length);
        }

#endregion

#region Log

        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        void Log(string title, string text = null, string item = null)
        {
            LogUtils.Log("UT.Torrents", title, text, item);
        }

#endregion

#region Private Class: InternalTorrentCollection 

        class InternalTorrentCollection : MyKeyedCollection<string, TorrentJob>
        {
            protected override Dictionary<Type, Func<object, string>> KeySelectors { get; }
                = new Dictionary<Type, Func<object, string>>()
                  {
                      [typeof (TorrentJob)] = item => ((TorrentJob) item).Hash,
                      [typeof (Torrent)] = (item) => ((Torrent) item).Hash,
                      [typeof (TorrentItem)] = (item) => ((TorrentItem) item).Info.Hash
                  };

            public TorrentJob this[Torrent item]
                => this[GetKeyForItem(item)];

            public TorrentJob this[TorrentItem item]
                => this[GetKeyForItem(item)];
        }

#endregion
    }
}
