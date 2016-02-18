#define LOG_QUEUE_ITEM_CHANGED

namespace Torrent
{
    using System;
    public enum UTorrentJobStarted
    {
        NotStarted,
        Started,
        Started2,
        Started3,
        Started4
    }
    public enum UTorrentJobMoveStatus
    {
        Unneeded,
        AlreadyMoved,
        NotFound,
        NotFoundError,
        AccessError,
        UnknownError,
        Success
    }
    public class UTorrentJobMoveResult
    {
        public UTorrentJobMoveStatus Status { get; set; }
        public Exception Error { get; set; }
        public UTorrentJobMoveResult(UTorrentJobMoveStatus status, Exception ex = null)
        {
            this.Status = status;
            this.Error = ex;
        }
    }
}
