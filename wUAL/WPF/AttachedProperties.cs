using System.Windows;

namespace wUAL
{

    public class IconExtension
    {
        public static DependencyProperty IconWidthProperty =
            DependencyProperty.RegisterAttached("IconWidth",
                                                typeof(int),
                                                typeof(IconExtension),
                                                new PropertyMetadata(32));
        public static int GetIconWidth(DependencyObject target)
        {
            return (int)target.GetValue(IconWidthProperty);
        }
        public static void SetIconWidth(DependencyObject target, int value)
        {
            target.SetValue(IconWidthProperty, value);
        }

        public static DependencyProperty IconMarginProperty =
            DependencyProperty.RegisterAttached("IconMargin",
                                                typeof(string),
                                                typeof(IconExtension),
                                                new PropertyMetadata("0,0,0,0"));
        public static string GetIconMargin(DependencyObject target)
        {
            string text = (string)target.GetValue(IconTextProperty);
            return (text == "" ? "0,0,0,0" : "0,0,5,0");
        }

        public static DependencyProperty IconTextProperty =
            DependencyProperty.RegisterAttached("IconText",
                                                typeof(string),
                                                typeof(IconExtension),
                                                new PropertyMetadata(""));
        public static string GetIconText(DependencyObject target)
        {
            return (string)target.GetValue(IconTextProperty);
        }
        public static void SetIconText(DependencyObject target, string value)
        {
            target.SetValue(IconTextProperty, value);
        }
    }

}
