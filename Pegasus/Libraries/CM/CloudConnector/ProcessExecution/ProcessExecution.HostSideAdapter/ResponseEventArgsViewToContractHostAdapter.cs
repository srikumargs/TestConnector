using System;
using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// Response Event Args View To Contract Host Adapter
    /// </summary>
    public class ResponseEventArgsViewToContractHostAdapter : ContractBase, IResponseEventArgs
    {
        private readonly ResponseEventArgs _view;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"><see cref="ResponseEventArgs"/></param>
        public ResponseEventArgsViewToContractHostAdapter(ResponseEventArgs view)
        {
            _view = view;
        }

        /// <summary>
        /// Response payload
        /// </summary>
        public string Payload
        {
            get
            {
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
        /// Tenant Id
        /// </summary>
        public string TenantId
        {
            get { return _view.TenantId; }
        }

        /// <summary>
        /// Tracking Id
        /// </summary>
        public Guid TrackingId
        {
            get { return _view.TrackingId; }
        }

        /// <summary>
        /// Get the Response Event Args Source View
        /// </summary>
        /// <returns><see cref="ResponseEventArgs"/></returns>
        internal ResponseEventArgs GetSourceView()
        {
            return _view;
        }
    }
}
