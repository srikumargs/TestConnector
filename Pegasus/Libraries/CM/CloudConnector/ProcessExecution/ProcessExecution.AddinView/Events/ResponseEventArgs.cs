
using System;

namespace Sage.Connector.ProcessExecution.AddinView.Events
{
    /// <summary>
    /// The Response Event Args
    /// </summary>
    public abstract class ResponseEventArgs : System.ComponentModel.CancelEventArgs
    {
        /// <summary>
        /// The response payload
        /// </summary>
        public abstract string Payload { get; }

        /// <summary>
        /// The request id
        /// </summary>
        public abstract Guid RequestId { get; }

        /// <summary>
        /// True if the response if complete, otherwise false.
        /// </summary>
        public abstract bool Completed { get; }


        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        public abstract string TenantId { get; }

        /// <summary>
        /// Gets the tracking identifier.
        /// </summary>
        public abstract Guid TrackingId { get; }
    }

}
