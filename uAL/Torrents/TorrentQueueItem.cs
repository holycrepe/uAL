using System.IO;
using Torrent.Helpers.Utils;
using Torrent;
using Torrent.Queue;

namespace uAL.Torrents
{
    using Torrent.Enums;
    using Torrent.Helpers.StringHelpers;

    public class TorrentQueueItem : TorrentItem
    {        
        FileInfo _dupe;
        public int Number { get { return QueueItem.Number; } private set { QueueItem.Number = value; } }
        
        public FileInfo Dupe
        {
            get { return _dupe; }
            set { if (_dupe != value) { _dupe = value; OnPropertyChanged(_dupe == null ? null : _dupe.FullName, "Dupe"); } }
        }
        
        public QueueStatusMember Status
        {
            get { return QueueItem.Status; }
            set { value = value ?? QueueStatus.Uninitialized;  if (QueueItem.Status != value) { QueueItem.Status = value; OnPropertyChanged(value, "Status"); } }
        }
        public TorrentQueueItem(string oldRootDirectory, string newRootDirectory, string fileName, int fileNumber) 
        	: this(oldRootDirectory, newRootDirectory, fileName, label: null, fileNumber: fileNumber) { }
        public TorrentQueueItem(string oldRootDirectory, string newRootDirectory, string fileName, TorrentLabel label = null, QueueStatusMember status = null, int fileNumber = 0) : base(oldRootDirectory, newRootDirectory, fileName, label)
        {            
            Status = status;
            Number = fileNumber;
        }
		[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
        public void Log(string prefix = "+", string text = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0) { 
            #if DEBUG || TRACE
            var fileNumber = (prefix == "+" ? Number : 0);
        	if (fileNumber == 0 || fileNumber % 100 == 1) {
        		LogUtils.Log((fileNumber > 0 ? (fileNumber + ".").PadRight(4) + " " : "") + (prefix ?? " ") + Status, Label.Base, text, TorrentName, textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);
        	}
        	#endif
		}
    }
    
}
