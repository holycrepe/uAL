using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace uAL.Extensions
{
    using Torrent.Helpers.Utils;
    public static class SetterBaseCollectionExtensions
    {
        public static void AddColor(this SetterBaseCollection setters, string hex, DependencyProperty property)
        {
            if (string.IsNullOrEmpty(hex))
                return;
            var brush = hex.StartsWith("#") 
                ? ColorUtils.HexToBrush(hex)
                : ResourceUtils.Get<Brush>(hex);
            if (brush == null)
            {
                Debugger.Break();
                return;
            }
            setters.Add(new Setter(property, brush));
        }
    }
    public static class StyleExtensions
    {
        [DebuggerNonUserCode]
        public static void AddForeground(this Style style, string hex)
            => style.AddColor(hex, Control.ForegroundProperty);
        [DebuggerNonUserCode]
        public static void AddBackground(this Style style, string hex)
            => style.AddColor(hex, Control.BackgroundProperty);
        [DebuggerNonUserCode]
        public static void AddColor(this Style style, string hex, DependencyProperty property)
            => style.Setters.AddColor(hex, property);

        [DebuggerNonUserCode]
        public static Style AddForeground(this Style style, Brush brush)
        => style.AddSetter(Control.ForegroundProperty, brush);
        [DebuggerNonUserCode]
        public static Style AddBackground(this Style style, Brush brush)
        => style.AddSetter(Control.BackgroundProperty, brush);
        [DebuggerNonUserCode]
        public static Style AddSetter(this Style style, DependencyProperty property, object value)
        {
            style.Setters.Add(new Setter(property, value));
            return style;
        }
    }
}
