

namespace Torrent
{
    using System.IO;
    using System.ComponentModel;
    using Helpers.Utils;
    using Infrastructure;

    public class TorrentLabel : NotifyPropertyChangedBase
    {

        string _base;
        string _extended;
        string _full;
        string _name;
        string _largestName;
        string _failedWord;
        bool _useExtendedName = false;

        public string Full {
            get { return _full; }
            set { if (_full != value) { _full = value; OnPropertyChanged(nameof(Full)); } }
        }
        
        public string Base
        {
            get { return _base; }
            set { if (_base != value) { _base = value; OnPropertyChanged(nameof(Base)); } }
        }
        public string Extended
        {
            get { return _extended; }
            set { if (_extended != value) { _extended = value; OnPropertyChanged(nameof(Extended)); } }
        }

        public string Name
        {
            get { return _name; }
            set { if (_name != value) { _name = value; OnPropertyChanged(nameof(Name)); }  }
        }
        public string FailedWord
        {
            get { return _failedWord; }
            set { if (_failedWord != value) { _failedWord = value; OnPropertyChanged(nameof(FailedWord)); OnPropertyChanged(nameof(IsExtendable)); } }
        }

        public string NameWithoutExtension => Name == null ? null : Path.GetFileNameWithoutExtension(Name);

        public string LargestName
        {
            get { return _largestName; }
            set { if (_largestName != value) { _largestName = value; OnPropertyChanged(nameof(LargestName)); } }
        }

        public bool UseExtendedName
        {
            get { return _useExtendedName; }
            set { if (_useExtendedName != value) { _useExtendedName = value; OnPropertyChanged(nameof(UseExtendedName)); } }
        }

        public string Computed => Base + (!UseExtendedName || Extended == null ? "" : "\\" + Extended);

        public bool IsExtendable => FailedWord != null;
        public bool IsExtended => IsExtendable && Extended != null;
        public string IsExtendedText => IsExtended ? "Extended Labels" : "Unextended Labels";

        public TorrentLabel(string baseDirectory, string fullPath) {
        	Base = PathUtils.MakeRelativePath(baseDirectory, fullPath);
        	Full = fullPath;
        }
        
        public TorrentLabel(string baseLabel)
        {
            Base = baseLabel;
        }
    }

}
