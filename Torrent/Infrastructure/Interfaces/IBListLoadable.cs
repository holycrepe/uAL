namespace Torrent.Infrastructure.Interfaces
{
    using BencodeNET.Objects;
    public interface IBListLoadable
    {
        void LoadFromBList(BList list);
        BList ToBList();        
    }   
}
