using System.Windows.Media;

namespace Torrent.Extensions.UI
{
    public static class ColorExtensions
    {
        public static Brush ToBrush(this Color color)
            => new SolidColorBrush(color);
    }
}