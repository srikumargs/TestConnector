using System;
using System.Net.Http;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;

namespace Sage.Connector.MessagingService.ClientProxies
{
    internal sealed class APIBeginUploadSessionServiceProxy : APIBaseServiceProxy
    {
        public APIBeginUploadSessionServiceProxy(
            Uri endpointAddress,
            string resourcePath,
            string tenantId,
            string premiseKey,
            string tenantClaim,
            PremiseAgent premiseAgent,
            MessageLogger logger) : base(endpointAddress, resourcePath, tenantId, premiseKey, tenantClaim, premiseAgent, logger) {}

        public UploadSessionInfo CreateAndBeginUploadSession(
            Guid requestGuid,
            Guid uploadGuid,
            string premiseDocumentId,
            string purposeDescription,
            int expectedSizeInBytes)
        {
            var message = new WebAPIMessage()
            {
                Id = uploadGuid,
                TimeStamp = DateTime.UtcNow,
                Version = 1,
            };
            var response = PostMessage(requestGuid.ToString(), message);
            var retMessage = response.Content.ReadAsAsync<WebAPIMessage>().Result;
            return retMessage.UploadSessionInfo;
        }
    }
}
