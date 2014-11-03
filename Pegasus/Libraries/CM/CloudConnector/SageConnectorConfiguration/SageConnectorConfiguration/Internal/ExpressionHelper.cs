using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SageConnectorConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ExpressionHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static String MemberAccessAsString<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("expression");
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("expression");
            }

            if (propertyInfo.GetGetMethod(true).IsStatic)
            {
                throw new ArgumentException("expression");
            }

            return memberExpression.Member.Name;
        }
    }
}
