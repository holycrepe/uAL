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
    public partial class Member<T>
    {
        public MemberInfo Info { get; }
        public FieldInfo Field { get; }
        public PropertyInfo Property { get; }
        public object Value { get; }
        public T Cast { get; }
        public Type Type { get; }

        public string Name
            => Info.Name;
        public string Description { get; }
        public bool? Browsable { get; }

        public Member(MemberInfo memberInfo, Type type, object value, Func<object, T> valueSelector = null)
        {
            Info = memberInfo;
            Type = type;
            Description = memberInfo.GetMemberDescription();
            Browsable = Info.GetCustomAttribute<BrowsableAttribute>()?.Browsable;

            if (value == null)
                return;

            Value = value;
            Cast = (valueSelector ?? ((obj) => (T)obj))(Value);
        }

        public Member(FieldInfo fi, object value, Type type, Func<object, T> valueSelector) : this(fi, type, value, valueSelector)
        {
            this.Field = fi;
        }
        public Member(FieldInfo fi, Type type, Func<object, T> valueSelector = null, bool parseValue = true)
            : this(fi, type, parseValue ? fi.GetValue(type) : null, valueSelector)
        {
            this.Field = fi;
        }

        public Member(PropertyInfo pi, object value, Type type, Func<object, T> valueSelector) : this(pi, type, value, valueSelector)
        {
            this.Property = pi;
        }
        public Member(PropertyInfo pi, Type type, Func<object, T> valueSelector = null, bool parseValue = true)
            : this(pi, type, parseValue ? pi.GetValue(type) : null, valueSelector)
        {
            Property = pi;
        }
        public static implicit operator T(Member<T> value)
            => value.Cast;
    }
}
