using System.Windows.Media;
using Torrent.Extensions.UI;

namespace Torrent.Helpers.Utils
{
    public static class ColorUtils
    {
        public static Color HexToColor(string hex)
            => (Color)ColorConverter.ConvertFromString(hex);
        public static Brush HexToBrush(string hex)
            => HexToColor(hex).ToBrush();

        public static RadialGradientBrush CreateRadialGradient(SolidColorBrush brush, double radius = 0.9)
        {
            var gradient = new RadialGradientBrush();
            gradient.RadiusX = gradient.RadiusY = radius;
            gradient.GradientStops.Add(new GradientStop(brush.Color, 0));
            gradient.GradientStops.Add(new GradientStop(Colors.Black, 0.999));
            return gradient;
        }
    }
}
