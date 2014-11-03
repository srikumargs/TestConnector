using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;

namespace Sage.Connector.MessagingService.ClientProxies
{
    internal sealed class APITenantListProxy : APIBaseServiceProxy
    {
        public APITenantListProxy(
            Uri endpointAddress,
            string resoucePath,
            string tenantId,
            string claim,
            PremiseAgent premiseAgent,
            MessageLogger logger)
            : base(endpointAddress, resoucePath, tenantId, string.Empty, claim, premiseAgent, logger)
        {
        }

        /// <summary>
        /// Tenants the list.
        /// </summary>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public Cloud.Integration.Interfaces.WebAPI.TenantInfo[] TenantList(string claim)
        {
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + claim);
            var response = GetResponse();
            var retVal =
                response.Content.ReadAsAsync<IEnumerable<Cloud.Integration.Interfaces.WebAPI.TenantInfo>>
                    ().Result;
            return retVal.ToArray();
        }
    }
}
