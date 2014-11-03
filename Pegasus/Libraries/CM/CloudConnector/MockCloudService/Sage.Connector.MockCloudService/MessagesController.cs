using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Owin;
using Sage.Connector.Cloud.Integration.Interfaces.Headers;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Cloud.Integration.Interfaces.Utils;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.ExtensionMethods;

namespace Sage.Connector.SageCloudService
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiRouteConfiguration
    {
        /// <summary>
        /// Configures routing for the message controller
        /// </summary>
        /// <param name="appBuilder"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            appBuilder.UseWebApi(config);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessagesController : ApiController
    {
        /// <summary>
        /// Inject a request for the tenant
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        [Route("api/messages/requests")]
        [HttpPost]
        public WebAPIMessage PostRequestMessage(WebAPIMessage requestMessage)
        {
            IEnumerable<String> responseTenantIds;
            if (!Request.Headers.TryGetValues(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.TenantId], out responseTenantIds))
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            if (AddRequest(responseTenantIds.FirstOrDefault(), requestMessage))
                return requestMessage;
            return GetUnknownMessage();
        }

        /// <summary>
        /// Retrieves messages for the tenant
        /// </summary>
        /// <returns></returns>
        [Route("api/messages/requests")]
        [HttpGet]
        public IEnumerable<WebAPIMessage> GetRequestMessages()
        {
            IEnumerable<String> requestTenantIds;
            //if (!Request.Headers.TryGetValues("tenantId", out requestTenantIds) ||
            //    (null == requestTenantIds) || 
            //    (1 != requestTenantIds.Count()))
            //    return new List<WebAPIMessage>();

            if (!Request.Headers.TryGetValues(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.TenantId], out requestTenantIds))
                requestTenantIds = new List<String>()
                {
                    Sage.Connector.SageCloudService.MockCloudService.TenantIds.First()
                };

            return RetrieveMessages(requestTenantIds.FirstOrDefault());
        }

        /// <summary>
        /// Inject a response for a tenant request
        /// </summary>
        /// <param name="requestGuid"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        [Route("api/messages/responses/{requestGuid}")]
        [HttpPost]
        public WebAPIMessage PostResponseMessage(Guid requestGuid, WebAPIMessage responseMessage)
        {
            IEnumerable<String> responseTenantIds;
            if (!Request.Headers.TryGetValues(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.TenantId], out responseTenantIds))
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            if (ProcessResponse(responseTenantIds.FirstOrDefault(), responseMessage, requestGuid))
                return responseMessage;
            return GetUnknownMessage();
        }

        /// <summary>
        /// Retrieve a tenant response message for a request
        /// </summary>
        /// <param name="requestGuid"></param>
        /// <returns></returns>
        [Route("api/messages/responses/{requestGuid}")]
        [HttpGet]
        public WebAPIMessage GetResponseMessage(Guid requestGuid)
        {
            IEnumerable<String> responseTenantIds;
            if (!Request.Headers.TryGetValues(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.TenantId], out responseTenantIds))
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            return RetrieveResponse(responseTenantIds.FirstOrDefault(), requestGuid);
        }

        /// <summary>
        /// Retrieve upload session information for a request
        /// </summary>
        /// <param name="requestGuid"></param>
        /// <param name="largeUploadRequestMessage"></param>
        /// <returns></returns>
        [Route("api/messages/requests/startuploadrequest/{requestGuid}")]
        [HttpPost]
        public WebAPIMessage PostRequestLargeUpload(Guid requestGuid, WebAPIMessage largeUploadRequestMessage)
        {
            IEnumerable<String> requestTenantIds;
            //if (!Request.Headers.TryGetValues("tenantId", out requestTenantIds) ||
            //    (null == requestTenantIds) || 
            //    (1 != requestTenantIds.Count()))
            //    return new List<WebAPIMessage>();

            if (!Request.Headers.TryGetValues(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.TenantId], out requestTenantIds))
                requestTenantIds = new List<String>()
                {
                    Sage.Connector.SageCloudService.MockCloudService.TenantIds.First()
                };

            return GetUploadSessionMessage(requestTenantIds);
        }

        /// <summary>
        /// Indicates upload session completed for a upload request
        /// </summary>
        /// <param name="requestGuid"></param>
        /// <param name="uploadGuid"></param>
        /// <param name="largeUploadResponseMessage"></param>
        /// <returns></returns>
        [Route("api/messages/responses/enduploadrequest/{requestGuid}/{uploadGuid}")]
        [HttpPost]
        public WebAPIMessage PostResponseLargeUploadCompleted(Guid requestGuid, Guid uploadGuid, WebAPIMessage largeUploadResponseMessage)
        {
            // TODO: Anything to do to mark upload completion?
            return GetUnknownMessage();
        }

        /// <summary>
        /// Retrieves tenant configuration information
        /// </summary>
        /// <returns></returns>
        [Route("api/configuration")]
        [HttpGet]
        public String GetConfigurationMessage()
        {
            IEnumerable<String> requestTenantIds;
            //if (!Request.Headers.TryGetValues("tenantId", out requestTenantIds) ||
            //    (null == requestTenantIds) || 
            //    (1 != requestTenantIds.Count()))
            //    return new List<WebAPIMessage>();

            if (!Request.Headers.TryGetValues(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.TenantId], out requestTenantIds))
                requestTenantIds = new List<String>()
                {
                    Sage.Connector.SageCloudService.MockCloudService.TenantIds.First()
                };

            return Newtonsoft.Json.JsonConvert.SerializeObject(GetConfigurationMessage(requestTenantIds));
        }

        /// <summary>
        /// Retrieves tenant registration information
        /// </summary>
        /// <returns></returns>
        [Route("api/tenantregistration")]
        [HttpGet]
        public String GetTenantRegistration()
        {
            IEnumerable<String> requestTenantIds;
            //if (!Request.Headers.TryGetValues("tenantId", out requestTenantIds) ||
            //    (null == requestTenantIds) || 
            //    (1 != requestTenantIds.Count()))
            //    return new List<WebAPIMessage>();

            if (!Request.Headers.TryGetValues(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.TenantId], out requestTenantIds))
                requestTenantIds = new List<String>()
                {
                    Sage.Connector.SageCloudService.MockCloudService.TenantIds.First()
                };

            return Newtonsoft.Json.JsonConvert.SerializeObject(GetTenantRegistrationMessage(requestTenantIds));
        }

        /// <summary>
        /// Retrieves tenant List from mock
        /// </summary>
        /// <returns></returns>
        [Route("api/tenantList")]
        [HttpGet]
        public IEnumerable<Cloud.Integration.Interfaces.WebAPI.TenantInfo> GetTenantList()
        {
            return BuildTenantRecords();
        }

        #region MessageRouting

        /// <summary>
        /// Upload Session Info
        /// </summary>
        /// <returns></returns>
        private WebAPIMessage GetUploadSessionMessage(IEnumerable<String> tenantIds)
        {
            var tenantContainer =
                MockCloudService.GetTenantContainer(tenantIds.FirstOrDefault());
            var premiseKey = tenantContainer.PremiseKey;
            var body = Utils.JsonSerialize(Utils.MockUploadSessionInfo);
            return new WebAPIMessage()
            {
                Id = Guid.NewGuid(),
                BodyType = Utils.MockUploadSessionInfo.GetType().FullName,
                TimeStamp = DateTime.UtcNow,
                Version = 1,
                Body = body,
                BodyHash = new MessageHashManager(premiseKey).ComputeMessageHash(body),
                CorrelationId = Guid.Empty,
                UploadSessionInfo = Utils.MockUploadSessionInfo
            };
        }

        private Cloud.Integration.Interfaces.WebAPI.Configuration GetConfigurationMessage(IEnumerable<String> tenantIds)
        {
            var retConfig = new Cloud.Integration.Interfaces.WebAPI.Configuration();

            if ((null != tenantIds && 1 == tenantIds.Count()))
            {
                var tenantContainer = MockCloudService.GetTenantContainer(tenantIds.FirstOrDefault());
                retConfig = tenantContainer.ConfigParams;
            }

            return retConfig;
        }

        private Cloud.Integration.Interfaces.WebAPI.TenantRegistration GetTenantRegistrationMessage(IEnumerable<String> tenantIds)
        {
            var retRegistration = new Cloud.Integration.Interfaces.WebAPI.TenantRegistration();

            if ((null != tenantIds && 1 == tenantIds.Count()))
            {
                var tenantContainer = MockCloudService.GetTenantContainer(tenantIds.FirstOrDefault());
                retRegistration.TenantId = tenantContainer.TenantId;
                retRegistration.TenantKey = tenantContainer.PremiseKey;
                retRegistration.TenantClaim = tenantContainer.PremiseKey;
                retRegistration.TenantName = tenantContainer.Name;
                retRegistration.TenantUrl = tenantContainer.ConfigParams.TenantPublicUri;
                retRegistration.SiteAddressBaseUri = tenantContainer.ConfigParams.SiteAddressBaseUri;
            }

            return retRegistration;
        }

        /// <summary>
        /// Unrecognized Gets
        /// </summary>
        /// <returns></returns>
        private WebAPIMessage GetUnknownMessage()
        {
            return new WebAPIMessage();
        }

        #endregion


        #region Tenant Request / Response Handlers

        private IEnumerable<WebAPIMessage> RetrieveMessages(String tenantId)
        {
            var tenantContainer = MockCloudService.GetTenantContainer(tenantId);
            var premiseKey = tenantContainer.PremiseKey;
            var requests = tenantContainer.GetRequests();
            var messages = new List<WebAPIMessage>();
            requests.ForEach(request => messages.Add(WebAPIMessageHelper.ConvertRequestToWebAPIMessage(request, premiseKey)));
            return messages;
        }

        private bool ProcessResponse(String tenantId, WebAPIMessage message, Guid requestGuid)
        {
            var tenantContainer = MockCloudService.GetTenantContainer(tenantId);

            if (String.IsNullOrEmpty(message.Body))
            {
                // Assume message went thru BLOB, but it is not here
                tenantContainer.RemoveSpecificMessageFromCloudOutbox(requestGuid);
                return true;
            }

            var premiseKey = tenantContainer.PremiseKey;
            var responses = new List<Response>();
            var response = WebAPIMessageHelper.ConvertWebAPIMessageToResponse(message, premiseKey);
            if (null != response)
            {
                responses.Add(response);
                tenantContainer.PutResponses(responses.ToArray());
                return true;
            }
            return false;
        }

        private bool AddRequest(String tenantId, WebAPIMessage message)
        {
            var tenantContainer = MockCloudService.GetTenantContainer(tenantId);
            var premiseKey = tenantContainer.PremiseKey;
            var request = WebAPIMessageHelper.ConvertWebAPIMessageToRequest(message, premiseKey);
            if (null != request)
            {
                tenantContainer.ExternalAddToPremiseMessage(request);
                return true;
            }
            return false;
        }

        private WebAPIMessage RetrieveResponse(String tenantId, Guid requestGuid)
        {
            var tenantContainer = MockCloudService.GetTenantContainer(tenantId);
            var premiseKey = tenantContainer.PremiseKey;
            var response = tenantContainer.GetResponse(requestGuid);
            if (null != response) 
                return WebAPIMessageHelper.ConvertResponseToWebAPIMessage(response, premiseKey);

            return GetUnknownMessage();
        }

        #endregion

        #region TenantList Support

        private IList<Cloud.Integration.Interfaces.WebAPI.TenantInfo> BuildTenantRecords()
        {
            var tenants = new List<Cloud.Integration.Interfaces.WebAPI.TenantInfo>();
            foreach (string tenantId in MockCloudService.TenantIds)
            {
                TenantContainer tc = MockCloudService.GetTenantContainer(tenantId);
                var newTenant = new Cloud.Integration.Interfaces.WebAPI.TenantInfo()
                {
                    RegisteredCompany = String.Empty,
                    RegisteredConnectorId = String.Empty,
                    TenantGuid = Guid.Parse(tc.TenantId),
                    TenantName = tc.Name
                };
                tenants.Add(newTenant);
            }
            return tenants;
        }

        #endregion
    }
}

