using System;

namespace Sage.Connector.ProcessExecution.Events
{
    /// <summary>
    /// Response Event Args Abstract class
    /// </summary>
    public abstract class ResponseEventArgs : System.ComponentModel.CancelEventArgs
    {
        /// <summary>
        /// The Response payload
        /// </summary>
        public abstract string Payload { get; }

        /// <summary>
        /// The Request Id
        /// </summary>
        public abstract Guid RequestId { get; }
    
        /// <summary>
        /// Boolean flag to determine if the response is complete.
        /// </summary>
        public abstract bool Completed { get; }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public abstract string TenantId { get; }
        /// <summary>
        /// Gets the tracking identifier.
        /// </summary>
        /// <value>
        /// The tracking identifier.
        /// </value>
        public abstract Guid TrackingId { get; }
    }
}
