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
    public class Expando : SimpleDynamicObject, IDynamicMetaObjectProvider, IExpando
    {
        /// <summary>
        /// Instance of object passed in
        /// </summary>
        private object _instance;

        /// <summary>
        /// Instance of object passed in
        /// </summary>
        protected virtual object Instance
            => this._instance;

        /// <summary>
        /// Cached type of the instance
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Type InstanceType;

        PropertyInfo[] InstancePropertyInfo
            => _InstancePropertyInfo ?? (_instance == null ? null 
            : _InstancePropertyInfo = _instance.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));

        PropertyInfo[] _InstancePropertyInfo;

        /// <summary>
        /// Enables/Disables Property Setter
        /// </summary>
        protected bool SetterEnabled { get; set; } = true;

        /// <summary>
        /// String Dictionary that contains the extra dynamic values
        /// stored on this object/instance
        /// </summary>        
        /// <remarks>Using PropertyBag to support XML Serialization of the dictionary</remarks>
        public virtual PropertyBag Properties { get; }
            = new PropertyBag();

        //public Dictionary<string,object> Properties = new Dictionary<string, object>();

        

        /// <summary>
        /// Allows passing in an existing instance variable to 'extend'.        
        /// </summary>
        /// <remarks>
        /// You can pass in null here if you don't want to 
        /// check native properties and only check the Dictionary!
        /// </remarks>
        /// <param name="instance"></param>
        public Expando(object instance=null)
        {
            Initialize(instance);
        }

        protected virtual void Initialize(object instance)
        {
            _instance = instance;
            if (instance != null)
            {
                InstanceType = instance.GetType();
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool TryGetMember(GetMemberBinder binder, out object result)
            => TryGetMember<object>(binder, out result);

        public virtual bool TryGetMember<TResult>(GetMemberBinder binder, out TResult result)
        { 
            result = default(TResult);

            // first check the Properties collection for member
            if (Properties.Keys.Contains(binder.Name))
            {
                result = (TResult) Properties[binder.Name];
                return true;
            }


            // Next check for Public properties via Reflection
            if (_instance != null)
            {
                try
                {
                    return GetProperty(_instance, binder.Name, out result);
                }
                catch { }
            }

            // failed to retrieve a property
            result = default(TResult);
            return false;
        }


        ///// <summary>
        ///// Property setter implementation tries to retrieve value from instance 
        ///// first then into this object
        ///// </summary>
        ///// <param name="binder"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public override bool TrySetMember(SetMemberBinder binder, object value)
        //    => TrySetMember(binder, (T)value);

        /// <summary>
        /// Property setter implementation tries to retrieve value from instance 
        /// first then into this object
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!SetterEnabled)
            {
                throw new DynamicReadOnlyException($"{nameof(Expando)}: Attempted to set member `{binder.Name}` with all setters disabled");
            }
            // first check to see if there's a native property to set
            if (_instance != null)
            {
                try
                {
                    if (SetProperty(_instance, binder.Name, value))
                    {
                        return true;
                    }
                }
                catch { }
            }

            // no match - set or add to dictionary
            Properties[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Dynamic invocation method. Currently allows only for Reflection based
        /// operation (no ability to add methods dynamically).
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            => TryInvokeMember<object>(binder, args, out result);

        /// <summary>
        /// Dynamic invocation method. Currently allows only for Reflection based
        /// operation (no ability to add methods dynamically).
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool TryInvokeMember<TResult>(InvokeMemberBinder binder, object[] args, out TResult result)
        {
            if (_instance != null)
            {
                try
                {
                    // check instance passed in for methods to invoke
                    if (InvokeMethod(_instance, binder.Name, args, out result))
                        return true;
                }
                catch { }
            }

            result = default(TResult);
            return false;
        }


        /// <summary>
        /// Reflection Helper method to retrieve a property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected bool GetProperty<TResult>(object instance, string name, out TResult result)
        {
            if (instance == null)
                instance = this;

            var miArray = InstanceType.GetMember(name, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
            if (miArray != null && miArray.Length > 0)
            {
                var mi = miArray[0];
                if (mi.MemberType == MemberTypes.Property)
                {
                    var objResult = ((PropertyInfo)mi).GetValue(instance, null); 
                    if (objResult is TResult)
                    {
                        result = (TResult) objResult;
                        return true;
                    }
                    
                }
            }

            result = default(TResult);
            return false;
        }

        /// <summary>
        /// Reflection helper method to set a property value
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected bool SetProperty(object instance, string name, object value)
        {
            if (instance == null)
                instance = this;

            var miArray = InstanceType.GetMember(name, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
            if (miArray != null && miArray.Length > 0)
            {
                var mi = miArray[0];
                if (mi.MemberType == MemberTypes.Property)
                {
                    ((PropertyInfo)mi).SetValue(_instance, value, null);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reflection helper method to invoke a method
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected bool InvokeMethod<TResult>(object instance, string name, object[] args, out TResult result)
        {
            if (instance == null)
                instance = this;

            // Look at the instanceType
            var miArray = InstanceType.GetMember(name,
                                    BindingFlags.InvokeMethod |
                                    BindingFlags.Public | BindingFlags.Instance);

            if (miArray != null && miArray.Length > 0)
            {
                var mi = miArray[0] as MethodInfo;
                result = (TResult) mi.Invoke(_instance, args);
                return true;
            }

            result = default(TResult);
            return false;
        }



        /// <summary>
        /// Convenience method that provides a string Indexer 
        /// to the Properties collection AND the strongly typed
        /// properties of the object by name.
        /// 
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane"; 
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string; 
        /// </summary>
        /// <remarks>
        /// The getter checks the Properties dictionary first
        /// then looks in PropertyInfo for properties.
        /// The setter checks the instance properties before
        /// checking the Properties dictionary.
        /// </remarks>
        /// <param name="key"></param>
        /// 
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                try
                {
                    // try to get from properties collection first
                    return Properties[key];
                }
                catch (KeyNotFoundException ex)
                {
                    // try reflection on instanceType
                    object result;
                    if (GetProperty(_instance, key, out result))
                        return result;

                    // nope doesn't exist
                    throw ex;
                }
            }
            set
            {
                if (Properties.ContainsKey(key))
                {
                    Properties[key] = value;
                    return;
                }

                // check instance for existance of type first
                var miArray = InstanceType.GetMember(key, BindingFlags.Public | BindingFlags.GetProperty);
                if (miArray?.Length > 0)
                    SetProperty(_instance, key, value);
                else
                    Properties[key] = value;
            }
        }


        /// <summary>
        /// Returns and the properties of 
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual IEnumerable<KeyValuePair<string, object>> GetProperties(bool includeInstanceProperties = false)
        {
            if (includeInstanceProperties && _instance != null)
            {
                foreach (var prop in this.InstancePropertyInfo)
                    yield return new KeyValuePair<string, object>(prop.Name, prop.GetValue(_instance, null));
            }

            foreach (var key in this.Properties.Keys)
                yield return new KeyValuePair<string, object>(key, this.Properties[key]);

        }

        /// <summary>
        /// Checks whether a property exists in the Property collection
        /// or as a property on the instance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Contains<TValue>(KeyValuePair<string, TValue> item, bool includeInstanceProperties = false)
            => Contains(item.Key, includeInstanceProperties);

        /// <summary>
        /// Checks whether a property exists in the Property collection
        /// or as a property on the instance
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool Contains(string key, bool includeInstanceProperties = false)
        {
            bool res = Properties.ContainsKey(key);
            if (res)
                return true;

            if (includeInstanceProperties && _instance != null)
            {
                foreach (var prop in this.InstancePropertyInfo)
                {
                    if (prop.Name == key)
                        return true;
                }
            }
            return false;
        }

    }
}