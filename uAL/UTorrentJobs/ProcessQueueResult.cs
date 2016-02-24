namespace uAL.UTorrentJobs
{
    public struct ProcessQueueResult
    {
        public ProcessQueueStatus Status { get; }
        public ProcessQueueResult(ProcessQueueStatus status) { this.Status = status; }
    }
}