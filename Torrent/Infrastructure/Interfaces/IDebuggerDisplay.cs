namespace Torrent.Infrastructure
{
    public interface IDebuggerDisplay
    {
        string DebuggerDisplay(int level = 1);
        string DebuggerDisplaySimple(int level = 1);
    }
}
