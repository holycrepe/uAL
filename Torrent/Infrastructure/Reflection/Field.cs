using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Torrent.Extensions;

namespace Torrent.Infrastructure.Reflection
{
    public class Field<T>
    {
        public FieldInfo Info { get; }
        public object Value { get; }
        public T Cast { get; }
        public Type Type { get; }

        public string Name
            => Info.Name;
        public string Description { get; }     
        public bool? Browsable { get; }
        public Field(FieldInfo fi, Type type, Func<object, T> valueSelector = null)
        {
            Info = fi;
            Value = fi.GetValue(type);
            Cast = (valueSelector ?? ((obj) => (T) obj))(Value);
            Type = type;
            Description = Info.GetDescription();
            Browsable = Info.GetCustomAttribute<BrowsableAttribute>()?.Browsable;
        }

        public static implicit operator T(Field<T> value)
            => value.Cast;
    }
}
