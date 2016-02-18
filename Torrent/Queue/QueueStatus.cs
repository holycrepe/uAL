namespace Torrent.Queue
{
    public class QueueStatus
    {
        public static readonly QueueStatusMember Uninitialized = new QueueStatusMember(nameof(Uninitialized), -1);
        public static readonly QueueStatusMember Inactive = new QueueStatusMember(nameof(Inactive), Uninitialized);
        public static readonly QueueStatusMember LoadError = new QueueStatusMember(nameof(LoadError), "Load Error", 1);

        public static readonly QueueStatusMember TorrentInfoError = new QueueStatusMember(nameof(TorrentInfoError),
            "Error: Invalid Torrent Info", LoadError, -1 + 10);

        public static readonly QueueStatusMember TorrentBDecodeError = new QueueStatusMember(nameof(TorrentBDecodeError),
                                                                                             "Error: Unable to Decode Torrent",
                                                                                             TorrentInfoError);

        public static readonly QueueStatusMember Invalid = new QueueStatusMember(nameof(Invalid), "Invalid Queue Item",
                                                                                 TorrentInfoError, 10);

        public static readonly QueueStatusMember Dupe = new QueueStatusMember(nameof(Dupe), "Dupe: Downloaded", Invalid, 10);
        public static readonly QueueStatusMember QueueDupe = new QueueStatusMember(nameof(QueueDupe), "Dupe: Queued", Dupe);
        public static readonly QueueStatusMember Pending = new QueueStatusMember(nameof(Pending), Dupe, 10);
        public static readonly QueueStatusMember Ready = new QueueStatusMember(nameof(Ready), Pending);
        public static readonly QueueStatusMember Active = new QueueStatusMember(nameof(Active), Ready);
        public static readonly QueueStatusMember Incomplete = new QueueStatusMember(nameof(Incomplete), Active, 10);
        public static readonly QueueStatusMember Success = new QueueStatusMember(nameof(Success), Incomplete, 10);

        public static readonly QueueStatusMember Queued = new QueueStatusMember(nameof(Queued), Success, 20);
    }
}
