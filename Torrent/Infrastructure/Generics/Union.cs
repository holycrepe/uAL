using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torrent.Infrastructure.Generics
{
    public class UnionBase
    {
        object value;
                
        protected UnionBase(object x) { value = x; }

        protected T InternalMatch<T>(params Delegate[] ds)
        {
            var vt = value.GetType();
            foreach (var d in ds)
            {
                var mi = d.Method;

                // These are always true if InternalMatch is used correctly.
                Debug.Assert(mi.GetParameters().Length == 1);
                Debug.Assert(typeof(T).IsAssignableFrom(mi.ReturnType));

                var pt = mi.GetParameters()[0].ParameterType;
                if (pt.IsAssignableFrom(vt))
                    return (T)mi.Invoke(null, new object[] { value });
            }
            throw new Exception("No appropriate matching function was provided");
        }

        //public T Match<T>(Func<object, T> fa) { return InternalMatch<T>(fa); }
    }

    public class Union<A, B> : UnionBase
    {
        public Union(A a) : base(a) { }
        public Union(B b) : base(b) { }
        protected Union(object x) : base(x) { }
        public T Match<T>(Func<A, T> fa, Func<B, T> fb) { return InternalMatch<T>(fa, fb); }
    }

    public class Union<A, B, C> : UnionBase
    {
        public Union(A a) : base(a) { }
        public Union(B b) : base(b) { }
        public Union(C c) : base(c) { }
        protected Union(object x) : base(x) { }
        public T Match<T>(Func<A, T> fa, Func<B, T> fb, Func<C, T> fc) { return InternalMatch<T>(fa, fb, fc); }
    }
}
