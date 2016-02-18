using System.ComponentModel;

namespace Torrent.Infrastructure
{
    public interface INotifyPropertyChangedExtended : INotifyPropertyChanged
    {
        string LastUpdatedPropertyName { get; }
        object LastUpdatedPropertyValue { get; }
    }
}
