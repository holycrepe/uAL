namespace wUAL.Properties.Settings.Binding
{
    using uALBinding = uAL.Properties.Settings.Binding;
    using TorrentBinding = Torrent.Properties.Settings.Binding;
    public class LibExtension : uALBinding.LibExtension {
        public LibExtension(string path) : base(path) { }
    }
    public class ToggleExtension : uALBinding.ToggleExtension
    {
        public ToggleExtension(string path) : base(path) { }
    }
    public class TorrentExtension : uALBinding.TorrentExtension
    {
        public TorrentExtension(string path) : base(path) { }
    }
    public class MetalinkExtension : uALBinding.MetalinkExtension
    {
        public MetalinkExtension(string path) : base(path) { }
    }
    public class MyExtension : TorrentBinding.MyExtension
    {
        public MyExtension(string path) : base(path) { }
    }
}