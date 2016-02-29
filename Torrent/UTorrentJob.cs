#define LOG_QUEUE_ITEM_CHANGED

using System.Runtime.CompilerServices;

namespace Torrent
{
    using Enums;
    using Helpers.Utils;
    using Queue;
    using Extensions;
    using Extensions.BEncode;
    using BencodeNET.Objects;
    using PostSharp.Patterns.Model;
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using IO = System.IO;
    using System.Diagnostics;
    using System.Collections.ObjectModel;
    using Infrastructure;
    using Puchalapalli.Extensions.Primitives;
    using Puchalapalli.Infrastructure.Interfaces;

    [NotifyPropertyChanged]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class UTorrentJob : QueueItemBase, IDebuggerDisplay
    {
        private static int _counter = 0;
        private ObservableCollection<string> _labels = null;
        long _completedOn = 0;
        string _originalPath;
        string[] _ignoredAutoChangeNotificationProperties = new string[] {
            nameof(Changed), nameof(Initialized), nameof(HasSecondaryLabel), nameof(PrimaryLabel), nameof(SecondaryLabel), 
            nameof(LabelsAreValid), nameof(ParentHash)
        };
        string[] _silentAutoChangeNotificationProperties = new string[] {
            nameof(ChangedText)
        };
        #region Public Fields
        #region Public Fields: Main
        public bool IsComplete
            => this._completedOn > 0;
        public bool Changed { get; set; } = false;
        public string ChangedText
            => Changed ? "Changed" : "Unchanged";
        public bool Initialized{ get; set; } = false;
        public string Name { get; set; }
        public int Number { get; set; }
        public QueueStatusMember Status {
            get { return this.Torrent?.Status; }
            set
            {
                if (this.Torrent == null)
                    throw new InvalidOperationException($"Attempted to set {nameof(UTorrentJob)}.{nameof(Label)} when {nameof(Torrent)} is null");
                this.Torrent.Status = value;
            }
        }
        public List<string> Trackers { get; set; }
        public UTorrentJobTarget[] Targets { get; set; } = new UTorrentJobTarget[0];
        public string Caption { get; set; }
        BDictionary Info { get; set; }
        public TorrentInfo Torrent { get; set; }
        public string Hash { get; set; }
        public string ParentHash { get; set; }
        public byte[] Priority { get; set; }
        public int PriorityLength { get; set; }
        public long Priority2 { get; set; }
        #endregion
        #region Public Fields: Label
        public string OriginalLabel { get; set; }
        [SafeForDependencyAnalysis]
        public string PrimaryLabel
        {
            get
            {
                return this.Labels.OrderByDescending(s => s.Length).First();
            }
            set
            {
                var current = this.PrimaryLabel;
                if (value != current)
                {
                    this.Labels.Insert(0, value);
                    this.Labels.Remove(current);
                }
            }
        }
        [SafeForDependencyAnalysis]
        public string SecondaryLabel
        {
            get
            {
                return this.Labels.OrderBy(s => s.Length).First();
            }
            set
            {
                var current = this.SecondaryLabel;
                if (value != current)
                {
                    this.Labels.Add(value);
                    this.Labels.Remove(current);
                }
            }
        }
        [SafeForDependencyAnalysis]
        public string SubLabel
            => this.PrimaryLabel == this.SecondaryLabel 
            ? ""  : this.PrimaryLabel.TrimStart(this.SecondaryLabel).TrimStart(IO.Path.DirectorySeparatorChar);
        [SafeForDependencyAnalysis]
        public bool HasSecondaryLabel
            => this.Labels.Count > 1;
        public bool LabelsAreValid
            => !this.HasSecondaryLabel || this.Label == this.PrimaryLabel;
        [SafeForDependencyAnalysis]
        public string Label
        {
            get
            {
                if (Depends.Guard)
                {
                    Depends.On(this.Labels);
                }
                return this.Labels[0];
            }
            set
            {
                if (Depends.Guard)
                {
                    Depends.On(this.Labels);
                }
                if (this.Labels.Contains(value))
                {
                    this.Labels.Remove(value);
                }
                this.Labels.Insert(0, value);
            }
        }

        public ObservableCollection<string> Labels
        {
            get { return this._labels ?? (this._labels = new ObservableCollection<string>()); }
            set { this._labels = value; }
        }
        #endregion
        #region Public Fields: Path
        public string Path { get; set; }
        public string OriginalPath
        {
            get { return this._originalPath; }
            set {
                this._originalPath = this.Path = value;
                this.Torrent?.SetRootDirectory(value);
            }
        }
        [SafeForDependencyAnalysis]
        public string NewPath
            => Path.Replace(this.SecondaryLabel, this.PrimaryLabel);
        [SafeForDependencyAnalysis]
        public string RelativePath
        {
            get
            {
                return string.IsNullOrEmpty(this.Path) ? this.Path : this.Path
                    .TrimStart(this.RootDirectory)
                    .TrimStart(new char[] { IO.Path.DirectorySeparatorChar })
                    .TrimStart(this.Label)
                    .TrimStart(new char[] { IO.Path.DirectorySeparatorChar })
                    .Replace(this.Caption, "*c*");
            }
            set
            {
                if (!IO.Path.IsPathRooted(value))
                {
                    value = IO.Path.Combine(this.RootDirectory, this.Label, value);
                }
                this.Path = value.Replace("*c*", this.Caption);
            }
        }
        [SafeForDependencyAnalysis]
        public bool DownloadedFileExists
            => CheckDupePath() || (this.Path != this.OriginalPath && CheckDupePath(this.Path));        
        public string RootDirectory { get; set; }
        #endregion
        #region Public Fields: Date/Time
        public DateTime AddedOn { get; set; }
        public UTorrentJobStarted Started { get; set; }
        public DateTime Time { get; set; }
        public DateTime CompletedOn { get; set; }        
        public TimeSpan LastActive { get; set; }
        public TimeSpan RunTime { get; set; }
        public TimeSpan SeedTime { get; set; }
        public long LastSeenComplete { get; set; }
        #endregion
        #region Public Fields: DL/UL
        public long Downloaded { get; set; }
        public long Uploaded { get; set; }        
        public long UploadSpeed { get; set; }
        public long DownloadSpeed { get; set; }
        #endregion
        #region Public Fields: Boolean
        public bool Moved { get; set; }        
        public bool Relative { get; set; }
        public bool Valid { get; set; }
        public bool Visible { get; set; }
        public bool WasForce { get; set; }
        #endregion        
        #endregion
        #region Verify / Check File
        public bool VerifyFile(string name=null,string rootDirectory=null)
            => Torrent.VerifyFile(name, rootDirectory);        
        public double CheckFile(string name = null, string rootDirectory = null)
            => Torrent.CheckFile(name, rootDirectory);
        public bool VerifyFileWithNewPath(string name = null)
            => VerifyFile(name, this.Path);
        public double CheckFileWithNewPath(string name = null)
            => CheckFile(name, this.Path);
        #endregion
        #region Public Constructors
        public UTorrentJob(string name, BDictionary info, int number = 0, TorrentInfo torrentInfo = null) : this()
        {
            Number = number;
            Torrent = torrentInfo;
            this.LoadFromBDictionary(name, info);
        }
        /// <summary>
        /// Automatically creates UTorrent Job from Torrent filename. Intended to be used for testing/design only
        /// </summary>
        /// <param name="path"></param>
        public UTorrentJob(string basePath, params string[] paths) : this(new TorrentInfo(PathUtils.CombineSafe(basePath, paths))) { }
        /// <summary>
        /// Automatically creates UTorrent Job from TorrentInfo instance. Intended to be used for testing/design only
        /// </summary>
        /// <param name="torrentInfo"></param>
        public UTorrentJob(TorrentInfo torrentInfo)
        {
            this.Torrent = torrentInfo;
            this.Number = _counter++;
            this.Name = this.Torrent.Info.GetNameWithoutExtension();
            this.Caption = this.Torrent.Name;
            this.RootDirectory = torrentInfo.RootDirectory;
            var label = this.Torrent.Info.DirectoryName;
            label = (string.IsNullOrEmpty(RootDirectory)
                ? label?.SubstringAfter(@"\$\", true)
                : label.TrimStart(RootDirectory)).TrimStart("\\");
            this.Label = label;
            this.Path = PathUtils.CombineSafe(IO.Path.GetPathRoot(RootDirectory), this.Label, this.Torrent.Name);
        }
        public UTorrentJob()
        {
            this.PropertyChanged += (s, e) =>
            {
                if (!Initialized || _silentAutoChangeNotificationProperties.Contains(e.PropertyName))
                {
                    return;
                }
                var ignored = _ignoredAutoChangeNotificationProperties.Contains(e.PropertyName);
                LogQueueItemChanged((ignored ? "*" : " ") + e.PropertyName);
                if (!ignored)
                {
                    Changed = true;
                }
            };
        }
        #endregion
        #region Processing
        public bool RemoveExtraPrimaryLabel()
        {
            if (Labels.Count == 2 && IsComplete)
            {
                Labels.RemoveAt(0);
                return true;
            }
            return false;
        }
        public bool FixSecondaryLabel()
        {
            if (HasSecondaryLabel && !LabelsAreValid)
            {
                var newPath = NewPath;
                if (IsComplete)
                {
                    Path = NewPath;
                }
                var primary = PrimaryLabel;
                var secondary = SecondaryLabel;
                Labels.Remove(secondary);
                Labels.Remove(primary);
                Labels.Insert(0, secondary);
                Labels.Insert(0, primary);
                return true;
            }
            return false;
        }
        
        public UTorrentJobMoveResult Move()
        {
            if (OriginalPath == Path)
            {
                return new UTorrentJobMoveResult(UTorrentJobMoveStatus.Unneeded);
            }

            string type = "???";
            try
            {
                var targetDirectory = new DirectoryInfo(IO.Path.GetDirectoryName(Path));                
                if ((File.Exists(Path) && !File.Exists(OriginalPath)) || (Directory.Exists(Path) && !Directory.Exists(OriginalPath)))
                {
                    return new UTorrentJobMoveResult(UTorrentJobMoveStatus.AlreadyMoved);
                }
                if (File.Exists(OriginalPath))
                {
                    if (!targetDirectory.Exists)
                    {
                        targetDirectory.Create();
                    }
                    type = "file";
                    File.Move(OriginalPath, Path);
                    return new UTorrentJobMoveResult(UTorrentJobMoveStatus.Success);
                }
                else if (Directory.Exists(OriginalPath))
                {
                    if (!targetDirectory.Exists)
                    {
                        targetDirectory.Create();
                    }
                    type = "directory";
                    Directory.Move(OriginalPath, Path);
                    return new UTorrentJobMoveResult(UTorrentJobMoveStatus.Success);
                }
                return new UTorrentJobMoveResult(UTorrentJobMoveStatus.NotFound);
            }
            catch (FileNotFoundException ex)
            {
                Log("!", $"Couldn't open the downloaded {type}: \r\n" + ex);
                return new UTorrentJobMoveResult(UTorrentJobMoveStatus.NotFoundError, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                Log("!", $"Couldn't open the downloaded {type}: \r\n" + ex);
                return new UTorrentJobMoveResult(UTorrentJobMoveStatus.NotFoundError, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log("!", $"Couldn't access the downloaded {type}: \r\n" + ex);
                return new UTorrentJobMoveResult(UTorrentJobMoveStatus.AccessError, ex);
            }
            catch (Exception ex)
            {
                Log("!", $"Unknown exception occurred while accessing the downloaded {type}: \r\n" + ex);
                Debugger.Break();
                throw;
                //return new UTorrentJobMoveResult(UTorrentJobMoveStatus.UnknownError, ex);
            }
        }
        #endregion
        #region Check Dupe Path
        bool CheckDupePath()
            => CheckDupePath(OriginalPath);
        bool CheckDupePath(string path)
            => CheckDupePath(path, Torrent.Largest);
        bool CheckDupePath(string path, TorrentFileInfo largestFile)
        {
            var fi = FileUtils.GetInfo(path);
            if (fi.Exists && fi.Length == largestFile.Length)
            {
                return true;
            }
            if (Directory.Exists(path))
            {
                fi = FileUtils.GetInfo(IO.Path.Combine(path, largestFile.FullName));
                if (fi.Exists && fi.Length == largestFile.Length)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region BDictionary: Load From / Get Updated
        public void LoadFromBDictionary(string name, BDictionary info)
        {
            this.Name = name;
            this.Info = info;
            foreach (var infoItem in info)
            {
                var key = infoItem.Key.ToString();
                var value = infoItem.Value;
                switch (key)
                {
                    case "caption":
                        Caption = value.ToString();
                        break;
                    case "path":
                        OriginalPath = value.ToString();
                        break;
                    case "label":
                        OriginalLabel = value.ToString();
                        break;
                    case "labels":
                        Labels = value.ToObservableCollection();
                        break;
                    case "trackers":
                        Trackers = value.ToList();
                        break;
                    case "targets":
                        Targets = value.ToArray<UTorrentJobTarget>();
                        break;
                    case "rootdir":
                        RootDirectory = value.ToString();
                        break;
                    case "added_on":
                        AddedOn = value.ToDateTime();
                        break;
                    case "completed_on":
                        _completedOn = (BNumber)value;
                        CompletedOn = value.ToDateTime();
                        break;
                    case "downloaded":
                        Downloaded = (BNumber)value;
                        break;
                    case "last_seen_complete":
                        LastSeenComplete = (BNumber)value;
                        break;
                    case "last_active":
                        LastActive = value.ToTimeSpan();
                        break;
                    case "started":
                        Started = (UTorrentJobStarted)((int)((BNumber)value));
                        break;
                    case "uploaded":
                        Uploaded = (BNumber)value;
                        break;
                    case "time":
                        Time = value.ToDateTime();
                        break;
                    case "upspeed":
                        UploadSpeed = (BNumber)value;
                        break;
                    case "downspeed":
                        DownloadSpeed = (BNumber)value;
                        break;
                    case "moved":
                        Moved = value.ToBoolean();
                        break;
                    case "info":
                        Hash = value.ToHex();
                        break;
                    case "parent_info":
                        ParentHash = value.ToHex();
                        break;
                    case "prio":
                        Priority = value.ToBytes();
                        PriorityLength = Priority.Length;
                        break;
                    case "prio2":
                        Priority2 = (BNumber)value;
                        break;
                    case "relative":
                        Relative = value.ToBoolean();
                        break;
                    case "valid":
                        Valid = value.ToBoolean();
                        break;
                    case "visible":
                        Visible = value.ToBoolean();
                        break;
                    case "wasforce":
                        WasForce = value.ToBoolean();
                        break;
                    case "runtime":
                        RunTime = value.ToTimeSpan();
                        break;
                    case "seedtime":
                        SeedTime = value.ToTimeSpan();
                        break;
                    default:
                        break;
                }
            }
            this.Initialized = true;
            this.Changed = false;
        }

        public BDictionary GetUpdatedBDictionary()
        {
            var info = new BDictionary();
            info.Concat(this.Info);
            var updated = new BDictionary();
            updated["caption"] = new BString(Caption);
            updated["label"] = new BString(Label);
            updated["labels"] = Labels.ToBList();
            updated["trackers"] = Trackers.ToBList();
            updated["path"] = new BString(Path);
            updated["rootdir"] = new BString(RootDirectory);
            if (Targets.Length > 0)
            {
                updated["targets"] = Targets.ToBList();
            }

            var priority = new byte[PriorityLength];
            byte priorityByte = 8;
            for (var i = 0; i < PriorityLength; i++)
            {
                priority[i] = priorityByte;
            }
            updated["priority"] = new BString(priority);
            //updated["added_on"] = AddedOn.ToBNumber();
            //updated["completed_on"] = CompletedOn.ToBNumber();
            //updated["downloaded"] = DownloadedOn.ToBNumber();
            //updated["last_seen_complete"] = LastSeenComplete.ToBNumber();
            //updated["last_active"] = LastActive.ToBNumber();
            //updated["started"] = StartedOn.ToBNumber();
            //updated["uploaded"] = UploadedOn.ToBNumber();
            //updated["time"] = Time.ToBNumber();

            updated["moved"] = Moved.ToBNumber();
            updated["relative"] = Relative.ToBNumber();
            updated["valid"] = Valid.ToBNumber();
            updated["visible"] = Visible.ToBNumber();
            updated["wasforce"] = WasForce.ToBNumber();

            return info;
        }
        #endregion
        #region Interfaces
        #region Interfaces: IDebuggerDisplay
        [DebuggerNonUserCode]
        public override string ToString()
            => DebuggerDisplaySimple();
        public string DebuggerDisplay(int level = 1)
            => $"<{nameof(UTorrentJob)}> {this.DebuggerDisplaySimple(level)}";
        public string DebuggerDisplaySimple(int level = 1)
            => $"{this.Label}: {this.Caption}";
        #endregion
        #endregion
        #region Operators
        public static implicit operator TorrentInfo(UTorrentJob value)
            => value.Torrent;
        #endregion
        #region Logging
        [System.Diagnostics.Conditional("LOG_QUEUE_ITEM_CHANGED"), System.Diagnostics.Conditional("LOG_ALL")]
        public void LogQueueItemChanged([CallerMemberName] string propertyName = null)
            => Log("Δ ", propertyName);
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        public void Log(string prefix = "+", string text = null, PadDirection textPadDirection = PadDirection.Default,
                        string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default,
                        string titleSuffix = null, int random = 0)
        {
#if DEBUG || TRACE_EXT
            var fileNumber = (prefix == "+" ? Number : 0);
            if (fileNumber == 0 || fileNumber % 100 == 1)
            {
                LogUtils.Log(
                             (fileNumber > 0 ? (fileNumber + ".").PadRight(4) + " " : "") + (prefix ?? " ") + this.Started,
                             text, this.SecondaryLabel, this.Caption, textPadDirection, textSuffix,
                             titlePadDirection, titleSuffix, random);
            }
#endif
        }

        #endregion
    }
}