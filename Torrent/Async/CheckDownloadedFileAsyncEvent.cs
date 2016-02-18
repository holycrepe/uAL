using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torrent.Async
{
    using Infrastructure.EAP;
    public class CheckDownloadedFileUserState : CheckDownloadedFile.AsyncUserState
    {
        public TorrentFileInfo File;
        public CheckDownloadedFileUserState(TorrentInfo item) : this(item, item.Largest) { }
        public CheckDownloadedFileUserState(TorrentInfo item, TorrentFileInfo file) : base(item)
        {
            this.File = file;
        }
    }
    public partial class CheckDownloadedFile
    {
        public class CheckDownloadedFileEvents : AsyncEvents
        {

            //public new delegate void CompletedHandler(CheckDownloadedFile sender,
            //CompletedArgs e);

            //public new class CompletedArgs : AsyncEvent.CompletedArgs
            //{
            //    public CompletedArgs(bool result, CheckDownloadedFileUserState userState) : base(result, userState) { }
            //}
        }
    }

}
