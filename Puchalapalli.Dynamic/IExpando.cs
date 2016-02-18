using System.Collections.Generic;
using System.Dynamic;

namespace Puchalapalli.Dynamic
{
    public interface IExpando
    {
        object this[string key] { get; set; }

        bool Contains(string key, bool includeInstanceProperties = false);
        bool Contains<TValue>(KeyValuePair<string, TValue> item, bool includeInstanceProperties = false);
        IEnumerable<KeyValuePair<string, object>> GetProperties(bool includeInstanceProperties = false);
        bool TryGetMember(GetMemberBinder binder, out object result);
        bool TryGetMember<TResult>(GetMemberBinder binder, out TResult result);
        bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result);
        bool TryInvokeMember<TResult>(InvokeMemberBinder binder, object[] args, out TResult result);
        bool TrySetMember(SetMemberBinder binder, object value);
    }
}