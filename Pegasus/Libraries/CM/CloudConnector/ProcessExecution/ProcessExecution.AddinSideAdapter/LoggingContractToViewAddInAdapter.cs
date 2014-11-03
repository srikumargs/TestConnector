using System;
using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.AddinView;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// 
    /// </summary>
    public class LoggingContractToViewAddInAdapter : LoggerCore, IDisposable
    {
        private ILoggingContract _contract;
        private readonly System.AddIn.Pipeline.ContractHandle _handle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        public LoggingContractToViewAddInAdapter(ILoggingContract contract)
        {
            _contract = contract;
            _handle = new ContractHandle(contract);
        }

        internal ILoggingContract GetSourceContract()
        {
            return _contract;
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public override void WriteVerbose(string caller, string message)
        {
           _contract.WriteVerbose(caller, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public override void WriteInfo(string caller, string message)
        {
            _contract.WriteInfo(caller, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public override void WriteWarning(string caller, string message)
        {
            _contract.WriteWarning(caller, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public override void WriteError(string caller, string message)
        {
            _contract.WriteError(caller, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public override void WriteCriticalWithEventLogging(string caller, string source, string message)
        {
            _contract.WriteCriticalWithEventLogging(caller, source, message);
        }


        /// <summary>
        /// Writes the critical for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public override void WriteCriticalForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _contract.WriteCriticalForRequest(caller, requestId, tenantId, trackingId, message);
        }

        /// <summary>
        /// Writes the error for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public override void WriteErrorForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _contract.WriteErrorForRequest(caller, requestId, tenantId, trackingId, message);
        }

        /// <summary>
        /// Writes the warning for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public override void WriteWarningForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _contract.WriteWarningForRequest(caller, requestId, tenantId, trackingId, message);
        }

        /// <summary>
        /// Writes the information for request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="message">The message.</param>
        public override void WriteInfoForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _contract.WriteInfoForRequest(caller, requestId, tenantId, trackingId, message);
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
        public override void AdvanceActivityState(Object caller, Guid requestId, string tenantId, Guid trackingId, int newState, int newStatus)
        {
            _contract.AdvanceActivityState(caller, requestId, tenantId, trackingId, newState, newStatus);
        }

        /// <summary>
        /// Dispose properly of this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios. 
        /// If disposing equals true, the method has been called directly 
        /// or indirectly by a user's code. Managed and unmanaged resources 
        /// can be disposed. 
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed. 
        /// </summary>
        /// <param name="disposing">When false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed. 
        ///  </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    _handle.Dispose();
                }

                // Note disposing has been done.
                _disposed = true;

            }
        }
        // Track whether Dispose has been called. 
        private bool _disposed;


    }
}
