using Torrent.Enums;
namespace Torrent.Infrastructure
{    
    using Helpers.StringHelpers;

    public class NotifyPropertyChangedExtendedBase : NotifyPropertyChangedBase, INotifyPropertyChangedExtended
    {
        public string LastUpdatedPropertyName { get; private set; }
        public object LastUpdatedPropertyValue { get; private set; }

        public string LastUpdatedProperty
            => new TitlePadder(LastUpdatedPropertyName, LastUpdatedPropertyValue, 10, PadDirection.Right);

        protected virtual void OnPropertyChanged(object propertyValue, params string[] propertyNames)
        {
            LastUpdatedPropertyName = string.Join(", ", propertyNames);
            LastUpdatedPropertyValue = propertyValue;

            base.OnPropertyChanged(propertyNames);
        }
    }
}
