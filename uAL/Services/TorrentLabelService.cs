using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Torrent.Extensions;
using uAL.Properties;
using uAL.Torrents;
using NLog;
using Torrent;
using Torrent.Infrastructure;
using Torrent.Helpers.Utils;

namespace uAL.Services
{
    using static uAL.Properties.Settings.LibSettings;
    public static class TorrentLabelService
    {
        static readonly Logger Logger = LogManager.GetLogger("Simple.TorrentLabelService");
        static readonly List<string> History = new List<string>();
                
        public static TorrentLabel CreateSimpleTorrentLabel(string downloadDir, TorrentQueueItem queueItem) {
        	return CreateTorrentLabel(downloadDir, queueItem.File, false, queueItem);
        }
        public static TorrentLabel CreateTorrentLabel(string downloadDir, TorrentQueueItem queueItem, bool extend = true, bool? updateQueueItemLabel = null)
        {
        	return CreateTorrentLabel(downloadDir, queueItem.File, extend, queueItem, (updateQueueItemLabel.HasValue ? updateQueueItemLabel.Value : extend));
        }
        public static TorrentLabel CreateTorrentLabel(string downloadDir, string newFilePath, bool extend = true, TorrentQueueItem queueItem = null, bool updateQueueItemLabel = true)
        {
            return CreateTorrentLabel(downloadDir, new FileInfo(newFilePath), extend, queueItem, updateQueueItemLabel);
        }
        public static TorrentLabel CreateTorrentLabel(string downloadDir, FileInfo fi, bool extend=true, TorrentQueueItem queueItem=null, bool updateQueueItemLabel = true)
        {
            string newFileDirectory = fi.Directory.ToString();
            if (!downloadDir.EndsWith("\\", StringComparison.CurrentCulture)) {
            	downloadDir += "\\";
            }

            var label = new TorrentLabel(downloadDir, newFileDirectory);            
            label.Name = fi.Name;

            if (fi.Extension == ".torrent" && extend)
            {
                TorrentInfo tfi = (queueItem == null ? TorrentInfoCache.GetTorrentInfo(fi) : queueItem.UpdateTorrentInfo());

                if (tfi.success && !tfi.HasFileList)
                {                    
                    label.LargestName = tfi.Largest.Name;
                    var failed = false;
                    var torrentNameStripped = label.NameWithoutExtension.StripFilename();
                    var torrentContentsFileNameStripped = Path.GetFileNameWithoutExtension(label.LargestName).StripFilename();
                    var words = torrentNameStripped.Split(' ').Concat(RegexUtils.GetDates(label.NameWithoutExtension));
                    
                    LogUtils.DebugLineLabel("Important Words", string.Join(", ", words));
                    LogUtils.DebugLineLabel("Torrent Name", torrentContentsFileNameStripped);
                    
                    foreach (string word in words)
                    {
                        if (word.Length > 3 && !torrentContentsFileNameStripped.Contains(word))
                        {
                            failed = true;
                            label.FailedWord = word;
                            break;
                        }
                    }
                    
                    if (failed)
                    {
                        label.Extended = label.NameWithoutExtension.StripFilename(false);
                        label.UseExtendedName = LibSetting.EnableExtendedLabelsByDefault;
                        if (!History.Contains(label.Base))
                        {
                            History.Add(label.Base);
                            var eventInfo = new Dictionary<string, object>
                            {
                            	{"Category", label.Base},
                            	{"Torrent", label.Name},
                            	{"Contents", label.LargestName},
                            	{"TitleSuffix", label.Base}
                            };
                            var logEvent = new LogEventInfoCloneable(LogLevel.Info, Logger.Name, "Extended Label: ", eventInfo: eventInfo);
                            Logger.Log(logEvent);
                        }
                    }                    
                }
            }
            
            if (queueItem != null && updateQueueItemLabel)
            {
                queueItem.Label = label;
            }
            return label;

        }
    }
}
