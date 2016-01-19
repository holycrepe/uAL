namespace Torrent.Queue
{

    public class QueueStatus
	{
		public static readonly QueueStatusMember Uninitialized = new QueueStatusMember("Uninitialized", -1);
		public static readonly QueueStatusMember LoadError = new QueueStatusMember("Load Error", 1);
		public static readonly QueueStatusMember TorrentInfoError = new QueueStatusMember("Error: Invalid Torrent Info", LoadError, -1 + 10);
		public static readonly QueueStatusMember TorrentBDecodeError = new QueueStatusMember("TorrentBDecodeError", "Error: Unable to Decode Torrent", TorrentInfoError);
		public static readonly QueueStatusMember Invalid = new QueueStatusMember("Invalid", "Invalid Queue Item", TorrentInfoError, 10);		
		public static readonly QueueStatusMember Dupe = new QueueStatusMember("Dupe", "Dupe: Downloaded", Invalid, 10);
		public static readonly QueueStatusMember QueueDupe = new QueueStatusMember("QueueDupe", "Dupe: Queued", Dupe);
		public static readonly QueueStatusMember Ready = new QueueStatusMember("Ready", Dupe, 10);
		public static readonly QueueStatusMember Active = new QueueStatusMember("Active", Ready);
		public static readonly QueueStatusMember Success = new QueueStatusMember("Success", Ready, 20);
		public static readonly QueueStatusMember Queued = new QueueStatusMember("Queued", Success, 20);		
		static QueueStatus() {
			
		}
	}
}


