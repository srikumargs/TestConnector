using System;
using Sage.Connector.ProcessExecution.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// Response Event Handler Contract To View Hos tAdapter
    /// </summary>
    public class ResponseEventHandlerContractToViewHostAdapter : IDisposable
    {

        private readonly IResponseEventHandler _contract;

        private readonly System.AddIn.Pipeline.ContractHandle _handle;

        // Track whether Dispose has been called. 
        private bool _disposed;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract"><see cref="IResponseEventHandler"/></param>
        public ResponseEventHandlerContractToViewHostAdapter(IResponseEventHandler contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
        }

        /// <summary>
        /// Handler for the Response event
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="args"><see cref="ResponseEventArgs"/></param>
        public void Handler(object sender, ResponseEventArgs args)
        {
            if (_contract.Handler(ResponseEventArgsHostAdapter.ViewToContractAdapter(args)))
            {
                args.Cancel = true;
            }
        }

        /// <summary>
        /// Get the Response Event Handler source contract
        /// </summary>
        /// <returns><see cref="IResponseEventHandler"/></returns>
        internal IResponseEventHandler GetSourceContract()
        {
            return _contract;
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
            if (_disposed) return;

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
}
