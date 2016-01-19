using Torrent.Helpers.Utils;
// ReSharper disable InconsistentNaming

namespace Torrent.Properties.Settings.MySettings
{
    using Enums;

    public static partial class MySettings
    {
        #region Subclasses

        public static class METHODS
        {
            public static GetFileMethod GET_FILES => MySetting.METHODS.GET_FILES;
            public static ProcessQueueMethod PROCESS_QUEUE => MySetting.METHODS.PROCESS_QUEUE;
        }

        #endregion
    }
}