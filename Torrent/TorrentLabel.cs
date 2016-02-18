namespace Torrent
{
    using System.IO;
    using System.ComponentModel;
    using Helpers.Utils;
    using Infrastructure;
    using PostSharp.Patterns.Model;
    using System.Diagnostics;
    [NotifyPropertyChanged]
    public class TorrentLabel : NotifyPropertyChangedBase
    {
        string _base;
        public string Full { get; set; }

        public string Base {
            get { return this._base; }
            set {
                if (this._base != value)
                {
                    if (value == null && this._base != null)
                    {
                        return;
                    }
                    this._base = value;
                    this.OnPropertyChanged(nameof(Base));
                }
            }
        }
        public string Extended { get; set; }

        public string Name { get; set; }
        public string FailedWord { get; set; }

        public string NameWithoutExtension =>
            this.Name == null ? null : Path.GetFileNameWithoutExtension(this.Name);

        public string LargestName { get; set; }

        public bool UseExtendedName { get; set; }

        public string Computed
            => this.Base + (this.IsComputed ?  "\\" + this.Extended : "");

        public bool IsComputed
            => this.UseExtendedName && !string.IsNullOrEmpty(this.Extended);

        public bool IsExtendable
            => this.FailedWord != null;

        public bool IsExtended
            => this.IsExtendable && this.Extended != null;

        public string IsExtendedText
            => this.IsExtended ? "Extended Labels" : "Unextended Labels";

        public TorrentLabel(string baseDirectory, string fullPath)
        {
            Base = PathUtils.MakeRelative(baseDirectory, fullPath);
            Full = fullPath;
        }

        public TorrentLabel(string baseLabel) { Base = baseLabel; }
    }
}
