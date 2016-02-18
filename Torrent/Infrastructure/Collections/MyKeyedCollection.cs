namespace Torrent.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public abstract class MyKeyedCollection<TKey, TValue> : KeyedCollection<TKey, TValue> where TKey : class
    {
        protected abstract Dictionary<Type, Func<object, TKey>> KeySelectors { get; }

        #region Type Validation

        private bool HasKeyForItem<T>()
            => KeySelectors.ContainsKey(typeof (T));

        #endregion

        #region Search Functions

        #region Search Functions: Get Key: Generic Implementation

        protected override TKey GetKeyForItem(TValue item)
            => KeySelectors[typeof (TValue)](item);

        protected TKey GetKeyForItem<T>(T item)
        {
            if (HasKeyForItem<T>()) {
                return KeySelectors[typeof (T)](item);
            }
            throw new ArgumentException(
                $"Invalid Item Type: Collection {this.GetType().Name} does not support Item {typeof (T).Name} ");
        }

        #endregion

        #region Search Functions: Get Item By Key

        public TValue GetByKey<T>(T item)
            => this[GetKeyForItem(item)];

        #endregion

        #region Search Functions: Contains (By Key): Generic Implementation

        public new bool Contains(TKey key)
            => base.Contains(key);

        public bool Contains<T>(T item)
            => base.Contains(GetKeyForItem(item));

        #endregion

        #region Search Functions: Find Entry

        public int FindEntry(TKey key)
            => base.Contains(key) ? IndexOf(this[key]) : -1;

        public int FindEntry<T>(T item)
            => FindEntry(GetKeyForItem(item));

        #endregion

        #region Search Functions: Index Of By Key

        public int IndexOfByKey<T>(T item)
        {
            var index = FindEntry(item);
            if (index >= 0) {
                return index;
            }
            throw new KeyNotFoundException();
        }

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
            if (items != null) {
                foreach (var item in items) {
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
            } else {
                Add(newItem);
            }
        }

        public void AddOrReplace(IEnumerable<TValue> items)
        {
            if (items != null) {
                foreach (var item in items) {
                    AddOrReplace(item);
                }
            }
        }

        #endregion

        #endregion

        #region Actions: Remove & Clear

        public void Remove(IEnumerable<TKey> keys)
        {
            if (keys != null) {
                foreach (var key in keys) {
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
