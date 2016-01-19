using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Threading.Tasks;
using Torrent.Extensions;
using Torrent.Helpers.Utils;
using Torrent.Queue;
using Torrent.Infrastructure;

namespace Torrent
{
    using Enums;
    using Helpers.StringHelpers;
    public class TorrentItem : QueueItemBase
    {
        TorrentLabel _label;
        FileInfo _file;
        TorrentInfo _info;
        bool _valid = true;
        bool _allowUpdatePath = false;
        static readonly string[] pathDeterminingProperties = {"Label", "File", "AllowUpdatePath"};

        public bool AllowUpdatePath { 
        	get { return _allowUpdatePath; }
        	set { if (_allowUpdatePath != value) { _allowUpdatePath = value;  OnPropertyChanged(value, "AllowUpdatePath"); }
        	}
        }
        
        public string RootDirectory => FileName.StartsWith(NewRootDirectory, StringComparison.CurrentCulture) ? NewRootDirectory : OldRootDirectory;

        public string NewRootDirectory { get; }

        public string OldRootDirectory { get; }

        public bool Valid {
        	get {
        		return _valid;
        	}
        	set { if (_valid != value) { _valid = value; OnPropertyChanged(value, "Valid"); }
        	}
        }
        
        public string Torrent
        {
            get
            {
                var name = FileName.Replace(RootDirectory, "");
                if (name.StartsWith("\\") || name.StartsWith("/"))
                {
                    name = name.Substring(1);
                }
                return name;
            }
        }
        public string TorrentName => Path.GetFileNameWithoutExtension(FileName).UnescapeHTML();

        void labelPropertyChanged()
        {
            OnPropertyChanged(Label == null ? null : Label.Computed, "Label");
        }
        void labelPropertyChanged(object sender, PropertyChangedEventArgs e) {
        	OnPropertyChanged(Label.Computed, "Label", "Label." + e.PropertyName);	
        }
        public TorrentLabel Label
        {
            get { return _label; }
            set {
                if (_label == value)
                {
                    return;
                }
                if (_label != null) {
            		_label.PropertyChanged -= labelPropertyChanged;
            	}
            	_label = value;
            	labelPropertyChanged();
            	if (_label != null) {
            		try {
            			_label.PropertyChanged += labelPropertyChanged;
            			return;
            		} catch (NullReferenceException) {            			
            			return;
            		}            		
            	}            	             	
            }
        }
        public string FileName
        {
            get {  
        		return (_file == null ? null : _file.FullName);
        	}
            set { 
            	if (FileName != value) {
            		_file = new FileInfo(value); OnPropertyChanged(value, "FileName", "File");
            	}
            }
        }
        public FileInfo File
        {
            get { return _file; }
            set { if (FileName != value.FullName) { _file = value; OnPropertyChanged(_file == null ? null : _file.FullName, "FileName", "File"); } }
        }
        public TorrentInfo Info
        {
            get {
                return UpdateTorrentInfo();
            }
            set { if (_info != value) { _info = value; OnPropertyChanged(_info, "Info"); } }
        }
        
        public static Func<bool, IEnumerable<string>> getWordsToStrip = null;
        
        string TorrentNameStripped => TorrentName.StripFilename(false, getWordsToStrip);

        public string ComputedFileName => Label == null ? FileName : Path.Combine(RootDirectory, Label.Base, (Label.IsExtendable && Label.UseExtendedName ? Label.Extended : TorrentNameStripped) + ".torrent");

        
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
        void Log(string title, string text = null, string item = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            => LogUtils.Log("TorrentItem", title, text, item, textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);

        public async Task UpdatePath() {
        	if (AllowUpdatePath && FileName != ComputedFileName && File.Exists) {
        		var result = await FileSystemUtils.MoveFile(File, ComputedFileName);
        		if (result.Status.IsDupe()) {        			
                    // File is an exact dupe and has been deleted
        			Log("UpdatePath: Dupe", Label.Base, TorrentName);
        			Valid = false;
        		}
        		else if (result.Status == FileSystemUtils.MoveFileResultStatus.Success) {        			         			
        			Log("UpdatePath: Moved", Label.Base, TorrentName);
        			FileName = result.NewFileName;        			
        			Log("", "→", TorrentName, PadDirection.Alternate, " ");
        		}
        		else {
        			Log("UpdatePath: Error", Label.Base, TorrentName);
        		}
        	}
        }
        
        public TorrentItem(string oldRootDirectory, string newRootDirectory, string fileName, TorrentLabel label = null) 
        {            
        	this.OldRootDirectory = oldRootDirectory;
            this.NewRootDirectory = newRootDirectory;
            FileName = fileName;
            Label = label;
            QueueItem.SetLabel(() => Label.Base);
            QueueItem.SetName(() => TorrentName);        
        }

        public TorrentInfo UpdateTorrentInfo()
        {
            if (_info == null || _info.FileName != FileName)
            {
                _info = TorrentInfoCache.GetTorrentInfo(File);
            }
            return _info;
        }

        #pragma warning disable 4014
        protected override void OnPropertyChanged(object propertyValue, params string[] propertyNames)
        {
        	if (pathDeterminingProperties.ContainsAny(propertyNames)) {        		
        		UpdatePath();        		
        	}

            base.OnPropertyChanged(propertyValue, propertyNames);
        }
		#pragma warning restore 4014
        
    }
}
