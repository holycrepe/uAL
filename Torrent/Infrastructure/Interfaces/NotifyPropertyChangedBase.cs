using System.ComponentModel;

namespace Torrent.Infrastructure
{
    using System;
    public class NotifyPropertyChangedBase : INotifyPropertyChanged {
    	public event PropertyChangedEventHandler PropertyChanged;
        public static void DoOnPropertyChanged(object sender, PropertyChangedEventHandler PropertyChanged, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (!string.IsNullOrWhiteSpace(propertyName) && PropertyChanged != null)
                {
                    PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        protected void OnPropertyChanged(string propertyName) => OnPropertyChanged(propertyName.Split(';'));
        public virtual void OnPropertyChanged(params string[] propertyNames) => DoOnPropertyChanged(this, PropertyChanged, propertyNames);
    }
}
