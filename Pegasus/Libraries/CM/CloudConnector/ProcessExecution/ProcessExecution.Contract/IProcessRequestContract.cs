using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;

namespace Sage.Connector.ProcessExecution.Interfaces
{
    /// <summary>
    /// The Process Request AddIn Interface contract
    /// </summary>
    [AddInContract]
    public interface IProcessRequestContract : IContract
    {
        /// <summary>
        /// Initializes the request sending in an application contract used to send and receive
        /// event from the calling application
        /// </summary>
        /// <param name="appContract">Application contract used to send and receive event notifications
        /// to and from the addin.</param>
        void Initialize(IAppContract appContract);

        /// <summary>
        /// Pass a cancel request to the remote
        /// </summary>
        void RequestCancellation();

        /// <summary>
        /// Process Request
        /// </summary>
        /// <param name="requestId">The request id</param>
        /// <param name="tenantId">The tenant id</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="backOfficeCompanyConfiguration"><see cref="IBackOfficeCompanyConfiguration" /></param>
        /// <param name="featureId">The request feature</param>
        /// <param name="payload">The request payload</param>
        void ProcessRequest(Guid requestId, String tenantId, Guid trackingId, IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string featureId, string payload);

        /// <summary>
        /// The Response Event handler Add method
        /// </summary>
        /// <param name="handler"><see cref="IResponseEventHandler"/></param>
        void ResponseEventAdd(IResponseEventHandler handler);

        /// <summary>
        /// The Response Event Handler Remove method
        /// </summary>
        /// <param name="handler"><see cref="IResponseEventHandler"/></param>
        void ResponseEventRemove(IResponseEventHandler handler);

    }

    #region Events fired by the add-in

    /// <summary>
    /// Response Event Handler interface contract
    /// </summary>
    public interface IResponseEventHandler : IContract
    {
        /// <summary>
        /// Response handler which returns whether or not the event was canceled.
        /// </summary>
        /// <param name="args"><see cref="IResponseEventArgs"/></param>
        /// <returns>True if the event was canceled, false otherwise</returns>
        bool Handler(IResponseEventArgs args);
    }

    /// <summary>
    /// Response Event Args interface contract
    /// </summary>
    public interface IResponseEventArgs : IContract
    {
        /// <summary>
        /// The Response payload
        /// </summary>
        string Payload { get; }

        /// <summary>
        /// The Request id
        /// </summary>
        Guid RequestId { get; }

        /// <summary>
        /// True if the response if complete, otherwise false.
        /// </summary>
        bool Completed { get; }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        string TenantId { get; }
        /// <summary>
        /// Gets the tracking identifier.
        /// </summary>
        /// <value>
        /// The tracking identifier.
        /// </value>
        Guid TrackingId { get; }
    }

    #endregion
}

