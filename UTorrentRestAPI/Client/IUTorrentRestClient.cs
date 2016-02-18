using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UTorrentRestAPI.RestClient
{
    public interface IUTorrentRestClient
    {
        bool IsConnected { get; }
        ConcurrentQueue<uTorrentDirectory> StorageDirectories { get; }

        //void AddTorrentFromFile(string fileName);
        //void AddTorrentFromFile(string fileName, int downloadDir, string path);
        //void AddTorrentFromUrl(string torrentUrl);
        //void AddTorrentFromUrl(string torrentUrl, int downloadDir, string path);
        Task<UTorrentRestClientException> ConnectAsync();
        Task<UTorrentRestClientException> ConnectAsync(Uri webApiUri, string userName, string password);
        Task<UTorrentRestClientException> ConnectAsync(string host, string userName, string password);
        Task<UTorrentRestClientException> ConnectAsync(string host, string port, string userName, string password);
        Task<UTorrentRestClientException> ConnectAsync(string host, int port, string userName, string password);
        string GetAutoImportDirectory();
        RestStruct GetSetting(string name, RestStruct defaultValue = null);
    }
}
