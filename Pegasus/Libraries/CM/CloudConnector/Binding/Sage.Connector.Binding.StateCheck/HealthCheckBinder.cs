using System;
using System.Threading;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Connector.Utilities;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Diagnostics;

namespace Sage.Connector.Binding
{
    /// <summary>
    /// Binds health check request to health check result delivery
    /// </summary>
    public class HealthCheckBinder : IBindable
    {
        /// <summary>
        /// Basic error status
        /// </summary>
        public static string HEALTH_CHECK_ERROR = "Health Check Error";

        #region IBindable Members

        /// <summary>
        /// Indicates if this IBindable supports the provided request kind
        /// </summary>
        /// <param name="requestKind"></param>
        /// <returns></returns>
        public bool SupportRequestKind(string requestKind)
        { return requestKind == typeof(HealthCheckRequest).FullName; }

        /// <summary>
        /// The actual work of processing the request
        /// Note: Error response only if we cannot make the request to the back office (no config) or
        /// We could not get any kind of response back from the back office.
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
                throw new ArgumentException(String.Format("Unsupported request type: {0}", requestWrapper.ActivityTrackingContext.RequestType));
            }

            ResponseWrapper response = null;

            // If the tenant doesn't exist then create an error response with a delete action
            PremiseConfigurationRecord iConfiguration = ConfigurationSettingFactory.RetrieveConfiguration(requestWrapper.ActivityTrackingContext.TenantId);
            if (iConfiguration == null)
            {
                ErrorResponse configurationMissingErrorResponse = ErrorResponseUtils.GenerateErrorResponse(
                        requestWrapper.ActivityTrackingContext.RequestId, ConfigurationSettingFactory.TENANT_CONFIG_NOT_FOUND, string.Empty);

                response = new ResponseWrapper(requestWrapper, configurationMissingErrorResponse, null);
            }

            if (response == null)
            {
                try
                {

                    // Call the state service to verify the back office
                    ValidateBackOfficeConnectionResponse rawResponse = null;
                    using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        rawResponse = proxy.ValidateBackOfficeConnectionCredentialsAsString(iConfiguration.ConnectorPluginId,iConfiguration.BackOfficeConnectionCredentials);
                    }


                    if (rawResponse != null)
                    {
                        // Convert to health check status
                        // Very simple right now, but left room for more possible statuses
                        HealthCheckStatus healthCheckStatus =
                            (rawResponse.BackOfficeConnectivityStatus == BackOfficeConnectivityStatus.Normal)
                            ? HealthCheckStatus.Passed : HealthCheckStatus.Failed;

                        // Create the actual health check response
                        HealthCheckRequestResponse innerResponse = new HealthCheckRequestResponse(
                            requestWrapper.ActivityTrackingContext.RequestId, Guid.NewGuid(), DateTime.UtcNow,
                            healthCheckStatus, rawResponse.UserFacingMessages, rawResponse.RawErrorMessage);

                        response = new ResponseWrapper(requestWrapper, innerResponse, null);
                    }
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

            if (response == null)
            {
                // Retrieve retry count from the original request
                var request = Utils.JsonDeserialize<Request>(requestWrapper.RequestPayload);

                // No response created, default to a retry error response
                ErrorResponse genericErrorResponse = ErrorResponseUtils.GenerateRetryErrorResponse(
                    requestWrapper.ActivityTrackingContext.RequestId,
                    HEALTH_CHECK_ERROR,
                    string.Empty,
                    request != null ? request.RetryCount : 0,
                    null);

                response = new ResponseWrapper(requestWrapper, genericErrorResponse, null);
            }

            return response;
        }

        #endregion


        #region Private Members

        private static readonly String _myTypeName = typeof(HealthCheckBinder).FullName;

        #endregion
    }
}
