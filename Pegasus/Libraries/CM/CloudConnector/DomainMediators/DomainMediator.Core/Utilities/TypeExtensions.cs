using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.DomainMediator.Core.Utilities
{
    /// <summary>
    /// Extension methods for the Type class
    /// </summary>
    public static class TypeExtensions
    {

        /// <summary>
        /// Get the external id property from the type.
        /// </summary>
        /// <param name="t"><see cref="Type"/></param>
        /// <returns>The name of the ExternalId property</returns>
        public static string GetExternalIdPropertyName(this Type t)
        {
            var propInfo = t.GetExternalIdProperty();

            return propInfo == null ? null : propInfo.Name;
        }

        /// <summary>
        /// Get property info of the External Id property
        /// </summary>
        /// <param name="t"><see cref="Type"/></param>
        /// <returns><see cref="PropertyInfo"/></returns>
        public static PropertyInfo GetExternalIdProperty(this Type t)
        {
            var props = t.GetProperties().Where(
                p => p.GetCustomAttributes(typeof(ExternalIdentifierAttribute), true).Length != 0);
            var propertyInfos = props as IList<PropertyInfo> ?? props.ToList();
            if (!propertyInfos.Any())
                return null;
            
            return !propertyInfos.Any() ? null : propertyInfos.First();
        }

        /// <summary>
        /// Get the external id reference properties from the type.
        /// </summary>
        /// <param name="t"><see cref="Type"/></param>
        /// <returns>The name of the ExternalId property</returns>
        public static IEnumerable<PropertyInfo> GetExternalIdRefProperties(this Type t)
        {
            var props = t.GetProperties().Where(
                 p => p.GetCustomAttributes(typeof(ExternalIdReferenceAttribute), true).Length != 0);

            return props;
        }

        /// <summary>
        /// Get the External Reference Properties
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetExternalReferenceProperties(this Type t)
        {
            var props = t.GetProperties().Where(
                p => p.GetType() == typeof(ExternalReference));

            return props;
        }


    }
}
