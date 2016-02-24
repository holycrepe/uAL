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

    // A modified KeyValuePair holds an Enum key and a class-based value from a dictionary.
    [Serializable]
    [ContentProperty(nameof(Value))]
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

        public TValue Value { get; set; }

        public override string ToString()
        {
            var s = new StringBuilder($"[{Key}, ");
            if (Value != null)
            {
                s.Append(Value);
            }
            s.Append(']');
            return s.ToString();
        }
    }
}