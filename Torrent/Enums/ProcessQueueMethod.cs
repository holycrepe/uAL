using System;
using System.ComponentModel;

namespace Torrent.Enums
{
    [Serializable]
    public enum ProcessQueueMethod
    {
        [Description("Use Default Method")]
        Default,
        [Description("Synchronous")]
        Plain,
        [Description("Parallel.ForEach")]
        Parallel,
        [Description("PLINQ.ForAll")]
        PLINQ
    }
}
