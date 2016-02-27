namespace wUAL.WPF.Models.Base
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using AddGenericConstraint;
    using Torrent.Infrastructure.Enums;

    public class SelectorCollection<[AddGenericConstraint(typeof(Enum))] TKey, TValue>
    : SelectorCollection<EnumKeyValuePair<TKey, TValue>, TKey, TValue>
    where TKey : struct
    { }
    public class SelectorCollection<TSelection, [AddGenericConstraint(typeof(Enum))] TKey, TValue>
    : KeyedCollection<TKey, TSelection>
    where TKey : struct
    where TSelection : class, IEnumKeyValuePair<TKey,TValue>
    {
        #region Overrides of KeyedCollection<TKey,TModel>

        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <returns>
        /// The key for the specified element.
        /// </returns>
        /// <param name="item">The element from which to extract the key.</param>
        [DebuggerNonUserCode]
        protected override TKey GetKeyForItem(TSelection item)
        => item.Key;
        #endregion

        #region ContainsKey
        [DebuggerNonUserCode]
        public new bool Contains(TKey key)
            => base.Contains(key);
        [DebuggerNonUserCode]
        public bool Contains(TKey? key)
            => key.HasValue && Contains(key.Value);
        [DebuggerNonUserCode]
        public bool Contains(IEnumKey<TKey> key)
            => Contains(key.Key);
        [DebuggerNonUserCode]
        public bool Contains(IEnumValue<TKey> key)
            => Contains(key.Value);
        #endregion

        #region GetValue()
        [DebuggerNonUserCode]
        public TValue GetValue(TKey key)
        => Get(key).Value;
        [DebuggerNonUserCode]
        public TValue GetValue(TKey? key)
        => key.HasValue ? GetValue(key.Value) : default(TValue);
        [DebuggerNonUserCode]
        public TValue GetValue(IEnumKey<TKey> item)
        => GetValue(item.Key);
        [DebuggerNonUserCode]
        public TValue GetValue(IEnumValue<TKey> item)
        => GetValue(item.Value);
        #endregion

        #region Get()
        [DebuggerNonUserCode]
        public TSelection Get(TKey key)
        => this[key];
        [DebuggerNonUserCode]
        public TSelection Get(TKey? key)
        => key.HasValue ? Get(key.Value) : default(TSelection);
        [DebuggerNonUserCode]
        public TSelection Get(IEnumKey<TKey> item)
        => Get(item.Key);
        [DebuggerNonUserCode]
        public TSelection Get(IEnumValue<TKey> item)
        => Get(item.Value);
        #endregion
    }
}