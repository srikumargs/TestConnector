using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sage.Connector.Common;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;

namespace Sage.Connector.Management
{
    /// <summary>
    /// 
    /// </summary>
    public class FeatureServiceManager
    {

        /// <summary>
        /// Get the set Feature payload response
        /// </summary>
        /// <param name="backOfficeId">The back office identifier.</param>
        /// <param name="backOfficeCredentials">The back office credentials.</param>
        /// <param name="featureId">The feature identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public FeatureResponse GetFeatureResponse(string backOfficeId, string backOfficeCredentials, string featureId, string tenantId, string payload)
        {
            FeatureResponse response = null;
            try
            {
                // do the real work
                using (var proxy = FeatureServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    response = proxy.GetFeatureResponse(backOfficeId, backOfficeCredentials, featureId, tenantId, payload);
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            return response;
        }
    }
}
