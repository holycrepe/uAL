using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Threading.Tasks;
using Torrent.Extensions;
using Torrent.Helpers.Utils;
using Torrent.Helpers.Utils.IO;
using Torrent.Queue;
using Torrent.Infrastructure;

namespace Torrent
{
    using Enums;
    using Helpers.StringHelpers;
    using PostSharp.Patterns.Model;
    using System.Linq;

    [NotifyPropertyChanged]
    public class TorrentItem : QueueItemBase
    {
        TorrentLabel _label;
        //FileInfo _file;
        TorrentInfo _info;
        //bool _valid = true;
        //bool _allowUpdatePath = false;
        static readonly string[] pathDeterminingProperties = {nameof(Label), nameof(File), nameof(AllowUpdatePath)};

        public bool AllowUpdatePath { get; set; }

        [SafeForDependencyAnalysis]
        public string RootDirectory
            => this.FileName.StartsWith(this.NewRootDirectory, StringComparison.CurrentCulture)
                   ? this.NewRootDirectory
                   : this.OldRootDirectory;

        public string NewRootDirectory { get; }

        public string OldRootDirectory { get; }

        public bool Valid { get; set; } = true;

        [SafeForDependencyAnalysis]
        public string Torrent
        {
            [Pure]
            get
            {
                if (Depends.Guard) {
                    Depends.On(this.FileName, this.RootDirectory);
                }
                var name = this.FileName.Replace(this.RootDirectory, "");
                if (name.StartsWith("\\") || name.StartsWith("/")) {
                    name = name.Substring(1);
                }
                return name;
            }
        }

        public string TorrentName
        {
            get { return Path.GetFileNameWithoutExtension(this.FileName); }
            set { this.FileName = value; }
        }

        protected void OnLabelChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this._label.Base)) {
                this._queueItem.OnPropertyChanged(nameof(this._queueItem.Label));
            }
        }

        public TorrentLabel Label
        {
            get { return this._label; }
            set
            {
                if (this._label != null) {
                    this._label.PropertyChanged -= OnLabelChanged;
                }
                this._label = value;
                if (this._label != null) {
                    this._label.PropertyChanged += OnLabelChanged;
                }
            }
        }

        [SafeForDependencyAnalysis]
        public string FileName
        {
            get
            {
                if (Depends.Guard) {
                    Depends.On(this.File);
                }
                return (this.File == null ? null : this.File.FullName);
            }
            set
            {
                if (Depends.Guard) {
                    Depends.On(this.File);
                }
                if (!Path.HasExtension(value)) {
                    value += ".torrent";
                }
                if (!Path.IsPathRooted(value)) {
                    value = Path.Combine(this.File.DirectoryName, value);
                }
                if (this.FileName != value) {
                    this.File = FileUtils.GetInfo(value);
                }
            }
        }

        public FileInfo File { get; set; }

        [SafeForDependencyAnalysis]
        public TorrentInfo Info
        {
            get
            {
                if (Depends.Guard) {
                    Depends.On(this.FileName);
                }
                return this.UpdateTorrentInfo();
            }

            set
            {
                if (Depends.Guard) {
                    Depends.On(this.FileName);
                }
                this._info = value;
            }
        }

        public static Func<bool, IEnumerable<string>> getWordsToStrip = null;

        [SafeForDependencyAnalysis]
        string TorrentNameStripped
            => this.TorrentName.StripFilename(false, getWordsToStrip);

        [SafeForDependencyAnalysis]
        public string ComputedFileName
            => this._label == null
                   ? CleanedFileName
                   : Path.Combine(this.RootDirectory, this._label.Base,
                                  (this._label.IsExtendable && this._label.UseExtendedName
                                       ? this._label.Extended
                                       : this.TorrentNameStripped) + ".torrent");

        [SafeForDependencyAnalysis]
        public string CleanedFileName
            => Path.Combine(this.File.DirectoryName, this.TorrentName.UnescapeHTML()) + ".torrent";

        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        void Log(string title, string text = null, string item = null,
                 PadDirection textPadDirection = PadDirection.Default, string textSuffix = null,
                 PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            =>
                LogUtils.Log("TorrentItem", title, text, item, textPadDirection, textSuffix, titlePadDirection,
                             titleSuffix, random);

        public Task InitializePath()
            => UpdatePath(true);

        protected string Base
            => this._label?.Base ?? Path.GetDirectoryName(this.CleanedFileName)
                                        ?.Replace(this.OldRootDirectory, "")
                                        .Replace(this.NewRootDirectory, "").Trim('\\');

        public async Task UpdatePath(bool force = false)
        {
            string fileName = force ? CleanedFileName : ComputedFileName;
            if ((AllowUpdatePath || force) && FileName != fileName && File.Exists) {
                var result = await FileSystemUtils.MoveFile(File, fileName);
                if (result.Status.IsDupe()) {
                    // File is an exact dupe and has been deleted
                    Log("UpdatePath: Dupe", this.Base, TorrentName);
                    Valid = false;
                } else if (result.Status == MoveFileResultStatus.Success) {
                    Log("UpdatePath: Moved", this.Base, TorrentName);
                    FileName = result.NewFileName;
                    Log("", "→", TorrentName, PadDirection.Alternate, " ");
                } else {
                    Log("UpdatePath: Error", this.Base, TorrentName);
                }
            }
        }

        public TorrentItem(string oldRootDirectory, string newRootDirectory, string fileName, TorrentLabel label = null)
        {
            this.OldRootDirectory = oldRootDirectory;
            this.NewRootDirectory = newRootDirectory;
            FileName = fileName;
            Label = label;
            this._queueItem.SetLabel(() => this.Base);
            this._queueItem.SetName(() => this.TorrentName);
            this.PropertyChanged += (s, e) =>
                                    {
                                        if (e.PropertyName == nameof(TorrentName)) {
                                            this._queueItem.OnPropertyChanged(nameof(this._queueItem.Name));
                                        }
                                        if (AllowUpdatePath && pathDeterminingProperties.Contains(e.PropertyName)) {
#pragma warning disable 4014
                                            UpdatePath();
#pragma warning restore 4014
                                        }
                                    };
        }

        [Pure]
        public TorrentInfo UpdateTorrentInfo()
        {
            if (Depends.Guard) {
                Depends.On(this.FileName, this.File);
            }
            if (_info == null || _info.FileName != this.FileName) {
                _info = TorrentInfoCache.GetTorrentInfo(this.File);
            }
            return _info;
        }

        //      #pragma warning disable 4014
        //      protected override void OnPropertyChanged(object propertyValue, params string[] propertyNames)
        //      {
        //      	if (pathDeterminingProperties.ContainsAny(propertyNames)) {        		
        //      		UpdatePath();        		
        //      	}

        //          base.OnPropertyChanged(propertyValue, propertyNames);
        //      }
        //#pragma warning restore 4014
    }
}
