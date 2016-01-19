namespace uAL.Properties.Settings.ToggleSettings
{
    public enum QueueToggleStatus
    {
        Disabled = 0,
        All = 1,
        Torrent = 2,
        Metalink = 3
    }

    public static class Extensions 
    {
    	public static bool IsAll(this QueueToggleStatus value) {
    		return value == QueueToggleStatus.All;
    	}
    	public static bool IsTorrent(this QueueToggleStatus value) {
    		return value == QueueToggleStatus.Torrent;
    	}
    	public static bool IsMetalink(this QueueToggleStatus value) {
    		return value == QueueToggleStatus.Metalink;
    	}
    	public static bool IncludesAll(this QueueToggleStatus value) {
    		return value.IsAll() || (value.IsTorrent() && value.IsMetalink());
    	}
    	public static bool IncludesTorrent(this QueueToggleStatus value) {
    		return value.IsAll() || value.IsTorrent();
    	}
    	public static bool IncludesMetalink(this QueueToggleStatus value) {
    		return value.IsAll() || value.IsMetalink();
    	}
    }

}
