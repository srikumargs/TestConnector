using System;
using System.Net.Http;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;

namespace Sage.Connector.MessagingService.ClientProxies
{
    internal sealed class APIClearConnectorRegistrationProxy : APIBaseServiceProxy
    {
        public APIClearConnectorRegistrationProxy(
            Uri endpointAddress,
            string resoucePath,
            string tenantId,
            PremiseAgent premiseAgent,
            MessageLogger logger)
            : base(endpointAddress, resoucePath, tenantId, string.Empty, string.Empty, premiseAgent, logger)
        {
        }

        public Cloud.Integration.Interfaces.WebAPI.TenantRegistration ClearConnectorRegistration(string authenticationToken)
        {
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authenticationToken);
            var response = GetResponse();
            var serializedRegistration = response.Content.ReadAsAsync<String>().Result;
            var deserializedRegistration =
                Newtonsoft.Json.JsonConvert.DeserializeObject<Cloud.Integration.Interfaces.WebAPI.TenantRegistration>(
                    serializedRegistration);
            return deserializedRegistration;
        }
    }
}
