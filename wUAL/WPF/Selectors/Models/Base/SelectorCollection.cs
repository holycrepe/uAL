using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using AddGenericConstraint;
using Torrent.Infrastructure.Enums;

namespace wUAL.WPF.Selectors.Models.Base
{    
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
        [DebuggerNonUserCode]
        public TValue GetValue(IEnumKey<TKey> item)
        => GetValue(item.Key);
        [DebuggerNonUserCode]
        public TValue GetValue(IEnumValue<TKey> item)
        => GetValue(item.Value);

        [DebuggerNonUserCode]
        public TValue GetValue(TKey key)
        => Get(key).Value;
        [DebuggerNonUserCode]
        public TSelection Get(IEnumKey<TKey> item)
        => Get(item.Key);
        [DebuggerNonUserCode]
        public TSelection Get(IEnumValue<TKey> item)
        => Get(item.Value);
        [DebuggerNonUserCode]
        public TSelection Get(TKey key)
        => this[key];
    }
}