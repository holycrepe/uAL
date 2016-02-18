namespace UTorrentRestAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;
    using System.Threading.Tasks;
    using Torrent;
    using Torrent.Helpers.Utils;
    using Torrent.Infrastructure;
    using Torrent.Infrastructure.ContextHandlers;
    using RestClient;
    using System.Collections.Concurrent;

    /// <summary>
    /// This is the main entrypoint to the UTorrentAPI
    /// and provides access to the torrent job list,
    /// program settings, etc.
    /// </summary>
    /// <remarks>All of the objects created by this API have some shared resources
    /// (for example, the underlying channel used to connect to uTorrent).  I have done
    /// my best to allow for threadsafe access across objects in the API, but threadsafety
    /// is not guaranteed.</remarks>    
    public sealed class UTorrentClient : DisposableBase, IUTorrentRestClient
    {
        #region Private Variables

        /// <summary>
        /// The RestClient used to communicate with uTorrent.
        /// </summary>
        private UTorrentRestClient client;

        
        TorrentCollection _torrents;

        #endregion

        #region Fields

        #region Private Fields

        /// <summary>
        /// uTorrent Settings
        /// </summary>

        #endregion

        #region Public Fields

        /// <summary>
        /// Gets the current collection of torrent jobs.
        /// </summary>
        public TorrentCollection Torrents
        {
            get
            {
                return (IsConnected && (_torrents == null || !_torrents.IsConnected))
                           ? _torrents = new TorrentCollection(client)
                           : _torrents;
            }
            private set { _torrents = value; }
        }

        public ContextHandlers DeferUpdates
            => Torrents?.DeferUpdates;

        /// <summary>
        /// uTorrent Client is Connected to Web API 
        /// </summary>
        public bool IsConnected => client?.IsConnected ?? false;

        /// <summary>
        /// Gets the directories that can be used for storing torrent data.
        /// </summary>
        public ConcurrentQueue<uTorrentDirectory> StorageDirectories => client.StorageDirectories;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the UTorrentClient class.
        /// </summary>        
        public UTorrentClient() : base() {}

        public UTorrentRestClient GetClient()
            => (client ?? (client = new UTorrentRestClient()));

        #endregion

        #region Client Shortcuts

        #region Connect

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>        

        #region Connect: Overloads

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public Task<UTorrentRestClientException> ConnectAsync(string host, int port, string userName, string password)
            => GetClient().ConnectAsync(host, port, userName, password);

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public Task<UTorrentRestClientException> ConnectAsync(string host, string port, string userName, string password)
            => GetClient().ConnectAsync(host, port, userName, password);

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public Task<UTorrentRestClientException> ConnectAsync(string host, string userName, string password)
            => GetClient().ConnectAsync(host, userName, password);

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>
        /// <param name="webApiUri"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public Task<UTorrentRestClientException> ConnectAsync(Uri webApiUri, string userName, string password)
            => GetClient().ConnectAsync(webApiUri, userName, password);

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>
        public Task<UTorrentRestClientException> ConnectAsync()
            => GetClient().ConnectAsync();

        #endregion

        #endregion

        #region Settings

        #region Settings: Fields

        /// <summary>
        /// Get uTorrent Auto Import Directory
        /// </summary>
        /// <returns></returns>
        public string GetAutoImportDirectory()
            => client.GetAutoImportDirectory();

        #endregion

        #region Settings: Methods

        /// <summary>
        /// Get uTorrent Setting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public RestStruct GetSetting(string name, RestStruct defaultValue = null)
            => client.GetSetting(name, defaultValue);

        #endregion

        #endregion

        #endregion

        #region TorrentCollection Shortcuts

        #region TorrentCollection Shortcuts: BeginUpdate/EndUpdate

        /// <summary>
        /// Start Update
        /// </summary>
        public void BeginUpdate()
            => Torrents?.BeginUpdate();

        /// <summary>
        /// End Update
        /// </summary>
        public void EndUpdate()
            => Torrents?.EndUpdate();

        #endregion

        /// <summary>
        /// Causes the in-memory collection to be updated from uTorrent
        /// </summary>
        public Task<int> Update(string source = null, string logText = null, bool force = false)
            => Torrents?.Update(source, logText, force);

        #region TorrentCollection Shortcuts: Add File

        /// <summary>
        /// Add Torrent Item, and update torrent label
        /// </summary>
        /// <param name="torrent"></param>
        /// <param name="logInfo"></param>
        /// <param name="OnAddTorrentComplete"></param>
        /// <returns></returns>
        public Task<bool> AddFile(TorrentItem torrent, bool logInfo = true,
                                  Func<string, string, Task> OnAddTorrentComplete = null)
            => Torrents?.AddFile(torrent, logInfo, OnAddTorrentComplete);

        #endregion

        #region TorrentCollection Shortcuts: Contains

        /// <summary>
        /// Determines whether the collection contains a given torrent, only updating the collection before checking if needed
        /// </summary>
        /// <param name="item">A torrent object</param>
        /// <returns>True if the torrent is loaded in uTorrent</returns>
        public Task<bool> Contains(TorrentItem item)
            => Torrents?.ContainsAuto(item);

        #endregion

        #endregion

        #region Log

        /// <summary>
        /// Log A Message From UTorrentClient
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="item"></param>
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        static void Log(string title, string text = null, string item = null)
        {
            LogUtils.Log("UT.Client", title, text, item);
        }

        #endregion

        #region Interfaces: IDisposable

        /// <summary>
        /// Cleans up this instance and closes the underlying
        /// channel and channel factory.
        /// </summary>
        protected override void DoDispose()
        {
            if (client != null) {
                client.Dispose();
                client = null;
            }
        }

        #endregion
    }
}
