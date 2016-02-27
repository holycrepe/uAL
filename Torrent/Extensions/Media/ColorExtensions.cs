using System.Windows.Media;
using PostSharp.Patterns.Contracts;

namespace Torrent.Extensions.UI
{
    using System.Diagnostics;
    using Infrastructure.Media;

    public static class ColorExtensions
    {
        [DebuggerNonUserCode]
        public static HSBColor ToHSBColor(this Color color)
            => HSBColor.FromColor(color);

        [DebuggerNonUserCode]
        public static Brush ToBrush(this Color color)
            => new SolidColorBrush(color);

        /// <summary>
        /// Explicitly set Brightness
        /// </summary>
        /// <param name="color"></param>
        /// <param name="brightness">Should be between 0 and 1</param>
        /// <returns></returns>
        [DebuggerNonUserCode]
        public static Color SetBrightness(this Color color, [Range(0, 1)] double brightness)
            => HSBColor.FromColor(color).SetBrightness(brightness).ToColor();

        /// <summary>
        /// Adjust Brightness by adding the provided difference
        /// </summary>
        /// <param name="color"></param>
        /// <param name="difference">Should be between -1 and 1</param>
        /// <returns></returns>
        [DebuggerNonUserCode]
        public static Color AdjustBrightness(this Color color, [Range(-1, 1)] double difference)
            => HSBColor.FromColor(color).AdjustBrightness(difference).ToColor();
    }
}