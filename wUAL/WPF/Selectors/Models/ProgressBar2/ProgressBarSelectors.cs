using System;
using Torrent;

namespace wUAL.WPF.Selectors.Models.ProgressBar2
{
    public class ProgressBarSelectors : ProgressBarStates<Func<UTorrentJob, bool>>
    {
        public override Func<UTorrentJob, bool> Active
            => job => job.Torrent.IsActive;
        public override Func<UTorrentJob, bool> Resuming
            => job => job.Torrent.IsRunning && job.Torrent.PercentComplete > 0;
        public override Func<UTorrentJob, bool> Pending
            => job => job.Torrent.IsRunning;
        public override Func<UTorrentJob, bool> Partial
            => job => job.Torrent.PercentComplete > 0;
        public override Func<UTorrentJob, bool> Default
            => job => true;
    }


}