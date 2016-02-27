using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using PostSharp.Patterns.Contracts;

namespace Torrent.Extensions.UI
{
    using System.Diagnostics;
    using Infrastructure.Media;

    public static class BrushExtension
    {
        public static Color GetColor(this Brush brush)
        {
            var solid = brush as SolidColorBrush;
            if (solid != null)
                return solid.Color;
            var gradient = brush as GradientBrush;
            if (gradient == null)
                throw new ArgumentException(
                    $"Could not parse color from Brush Type {brush.GetType().FullName}");
            if (gradient.GradientStops.Count == 0)
                throw new ArgumentException(
                    $"Cannot Get Color from {nameof(GradientBrush)} with zero Gradient Stops");
            return gradient.GradientStops[0].Color;
        }

        /// <summary>
        /// Modify a brush by applying a transformation function to each color.
        /// Currently supports SolidColorBrush and GradientBrush
        /// </summary>
        /// <param name="originalBrush"></param>
        /// <param name="colorTransformer"></param>
        /// <returns></returns>
        public static Brush ModifyColors(this Brush originalBrush, Func<Color, Color> colorTransformer)
        {
            var brush = originalBrush as SolidColorBrush;
            if (brush != null)
                return new SolidColorBrush(colorTransformer(brush.Color));
            var gradient = originalBrush as GradientBrush;
            if (gradient == null)
                throw new ArgumentException(nameof(originalBrush),
                                            $"Only SolidColorBrush and GradientBrush are supported, but a {originalBrush.GetType().Name} was provided.");

            gradient = gradient.Clone();
            // change brightness of every gradient stop
            foreach (var gradientStop in gradient.GradientStops)            
                gradientStop.Color = colorTransformer(gradientStop.Color);            
            return gradient;
        }

        /// <summary>
        /// Adjust Brightness by adding the provided difference
        /// </summary>
        /// <param name="originalBrush"></param>
        /// <param name="difference">Should be between -1 and 1</param>
        /// <returns></returns>
        [DebuggerNonUserCode]
        public static Brush AdjustBrightness(this Brush originalBrush, [Range(-1, 1)] double difference)
            => originalBrush.ModifyColors(color => color.AdjustBrightness(difference));

        /// <summary>
        /// Explicitly set Brightness
        /// </summary>
        /// <param name="originalBrush"></param>
        /// <param name="brightness">Should be between 0 and 1</param>
        /// <returns></returns>
        [DebuggerNonUserCode]
        public static Brush SetBrightness(this Brush originalBrush, [Range(0, 1)] double brightness)
            => originalBrush.ModifyColors(color => color.SetBrightness(brightness));
    }
}
