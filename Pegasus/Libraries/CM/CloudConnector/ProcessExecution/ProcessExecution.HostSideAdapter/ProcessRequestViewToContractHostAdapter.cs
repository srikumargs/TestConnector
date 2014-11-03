using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using Sage.Connector.ProcessExecution.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// Process Request View to Contract Host Adapter
    /// </summary>
    public class ProcessRequestViewToContractHostAdapter : ContractBase, IProcessRequestContract
    {

        private readonly IProcessRequest _view;

        private readonly Dictionary<IResponseEventHandler, EventHandler<ResponseEventArgs>> _responseHandlers;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"><see cref="IProcessRequest"/></param>
        public ProcessRequestViewToContractHostAdapter(IProcessRequest view)
        {
            _view = view;
            _responseHandlers = new Dictionary<IResponseEventHandler, EventHandler<ResponseEventArgs>>();
        }

        /// <summary>
        /// Initialize with the app object 
        /// </summary>
        /// <param name="appObj"><see cref="IAppContract"/></param>
        public virtual void Initialize(IAppContract appObj)
        {
            _view.Initialize(AppHostAdapter.ContractToViewAdapter(appObj));
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void RequestCancellation()
        {
            _view.RequestCancellation();
        }

        /// <summary>
        /// Add the Response Event Handler
        /// </summary>
        /// <param name="handler"><see cref="IResponseEventHandler"/></param>
        public virtual void ResponseEventAdd(IResponseEventHandler handler)
        {
            EventHandler<ResponseEventArgs> adaptedHandler = new ResponseEventHandlerContractToViewHostAdapter(handler).Handler;
            _view.ProcessResponse += adaptedHandler;
            _responseHandlers[handler] = adaptedHandler;
        }

        /// <summary>
        /// Remove the Response Event Handler
        /// </summary>
        /// <param name="handler"><see cref="IResponseEventHandler"/></param>
        public virtual void ResponseEventRemove(IResponseEventHandler handler)
        {
            EventHandler<ResponseEventArgs> adaptedHandler;
            if (!_responseHandlers.TryGetValue(handler, out adaptedHandler)) return;

            _responseHandlers.Remove(handler);
            _view.ProcessResponse -= adaptedHandler;
        }


        /// <summary>
        /// Get the Process Request Source View
        /// </summary>
        /// <returns><see cref="IProcessRequest"/></returns>
        internal IProcessRequest GetSourceView()
        {
            return _view;
        }


        /// <summary>
        /// Process Request
        /// </summary>
        /// <param name="requestId">The request id</param>
        /// <param name="tenantId">The tenant id</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration" /></param>
        /// <param name="featureId">The request feature</param>
        /// <param name="payload">The request payload</param>
        public void ProcessRequest(Guid requestId, String tenantId, Guid trackingId, IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string featureId, string payload)
        {
            _view.ProcessRequest(requestId, tenantId,  trackingId, BackOfficeCompanyConfigHostAdapter.ContractToViewAdapter(backOfficeCompanyConfiguration), featureId, payload);
        }
    }
}
