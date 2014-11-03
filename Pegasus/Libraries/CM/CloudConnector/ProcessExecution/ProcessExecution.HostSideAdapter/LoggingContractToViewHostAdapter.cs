using System;
using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    class LoggingContractToViewHostAdapter : ILogging, IDisposable
    {
        private readonly ILoggingContract _contract;
        
        private ContractHandle _handle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        public LoggingContractToViewHostAdapter(ILoggingContract contract)
        {
            _contract = contract;
            _handle = new ContractHandle(contract);
        }
        internal ILoggingContract GetSourceContract()
        {
            return _contract;
        }

        public void WriteVerbose(string caller, string message)
        {
            _contract.WriteWarning(caller, message);
        }

        public void WriteInfo(string caller, string message)
        {
            _contract.WriteInfo(caller, message);
        }

        public void WriteWarning(string caller, string message)
        {
            _contract.WriteWarning(caller, message);
        }

        public void WriteError(string caller, string message)
        {
            _contract.WriteError(caller, message);
        }

        public void WriteCriticalWithEventLogging(string caller, string source, string message)
        {
            _contract.WriteCriticalWithEventLogging(caller, source, message);
        }


        public void WriteCriticalForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _contract.WriteCriticalForRequest(caller, requestId, tenantId, trackingId, message);
        }

        public void WriteErrorForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _contract.WriteErrorForRequest(caller, requestId, tenantId, trackingId, message);
        }

        public void WriteWarningForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _contract.WriteWarningForRequest(caller, requestId, tenantId, trackingId, message);
        }

        public void WriteInfoForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            _contract.WriteInfoForRequest(caller, requestId, tenantId, trackingId, message);
        }

        public void AdvanceActivityState(Object caller, Guid requestId, string tenantId, Guid trackingId, int newState, int newStatus)
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
