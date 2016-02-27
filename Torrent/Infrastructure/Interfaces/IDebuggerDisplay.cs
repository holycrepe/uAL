namespace Torrent.Infrastructure
{
    public interface IDebuggerDisplay
    {
        string DebuggerDisplay(int level = 1);
        string DebuggerDisplaySimple(int level = 1);
#if NEVER_COMPILE
        
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        #region Interfaces
        #region Interfaces: IDebuggerDisplay
            [DebuggerNonUserCode]
            public override string ToString()
            => DebuggerDisplaySimple();
            public string DebuggerDisplay(int level = 1)
            => $"<{this.GetType().GetFriendlyName()}> {DebuggerDisplaySimple(level)}";
            public string DebuggerDisplaySimple(int level = 1)
            => $"{this.Value?.ToString()}";
        #endregion
        #endregion
#endif
    }



}
