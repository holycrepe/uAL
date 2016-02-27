using System.Windows;
using System.Windows.Media;

namespace Torrent.Extensions.UI
{
    public static class StyleExtensions
    {
        public static Style New(this Style style)
             => new Style
             {
                 BasedOn = style,
                 TargetType = style?.TargetType
             };
    }
}