namespace UTorrentRestAPI.Extensions
{
    using System.IO;
    using RestSharp;
    using RestClient;
    using System.Collections.Generic;

    public static class TorrentListExtensions
    {
        public static List<TorrentJob> SetClient(this List<TorrentJob> torrents, UTorrentRestClient client)
        {
            foreach (var torrent in torrents) {
                torrent.Client = client;
            }
            return torrents;
        }
    }
}
