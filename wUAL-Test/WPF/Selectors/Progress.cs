using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Torrent.Queue;
using uAL.Extensions;

namespace wUAL.WPF.Selectors
{
    using Torrent;
    using wUAL.WPF.Styles;

    public static partial class Selectors
    {
        public static class Progress
        {
            public static string Key = "wUAL_ProgressBar";

            public static string[] Names = new[]
            {
                "Default",
                "Active",
                "Resuming",
                "Pending",
                "Partial"
            };

            public static SelectorBase<UTorrentJob> Selector = new SelectorBase<UTorrentJob>(
                (UTorrentJob job)
                    => new object[]
                    {
                        "Active", job.Torrent.IsActive,
                        "Resuming", job.Torrent.IsRunning && job.Torrent.PercentComplete > 0,
                        "Pending", job.Torrent.IsRunning,
                        "Partial", job.Torrent.PercentComplete > 0,
                        "Default"
                    }
                );
            [DebuggerNonUserCode]
            public static ProgressStyleSelector Style { get; }
                = new ProgressStyleSelector();
            [DebuggerNonUserCode]
            public static ProgressLabelStyleSelector Label { get; }
                = new ProgressLabelStyleSelector();
        }
    }
}