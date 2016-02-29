using System.ComponentModel;
using System.Diagnostics;

namespace Torrent.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using System.Runtime.Serialization;
    using Puchalapalli.Extensions.Collections;

    public delegate void LocalPropertyChangedEventHandler(string propertyName);
    [DataContract]
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual Dictionary<string, string[]> DerivedProperties { get; } = null;
        [DebuggerNonUserCode]
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
        [DebuggerNonUserCode]
        public static void DoOnPropertyChanged(object sender, PropertyChangedEventHandler propertyChanged,
                                               params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames) {
                if (!string.IsNullOrWhiteSpace(propertyName)) {
                    propertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        [DebuggerNonUserCode]
        public static void DoOnPropertyChanged(object sender, PropertyChangedEventHandler propertyChanged,
                                               Dictionary<string, string[]> derivedProperties,
                                               params string[] propertyNames)
        {
            DoOnPropertyChanged(sender, propertyChanged, propertyNames);
            DoOnPropertyChangedDerived(sender, propertyChanged, derivedProperties, propertyNames);
        }
        [DebuggerNonUserCode]
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
        [DebuggerNonUserCode]
        protected virtual void OnPropertyChanged(string propertyName) 
            => OnPropertyChanged(propertyName.Split(';'));
        [DebuggerNonUserCode]
        public virtual void OnPropertyChanged(params string[] propertyNames)
            => DoOnPropertyChanged(this, PropertyChanged, DerivedProperties, propertyNames);
    }
    /*      EXAMPLE IMPLEMENTATION

    
    #region Interfaces
    #region Interfaces: IPropertyChanged
    private void OnPropertyChangedLocal(string propertyName)
        => LogUtils.Log(this.GenericType.Name, $"Δ {propertyName}");
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => OnPropertiesChanged(propertyName.TrimStart("Set").TrimStart("Get"));
    public virtual void OnPropertiesChanged(params string[] propertyNames)
        => DoOnPropertyChanged(this, PropertyChanged, OnPropertyChangedLocal, propertyNames);
    #endregion
    #endregion


    */
}
