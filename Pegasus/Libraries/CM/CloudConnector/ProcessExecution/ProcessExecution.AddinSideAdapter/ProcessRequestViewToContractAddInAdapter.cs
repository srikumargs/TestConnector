using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.AddinView.Events;
using Sage.Connector.ProcessExecution.Interfaces;
using IBackOfficeCompanyConfiguration = Sage.Connector.ProcessExecution.Interfaces.IBackOfficeCompanyConfiguration;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// Process Request View to Contract AddIn Adapter
    /// </summary>
    [AddInAdapter]
    public class ProcessRequestViewToContractAddInAdapter :
        ContractBase, IProcessRequestContract
    {
        private readonly IProcessRequest _view;

        private readonly Dictionary<IResponseEventHandler, EventHandler<ResponseEventArgs>> _processResponseHandlers;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"><see cref="IProcessRequest"/></param>
        public ProcessRequestViewToContractAddInAdapter(IProcessRequest view)
        {
            _view = view;
            _processResponseHandlers = new Dictionary<IResponseEventHandler, EventHandler<ResponseEventArgs>>();
        }

        /// <summary>
        /// Initialize with the app contract
        /// </summary>
        /// <param name="appContract"><see cref="IAppContract"/></param>
        public void Initialize(IAppContract appContract)
        {
            _view.Initialize(AppAddInAdapter.ContractToViewAdapter(appContract));
        }

        /// <summary>
        /// 
        /// </summary>
        public void RequestCancellation()
        {
            _view.RequestCancellation();
        }

        /// <summary>
        /// Process the Request
        /// </summary>
        /// <param name="requestId">The request id</param>
        /// <param name="tenantId">The tenant id</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="backOfficeCompanyConfiguration"><see cref="IBackOfficeCompanyConfiguration" /></param>
        /// <param name="featureId">The request feature</param>
        /// <param name="payload">The request payload</param>
        public void ProcessRequest(Guid requestId, string tenantId, Guid trackingId, IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string featureId, string payload)
        {
            _view.ProcessRequest(requestId, tenantId, trackingId, BackOfficeCompanyConfigAddInAdapter.ContractToViewAdapter(backOfficeCompanyConfiguration), featureId, payload);
        }




        /// <summary>
        /// Add the Response event handler 
        /// </summary>
        /// <param name="handler"><see cref="IResponseEventHandler"/></param>
        public void ResponseEventAdd(IResponseEventHandler handler)
        {
            EventHandler<ResponseEventArgs> adaptedHandler
                = new ResponseEventHandlerContractToViewAddInAdapter(handler).Handler;
            _view.ProcessResponse += adaptedHandler;
            _processResponseHandlers[handler] = adaptedHandler;
        }

        /// <summary>
        /// Remove the Response Event Handler. 
        /// </summary>
        /// <param name="handler"><see cref="IResponseEventHandler"/></param>
        public void ResponseEventRemove(IResponseEventHandler handler)
        {
            EventHandler<ResponseEventArgs> adaptedHandler;
            if (!_processResponseHandlers.TryGetValue(handler, out adaptedHandler)) return;

            _processResponseHandlers.Remove(handler);
            _view.ProcessResponse -= adaptedHandler;
        }



        /// <summary>
        /// Get the Process Request source view
        /// </summary>
        /// <returns><see cref="IProcessRequest"/></returns>
        internal IProcessRequest GetSourceView()
        {
            return _view;
        }



    }
}
