using System;
using System.Net.Http;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;

namespace Sage.Connector.MessagingService.ClientProxies
{
    internal sealed class APIConfigurationServiceProxy : APIBaseServiceProxy
    {
        public APIConfigurationServiceProxy(
            Uri endpointAddress,
            string resourcePath,
            string tenantId,
            string premiseKey,
            string wireClaim,
            PremiseAgent premiseAgent,
            MessageLogger logger)
            : base(endpointAddress, resourcePath, tenantId, premiseKey, wireClaim, premiseAgent, logger) {}

        public Cloud.Integration.Interfaces.WebAPI.Configuration GetConfiguration()
        {
            var response = GetResponse();
            var serializedConfiguration = response.Content.ReadAsAsync<String>().Result;
            var deserializedConfiguration =
                Newtonsoft.Json.JsonConvert.DeserializeObject<Cloud.Integration.Interfaces.WebAPI.Configuration>(
                    serializedConfiguration);
            return deserializedConfiguration;
        }
    }
}
