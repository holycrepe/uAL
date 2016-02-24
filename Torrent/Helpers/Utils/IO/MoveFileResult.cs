namespace Torrent.Helpers.Utils.IO
{
    public struct MoveFileResult
    {
        public MoveFileResultStatus Status;
        public string NewFileName;
        public bool NewlyAdded;

        public MoveFileResult(string newFileName, bool newlyAdded = false,
            MoveFileResultStatus status = MoveFileResultStatus.InProgress)
        {
            this.Status = status;
            this.NewFileName = newFileName;
            this.NewlyAdded = newlyAdded;
        }
    }
}