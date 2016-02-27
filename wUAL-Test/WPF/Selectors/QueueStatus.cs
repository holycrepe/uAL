using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Torrent.Queue;
using uAL.Extensions;

namespace wUAL.WPF.Selectors
{

    public static partial class Selectors
    {
        public static SelectorBase<QueueStatusMember> QueueStatusSelector = new SelectorBase<QueueStatusMember>(
            (QueueStatusMember status)
                => new object[]
                {
                        status.Name, true,
                        status.Title, true,
                        "TorrentError", status.IsTorrentError,
                        "LoadError", status.IsLoadError,
                        "Error", status.IsError,
                        "InProgress", status.IsInProgress,
                        "Dupe", status.IsDupe,
                        "Invalid", status.IsInvalid,
                        "Activatable", status.IsActivatable,
                        "Queued", status.IsQueued,
                        "Success", status.IsSuccess
                }
            );
    }
}