namespace Torrent.Enums
{
}

namespace Torrent.Helpers.StringHelpers
{
    using Enums;

    public class PadStringOptions
    {
        public const int DEFAULT_WIDTH = 25;
        protected const PadDirection DEFAULT_DIRECTION = PadDirection.Left;
        protected const char DEFAULT_PADDING_CHAR = ' ';
        protected virtual PadDirection DefaultDirection => DEFAULT_DIRECTION;

        protected PadDirection AlternateDirection
            => DefaultDirection.GetAlternateDirection();

        public object Subject { get; set; }
        public int Width { get; set; } = 0;
        public char PaddingChar { get; set; } = DEFAULT_PADDING_CHAR;
        public bool UseValue { get; set; } = true;
        public bool DoPad { get; set; } = true;
        public PadDirection Direction { get; set; } = PadDirection.Default;

        public bool DoPadCenter
            => Direction == PadDirection.Center;

        public bool DoPadRight
            => Direction.DoRight(DefaultDirection);
    }
}
