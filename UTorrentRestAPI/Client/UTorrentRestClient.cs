namespace UTorrentRestAPI.RestClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Net;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Torrent;
    using Torrent.Helpers.Utils;
    using RestSharp;
    using RestSharp.Authenticators;
    using RestSharp.Deserializers;
    using Torrent.Extensions;
    using Torrent.Infrastructure;
    using Torrent.Infrastructure.ContextHandlers;
    using Extensions;
    using Deserializers;
    using Newtonsoft.Json.Linq;
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
    public class UTorrentRestClient : DisposableBase, IUTorrentRestClient
    {
        public delegate void ExceptionHandlerDelegate(Exception exception, out UTorrentRestClientException ex);

        #region Private Constants

        const bool DEFAULT_THROW_EXTENSIONS = false;
        const string DEFAULT_POST_FILENAME = @"torrent.torrent";
        const long MAX_INCOMING_MESSAGE_SIZE_IN_BYTES_DEFAULT = 524288;

        #endregion

        #region Private Variables

        /// <summary>
        /// The RestClient used to communicate with uTorrent.
        /// </summary>
        private RestClient client;

        private string token;
        private UTorrentRestClientException _out_ex;
        private IDeserializer JsonDeserializer;
        private IDeserializer XmlDeserializer;

        long maxIncomingMessageSizeInBytes;
        private UTorrentSettingCollection Settings = new UTorrentSettingCollection();
        readonly Dictionary<string, string> Cookies = new Dictionary<string, string>();

        #endregion

        #region Properties

        #region Public Properties
        public ContextHandlers ThrowExceptions { get; } = new ContextHandlers(DEFAULT_THROW_EXTENSIONS);
        public UTorrentRestClientException GetTokenException;
        public string Token => token ?? (token = GetToken(out GetTokenException));

        /// <summary>
        /// uTorrent Client is Connected to Web API 
        /// </summary>
        public bool IsConnected { get; protected internal set; } = false;

        public UTorrentRestClientException ClientException { get; private set; }
        public ConcurrentQueue<UTorrentRestClientException> ClientExceptions { get; } = new ConcurrentQueue<UTorrentRestClientException>();

        /// <summary>
        /// Gets the directories that can be used for storing torrent data.
        /// </summary>
        public ConcurrentQueue<uTorrentDirectory> StorageDirectories { get; } = new ConcurrentQueue<uTorrentDirectory>();

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the UTorrentClient class.
        /// </summary>        
        /// <param name="_maxIncomingMessageSizeInBytes">The size of message to accept from uTorrent web</param>        
        public UTorrentRestClient(long _maxIncomingMessageSizeInBytes = MAX_INCOMING_MESSAGE_SIZE_IN_BYTES_DEFAULT)
        {
            maxIncomingMessageSizeInBytes = _maxIncomingMessageSizeInBytes;
        }

        #endregion

        #region Initialize Client

        bool InitializeClient()
        {
            if (SetToken(out GetTokenException)) {
                client.ClearHandlers();
                client.AddHandler("text/plain", JsonDeserializer ?? (JsonDeserializer = new JsonNetDeserializer()));
                client.AddHandler("application/json", JsonDeserializer);
                client.AddHandler("text/xml", XmlDeserializer ?? (XmlDeserializer = new XmlDeserializer()));
                return true;
            }
            return false;
        }

        #endregion

        #region Connect

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>
        UTorrentRestClientException Connect()
        {
            IsConnected = InitializeClient();
            return GetTokenException;
            //try
            //{
            //    InitializeClient();
            //    // GetSettings();
            //    _isConnected = true;
            //    return null;
            //}
            //catch (WebException ex)
            //{
            //    return ex;
            //}
            //catch (Exception ex)
            //{
            //    return ex;
            //}
        }

        #region Connect: Overloads

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public async Task<UTorrentRestClientException> ConnectAsync(string host, int port, string userName,
                                                                    string password)
        {
            SaveCredentials(host, port.ToString(), userName, password);
            return await Task.Run(() => Connect());
        }

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public async Task<UTorrentRestClientException> ConnectAsync(string host, string port, string userName,
                                                                    string password)
        {
            SaveCredentials(host + ":" + port, userName, password);
            return await Task.Run(() => Connect());
        }

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public async Task<UTorrentRestClientException> ConnectAsync(string host, string userName, string password)
        {
            SaveCredentials(new Uri("http://" + host + "/gui/"), userName, password);
            return await Task.Run(() => Connect());
        }

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>
        /// <param name="webApiUri"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public async Task<UTorrentRestClientException> ConnectAsync(Uri webApiUri, string userName, string password)
        {
            SaveCredentials(webApiUri, userName, password);
            return await Task.Run(() => Connect());
        }

        /// <summary>
        /// Connect to uTorrent Web API
        /// </summary>
        public async Task<UTorrentRestClientException> ConnectAsync()
            => await Task.Run(() => Connect());

        #endregion

        #endregion

        #region Save Credentials

        /// <summary>
        /// Save uTorrent Web API Credentials
        /// </summary>
        /// <param name="webApiUri"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        void SaveCredentials(Uri webApiUri, string userName, string password)
        {
            client = new RestClient {BaseUrl = webApiUri, Authenticator = new HttpBasicAuthenticator(userName, password)};
        }

        #region Save Credentials: Overloads

        /// <summary>
        /// Save uTorrent Web API Credentials
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        void SaveCredentials(string host, int port, string userName, string password)
            => SaveCredentials(host, port.ToString(), userName, password);

        /// <summary>
        /// Save uTorrent Web API Credentials
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        void SaveCredentials(string host, string port, string userName, string password)
            => SaveCredentials(host + ":" + port, userName, password);

        /// <summary>
        /// Save uTorrent Web API Credentials
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        void SaveCredentials(string host, string userName, string password)
            => SaveCredentials(new Uri("http://" + host + "/gui/"), userName, password);

        #endregion

        #endregion

        #region Generate Request

        #region Generate Request: Method.POST

        RestRequest NewPost(string action, string rootElement = null)
            => NewAction(action, rootElement, Method.POST);

        RestRequest NewAddFileRequest()
            => NewPost("add-file");

        IRestRequest NewAddFileRequest(int downloadDir, string path)
            => NewAddFileRequest()
                .AddUrlQuery(nameof(downloadDir), downloadDir)
                .AddPath(path);

        #endregion

        #region Generate Request: Base

        RestRequest NewRequest(string action, string rootElement = null, Method method = Method.GET)
            => new RestRequest(method) {Resource = "?token={token}&" + action, RootElement = rootElement};

        #endregion

        #region Generate Request: Actions

        RestRequest NewAction(string action, Method method)
            => NewAction(action, null, null, method);

        RestRequest NewAction(string action, string rootElement, Method method)
            => NewAction(action, rootElement, null, method);

        RestRequest NewAction(string action, string rootElement = null, string query = null, Method method = Method.GET)
            => NewRequest($"action={action}" + (query == null ? "" : $"&{query}"), rootElement, method);

        #endregion

        #region Generate Request: Actions: TorrentAction

        RestRequest NewTorrentAction(string action, string hash, string rootElement = null, Method method = Method.GET)
            => NewAction(action, rootElement, $"{nameof(hash)}={hash}", method);

        #endregion

        #endregion

        #region Get Response

        public IRestResponse GetResponse(IRestRequest request)
            => client.Execute(request);

        public IRestResponse<T> GetResponse<T>(IRestRequest request) where T : new()
            => client.Execute<T>(request);

        #endregion

        #region Check Response

        static void CheckResponseException(IRestResponse response, out UTorrentRestClientException ex,
                                           ExceptionHandlerDelegate exceptionHandler)
            => exceptionHandler.Invoke(response.ErrorException, out ex);

        bool CheckResponseIsValid(IRestResponse response,
                                  out UTorrentRestClientException ex,
                                  ExceptionHandlerDelegate exceptionHandler)
        {
            exceptionHandler = exceptionHandler ?? ExceptionHandler;
            if (response == null) {
                ex = UTorrentRestClientException.New("Response was null");
                exceptionHandler?.Invoke(ex, out ex);
                return false;
            }
            ProcessValidResponse(response);
            CheckResponseException(response, out ex, exceptionHandler);
            return true;
        }

        #endregion

        #region Process Response

        void ProcessValidResponse(IRestResponse response)
            => Cookies.Concat(response.Cookies, ((c, d) => d[c.Name] = c.Value));

        public bool ProcessResponse(IRestResponse response, ExceptionHandlerDelegate exceptionHandler = null)
            => ProcessResponse(response, out _out_ex, exceptionHandler);

        public bool ProcessResponse(IRestResponse response, out UTorrentRestClientException ex,
                                    ExceptionHandlerDelegate exceptionHandler = null)
            => CheckResponseIsValid(response, out ex, exceptionHandler);

        public T ProcessResponse<T>(IRestResponse<T> response, ExceptionHandlerDelegate exceptionHandler = null)
            => ProcessResponse(response, out _out_ex, exceptionHandler);

        public T ProcessResponse<T>(IRestResponse<T> response, out UTorrentRestClientException ex,
                                    ExceptionHandlerDelegate exceptionHandler = null)
            => (CheckResponseIsValid(response, out ex, exceptionHandler) ? response.Data : default(T));

        #endregion

        #region Requests

        #region Requests: Exception Handler

        void ExceptionHandler(Exception exception, out UTorrentRestClientException ex, string message,
                              bool throwExceptions = true)
        {
            ClientException = ex = null;
            if (exception != null) {
                ClientException = ex = UTorrentRestClientException.New(exception, message);
                ClientExceptions.Enqueue(ClientException);
                if (ThrowExceptions && throwExceptions) {
                    throw ClientException;
                }
            }
        }

        void ExceptionHandler(Exception exception, out UTorrentRestClientException ex)
            => ExceptionHandler(exception, out ex, null);

        ExceptionHandlerDelegate CreateExceptionHandler(string message, bool throwExceptions = true)
            =>
                (Exception exception, out UTorrentRestClientException ex) =>
                ExceptionHandler(exception, out ex, message, throwExceptions);

        #endregion

        #region Requests: Execute

        #region Requests: Execute: Via Action Name

        #region Requests: Execute: Via Action Name: Non-Torrent Action

        #region Requests: Execute: Via Action Name: Non-Torrent Action: Returns Value (With Root Element)

        public T ExecuteAction<T>(string action, string rootElement, ExceptionHandlerDelegate exceptionHandler = null)
            where T : new()
            => ExecuteAction<T>(action, rootElement, out _out_ex, exceptionHandler);

        public T ExecuteAction<T>(string action, string rootElement, out UTorrentRestClientException ex,
                                  ExceptionHandlerDelegate exceptionHandler = null) where T : new()
            => Execute<T>(NewAction(action, rootElement), out ex, exceptionHandler);

        #endregion

        #endregion

        #region Requests: Execute: Via Action Name: Torrent Action

        #region Requests: Execute: Via Action Name: Torrent Action: No Return Value

        public void Execute(string action, string hash, ExceptionHandlerDelegate exceptionHandler = null)
            => Execute(action, hash, out _out_ex, exceptionHandler);

        public void Execute(string action, string hash, out UTorrentRestClientException ex,
                            ExceptionHandlerDelegate exceptionHandler = null)
            => Execute(NewTorrentAction(action, hash), out ex, exceptionHandler);

        #endregion

        #region Requests: Execute: Via Action Name: Torrent Action: Returns Value

        public T Execute<T>(string action, string hash, ExceptionHandlerDelegate exceptionHandler = null)
            where T : new()
            => Execute<T>(action, hash, out _out_ex, exceptionHandler);

        public T Execute<T>(string action, string hash, out UTorrentRestClientException ex,
                            ExceptionHandlerDelegate exceptionHandler = null) where T : new()
            => Execute<T>(action, hash, null, out ex, exceptionHandler);

        public T Execute<T>(string action, string hash, string rootElement,
                            ExceptionHandlerDelegate exceptionHandler = null) where T : new()
            => Execute<T>(action, hash, rootElement, out _out_ex, exceptionHandler);

        public T Execute<T>(string action, string hash, string rootElement, out UTorrentRestClientException ex,
                            ExceptionHandlerDelegate exceptionHandler = null) where T : new()
            => Execute<T>(NewTorrentAction(action, hash, rootElement), out ex, exceptionHandler);

        #endregion

        #endregion

        #endregion

        #region Requests: Execute: Via RestRequest

        #region Requests: Execute: Via RestRequest: No Return Value

        public void Execute(IRestRequest request, ExceptionHandlerDelegate exceptionHandler = null)
            => Execute(request, out _out_ex, exceptionHandler);

        public void Execute(IRestRequest request, out UTorrentRestClientException ex,
                            ExceptionHandlerDelegate exceptionHandler = null)
            => ProcessResponse(GetResponse(request), out ex, exceptionHandler);

        #endregion

        #region Execute: Via RestRequest: Returns Value

        public T Execute<T>(IRestRequest request, ExceptionHandlerDelegate exceptionHandler = null) where T : new()
            => Execute<T>(request, out _out_ex, exceptionHandler);

        public T Execute<T>(IRestRequest request, out UTorrentRestClientException ex,
                            ExceptionHandlerDelegate exceptionHandler = null) where T : new()
            => ProcessResponse(GetResponse<T>(request), out ex, exceptionHandler);

        #endregion

        #endregion

        #endregion

        #endregion

        #region Implementations

        #region Get Token

        private string GetToken(out UTorrentRestClientException ex)
        {
            var exceptionHandler = CreateExceptionHandler("Unable to retrieve Web API Token", false);
            var request = new RestRequest() {Resource = "token.html"};
            var response = GetResponse<RestStruct>(request);
            var value = ProcessResponse(response, out ex, exceptionHandler);
            //var value = Execute<RestStruct>(request, out ex, exceptionHandler);
            GetTokenException = ex;
            return value;
        }

        private bool SetToken(out UTorrentRestClientException ex)
        {
            ex = null;
            if (token != null) {
                return true;
            }
            token = GetToken(out ex);
            client.AddDefaultParameter(nameof(token), token, ParameterType.UrlSegment);
            if (Cookies.ContainsKey("GUID")) {
                client.AddDefaultParameter("GUID", Cookies["GUID"], ParameterType.Cookie);
            }
            return !ex;
        }

        #endregion

        #region Settings

        #region Settings: Fields

        /// <summary>
        /// Get uTorrent Auto Import Directory
        /// </summary>
        /// <returns></returns>
        public string GetAutoImportDirectory() => GetSetting("dir_autoload", "NA");

        #endregion

        #region Settings: Methods

        /// <summary>
        /// Get uTorrent Setting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public RestStruct GetSetting(string name, RestStruct defaultValue = null)
        {
            GetSettings(false);
            return (Settings.Contains(name) ? Settings[name].Value : defaultValue);
        }

        /// <summary>
        /// Refresh uTorrent Settings
        /// </summary>
        /// <param name="force">If false, only refreshes if Settings count is zero and client is connected</param>
        /// <returns></returns>
        public void GetSettings(bool force = true)
        {
            if (!force && !(Settings.Count == 0 && IsConnected)) {
                return;
            }
            //var request = NewAction("getsettings", "settings");
            //var response = GetResponse<JArray>(request);
            //var value = ProcessResponse(response);
            //var response2 = GetResponse<List<UTorrentSetting>>(request);
            //var value2 = ProcessResponse(response2);
            //var response3 = GetResponse<InternalUTorrentSettingCollection>(request);
            //Settings = ProcessResponse(response3);
            Settings = ExecuteAction<UTorrentSettingCollection>("getsettings", "settings");
        }

        #endregion

        #endregion

        #region Torrents

        #region Torrents Collection

        /// <summary>
        /// Calls the list action on the uTorrent web API.  This
        /// call includes the cache id parameter so that only
        /// incremental changes are returned from uTorrent.
        /// </summary>
        /// <param name="cid">The cacheid to use for the request.  "0" means no caching.</param>
        /// <returns>A collection of torrents</returns>
        public ListResponse ListTorrents(string cid = "0")
            => ListTorrents(cid, out _out_ex);

        public ListResponse ListTorrents(string cid, out UTorrentRestClientException ex)
            => Execute<ListResponse>(
                                             NewRequest($"list=1&{nameof(cid)}={cid}"), out ex)
                .SetClient(this);

        #endregion

        #region Torrent

        /// <summary>
        /// Adds the specified torrent and starts it immediately
        /// </summary>
        /// <param name="torrentUrl">A url to the torrent to start</param>
        /// <returns>An empty response</returns>
        public void AddTorrentFromUrl(string torrentUrl)
            => Execute(NewAction("add-url").AddUrlQuery("s", torrentUrl));

        /// <summary>
        /// Adds the specified torrent and starts it immediately
        /// </summary>
        /// <param name="torrentUrl">A url to the torrent to start</param>
        /// <param name="downloadDir">The index of the download directory from calling <c>ListDirectories</c></param>
        /// <param name="path">The sub path to use under the directory provided in <c>downloadDir</c></param>
        /// <returns>An empty response</returns>
        public void AddTorrentFromUrl(string torrentUrl, int downloadDir, string path)
            =>
                Execute(
                        NewAction("add-url")
                            .AddUrlQueries("s", torrentUrl, nameof(downloadDir), downloadDir)
                            .AddPath(path));

        /// <summary>
        /// Adds the specified torrent file and starts it immediately
        /// </summary>
        /// <param name="torrentFile">Absolute Filename of Torrent File</param>
        /// <returns>An empty response</returns>
        public void AddTorrentFromFile(string torrentFile)
            => Execute(NewAddFileRequest()
                           .AddFile("torrent_file", torrentFile, @"application/x-bittorrent"));

        /// <summary>
        /// Adds the specified torrent file and starts it immediately
        /// </summary>
        /// <param name="torrentFile">Absolute Filename of Torrent File</param>
        /// <param name="downloadDir">The index of the download directory from calling <c>ListDirectories</c></param>
        /// <param name="path">The sub path to use under the directory provided in <c>downloadDir</c></param>
        /// <returns>An empty response</returns>
        public void AddTorrentFromFile(string torrentFile, int downloadDir, string path)
            => Execute(NewAddFileRequest(downloadDir, path)
                           .AddFile(nameof(torrentFile), torrentFile, @"application/x-bittorrent"));

        /// <summary>
        /// Adds the specified torrent file and starts it immediately
        /// </summary>
        /// <param name="torrentFile">Stream Containing Torrent File Contents</param>
        /// <param name="fileName">Name of the file in the POST request</param>
        /// <returns>An empty response</returns>        
        public void AddTorrentFromFile(Stream torrentFile, string fileName = DEFAULT_POST_FILENAME)
            => Execute(NewAddFileRequest()
                           .AddFile(nameof(torrentFile), torrentFile, fileName, @"application/x-bittorrent"));

        /// <summary>
        /// Adds the specified torrent file and starts it immediately
        /// </summary>
        /// <param name="torrentFile">Stream Containing Torrent File Contents</param>
        /// <param name="downloadDir">The index of the download directory from calling <c>ListDirectories</c></param>
        /// <param name="path">The sub path to use under the directory provided in <c>downloadDir</c></param>
        /// <param name="fileName">Name of the file in the POST request</param>
        /// <returns>An empty response</returns>
        public void AddTorrentFromFile(Stream torrentFile, int downloadDir, string path,
                                       string fileName = DEFAULT_POST_FILENAME)
            => Execute(NewAddFileRequest(downloadDir, path)
                           .AddFile(nameof(torrentFile), torrentFile, fileName, @"application/x-bittorrent"));

        /// <summary>
        /// Adds the specified torrent file and starts it immediately
        /// </summary>
        /// <param name="torrentFile">Byte Array Containing Torrent File Contents</param>
        /// <param name="fileName">Name of the file in the POST request</param>
        /// <returns>An empty response</returns>        
        public void AddTorrentFromFile(byte[] torrentFile, string fileName = DEFAULT_POST_FILENAME)
            => Execute(NewAddFileRequest()
                           .AddFile(nameof(torrentFile), torrentFile, fileName, @"application/x-bittorrent"));

        /// <summary>
        /// Adds the specified torrent file and starts it immediately
        /// </summary>
        /// <param name="torrentFile">Byte Array Containing Torrent File Contents</param>
        /// <param name="downloadDir">The index of the download directory from calling <c>ListDirectories</c></param>
        /// <param name="path">The sub path to use under the directory provided in <c>downloadDir</c></param>
        /// <param name="fileName">Name of the file in the POST request</param>
        /// <returns>An empty response</returns>
        public void AddTorrentFromFile(byte[] torrentFile, int downloadDir, string path,
                                       string fileName = DEFAULT_POST_FILENAME)
            => Execute(NewAddFileRequest(downloadDir, path)
                           .AddFile(nameof(torrentFile), torrentFile, fileName, @"application/x-bittorrent"));

        #endregion

        /// <summary>
        /// List Files Associated with A Torrent
        /// </summary>
        /// <param name="hash">Torrent Hash</param>
        /// <returns>FilesCollection</returns>
        public List<TorrentContentsFile> ListFiles(string hash)
            => Execute<List<TorrentContentsFile>>("getfiles", hash, "files");

        /// <summary>
        /// Starts the torrent represented by the supplied hash
        /// </summary>
        /// <param name="hash">the hash of the torrent to start</param>
        /// <returns>An empty response</returns>
        public void StartTorrent(string hash, bool force = false)
            => Execute(force ? "forcestart" : "start", hash);

        /// <summary>
        /// Force Starts the torrent represented by the supplied hash
        /// </summary>
        /// <param name="hash">the hash of the torrent to force start</param>
        /// <returns>An empty response</returns>
        public void ForceStartTorrent(string hash)
            => StartTorrent(hash, true);

        /// <summary>
        /// Stop the torrent represented by the supplied hash
        /// </summary>
        /// <param name="hash">the hash of the torrent to stop</param>
        /// <returns>An empty response</returns>
        public void Stop(string hash)
            => Execute("stop", hash);

        /// <summary>
        /// Pause the torrent represented by the supplied hash
        /// </summary>
        /// <param name="hash">the hash of the torrent to pause</param>
        /// <returns>An empty response</returns>
        public void Pause(string hash)
            => Execute("pause", hash);

        /// <summary>
        /// UnPause the torrent represented by the supplied hash
        /// </summary>
        /// <param name="hash">the hash of the torrent to unpause</param>
        /// <returns>An empty response</returns>
        public void Unpause(string hash)
            => Execute("unpause", hash);

        /// <summary>
        /// Recheck the torrent represented by the supplied hash
        /// </summary>
        /// <param name="hash">the hash of the torrent to recheck</param>
        /// <returns>An empty response</returns>
        public void Recheck(string hash)
            => Execute("recheck", hash);

        /// <summary>
        /// Remove the torrent job represented by the supplied hash.  Torrent file and data is left intact.
        /// </summary>
        /// <param name="hash">the hash of the torrent to remove</param>
        /// <returns>An empty response</returns>
        public void Remove(string hash)
            => Execute("remove", hash);

        /// <summary>
        /// Remove the torrent represented by the supplied hash.  Data is left intact.
        /// </summary>
        /// <param name="hash">the hash of the torrent to remove</param>
        /// <returns>An empty response</returns>
        public void RemoveTorrent(string hash)
            => Execute("removetorrent", hash);

        /// <summary>
        /// Remove the torrent represented by the supplied hash including data
        /// </summary>
        /// <param name="hash">the hash of the torrent to remove</param>
        /// <returns>An empty response</returns>
        public void RemoveTorrentAndData(string hash)
            => Execute("removedatatorrent", hash);

        /// <summary>
        /// Removes the data associated with a torrent
        /// </summary>
        /// <param name="torrentHash">the hash of the torrent whose data should be removed</param>
        /// <returns>An empty response</returns>
        public void RemoveData(string hash)
            => Execute("removedata", hash);

        /// <summary>
        /// List the directories that uTorrent can save to
        /// </summary>
        /// <returns>A collection of directories that uTorrent can save to</returns>
        public List<uTorrentDirectory> ListDirectories()
            => Execute<List<uTorrentDirectory>>(NewAction("list-dirs", "download-dirs"));

        /// <summary>
        /// Sets a property on a torrent
        /// </summary>
        /// <param name="hash">The infohash of the torrent to modify</param>
        /// <param name="propertyName">The name of the property to modify</param>
        /// <param name="propertyValue">The new value the property should be set to</param>
        /// <returns>An empty response</returns>
        public void SetTorrentProperties(string hash, params string[] properties)
        {
            
            var action = NewTorrentAction("setprops", hash);
            for (int i=0, len=properties.Length; i < len; i+=2)
            {
                var propertyName = properties[i];
                if (i + 1 >= len)
                {
                    throw new ArgumentNullException($"No value supplied for property {propertyName}");
                }
                var propertyValue = properties[i + 1];
                action.AddUrlQueries("s", propertyName, "v", propertyValue);
            }
            Execute(action);
        }

        /// <summary>
        /// Gets the properties on a torrent
        /// </summary>
        /// <param name="hash">The infohash of the torrent to fetch properties</param>
        /// <returns>Dictionary of the torrent's properties</returns>
        public Dictionary<string, object> GetTorrentProperties(string hash)
            => Execute<Dictionary<string, object>>("getprops", hash, "props/0");

        /// <summary>
        /// Gets the trackers of a torrent
        /// </summary>
        /// <param name="hash">The infohash of the torrent to fetch properties</param>
        /// <returns>string[] of Trackers</returns>
        public string[] GetTrackers(string hash)
            =>
                GetTorrentProperties(hash)?["trackers"]?.ToString()
                                                        .Split(new char[] {'\r', '\n'},
                                                               StringSplitOptions.RemoveEmptyEntries);

        #endregion

        #endregion

        #region Log

        /// <summary>
        /// Log A Message From UTorrentRestClient
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="item"></param>
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        static void Log(string title, string text = null, string item = null)
        {
            LogUtils.Log("UT.RestClient", title, text, item);
        }

        #endregion

        #region Interfaces: IDisposable

        /// <summary>
        /// Cleans up this instance and closes the underlying
        /// channel and channel factory.
        /// </summary>
        protected override void DoDispose()
        {
            client = null;
        }

        #endregion
    }
}
