using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;
using uAL.Torrents;
using System.Threading.Tasks;

namespace uAL.Infrastructure
{
    using Torrent.Infrastructure;
    public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
    where T : NotifyPropertyChangedBase
    {
		public TrulyObservableCollection()
		{
			CollectionChanged += FullObservableCollectionCollectionChanged;
		}

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
					((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
				}
			}
			if (e.OldItems != null) {
				foreach (object item in e.OldItems) {
					((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
				}
			}
		}
        void doItemPropertyChanged(T item)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, item, IndexOf(item));
            try
            {
                OnCollectionChanged(args);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("thread"))
                {
                    var torrentQueueItem = item as TorrentQueueItem;
                    if (torrentQueueItem != null)
                    {
                        torrentQueueItem.Log("x");
                    }
                    return;
                }
                throw;
            }
        }
		public Task ItemPropertyChanged(T item) {
            return UI.StartNew(() => doItemPropertyChanged(item));
        }
		private async void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{            
			await ItemPropertyChanged((T) sender);
		}
	}
}