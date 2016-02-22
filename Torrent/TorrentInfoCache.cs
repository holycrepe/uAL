using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Torrent
{
    /// <summary>
    /// Cache of Torrent Infos
    /// </summary>
    public static class TorrentInfoCache
    {
        static readonly ConcurrentDictionary<string, TorrentInfo> cache = new ConcurrentDictionary<string, TorrentInfo>();

        public static TorrentInfo GetTorrentInfo(FileInfo fi) => GetTorrentInfo(fi.FullName);

        public static TorrentInfo GetTorrentInfo(string filename)
        {
            if (!cache.ContainsKey(filename) || !cache[filename].success) {
                var newItem = new TorrentInfo(filename);
                cache[filename] = newItem;
            }
            return cache[filename];
        }
    }
}
