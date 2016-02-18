#define DEBUG_TORRENT_INFO_PIECES_GENERATOR
#define LOG_QUEUE_ITEM_CHANGED

using System;
using BencodeNET;
using System.Collections.Generic;
using BencodeNET.Objects;
using System.IO;
using System.Diagnostics;
using Torrent.Enums;
using Torrent.Helpers.Utils;
using Torrent.Extensions;
using System.Linq;

namespace Torrent
{
    using Async;
    using PostSharp.Patterns.Model;
    using Queue;
    using static Helpers.Utils.DebugUtils;

    /// <summary>
    /// Torrent Info.
    /// </summary>
    [NotifyPropertyChanged]
    public class TorrentInfo : Torrent
    {
        public delegate void ProgressChangedEventHandler(double progress);
        string[] _silentAutoChangeNotificationProperties = new string[] {
            nameof(Progress), nameof(RootDirectory)
        };
        #region Properties
        public TorrentFile Torrent { get; }
        protected static DownloadedFileChecker DownloadedFileChecker 
            = new DownloadedFileChecker();
        #region Properties: Files
        public string RootDirectory { get; set; }
        public TorrentFileInfo[] Files { get; }
        readonly string[] FileNames;
        public TorrentFileInfo Largest { get; }
        public bool HasFileList { get; }
        protected long PieceLength { get; private set; }
        protected string[] PieceHashes { get; private set; }
        #endregion
        #region Properties: Status
        public bool success { get; } = false;
        public Exception error { get; } = null;
        public bool IsBDecodeError { get; } = false;
        #endregion
        #region Properties: Comment
        public string Comment { get; }
        readonly Uri commentLink;
        public Uri CommentLink => this.commentLink;
        public bool HasCommentLink { get; private set; }
        #endregion
        #region Properties: Progress
        public double PercentComplete { get; set; } = -1;
        public double Progress { get; set; } = 0;
        public QueueStatusMember Status { get; set; } = QueueStatus.Uninitialized;
        public bool IsRunning => this.Status.IsRunning;
        public bool IsActive => this.Status.IsActive;
        public string IsInProgressText
            => this.IsActive ? "In Progress"
            : this.IsRunning ? "Queued"
            : "Idle";
        #endregion
        #endregion
        public void SetRootDirectory(string value)
            => this.RootDirectory = value;
        #region File Verification
        #region File Verification: Generate Hashes
        public IEnumerator<string> GenerateHashes(string includedFile, ProgressChangedEventHandler progressChanged = null)
            => GenerateHashes(new string[] { includedFile }, progressChanged);
        public IEnumerator<string> GenerateHashes(string[] includedFiles, ProgressChangedEventHandler progressChanged =null)
            => GenerateHashes(true, includedFiles, progressChanged);
        public IEnumerator<string> GenerateHashes(bool allowMissingFiles=false, string[] includedFiles=null, ProgressChangedEventHandler progressChanged =null)
        {
            var piece = new List<byte>();
            var totalBytes = Files.Where(f => includedFiles?.Contains(f.FullName) ?? true).Sum(f => f.Length);
            long finishedBytes = 0;
            var nextIndex = 0;
            var totalFiles = Files.Length;
            foreach (var file in Files)
            {
                nextIndex++;
#if DEBUG_TORRENT_INFO_PIECES_GENERATOR
                Log(nameof(GenerateHashes), RootDirectory.TrimEnd(file.FullName).TrimEnd(Path.DirectorySeparatorChar), file.FullName);
#endif
                var fullPath = RootDirectory;
                if (Files.Length > 1 && !fullPath.EndsWith(file.FullName))
                {
                    fullPath = Path.Combine(fullPath, file.FullName);
                }
                if (!File.Exists(fullPath))
                {
                    fullPath += ".!ut";
                }
                var exclude = (includedFiles != null 
                    && !includedFiles.Contains(file.FullName))
                    || !File.Exists(fullPath);

                if (exclude)
                {
                    var nextFileIsIncluded = false;
                    if (nextIndex < totalFiles) {
                        var nextFile = Files[nextIndex];
                        var nextFullPath = Path.Combine(RootDirectory, nextFile.FullName);
                        if ((includedFiles == null || includedFiles.Contains(nextFile.FullName)) 
                            && (File.Exists(nextFullPath) || File.Exists(nextFullPath + ".!ut")))
                        {
                            nextFileIsIncluded = true;
                        }
                    } 
                    if (!allowMissingFiles)
                    {
                        throw new FileNotFoundException($"{nameof(TorrentInfo)}.{nameof(GenerateHashes)}: Could not find content file", fullPath);
                    }
                    long start = 0;
                    if (piece.Count > 0)
                    {
                        start = Math.Min(file.Length, PieceLength - piece.Count);
                        if (File.Exists(fullPath))
                        {
                            using (var stream = File.OpenRead(fullPath))
                            {
                                var partialResult = new byte[start];
                                stream.Read(partialResult, 0, (int)start);
                                piece.AddRange(partialResult);                                
                            }
                        }
                        else {
                            piece.AddRange(new byte[start]);
                        }
                        if (piece.Count == PieceLength)
                        {
                            yield return HashUtils.SHA1(piece);
                            piece.Clear();
                        }
                        else {
                            continue;
                        }
                    }
                    for (long i=start; i<file.Length; i+= PieceLength)
                    {
                        var partialLength = file.Length - i;
                        if (partialLength < PieceLength)
                        {
                            if (nextFileIsIncluded && File.Exists(fullPath))
                            {
                                using (var stream = File.OpenRead(fullPath))
                                {
                                    var partialResult = new byte[partialLength];
                                    stream.Read(partialResult, 0, (int)partialLength);
                                    piece.AddRange(partialResult);
                                }
                            }
                            else
                            {
                                piece.AddRange(new byte[partialLength]);
                            }
                        }
                        else {
                            yield return HashUtils.EmptySHA1Hash;
                        }
                    }
                    if (includedFiles?.Contains(file.FullName) ?? false)
                    {
                        DEBUG.Noop();
                    }
                    continue;
                }
                using (var stream = File.OpenRead(fullPath))
                {
                    var streamLength = stream.Length;
                    if (piece.Count > 0)
                    {
                        var partialLength = PieceLength - piece.Count;
                        var partialResult = new byte[partialLength];
                        stream.Read(partialResult, 0, (int)partialLength);
                        piece.AddRange(partialResult);                        
                        if (piece.Count == PieceLength)
                        {
                            yield return HashUtils.SHA1(piece);
                            piece.Clear();
                        }
                        else {
                            continue;
                        }
                    }
                    while (true)
                    {
                        var remaining = streamLength - stream.Position;
                        int length = (int) Math.Min(PieceLength, remaining);
                        var result = new byte[length];
                        stream.Read(result, 0, length);
                        piece.AddRange(result);
                        var progress = Convert.ToDouble(finishedBytes + stream.Position) / totalBytes * 100;
                        progressChanged?.Invoke(progress);
                        if (piece.Count == PieceLength)
                        {
                            yield return HashUtils.SHA1(piece);
                            piece.Clear();
                        }
                        else
                        {
                            break;
                        }
                    }
                    finishedBytes += streamLength;
                }
            }
            if (piece.Count > 0)
            {
                yield return HashUtils.SHA1(piece);
            }
        }
        #endregion
        #region File Verification: Verify / Check File
        public bool VerifyFile(string name = null, string rootDirectory = null)
            => CheckFile(name, rootDirectory) == 100;

        public double CheckFile(string name = null, string rootDirectory = null)
        {
            this.Status = QueueStatus.Active;
            this.Progress = 0;
            NotifyPropertyChangedServices.RaiseEventsImmediate(this);
            ProgressChangedEventHandler progressChanged = p =>
            {
                this.Progress = p;
                NotifyPropertyChangedServices.RaiseEventsImmediate(this);
            };
            var percent = this.PercentComplete = CheckFile(progressChanged, name, rootDirectory);
            this.Status = percent == 100 ? QueueStatus.Success : QueueStatus.Incomplete;
            return percent;
        }
        public bool VerifyFile(ProgressChangedEventHandler progressChanged, string name = null, string rootDirectory=null, bool verifyStartAndEnd = false)
            => CheckFile(progressChanged, name, rootDirectory, verifyStartAndEnd) == 100;
        public double CheckFile(ProgressChangedEventHandler progressChanged = null, string name = null, string rootDirectory =null, bool verifyStartAndEnd = false)
        {

            rootDirectory = rootDirectory ?? this.RootDirectory;
            TorrentFileInfo fileInfo;
            if (string.IsNullOrEmpty(name))
            {
                fileInfo = Largest;
                name = Largest.FullName;
            }
            else {
                fileInfo = Files.First(f => f.FullName == name);
            }
            var hashesEnum = GenerateHashes(name, progressChanged);
            var hashes = hashesEnum.ToArray(fileInfo.FirstPiece, fileInfo.LastPiece);

            if (Files.Length == 1)
            {
                verifyStartAndEnd = true;
            }
            var fails = 0;
            var totalChecked = hashes.Length;
            for (var i=0; i<hashes.Length;i++)
            {
                var offset = i + fileInfo.FirstPiece;
                var computedHash = PieceHashes[offset];
                var hash = hashes[i];
                if (hash != computedHash)
                {
                    if ((verifyStartAndEnd || (i > 0 && i < hashes.Length - 1)))
                    {
                        totalChecked--;
                    }
                    else {
                        fails++;
                    }
                }
            }
            return Convert.ToDouble(totalChecked - fails) / totalChecked * 100;
        }
        #endregion
        #endregion
        public TorrentInfo(string filename)
        {
            FileName = filename;
            Torrent = null;

            try {
                Torrent = Bencode.DecodeTorrentFile(FileName);
            } catch (Exception ex) {
                error = ex;
                IsBDecodeError =
                    (ex.GetType().Name.StartsWith("BencodeDecodingException", StringComparison.CurrentCulture));
                return;
            }


            // Calculate info hash (e.g. "B415C913643E5FF49FE37D304BBB5E6E11AD5101")
            Hash = Torrent.CalculateInfoHash();

            Comment = Torrent.Comment;
            Uri _commentLink;
            HasCommentLink = Uri.TryCreate(Comment, UriKind.Absolute, out _commentLink);
            commentLink = _commentLink;

            // Get name and size of each file in 'files' list of 'info' dictionary ("multi-file mode")
            var files = new List<TorrentFileInfo>();

            PieceLength = (BNumber)Torrent.Info["piece length"];
            PieceHashes = ((BString) Torrent.Info["pieces"]).Value.ChunkArray(20).Select(b => b.ToHex()).ToArray();

            Name = ((BString) Torrent.Info["name"]).ToString();
            long currentLength = 0;
            if (Torrent.Info.ContainsKey("length")) {
                long length = (BNumber) Torrent.Info["length"];                
                var tfi = new TorrentFileInfo(Name, length, currentLength, PieceLength);
                currentLength += length;
                if (files.Count == 0 || tfi.Length > Largest.Length) {
                    Largest = tfi;
                }
                files.Add(tfi);
            }

            var bFiles = (BList) Torrent.Info["files"];
            HasFileList = (bFiles != null);
            if (HasFileList) {
                if (files.Count > 0)
                {
                    Debugger.Break();
                }
                foreach (BDictionary file in bFiles) {
                    long length = (BNumber) file["length"];
                    var bPaths = (BList) file["path"];
                    var paths = new List<string>();

                    foreach (var bPath in bPaths) {
                        paths.Add(((BString) bPath).ToString());
                    }
                    var tfi = new TorrentFileInfo(paths, length, currentLength, PieceLength);
                    currentLength += length;
                    if (files.Count == 0 || tfi.Length > Largest.Length) {
                        Largest = tfi;
                    }
                    files.Add(tfi);
                }
            }

            // if (this.files.Count == 0) {}
            Files = files.ToArray();
            FileNames = Files.Select(f => f.FullName).ToArray();
            success = true;

            this.PropertyChanged += (s, e) =>
            {
                if (_silentAutoChangeNotificationProperties.Contains(e.PropertyName))
                {
                    return;
                }
                LogQueueItemChanged(e.PropertyName);
            };
        }

        public byte[] GetBytes()
        {
            Stream TorrentStream = File.OpenRead(FileName);
            var FileBytes = new byte[TorrentStream.Length];
            TorrentStream.Read(FileBytes, 0, FileBytes.Length);
            return FileBytes;
        }
        #region Log
        [System.Diagnostics.Conditional("LOG_QUEUE_ITEM_CHANGED"), System.Diagnostics.Conditional("LOG_ALL")]
        public void LogQueueItemChanged(string propertyName)
            => Log("Δ ", propertyName);
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        static void Log(string title, string text = null, string item = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            => LogUtils.Log(nameof(TorrentInfo), title, text, item, textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);
        #endregion
    }
}
