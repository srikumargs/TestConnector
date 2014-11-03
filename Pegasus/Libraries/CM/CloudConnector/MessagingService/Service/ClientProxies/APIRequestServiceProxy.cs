using System;
using System.Collections.Generic;
using System.Net.Http;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.ExtensionMethods;

namespace Sage.Connector.MessagingService.ClientProxies
{
    internal sealed class APIRequestServiceProxy : APIBaseServiceProxy
    {
        public APIRequestServiceProxy(
            Uri endpointAddress,
            string resourcePath,
            string tenantId,
            string premiseKey,
            string tenantClaim,
            PremiseAgent premiseAgent,
            MessageLogger logger)
            : base(endpointAddress, resourcePath, tenantId, premiseKey, tenantClaim, premiseAgent, logger) { }

        public Request[] GetRequests()
        {
            var response = GetResponse();
            var retVal = new List<Request>();
            var messages = response.Content.ReadAsAsync<IEnumerable<WebAPIMessage>>().Result;
            messages.ForEach(
                msg =>
                {
                    var request = WebAPIMessageHelper.ConvertWebAPIMessageToRequest(msg, _premiseKey);
                    if (null != request)
                    {
                        retVal.Add(request);
                    }
                    else
                    {
                        // TODO: Log warning
                    }
                });

            return retVal.ToArray();
        }

        public IEnumerable<WebAPIMessage> GetRequestMessages()
        {
            var response = GetResponse();
            return response.Content.ReadAsAsync<IEnumerable<WebAPIMessage>>().Result;
        }
    }
}
