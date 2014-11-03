using System;
using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// 
    /// </summary>
    public class LoggingViewToContractHostAdapter : ContractBase, ILoggingContract
    {
        private readonly ILogging _view;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        public LoggingViewToContractHostAdapter(ILogging view)
        {
            _view = view;
        }
        
        internal ILogging GetSourceView() {
            return _view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public void WriteVerbose(string caller, string message)
        {
            _view.WriteVerbose(caller, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public void WriteInfo(string caller, string message)
        {
            _view.WriteInfo(caller, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public void WriteWarning(string caller, string message)
        {
            _view.WriteWarning(caller, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public void WriteError(string caller, string message)
        {
            _view.WriteError(caller, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void WriteCriticalWithEventLogging(string caller, string source, string message)
        {
            _view.WriteCriticalWithEventLogging(caller, source, message);
        }


        /// <summary>
        /// Writes the critical for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public void WriteCriticalForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _view.WriteCriticalForRequest(caller, requestId, tenantId, trackingId, message);
        }

        /// <summary>
        /// Writes the error for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public void WriteErrorForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _view.WriteErrorForRequest(caller, requestId, tenantId, trackingId, message);
        }

        /// <summary>
        /// Writes the warning for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public void WriteWarningForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _view.WriteWarningForRequest(caller, requestId, tenantId, trackingId, message);
        }

        /// <summary>
        /// Writes the information for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public void WriteInfoForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _view.WriteInfoForRequest(caller, requestId, tenantId, trackingId, message);
        }

        /// <summary>
        /// Advances the state of the activity.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="newState">The new state.</param>
        /// <param name="newStatus">The new status.</param>
        public void AdvanceActivityState(Object caller, Guid requestId, string tenantId, Guid trackingId, int newState, int newStatus)
        {
            _view.AdvanceActivityState(caller, requestId, tenantId, trackingId, newState, newStatus);
        }
    }
}
