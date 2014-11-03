using System;
using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.Events;
using Sage.Connector.ProcessExecution.Interfaces;
using IBackOfficeCompanyConfiguration = Sage.Connector.DomainContracts.BackOffice.IBackOfficeCompanyConfiguration;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// Process Request Contract to View Host Side Adapter
    /// </summary>
    /// <remarks>
    /// The HostAdapterAttribute identifies this class as the host-side adapter pipeline segment.
    /// </remarks>
    [HostAdapter]
    public class ProcessRequestContractToViewHostSideAdapter : IProcessRequest , IDisposable
    {

        private readonly IProcessRequestContract _contract;

        private readonly ContractHandle _handle;

        private readonly ResponseEventHandlerViewToContractHostAdapter _responseHandler;

// ReSharper disable once InconsistentNaming
        private static readonly System.Reflection.MethodInfo _ResponseEventAddFire;

        private event EventHandler<ResponseEventArgs> ResponseHandler;

        // Track whether Dispose has been called. 
        private bool _disposed;

        /// <summary>
        /// Process Response event 
        /// </summary>
        public event EventHandler<ResponseEventArgs> ProcessResponse
        {
            add
            {
                if (ResponseHandler == null)
                {
                    _contract.ResponseEventAdd(_responseHandler);
                }
                ResponseHandler += value;
            }
            remove
            {
                ResponseHandler -= value;
                if (ResponseHandler == null)
                {
                    _contract.ResponseEventRemove(_responseHandler);
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        static ProcessRequestContractToViewHostSideAdapter()
        {
            _ResponseEventAddFire = typeof(ProcessRequestContractToViewHostSideAdapter).GetMethod("Fire_Response", ((System.Reflection.BindingFlags)(36)));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract"><see cref="IProcessRequestContract"/></param>
        public ProcessRequestContractToViewHostSideAdapter(IProcessRequestContract contract)
        {
            _contract = contract;
            _handle = new ContractHandle(contract);
            _responseHandler = new ResponseEventHandlerViewToContractHostAdapter(this, _ResponseEventAddFire);
        }

        /// <summary>
        /// Initialize with the application object
        /// </summary>
        /// <param name="appObj"><see cref="IApp"/></param>
        public void Initialize(IApp appObj)
        {
            _contract.Initialize(AppHostAdapter.ViewToContractAdapter(appObj));
        }

        /// <summary>
        /// 
        /// </summary>
        public void RequestCancellation()
        {
            _contract.RequestCancellation();
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="requestId">The request id</param>
        /// <param name="tenantId">The tenant id</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="backOfficeCompanyConfiguration"><see cref="IBackOfficeCompanyConfiguration" /></param>
        /// <param name="featureId">The request feature</param>
        /// <param name="payload">The request payload</param>
        public void ProcessRequest(Guid requestId, string tenantId, Guid trackingId, IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string featureId, string payload)
        {
            _contract.ProcessRequest(requestId, tenantId, trackingId, BackOfficeCompanyConfigHostAdapter.ViewToContractAdapter(backOfficeCompanyConfiguration), featureId, payload);

        }

        /// <summary>
        /// Fire the response event
        /// </summary>
        /// <param name="args"></param>
        internal virtual void Fire_Response(ResponseEventArgs args)
        {
            if ((ResponseHandler == null))
            {
            }
            else
            {
                ResponseHandler.Invoke(this, args);
            }
        }

        /// <summary>
        /// Get the Process Request source contract
        /// </summary>
        /// <returns><see cref="IProcessRequestContract"/></returns>
        internal IProcessRequestContract GetSourceContract()
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



