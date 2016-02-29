namespace Torrent.Infrastructure.Reflection
{
    using System;
    using System.Reflection;

    public class InstanceMember<TInstance, T> : Member<T>
    {
        public TInstance Instance { get; }
        public InstanceMember(TInstance instance, Type type, FieldInfo fi, Func<object, T> valueSelector = null)
            : base(fi, fi.GetValue(instance), type, valueSelector)
        {
            this.Instance = instance;
        }
        public InstanceMember(TInstance instance, Type type, PropertyInfo fi, Func<object, T> valueSelector = null)
            : base(fi, fi.GetValue(instance), type, valueSelector)
        {
            this.Instance = instance;
        }
    }
}