using System;
using System.Threading;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.Utilities;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Diagnostics;

namespace Sage.Connector.Binding
{
    /// <summary>
    /// Binds update site service info request to update service info action
    /// </summary>
    public class UpdateSiteServiceInfoBinder : IBindable
    {
        /// <summary>
        /// Basic error status
        /// </summary>
        public static string UPDATE_SITE_SERVICE_INFO_ERROR = "Update Site Service Info Error";


        #region IBindable Members

        /// <summary>
        /// Indicates if this IBindable supports the provided request kind
        /// </summary>
        /// <param name="requestKind"></param>
        /// <returns></returns>
        public bool SupportRequestKind(string requestKind)
        { return requestKind == typeof(UpdateSiteServiceInfoRequest).FullName; }

        /// <summary>
        /// The actual work of processing the request
        /// </summary>
        /// <param name="requestWrapper"></param>
        /// <param name="cancellationTokenSource"></param>
        /// <param name="maxBlobSize"></param>
        /// <returns></returns>
        public ResponseWrapper InvokeWork(RequestWrapper requestWrapper, CancellationTokenSource cancellationTokenSource, long maxBlobSize)
        {
            ArgumentValidator.ValidateNonNullReference(requestWrapper, "requestWrapperMessage", _myTypeName + ".InvokeWork()");
            ArgumentValidator.ValidateNonNullReference(requestWrapper.ActivityTrackingContext, "requestWrapperMessage.ActivityTrackingContext", _myTypeName + ".InvokeWork()");
            if (!SupportRequestKind(requestWrapper.ActivityTrackingContext.RequestType))
            {
                throw new ArgumentException(String.Format(
                    "Unsupported request type: {0}", 
                    requestWrapper.ActivityTrackingContext.RequestType));
            }

            ResponseWrapper response = null;

            // If the tenant doesn't exist then create an error response with a delete action
            PremiseConfigurationRecord iConfiguration =
                ConfigurationSettingFactory.RetrieveConfiguration(requestWrapper.ActivityTrackingContext.TenantId);
            if (iConfiguration == null)
            {
                ErrorResponse configurationMissingErrorResponse = 
                    ErrorResponseUtils.GenerateErrorResponse(
                        requestWrapper.ActivityTrackingContext.RequestId, 
                        ConfigurationSettingFactory.TENANT_CONFIG_NOT_FOUND,
                        string.Empty);

                response = new ResponseWrapper(
                    requestWrapper,
                    configurationMissingErrorResponse,
                    null);
            }

            if (response == null)
            {
                try
                {
                    // All we need to do is notify subscribers of this event 
                    using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        var updateRequest =
                            Utils.JsonDeserialize<UpdateSiteServiceInfoRequest>(requestWrapper.RequestPayload);
                        proxy.NotifyServiceInfoUpdated(
                            requestWrapper.ActivityTrackingContext.TenantId, 
                            updateRequest.SiteAddressBaseUri);
                    }

                    // Create the successful update service info response
                    UpdateSiteServiceInfoRequestResponse innerResponse = new UpdateSiteServiceInfoRequestResponse(
                        requestWrapper.ActivityTrackingContext.RequestId, Guid.NewGuid(), DateTime.UtcNow);

                    // Create the wrapper for the above response
                    response = new ResponseWrapper(requestWrapper, innerResponse, null);
                }
                catch (Exception ex)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteErrorForRequest(this, requestWrapper.ActivityTrackingContext, ex.ExceptionAsString());
                    }
                    throw;
                }
            }

            return response;
        }

        #endregion


        #region Private Members

        private static readonly String _myTypeName = typeof(UpdateSiteServiceInfoBinder).FullName;

        #endregion
    }
}
