using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sage.Connector.SageCloudService
{
    /// <summary>
    /// Support for PremiseKey
    /// </summary>
    static public class PremiseKeyHelper
    {
        /// <summary>
        /// Gets the Premise key for a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static string GetPremiseKeyForTenant(Guid tenantId)
        {
            return MockCloudService.GetPremiseKeyForTenant(tenantId);
        }
    }
}
