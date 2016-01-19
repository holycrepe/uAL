using Torrent.Queue;

namespace uAL.Metalinks
{
    public class MetalinkQueueStatus : QueueStatus
    {        				
    	public static new readonly QueueStatusMember LoadError = new QueueStatusMember("Download Error", QueueStatus.LoadError.Value);
    	public static new readonly QueueStatusMember Active = new QueueStatusMember("Active", QueueStatus.Active.Value);
    }    
}
