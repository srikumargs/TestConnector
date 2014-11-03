using System;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Diagnostics;
using System.Threading;

namespace Sage.Connector.DispatchService.Internal
{
    /// <summary>
    /// A binder that instantiate a 'no applicable' binder binding
    /// </summary>
    public class NoApplicableBinderBinder : IBindable
    {
        /// <summary>
        /// Basic error status
        /// </summary>
        public static string BINDER_ERROR = "The premise connector does not support this request.";

        /// <summary>
        /// Supports all requests
        /// </summary>
        /// <param name="requestKind"></param>
        /// <returns></returns>
        public bool SupportRequestKind(string requestKind)
        {
            return true;
        }

        /// <summary>
        /// Generates an error response for no applicable premise binder
        /// </summary>
        /// <param name="requestWrapperMessage"></param>
        /// <param name="cancellationTokenSource"></param>
        /// <param name="maxBlobSize"></param>
        /// <returns></returns>
        public ResponseWrapper InvokeWork(RequestWrapper requestWrapperMessage, CancellationTokenSource cancellationTokenSource, long maxBlobSize)
        {
            ArgumentValidator.ValidateNonNullReference(requestWrapperMessage, "requestWrapperMessage", _myTypeName + ".InvokeWork()");
            ArgumentValidator.ValidateNonNullReference(requestWrapperMessage.ActivityTrackingContext, "requestWrapperMessage.ActivityTrackingContext", _myTypeName + ".InvokeWork()");
            if (!SupportRequestKind(requestWrapperMessage.ActivityTrackingContext.RequestType))
            {
                throw new ArgumentException(String.Format("Unsupported request type: {0}", requestWrapperMessage.ActivityTrackingContext.RequestType));
            }

            ErrorResponse noBinderErrorResponse = ErrorResponseUtils.GenerateErrorResponse(
                        requestWrapperMessage.ActivityTrackingContext.RequestId, BINDER_ERROR, string.Empty);
            ResponseWrapper response = new ResponseWrapper(requestWrapperMessage, noBinderErrorResponse, null);

            return response;
        }

        private static readonly String _myTypeName = typeof(NoApplicableBinderBinder).FullName;
    }
}
