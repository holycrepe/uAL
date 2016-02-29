using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torrent.Infrastructure.Reflection
{
    using Extensions;

    public static partial class InstanceMember
    {
        public static IEnumerable<InstanceMember<TInstance, object>> GetMembers<TInstance>(
            TInstance instance, 
            Func<object, object> valueSelector=null, 
            MemberTypes includedTypes=MemberTypes.Property)
            => InstanceMember<object>.GetMembers(instance, valueSelector, includedTypes);
    }
    public static partial class InstanceMember<T>
    {
        public static IEnumerable<InstanceMember<TInstance, T>> GetMembers<TInstance>(TInstance instance, Func<object, T> valueSelector=null, MemberTypes includedTypes=MemberTypes.Property)
        {
            IEnumerable<InstanceMember<TInstance, T>> members = new InstanceMember<TInstance, T>[0];
            if (instance == null)
                return members;
            var type = instance.GetType();
            if (includedTypes.IncludesProperties())
                members = type.GetPublicPropertyInfo()
                    .Select(pi => new InstanceMember<TInstance, T>(instance, type, pi, valueSelector));
            return includedTypes.IncludesFields() 
                ? members.Concat(type.GetPublicFieldInfo()
                    .Select(pi => new InstanceMember<TInstance, T>(instance, type, pi, valueSelector)))
                : members;
        } 
    }
}
