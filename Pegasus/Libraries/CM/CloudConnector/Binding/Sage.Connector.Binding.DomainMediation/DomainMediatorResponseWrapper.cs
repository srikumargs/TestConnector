using System;
using System.IO;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Documents;

namespace Sage.Connector.Binding
{
    /// <summary>
    /// The Domain Mediator response wrapper keeps track of the response per request. 
    /// </summary>
    internal sealed class DomainMediatorResponseWrapper : IDisposable
    {
        #region Private fields

        private BlobContentWriter _contentWriter;
        private readonly Object _lock = new object();
        private bool _disposed;

        #endregion

        #region Private methods

        /// <summary>
        /// Perform resource cleanup.
        /// </summary>
        /// <param name="disposing">True if being disposed, otherwise false.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_contentWriter != null)
                {
                    _contentWriter.Dispose();
                    _contentWriter = null;
                }
            }

            _disposed = true;
        }

        #endregion

        #region Constructor and destructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requestWrapper"><see cref="RequestWrapper"/></param>
        /// <param name="domainMediation"><see cref="DomainMediation"/></param>
        /// <param name="maxBlobSize"><see cref="MaxBlobSize"/></param>
        public DomainMediatorResponseWrapper(RequestWrapper requestWrapper, DomainMediation domainMediation, long maxBlobSize)
        {
            if (requestWrapper == null) throw new ArgumentNullException("requestWrapper");
            if (domainMediation == null) throw new ArgumentNullException("domainMediation");

            RequestWrapper = requestWrapper;
            DomainMediationEntry = domainMediation;
            MaxBlobSize = maxBlobSize;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~DomainMediatorResponseWrapper()
        {
            Dispose(false);
        }

        #endregion

        #region Public methods
    
        /// <summary>
        /// Perform resource cleanup.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Allocates a blob content writer to store multi part responses in.
        /// </summary>
        public void AllocContentWriter()
        {
            lock (_lock)
            {
                if (_contentWriter == null)
                {
                    string tenantId = null;
                    ActivityTrackingContext atc = RequestWrapper.ActivityTrackingContext;
                    if (atc != null)
                    {
                        tenantId = atc.TenantId;
                    }
                    var folder = DocumentManager.GetResponseBlobPath(tenantId);
                    var fileName = Path.Combine(folder, String.Format("{0}.json", RequestWrapper.ActivityTrackingContext.RequestId));

                    _contentWriter = new BlobContentWriter(fileName, MaxBlobSize);
                }
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The <see cref="MaxBlobSize"/> property
        /// </summary>
        public long MaxBlobSize { get; set; }

        /// <summary>
        /// The <see cref="RequestWrapper"/> property
        /// </summary>
        public RequestWrapper RequestWrapper { get; set; }

        /// <summary>
        /// The <see cref="DomainMediation"/> property
        /// </summary>
        public DomainMediation DomainMediationEntry { get; set; }

        /// <summary>
        /// The content writer used for multi part responses.
        /// </summary>
        public BlobContentWriter ContentWriter
        {
            get { return _contentWriter; }
        }

        #endregion
    }
}
