using System;

namespace Sage.Connector.ProcessExecution.AddinView
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LoggerCore
    {
        /// <summary>
        /// Verbose information
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public abstract void WriteVerbose(string caller, String message);

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public abstract void WriteInfo(string caller, String message);

        /// <summary>
        /// A warning
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public abstract void WriteWarning(string caller, String message);

        /// <summary>
        /// An error possibly recoverable
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public abstract void WriteError(string caller, String message);

        /// <summary>
        /// A critical non recoverable error.
        /// This will write to the windows event log as well.
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public abstract void WriteCriticalWithEventLogging(string caller, string source, string message);



        /// <summary>
        /// Writes the critical for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public abstract void WriteCriticalForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message);

        /// <summary>
        /// Writes the error for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public abstract void WriteErrorForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message);

        /// <summary>
        /// Writes the warning for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public abstract void WriteWarningForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message);

        /// <summary>
        /// Writes the information for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public abstract void WriteInfoForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message);


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
        public abstract void AdvanceActivityState(Object caller, Guid requestId, string tenantId, Guid trackingId, int newState, int newStatus);


        //NOTE: these values must match with ActivityState, ActivityStatus, RequestStatus RequestState or there will be issues...
        //At some point we may want to consider passing these values from the connector core thru process exec so they always sync.

        /// <summary>
        /// Gets the state8_ invoking domain mediation.
        /// </summary>
        /// <value>
        /// The state8_ invoking domain mediation.
        /// </value>
        public int State8_InvokingDomainMediation {get { return 8; } }
        /// <summary>
        /// Gets the state9_ invoking mediation bound work.
        /// </summary>
        /// <value>
        /// The state9_ invoking mediation bound work.
        /// </value>
        public int    State9_InvokingMediationBoundWork {get { return 9; } }
        /// <summary>
        /// Gets the state10_ mediation bound work complete.
        /// </summary>
        /// <value>
        /// The state10_ mediation bound work complete.
        /// </value>
        public int State10_MediationBoundWorkComplete { get { return 10; } }
        /// <summary>
        /// Gets the state11_ domain mediation complete.
        /// </summary>
        /// <value>
        /// The state11_ domain mediation complete.
        /// </value>
        public int State11_DomainMediationComplete { get { return 11; } }
            
        /// <summary>
        /// Gets the in progress.
        /// </summary>
        /// <value>
        /// The in progress.
        /// </value>
        public int InProgress { get { return 1; } }
             
        /// <summary>
        /// Gets the in progress mediation bound work processing.
        /// </summary>
        /// <value>
        /// The in progress mediation bound work processing.
        /// </value>
        public int InProgressMediationBoundWorkProcessing { get { return 1; } }
    }
}
