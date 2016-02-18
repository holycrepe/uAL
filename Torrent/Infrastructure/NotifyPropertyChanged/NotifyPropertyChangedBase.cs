using System.ComponentModel;

namespace Torrent.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using System.Runtime.Serialization;
    public delegate void LocalPropertyChangedEventHandler(string propertyName);
    [DataContract]
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual Dictionary<string, string[]> DerivedProperties { get; } = null;
        public static void DoOnPropertyChanged(object sender, PropertyChangedEventHandler propertyChanged,
                                               LocalPropertyChangedEventHandler propertyChangedLocal,
                                               params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (!string.IsNullOrWhiteSpace(propertyName))
                {
                    propertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
                    propertyChangedLocal?.Invoke(propertyName);
                }
            }
        }
        public static void DoOnPropertyChanged(object sender, PropertyChangedEventHandler propertyChanged,
                                               params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames) {
                if (!string.IsNullOrWhiteSpace(propertyName)) {
                    propertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public static void DoOnPropertyChanged(object sender, PropertyChangedEventHandler propertyChanged,
                                               Dictionary<string, string[]> derivedProperties,
                                               params string[] propertyNames)
        {
            DoOnPropertyChanged(sender, propertyChanged, propertyNames);
            DoOnPropertyChangedDerived(sender, propertyChanged, derivedProperties, propertyNames);
        }

        public static void DoOnPropertyChangedDerived(object sender, PropertyChangedEventHandler propertyChanged,
                                                      Dictionary<string, string[]> derivedProperties,
                                                      params string[] propertyNames)
        {
            if (derivedProperties != null) {
                foreach (var settings in derivedProperties) {
                    if (settings.Value.ContainsAny(propertyNames)) {
                        DoOnPropertyChanged(sender, propertyChanged, derivedProperties, settings.Key);
                    }
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName) 
            => OnPropertyChanged(propertyName.Split(';'));

        public virtual void OnPropertyChanged(params string[] propertyNames)
            => DoOnPropertyChanged(this, PropertyChanged, DerivedProperties, propertyNames);
    }
}
