namespace wUAL.Properties.Settings.Binding
{
    using uALBinding = uAL.Properties.Settings.Binding;
    using TorrentBinding = Torrent.Properties.Settings.Binding;
    using System.Windows.Data;

    public class ToggleExtension : uALBinding.ToggleExtension
    {
        public ToggleExtension() : base() { }
        public ToggleExtension(string path) : base(path) { }
    }
    public class TorrentExtension : uALBinding.TorrentExtension
    {
        public TorrentExtension() : base() { }
        public TorrentExtension(string path) : base(path) { }
    }
    public class MetalinkExtension : uALBinding.MetalinkExtension
    {
        public MetalinkExtension() : base() { }
        public MetalinkExtension(string path) : base(path) { }
    }
    public class JobExtension : uALBinding.JobExtension
    {
        public JobExtension() : base() { }
        public JobExtension(string path) : base(path) { }
    }
}