using System.Windows.Media;

namespace uAL.Helpers.Utils
{
    public static class ColorUtils
    {
        public static Brush HexToBrush(string hex)
            => new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
    }
}
