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
        public static T Evaluate<T>(this Expression e)
            => e.NodeType == ExpressionType.Constant
            ? (T)((ConstantExpression)e).Value
            : (T)Expression.Lambda(e).Compile().DynamicInvoke();
        public static object Evaluate(this Expression e)
            => e.NodeType == ExpressionType.Constant
            ? ((ConstantExpression)e).Value
            : Expression.Lambda(e).Compile().DynamicInvoke();

        public static TResult GetResult<TResult>(this Expression<Func<TResult>> expression)
            => expression.Compile().Invoke();
        public static TResult GetResult<T, TResult>(this Expression<Func<T, TResult>> expression, T parameter)
            => expression.Compile().Invoke(parameter);

        public static string GetPropertySymbol<TDelegate>(this Expression<TDelegate> expression)
            => string.Join(".",
                GetMembersOnPath(expression.Body as MemberExpression)
                .Select(m => m.Member.Name)
                .Reverse());

        private static IEnumerable<MemberExpression> GetMembersOnPath(this MemberExpression expression)
        {
            while (expression != null)
            {
                yield return expression;
                expression = expression.Expression as MemberExpression;
            }
        }

        public static string GetPropertyName<TProperty>(this Expression<Func<TProperty>> propertyExpression)
            => propertyExpression.Body.GetMemberExpression().GetPropertyName();

        public static string GetPropertyName(this MemberExpression memberExpression)
        {
            if (memberExpression?.Member.MemberType != MemberTypes.Property)            
                return null;            

            var child = memberExpression.Member.Name;
            var parent = GetPropertyName(memberExpression.Expression.GetMemberExpression());

            return parent == null
                ? child
                : parent + "." + child;
        }

        public static MemberExpression GetMemberExpression(this Expression expression) 
            => expression as MemberExpression
            ?? (MemberExpression)(expression as UnaryExpression)?.Operand;

        public static void ShouldEqual<T>(this T actual, T expected, string name)
        {
            if (!object.Equals(actual, expected))            
                throw new Exception($"{name}: Expected <{expected}> Actual <{actual}>.");
            
        }

    }
}
