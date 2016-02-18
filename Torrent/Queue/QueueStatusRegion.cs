namespace Torrent.Queue
{
    public struct QueueStatusRegion
    {
        public readonly QueueStatusMember Start;
        public readonly QueueStatusMember End;

        public QueueStatusRegion(QueueStatusMember start, QueueStatusMember end)
        {
            Start = start;
            End = end;
        }
    }
}
