using Torrent.Queue;

namespace uAL.UTorrentJobs
{    
    public class UTorrentJobQueueStatus : QueueStatus
    {
        public static readonly QueueStatusMember NoLabel = new QueueStatusMember("NoLabel", "Error: Blank Label",
                                                                                 TorrentInfoError, 10);

        public static new readonly QueueStatusMember LoadError = new QueueStatusMember("Error Opening Torrent File",
                                                                                       QueueStatus.LoadError.Value);

        public static readonly QueueStatusMember TorrentDupe = new QueueStatusMember("TorrentDupe", "Dupe: Added",
                                                                                     QueueDupe);

        public static readonly QueueStatusMember Disabled = new QueueStatusMember("Disabled", Queued);
    }
}
