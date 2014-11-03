using System;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.ProcessExecution.Events;

namespace Sage.Connector.ProcessExecution.Interfaces
{
    /// <summary>
    /// The Process Request interface
    /// </summary>
    public interface IProcessRequest
    {
        /// <summary>
        /// The event used to notify the application that a response is ready.
        /// </summary>
        event EventHandler<ResponseEventArgs> ProcessResponse;

        /// <summary>
        /// Initialize the request processing system with the host application contract instance. 
        /// </summary>
        /// <param name="appObj"><see cref="IApp"/></param>
        void Initialize(IApp appObj);

        /// <summary>
        /// Request that the started process cancel
        /// </summary>
        void RequestCancellation();

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="requestId">The request id</param>
        /// <param name="tenantId">The tenant id</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="backOfficeCompanyConfiguration"><see cref="IBackOfficeCompanyConfiguration" /></param>
        /// <param name="featureId">The request feature</param>
        /// <param name="payload">The request payload</param>
        void ProcessRequest(Guid requestId, String tenantId, Guid trackingId, IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string featureId, string payload);
    }
}
