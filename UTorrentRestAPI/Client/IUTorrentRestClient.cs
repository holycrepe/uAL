using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UTorrentRestAPI.RestClient
{
    public interface IUTorrentRestClient
    {
        bool IsConnected { get; }
        DirectoryCollection StorageDirectories { get; }

        //void AddTorrentFromFile(string fileName);
        //void AddTorrentFromFile(string fileName, int downloadDir, string path);
        //void AddTorrentFromUrl(string torrentUrl);
        //void AddTorrentFromUrl(string torrentUrl, int downloadDir, string path);
        Task<Exception> ConnectAsync();
        Task<Exception> ConnectAsync(Uri webApiUri, string userName, string password);
        Task<Exception> ConnectAsync(string host, string userName, string password);
        Task<Exception> ConnectAsync(string host, string port, string userName, string password);
        Task<Exception> ConnectAsync(string host, int port, string userName, string password);
        string GetAutoImportDirectory();
        RestStruct GetSetting(string name, RestStruct defaultValue = null);
    }
}