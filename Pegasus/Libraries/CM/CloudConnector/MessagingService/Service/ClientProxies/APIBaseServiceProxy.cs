using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.Headers;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;
using Sage.Connector.Cloud.Integration.Interfaces.Utils;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;

namespace Sage.Connector.MessagingService.ClientProxies
{
    internal class APIBaseServiceProxy : IDisposable
    {
        protected String _premiseKey;
        protected String _wireClaim;
        protected HttpClient _client;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method; rule does not apply")]
        public APIBaseServiceProxy(
            Uri endpointAddress,
            string resourcePath,
            string tenantId,
            string premiseKey,
            string wireClaim,
            PremiseAgent premiseAgent,
            MessageLogger logger)
        {
            _premiseKey = premiseKey;
            _wireClaim = wireClaim;
            _client = new HttpClient()
            {
                BaseAddress = endpointAddress
                // TODO: Configuration supplied buffer size and time out values
                //client.MaxResponseContentBufferSize;
                //client.Timeout;
            };

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _client.DefaultRequestHeaders.Add(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.TenantId], tenantId);
            if (null != premiseAgent)
                _client.DefaultRequestHeaders.Add(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.PremiseAgent],
                    Utils.JsonSerialize(premiseAgent));
            _client.DefaultRequestHeaders.Add(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.ConnectorId],
                ConnectorRegistryUtils.ConnectorInstanceGuid.ToString());
            _client.DefaultRequestHeaders.Add("UserClaim", _wireClaim);

            ResourceUri = resourcePath;
        }

        public String ResourceUri { get; set; }

        public HttpResponseMessage GetResponse(String id = null)
        {
            string invokedResourceUri = ResourceUri;
            if (!String.IsNullOrEmpty(id))
            {
                invokedResourceUri += "/" + id;
            }

            UpdateCNonce();
            var result = _client.GetAsync(invokedResourceUri).Result;
            result.EnsureSuccessStatusCode();
            return result;
        }

        public WebAPIMessage GetMessage(String id = null)
        {
            var response = GetResponse(id);
            var message = response.Content.ReadAsAsync<WebAPIMessage>().Result;
            if (ValidWebAPIMessage(message))
                return message;
            return null;
        }

        public Boolean ValidWebAPIMessage(WebAPIMessage message)
        {
            var msh = new MessageHashManager(_premiseKey);
            var computedHash = msh.ComputeMessageHash(message.Body);
            return (computedHash == message.BodyHash);
        }

        public HttpResponseMessage Post(IEnumerable<WebAPIMessage> messages)
        {
            UpdateCNonce();
            var result = _client.PostAsJsonAsync(ResourceUri, messages).Result;
            result.EnsureSuccessStatusCode();
            return result;
        }

        public HttpResponseMessage PostMessage(string id, WebAPIMessage message)
        {
            string invokedResourceUri = ResourceUri;
            if (!String.IsNullOrEmpty(id))
            {
                invokedResourceUri += "/" + id;
            }

            UpdateCNonce();
            var result = _client.PostAsJsonAsync(invokedResourceUri, message).Result;
            result.EnsureSuccessStatusCode();
            return result;
        }

        private void UpdateCNonce()
        {
            if (_client.DefaultRequestHeaders.Contains(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.CNonce]))
                _client.DefaultRequestHeaders.Remove(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.CNonce]);
            _client.DefaultRequestHeaders.Add(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.CNonce], Guid.NewGuid().ToString());
        }

        public void Dispose()
        {
            try
            {
                if (null != _client)
                    _client.Dispose();
            }
            finally
            {
                _client = null;
            }
        }

    }
}
