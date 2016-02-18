namespace Torrent.Helpers.Utils
{
    using Extensions;

    public static class FLAGS
    {
        public static class DUPE_CHECK
        {
            const Flag All = Flag.On;
            const Flag Global = Flag.On;
            const Flag uTorrent = Flag.On;
            const Flag Files = Flag.On;

            public static bool GLOBAL => Global.Value(All);
            public static bool FILES => Files.Value(All);
            public static bool UTORRENT => uTorrent.Value(All);
        }
    }
}
