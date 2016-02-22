namespace Torrent.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Properties.Settings.MySettings;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class Extensions
    {
        const PadDirection BASE_PAD_DIRECTION = PadDirection.Right;
        const PadDirection ALTERNATE_PAD_DIRECTION = PadDirection.Left;

        public static GetFileMethod Value(this GetFileMethod method) => method == GetFileMethod.Default ? MySettings.METHODS.GET_FILES : method;

        public static ProcessQueueMethod Value(this ProcessQueueMethod method) => method == ProcessQueueMethod.Default ? MySettings.METHODS.PROCESS_QUEUE : method;

        public static PadDirection GetAlternateDirection(this PadDirection direction)
            => direction == ALTERNATE_PAD_DIRECTION ? BASE_PAD_DIRECTION : ALTERNATE_PAD_DIRECTION;

        public static PadDirection Value(this PadDirection direction, PadDirection defaultDirection = BASE_PAD_DIRECTION)
            => direction == PadDirection.Default
                   ? defaultDirection
                   : direction == PadDirection.Alternate ? defaultDirection.GetAlternateDirection() : direction;

        public static bool DoRight(this PadDirection direction, PadDirection defaultDirection = BASE_PAD_DIRECTION)
            => direction.Value(defaultDirection).IsRight();

        public static bool DoLeft(this PadDirection direction, PadDirection defaultDirection = BASE_PAD_DIRECTION)
            => direction.Value(defaultDirection).IsLeft();

        public static bool IsRight(this PadDirection direction)
            => direction == PadDirection.Right;

        public static bool IsLeft(this PadDirection direction)
            => direction == PadDirection.Left;
    }
}
