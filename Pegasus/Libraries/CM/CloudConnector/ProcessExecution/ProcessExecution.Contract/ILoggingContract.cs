using System;
using System.AddIn.Contract;

namespace Sage.Connector.ProcessExecution.AddinView
{
    /// <summary>
    /// A basic interface abstraction around certain methods we would like to be able to call on the LogManager
    /// from inside Common, but cannot, because the Common assembly would then create a circular reference on
    /// the Logging assembly.
    /// </summary>
    /// <remarks>
    /// Clone of the primary logging interface in core connector. 
    /// CPR in this case so that core of the process execution does not take a dependency on the core common libs of connector.
    /// </remarks>
    public interface ILoggingContract : IContract
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
        /// <param name="source">activity name shows up as "during {0}" in output</param>
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
