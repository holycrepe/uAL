using System.Collections.Generic;
using System.IO;

namespace Torrent
{
    /// <summary>
    /// Cache of Torrent Infos
    /// </summary>
    public static class TorrentInfoCache
    {
        static readonly Dictionary<string, TorrentInfo> cache = new Dictionary<string, TorrentInfo>();

        public static TorrentInfo GetTorrentInfo(FileInfo fi)
        {
            return GetTorrentInfo(fi.FullName);
        }

        public static TorrentInfo GetTorrentInfo(string filename)
        {
            if (!cache.ContainsKey(filename) || !cache[filename].success)
            {
                cache[filename] = new TorrentInfo(filename);
				                
            }         
            return cache[filename];
        }

    }
}
