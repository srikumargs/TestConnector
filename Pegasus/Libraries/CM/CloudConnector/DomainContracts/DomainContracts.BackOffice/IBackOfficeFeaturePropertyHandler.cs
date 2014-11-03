using Sage.Connector.DomainContracts.Responses;
using System;
using System.Collections.Generic;

namespace Sage.Connector.DomainContracts.BackOffice
{
    /// <summary>
    /// Interface included as part of IManageFeatureConfiguration from configuration back office contracts.
    /// </summary>
    public interface IBackOfficeFeaturePropertyHandler
    {
        /// <summary>
        /// Initialize the feature with the default property values
        /// </summary>
        /// <param name="defaultPropertyValues">read only set of property name - value pairs</param>
        /// <returns>The <see cref="Response"/></returns>
        Response Initialize(IDictionary<string, Object> defaultPropertyValues);
    }
}
