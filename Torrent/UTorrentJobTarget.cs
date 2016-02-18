
namespace Torrent
{
    using BencodeNET.Objects;
    using PostSharp.Patterns.Model;
    using Infrastructure;
    using Infrastructure.Interfaces;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [NotifyPropertyChanged]
    public class UTorrentJobTarget : IBListLoadable, IDebuggerDisplay
    {
        public int Number { get; set; }
        public string Path { get; set; }
        //public UTorrentJobTarget(IBObject obj) : this() { LoadFromIBObject(obj); }
        public UTorrentJobTarget() { }
        public UTorrentJobTarget(BList list) { LoadFromBList(list); }

        public UTorrentJobTarget(int number, string path)
        {
            this.Number = number;
            this.Path = path;
        }

        //public void LoadFromIBObject(IBObject obj)
        //    => LoadFromBList((BList)obj);
        public void LoadFromBList(BList list)
        {
            this.Number = ((BNumber)list[0]) + 1;
            this.Path = list[1].ToString();
        }

        public BList ToBList()
        {
            var targets = new BList();
            targets.Add(Number - 1);
            targets.Add(Path);
            return targets;
        }
        #region Interfaces
        #region Interfaces: IDebuggerDisplay
        public string DebuggerDisplay(int level = 1)
            =>
                $"<{nameof(UTorrentJobTarget)}> {DebuggerDisplaySimple(level)}";
        public string DebuggerDisplaySimple(int level = 1)
            => $"#{Number}: {Path}";
        #endregion
        #endregion
    }    
}
