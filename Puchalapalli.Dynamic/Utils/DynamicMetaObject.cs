using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Puchalapalli.Dynamic.Utils
{
    public class DynamicMetaObjectUtils
    {
        /// <summary>
        /// Returns the list of expressions represented by the <see cref="DynamicMetaObject"/> instances.
        /// </summary>
        /// <param name="objects">An array of <see cref="DynamicMetaObject"/> instances to extract expressions from.</param>
        /// <returns>The array of expressions.</returns>
        public static Expression[] GetExpressions(DynamicMetaObject[] objects)
        {
            ContractUtils.RequiresNotNull(objects, nameof(objects));

            Expression[] res = new Expression[objects.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                DynamicMetaObject mo = objects[i];
                ContractUtils.RequiresNotNull(mo, nameof(objects));
                Expression expr = mo.Expression;
                ContractUtils.RequiresNotNull(expr, nameof(objects));
                res[i] = expr;
            }

            return res;
        }
    }
}
