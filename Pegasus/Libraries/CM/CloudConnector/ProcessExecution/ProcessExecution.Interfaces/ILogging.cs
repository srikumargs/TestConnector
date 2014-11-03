using System;

namespace Sage.Connector.ProcessExecution.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILogging
    {
        /// <summary>
        /// Verbose information
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        void WriteVerbose(string caller, String message);

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        void WriteInfo(string caller, String message);

        /// <summary>
        /// A warning
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        void WriteWarning(string caller, String message);

        /// <summary>
        /// An error possibly recoverable
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        void WriteError(string caller, String message);

        /// <summary>
        /// A critical non recoverable error.
        /// This will write to the windows event log as well.
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        void WriteCriticalWithEventLogging(string caller, string source, string message);




        /// <summary>
        /// Writes the critical for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        void WriteCriticalForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message);

        /// <summary>
        /// Writes the error for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        void WriteErrorForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message);

        /// <summary>
        /// Writes the warning for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        void WriteWarningForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message);

        /// <summary>
        /// Writes the information for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        void WriteInfoForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message);


        //void AdvanceActivityState(Object caller, IActivityTrackingUpdateContext trackingContext, ActivityState newState, ActivityEntryStatus newStatus)
        /// <summary>
        /// Advances the state of the activity.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="newState">The new state.</param>
        /// <param name="newStatus">The new status.</param>
        void AdvanceActivityState(Object caller, Guid requestId, string tenantId, Guid trackingId, int newState, int newStatus);
    }
}
