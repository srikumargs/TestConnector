
using System;
using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.AddinView.Events;

namespace Sage.Connector.ProcessExecution.AddinView
{
    /// <summary>
    /// The Process Execution Request AddIn View
    /// </summary>
    [AddInBase]
    public interface IProcessRequest
    {
        /// <summary>
        /// The Process Response Event
        /// </summary>
        event EventHandler<ResponseEventArgs> ProcessResponse;

        /// <summary>
        /// The Initialize method to pass the application object.
        /// </summary>
        /// <param name="appObj"></param>
        void Initialize(IApp appObj);

        /// <summary>
        /// 
        /// </summary>
        void RequestCancellation();

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="requestId">The request id</param>
        /// <param name="tenantId">The tenant id</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration.</param>
        /// <param name="featureId">The request feature</param>
        /// <param name="payload">The request payload</param>
        void ProcessRequest(Guid requestId, String tenantId, Guid trackingId, BackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string featureId, string payload);
    }
}
