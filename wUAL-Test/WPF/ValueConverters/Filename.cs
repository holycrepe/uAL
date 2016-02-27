using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;


namespace wUAL
{
    using static uAL.Properties.Settings.LibSettings.LibSettings;

    public class FilenameConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty RootDirectoryProperty =
            DependencyProperty.Register("RootDirectory",
                                        typeof (string),
                                        typeof (FilenameConverter),
                                        new PropertyMetadata(LibSetting.Directories.ACTIVE));

        public string RootDirectory
        {
            get { return (string) this.GetValue(RootDirectoryProperty); }
            set { this.SetValue(RootDirectoryProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("fcLabel",
                                        typeof (string),
                                        typeof (FilenameConverter),
                                        new PropertyMetadata(null));

        public string fcLabel
        {
            get { return (string) this.GetValue(LabelProperty); }
            set { this.SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty ExtensionProperty =
            DependencyProperty.Register("Extension",
                                        typeof (string),
                                        typeof (FilenameConverter),
                                        new PropertyMetadata(null));

        public string Extension
        {
            get { return (string) this.GetValue(ExtensionProperty); }
            set { this.SetValue(ExtensionProperty, value); }
        }

        public bool FilenameOnly { get; set; }

        public FilenameConverter() { FilenameOnly = false; }

        string label
        {
            get
            {
                if (fcLabel == null) {
                    return null;
                }
                return fcLabel.Replace(": ", "\\");
            }
        }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var filename = (string) value;
            var rootDirectory = RootDirectory ?? LibSetting.Directories.ACTIVE;
            filename = filename.Replace(rootDirectory, "");
            if (filename.StartsWith("\\") || filename.StartsWith("/")) {
                filename = filename.Substring(1);
            }
            if (label != null && filename.StartsWith(label)) {
                filename = filename.Substring(label.Length);
            }
            if (filename.StartsWith("\\") || filename.StartsWith("/")) {
                filename = filename.Substring(1);
            }
            if (Extension != null && filename.EndsWith(Extension)) {
                filename = filename.Substring(0, filename.Length - Extension.Length);
            }
            return filename;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            var rootDirectory = (RootDirectory ?? LibSetting.Directories.ACTIVE);
            var filename = rootDirectory;
            if (label != null) {
                filename = Path.Combine(filename, label);
            }
            filename = Path.Combine(filename, (string) value);
            if (Extension != null) {
                filename += Extension;
            }
            return filename;
        }
    }
}
