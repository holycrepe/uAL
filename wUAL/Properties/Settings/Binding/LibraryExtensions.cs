namespace wUAL.Properties.Settings.Binding
{
    using uALBinding = uAL.Properties.Settings.Binding;
    using TorrentBinding = Torrent.Properties.Settings.Binding;
    using System.Windows.Data;
    public class LibExtension : uALBinding.LibExtension
    {
        public LibExtension() : base() { }
        public LibExtension(string path) : this(path, null) { }
        public LibExtension(string path, string format) : base(path, format) { }
        public LibExtension(string path, IValueConverter converter, BindingMode mode) : base(path)
        {
            this.Converter = converter;
            this.Mode = mode;
        }
    }
    public class MyExtension : TorrentBinding.MyExtension
    {
        public MyExtension() : base() { }
        public MyExtension(string path) : base(path) { }
    }
}