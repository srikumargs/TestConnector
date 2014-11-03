using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Security.Principal;

namespace Sage.Connector.Common
{
    /// <summary>
    /// General utility helper class
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringToSecure"></param>
        /// <returns></returns>
        public static SecureString ToSecureString(this String stringToSecure)
        { return Sage.Connector.LinkedSource.SecureStringUtils.ToSecureString(stringToSecure); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringToSecure"></param>
        /// <returns></returns>
        public static String ToNonSecureString(this SecureString stringToSecure)
        { return Sage.Connector.LinkedSource.SecureStringUtils.ToNonSecureString(stringToSecure); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static String ExceptionAsString(this Exception ex)
        {
            String result = String.Empty;
            if (ex != null)
            {
                result = String.Format(CultureInfo.InvariantCulture, "{0}{1}", ex.ToString(), ex.InnerException != null ? String.Format("\n\nInner exception:{0}", ex.InnerException.ExceptionAsString()) : String.Empty);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Boolean IsUserAdmin(this WindowsIdentity id)
        {
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static PropertyInfo PropertyInfo<T, TProperty>(this T instance, Expression<Func<T, TProperty>> propertyLambda)
        {
            Type type = typeof(T);

            MemberExpression memberExpression = propertyLambda.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propertyInfo.ReflectedType &&
                !type.IsSubclassOf(propertyInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propertyInfo;
        }
    }
}
