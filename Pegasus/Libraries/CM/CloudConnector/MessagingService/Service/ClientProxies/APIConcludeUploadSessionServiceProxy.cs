using System;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;

namespace Sage.Connector.MessagingService.ClientProxies
{
    internal sealed class APIConcludeUploadSessionServiceProxy : APIBaseServiceProxy
    {
        public APIConcludeUploadSessionServiceProxy(
            Uri endpointAddress,
            string resourcePath,
            string tenantId,
            string premiseKey,
            string tenantClaim,
            PremiseAgent premiseAgent,
            MessageLogger logger) : base(endpointAddress, resourcePath, tenantId, premiseKey, tenantClaim, premiseAgent, logger)
        {
        }

        public void ConcludeUploadSession(Guid requestGuid, Guid uploadGuid)
        {
            var message = new WebAPIMessage()
            {
                Id = Guid.NewGuid(),
                TimeStamp = DateTime.UtcNow,
                Version = 1,
            };
            PostMessage(requestGuid + @"/" + uploadGuid, message);
        }

    }
}
