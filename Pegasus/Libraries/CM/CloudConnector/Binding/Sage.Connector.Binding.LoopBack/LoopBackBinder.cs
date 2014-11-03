using System;
using System.Threading;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Diagnostics;

namespace Sage.Connector.Binding
{
    /// <summary>
    /// Binds report list request to reporting list delivery
    /// </summary>
    public class LoopBackBinder : IBindable
    {
        /// <summary>
        /// Basic error status
        /// </summary>
        public static string LOOPBACK_ERROR = "DISPATCH_ERROR";

        #region IBindable Members

        /// <summary>
        /// Returns true for LOOPBACK_MESSAGE; false otherwise
        /// </summary>
        /// <param name="requestKind"></param>
        /// <returns></returns>
        public bool SupportRequestKind(String requestKind)
        { return requestKind == typeof(LoopBackRequest).FullName; }

        /// <summary>
        /// Invokes Work associated with LoopBack Request.
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
            // Create the inner response
            LoopBackRequestResponse innerResponse = new LoopBackRequestResponse(
                requestWrapperMessage.ActivityTrackingContext.RequestId,
                Guid.NewGuid(),
                DateTime.UtcNow);
            // Return the response wrapper
            return new ResponseWrapper(requestWrapperMessage, innerResponse, null);
        }

        #endregion

        private static readonly String _myTypeName = typeof(LoopBackBinder).FullName;

    }
}
