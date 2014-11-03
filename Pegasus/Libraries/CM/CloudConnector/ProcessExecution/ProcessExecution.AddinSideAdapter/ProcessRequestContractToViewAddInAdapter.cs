using System;
using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.AddinView.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// Process Request Contract to View AddIn Adapter
    /// </summary>
    public class ProcessRequestContractToViewAddInAdapter : IProcessRequest, IDisposable
    {

        private readonly IProcessRequestContract _contract;

        private readonly System.AddIn.Pipeline.ContractHandle _handle;

        private readonly ResponseEventHandlerViewToContractAddInAdapter _responseHandler;

// ReSharper disable once InconsistentNaming
        private static readonly System.Reflection.MethodInfo _ResponseEventAddFire;

        // Track whether Dispose has been called. 
        private bool _disposed;

        /// <summary>
        /// Response event adds and removes response event handlers
        /// </summary>
        public event EventHandler<ResponseEventArgs> Response
        {
            add
            {
                if (_Response == null)
                {
                    _contract.ResponseEventAdd(_responseHandler);
                }
                _Response += value;
            }
            remove
            {
                _Response -= value;
                if (_Response == null)
                {
                    _contract.ResponseEventRemove(_responseHandler);
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        static ProcessRequestContractToViewAddInAdapter()
        {
            _ResponseEventAddFire = typeof(ProcessRequestContractToViewAddInAdapter).GetMethod("Fire_Response", ((System.Reflection.BindingFlags)(36)));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract"><see cref="IProcessRequestContract"/></param>
        public ProcessRequestContractToViewAddInAdapter(IProcessRequestContract contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
            _responseHandler = new ResponseEventHandlerViewToContractAddInAdapter(this, _ResponseEventAddFire);
        }

        
// ReSharper disable once InconsistentNaming
        private event EventHandler<ResponseEventArgs> _Response;

        /// <summary>
        /// Initialize with the App object
        /// </summary>
        /// <param name="appObj"></param>
        public void Initialize(IApp appObj)
        {
            _contract.Initialize(AppAddInAdapter.ViewToContractAdapter(appObj));
        }

        /// <summary>
        /// 
        /// </summary>
        public void RequestCancellation()
        {
            _contract.RequestCancellation();
        }


        /// <summary>
        /// Fire the response event 
        /// </summary>
        /// <param name="args"><see cref="ResponseEventArgs"/></param>
        internal virtual void Fire_Response(ResponseEventArgs args)
        {
            if ((_Response == null))
            {
            }
            else
            {
                _Response.Invoke(this, args);
            }
        }

        internal IProcessRequestContract GetSourceContract()
        {
            return _contract;
        }

        /// <summary>
        /// Process Response Event 
        /// </summary>
        public event EventHandler<ResponseEventArgs> ProcessResponse;

        /// <summary>
        /// Process Request
        /// </summary>
        /// <param name="requestId">The request id</param>
        /// <param name="tenantId">The tenant id</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration.</param>
        /// <param name="featureId">The request feature</param>
        /// <param name="payload">The request payload</param>
        public void ProcessRequest(Guid requestId, String tenantId, Guid trackingId, BackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string featureId, string payload)
        {
            _contract.ProcessRequest(requestId, tenantId, trackingId, BackOfficeCompanyConfigAddInAdapter.ViewToContractAdapter(backOfficeCompanyConfiguration), featureId, payload);
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



    }
}
