using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Torrent.Extensions.Expressions
{
    public static class ExpressionExtensions
    {
        public static string GetPropertyName<TProperty>(this Expression<Func<TProperty>> propertyExpression)
            => propertyExpression.Body.GetMemberExpression().GetPropertyName();

        public static string GetPropertyName(this MemberExpression memberExpression)
        {
            if (memberExpression == null)
            {
                return null;
            }

            if (memberExpression.Member.MemberType != MemberTypes.Property)
            {
                return null;
            }

            var child = memberExpression.Member.Name;
            var parent = GetPropertyName(memberExpression.Expression.GetMemberExpression());

            if (parent == null)
            {
                return child;
            }
            else
            {
                return parent + "." + child;
            }
        }

        public static MemberExpression GetMemberExpression(this Expression expression)
        {
            var memberExpression = expression as MemberExpression;

            if (memberExpression != null)
            {
                return memberExpression;
            }

            var unaryExpression = expression as UnaryExpression;


            if (unaryExpression != null)
            {
                memberExpression = (MemberExpression)unaryExpression.Operand;

                if (memberExpression != null)
                {
                    return memberExpression;
                }

            }
            return null;
        }

        public static void ShouldEqual<T>(this T actual, T expected, string name)
        {
            if (!Object.Equals(actual, expected))
            {
                throw new Exception(String.Format("{0}: Expected <{1}> Actual <{2}>.", name, expected, actual));
            }
        }

    }
}
