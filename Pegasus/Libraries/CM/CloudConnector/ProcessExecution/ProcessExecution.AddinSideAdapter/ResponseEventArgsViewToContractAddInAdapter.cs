using System;
using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.AddinView.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// Response Event Args View to Contract AddIn Adapter
    /// </summary>
    public class ResponseEventArgsViewToContractAddInAdapter: ContractBase, IResponseEventArgs {
        
        private readonly ResponseEventArgs _view;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"><see cref="ResponseEventArgs"/></param>
        public ResponseEventArgsViewToContractAddInAdapter(ResponseEventArgs view)
        {
            _view = view;
        }

        /// <summary>
        /// Response Payload
        /// </summary>
        public string Payload {
            get {
                return _view.Payload;
            }
        }

        /// <summary>
        /// Request Id
        /// </summary>
        public Guid RequestId
        {
            get { return _view.RequestId; }
        }

        /// <summary>
        /// True if the response if complete, otherwise false.
        /// </summary>
        public bool Completed
        {
            get { return _view.Completed; }
        }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public string TenantId
        {
            get { return _view.TenantId; }
        }

        /// <summary>
        /// Gets the tracking identifier.
        /// </summary>
        /// <value>
        /// The tracking identifier.
        /// </value>
        public Guid TrackingId
        {
            get { return _view.TrackingId; }
        }

        /// <summary>
        /// Response Event Args Source View
        /// </summary>
        /// <returns></returns>
        internal ResponseEventArgs GetSourceView() {
            return _view;
        }
    }
}
