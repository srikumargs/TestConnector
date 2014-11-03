using System;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Connector.Cloud.Integration.Interfaces.Utils;

namespace Sage.Connector.MessagingService.ClientProxies
{
    internal sealed class APIResponseServiceProxy : APIBaseServiceProxy
    {
        public APIResponseServiceProxy(
            Uri endpointAddress,
            string resourcePath,
            string tenantId,
            string premiseKey,
            string tenantClaim,
            PremiseAgent premiseAgent,
            MessageLogger logger)
            : base(endpointAddress, resourcePath, tenantId, premiseKey, tenantClaim, premiseAgent, logger) { }

        public void PutResponse(Response response)
        {
            var message = WebAPIMessageHelper.ConvertResponseToWebAPIMessage(response, _premiseKey);
            PostMessage(response.RequestId.ToString(), message);
        }

        public void PutResponse(Guid originalRequestId, String bodyType, String responsePayload, UploadSessionInfo uploadSessionInfo)
        {
            var bodyHash = String.IsNullOrEmpty(responsePayload)
                ? String.Empty
                : new MessageHashManager(_premiseKey).ComputeMessageHash(responsePayload);
            var message = new WebAPIMessage()
            {
                Id = Guid.NewGuid(),
                BodyType = bodyType,
                TimeStamp = DateTime.UtcNow,
                Version = 1,
                Body = responsePayload,
                BodyHash = bodyHash,
                CorrelationId = originalRequestId,
                UploadSessionInfo = uploadSessionInfo
            };
            PostMessage(originalRequestId.ToString(), message);
        }
    }
}
