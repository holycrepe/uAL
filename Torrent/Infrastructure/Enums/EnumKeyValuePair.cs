using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using AddGenericConstraint;
using Torrent.Helpers.Utils;

namespace Torrent.Infrastructure.Enums
{
    using System;
    using System.Text;
    using Extensions;
    // A modified KeyValuePair holds an Enum key and a class-based value from a dictionary.
    [Serializable]
    [ContentProperty(nameof(Value))]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class EnumKeyValuePair<[AddGenericConstraint(typeof(Enum))] TKey, TValue> : IEnumKeyValuePair<TKey, TValue>
          where TKey : struct 
    {
        public EnumKeyValuePair()
        {
            this.Key = EnumUtils.GetDefault<TKey>();
        }
        public EnumKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; set; }

        public virtual TValue Value { get; set; }

        #region Interfaces
        #region Interfaces: IDebuggerDisplay
        [DebuggerNonUserCode]
        public override string ToString()
            => DebuggerDisplaySimple();
        public string DebuggerDisplay(int level = 1)
        => $"<{this.GetType().GetFriendlyName()}> {DebuggerDisplaySimple(level)}";
        public string DebuggerDisplaySimple(int level = 1)
        {
            var s = new StringBuilder($"[{Key}");
            if (Value != null)            
                s.Append("=" + ((Value as IDebuggerDisplay)?.DebuggerDisplaySimple(level) ?? Value.ToString()));
            s.Append(']');
            return s.ToString();
        }
        #endregion
        #endregion
    }
}