using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Puchalapalli.Dynamic.Utils
{

    public class BindingRestrictionsUtils
    {
        /// <summary>
        /// The method takes a DynamicMetaObject, and returns an instance restriction for testing null if the object
        /// holds a null value, otherwise returns a type restriction.
        /// </summary>
        internal static BindingRestrictions GetTypeRestriction(DynamicMetaObject obj)
        {
            if (obj.Value == null && obj.HasValue)
            {
                return BindingRestrictions.GetInstanceRestriction(obj.Expression, null);
            }
            else {
                return BindingRestrictions.GetTypeRestriction(obj.Expression, obj.LimitType);
            }
        }
    }
}
