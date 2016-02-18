using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PostSharp.Patterns.Model;

namespace Torrent.Infrastructure
{    
    [NotifyPropertyChanged]
    public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
    {
        public TrulyObservableCollection() { CollectionChanged += FullObservableCollectionCollectionChanged; }

        public TrulyObservableCollection(IEnumerable<T> pItems)
            : this()
        {
            foreach (var item in pItems) {
                Add(item);
            }
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {            
            if (e.NewItems != null) {
                foreach (object item in e.NewItems) {
                    if (item != null && !(item is INotifyPropertyChanged))
                    {
                        return;
                    }
                    ((INotifyPropertyChanged) item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null) {
                foreach (object item in e.OldItems) {
                    if (item != null && !(item is INotifyPropertyChanged))
                    {
                        return;
                    }
                    ((INotifyPropertyChanged) item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        void doItemPropertyChanged(T item)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, item,
                                                            IndexOf(item));
            try {
                OnCollectionChanged(args);
            } catch (InvalidOperationException ex) {
#if DEBUG
                if (ex.Message.Contains("thread")) {
                    System.Diagnostics.Debugger.Break();
                    return;
                }
#endif
                throw;
            }
        }

        public Task ItemPropertyChanged(T item) 
            => UI.StartNew(() => doItemPropertyChanged(item));

        private async void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
            => await ItemPropertyChanged((T)sender);
        void OnPropertyChanged(string propertyName)
            => base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
}
