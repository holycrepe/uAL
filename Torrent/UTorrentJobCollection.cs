
namespace Torrent
{
    using System;
    using System.Collections.Generic;
    using Infrastructure;
    using Extensions;
    using BencodeNET.Objects;
    using System.IO;
    using BencodeNET;
    using Queue;
    using Extensions.BEncode;
    using Helpers.Utils;
    using System.Diagnostics;
    using System.Linq;    /// <summary>
                          /// Holds a collection of uTorrent Settings
                          /// </summary>
    public class UTorrentJobCollection : MyKeyedCollection<string, UTorrentJob>
    {
        static readonly Func<string, bool> isSkippedKey = k 
            => k == "rec" || k.StartsWith("magnet:");
        BDictionary Data = null;
        string OriginalFileGuard = null;
        string _fileName = null;
        string FileGuard => Data?.CalculateFileguard();
        DirectoryInfo AppDataFolder;
        string FileName {
            get { return this._fileName; }
            set { if (this._fileName != value) { _fileName = value; AppDataFolder = Directory.GetParent(value); } }
        }
        protected override Dictionary<Type, Func<object, string>> KeySelectors { get; }
            = new Dictionary<Type, Func<object, string>>()
              {
                  [typeof (UTorrentJob)] = item => ((UTorrentJob) item).Name
              };
        public UTorrentJobCollection() : base() { }
        public UTorrentJobCollection(string fileName, QueueOnStartHandler OnStart = null, Action<UTorrentJob> callback = null) : this()
        {
            this.LoadUTorrentJobs(fileName, OnStart, callback);
        }
        public void Save(bool overwrite=false)
        {
            var fileName = overwrite ? FileName : PathUtils.GetFullNameWithSuffix(FileName, "-wUAL");
            foreach (var item in this)
            {
                if (item.Changed)
                {
                    var newDict = item.GetUpdatedBDictionary();
                    Data[item.Name] = newDict;
                    item.LoadFromBDictionary(item.Name, newDict);
                }
            }
            var data = Data.SetFileguard();
            using (var stream = File.OpenWrite(fileName))
            {
                data.EncodeToStream(stream);
            }
        }
        public void LoadUTorrentJobs(string fileName, QueueOnStartHandler OnStart = null, Action<UTorrentJob> callback = null)
        {
            var i = 0;
            this.FileName = fileName;
            using (var stream = File.OpenRead(fileName))
            {                
                Data = Bencode.Decode(stream) as BDictionary;
            }
            if (Data.ContainsKey(".fileguard"))
            {
                OriginalFileGuard = ((BString)Data[".fileguard"]).ToString();
            }
            var originalFileGuardCalculated = Data.CalculateFileguard();
            var count = Data.Count - Data.Select(kvp=>kvp.Key.ToString()).Count(isSkippedKey);
            OnStart?.Invoke(count);
            this.Clear();
            foreach (var job in Data)
            {
                var key = job.Key.ToString();
                if (isSkippedKey(key))
                {
                    continue;
                }
                var jobInfo = (BDictionary)job.Value;
                var torrentInfo = TorrentInfoCache.GetTorrentInfo(Path.Combine(AppDataFolder.FullName, key));
                var newItem = new UTorrentJob(key, jobInfo, ++i, torrentInfo);
                this.Add(newItem);

                if (callback != null)
                {
                    callback(newItem);
                }                
            }
        }
    }
    
}
