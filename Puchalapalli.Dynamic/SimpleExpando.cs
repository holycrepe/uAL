using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using Torrent.Exceptions;
using System.ComponentModel;

namespace Puchalapalli.Dynamic
{
    using Westwind.Utilities;

    /// <summary>
    /// Class, with typed instance and results, that provides extensible properties and methods. This
    /// dynamic object stores 'extra' properties in a dictionary or
    /// checks the actual properties of the instance.
    /// 
    /// This means you can subclass this expando and retrieve either
    /// native properties or properties from values in the dictionary.
    /// 
    /// This type allows you three ways to access its properties:
    /// 
    /// Directly: any explicitly declared properties are accessible
    /// Dynamic: dynamic cast allows access to dictionary and native properties/methods
    /// Dictionary: Any of the extended properties are accessible via IDictionary interface
    /// </summary>
    [Serializable]
    public class SimpleExpando : Expando
    {
        public SimpleExpando() : base() { }
        public SimpleExpando(object instance) : base(instance) { }

        /// <summary>
        /// Returns and the properties of 
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override IEnumerable<KeyValuePair<string, object>> GetProperties(bool includeInstanceProperties = false)
            => base.GetProperties(includeInstanceProperties);

        /// <summary>
        /// Checks whether a property exists in the Property collection
        /// or as a property on the instance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Contains<TValue>(KeyValuePair<string, TValue> item, bool includeInstanceProperties = false)
            => base.Contains(item, includeInstanceProperties);


        /// <summary>
        /// Checks whether a property exists in the Property collection
        /// or as a property on the instance
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Contains(string key, bool includeInstanceProperties = false)
            => base.Contains(key, includeInstanceProperties);

        /// <summary>
        /// Dynamic invocation method. Currently allows only for Reflection based
        /// operation (no ability to add methods dynamically).
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool TryInvokeMember<TResult>(InvokeMemberBinder binder, object[] args, out TResult result)
            => base.TryInvokeMember(binder, args, out result);


        /// <summary>
        /// Property setter implementation tries to retrieve value from instance 
        /// first then into this object
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool TrySetMember(SetMemberBinder binder, object value)
            => base.TrySetMember(binder, value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool TryGetMember<TResult>(GetMemberBinder binder, out TResult result)
            => base.TryGetMember(binder, out result);

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void Initialize(object instance)
            => base.Initialize(instance);

        /// <summary>
        /// Instance of object passed in
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override object Instance
            => base.Instance;

        /// <summary>
        /// String Dictionary that contains the extra dynamic values
        /// stored on this object/instance
        /// </summary>        
        /// <remarks>Using PropertyBag to support XML Serialization of the dictionary</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override PropertyBag Properties
            => base.Properties;

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
            => base.GetHashCode();
    }
}