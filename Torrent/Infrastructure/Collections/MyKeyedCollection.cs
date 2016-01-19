namespace Torrent.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public abstract class MyKeyedCollection<TKey, TValue> : KeyedCollection<TKey, TValue>
    {
        public virtual Dictionary<Type, Func<object, TKey>> KeySelectors { get; } =
            new Dictionary<Type, Func<object, TKey>>();
        #region Type Validation
        private bool IsTValue<T>()
        	=> typeof(T) == typeof(TValue);
        private bool HasKeyForItem<T>()
        	=> KeySelectors.ContainsKey(typeof(T));
        #endregion
        #region Search Functions
        #region Search Functions: Get Key: Generic Implementation
        private TKey getKeyForOtherItem<T>(T item)
        	=> KeySelectors[typeof(T)](item);
        public TKey GetKeyForOtherItem<T>(T item)
        {
        	if (HasKeyForItem<T>()) {
        		return getKeyForOtherItem(item);
        	}
        	throw new ArgumentException($"Invalid Item Type: Collection {this.GetType().Name} does not support Item {typeof(T).Name} ");
        }
        #endregion
        #region Search Functions: Get Item By Key
        private TValue getByKey(TValue item)
        	=> this[GetKeyForItem(item)];
        private TValue getByKey<T>(T item)
        	=> this[getKeyForOtherItem(item)];
        public TValue GetByKey<T>(T item)
        	=> IsTValue<T>() ? getByKey(item) : getByKey<T>(item);
        #endregion
        #region Search Functions: Contains Key: Generic Implementation
        public bool ContainsKey<T>(T item)
        	=> IsTValue<T>() ? Contains(getByKey(item)): Contains(getKeyForOtherItem(item));
        #endregion
        #region Search Functions: Find Entry
        public int FindEntry(TKey key)
        	=> Contains(key) ? IndexOf(this[key]) : -1;
        private int findEntry(TValue item)
        	=> FindEntry(GetKeyForItem(item));
        private int findEntry<T>(T item)
        	=> FindEntry(getKeyForOtherItem(item));                        
        public int FindEntry<T>(T item)
        	=> IsTValue<T>() ? findEntry(item) : findEntry<T>(item);        
        #endregion
        #region Search Functions: Index Of By Key
        private int indexOfByKey(TValue item)
        {
        	var index = findEntry(item);
        	if (index >= 0) {
        		return index;
        	}
        	throw new KeyNotFoundException();
        }
        private int indexOfByKey<T>(T item)
        {
        	var index = findEntry<T>(item);
        	if (index >= 0) {
        		return index;
        	}
        	throw new KeyNotFoundException();
        }
        public int IndexOfByKey(TValue item)
        	=> indexOfByKey(item);
        public int IndexOfByKey<T>(T item)
        	=> IsTValue<T>() ? indexOfByKey(item) : indexOfByKey<T>(item);        
        #endregion
        #region Search Functions: Indexer
        public TValue this[TValue item]
            => GetByKey(item);
        #endregion
        #endregion
        #region Actions
        #region Actions: Add
        #region Actions: Add: Add Multiple Values
        public void Add(IEnumerable<TValue> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
        }        
        #endregion
        
        #region Actions: Add: Add Or Replace
    	public void AddOrReplace(TValue newItem)
        {
            var i = FindEntry(newItem);
            if (i >= 0) {
                RemoveAt(i);
                InsertItem(i, newItem);
            }
            else {
            	Add(newItem);
            }
        }
    	public void AddOrReplace(IEnumerable<TValue> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    AddOrReplace(item);
                }
            }
        }
        #endregion
        #endregion
        #region Actions: Remove & Clear
        
        public void Remove(IEnumerable<TKey> keys)
        {
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    Remove(key);
                }
            }
        }        

        public void Clear(IEnumerable<TValue> items)
        {
        	base.Clear();
            Add(items);
        }                
		#endregion
        #endregion
    }
}