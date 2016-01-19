namespace Torrent.Helpers.Utils
{
    using Infrastructure.InfoReporters;
    using System.Diagnostics;
    
    public static class DebugUtils
    {
    	public static readonly DebuggerInfoReporter DEBUG = new DebuggerInfoReporter();
    }
}