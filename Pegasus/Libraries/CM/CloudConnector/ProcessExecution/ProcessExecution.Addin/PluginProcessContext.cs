using System.Threading;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.ProcessExecution.AddinView;

namespace Sage.Connector.ProcessExecution.Addin
{
    /// <summary>
    /// 
    /// </summary>
    internal class PluginProcessContext : IProcessContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginProcessContext"/> class.
        /// </summary>
        /// <param name="responseHandler">The response handler.</param>
        /// <param name="pluginLogger">The plugin logger.</param>
        /// <param name="loggerBase">The logger base.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="entityChunkCount">The entity chunk count.</param>
        public PluginProcessContext(IResponseHandler responseHandler, ILogging pluginLogger, LoggerCore loggerBase, CancellationToken cancellationToken, uint entityChunkCount)
        {
            ResponseHandler = responseHandler;
            PluginLogger = pluginLogger;
            CancellationToken = cancellationToken;
            EntityChunkCount = entityChunkCount;
            _loggerBase = loggerBase;
        }

        private LoggerCore _loggerBase;
        /// <summary>
        /// 
        /// </summary>
        public IResponseHandler ResponseHandler { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ILogging PluginLogger { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CancellationToken CancellationToken { get; private  set; }

        /// <summary>
        /// 
        /// </summary>
        public uint EntityChunkCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ISessionContext GetSessionContext()
        {
            return new PluginSessionContext(PluginLogger, CancellationToken);
        }


        /// <summary>
        /// Gets or sets the tracking identifier.
        /// </summary>
        /// <value>
        /// The tracking identifier.
        /// </value>
        public System.Guid TrackingId { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public System.Guid RequestId { get; set; }



        public void TrackPluginInvoke()
        {
            _loggerBase.AdvanceActivityState(null, RequestId, TenantId, TrackingId, _loggerBase.State9_InvokingMediationBoundWork, _loggerBase.InProgressMediationBoundWorkProcessing);
        }

        public void TrackPluginComplete()
        {
            _loggerBase.AdvanceActivityState(null, RequestId, TenantId, TrackingId, _loggerBase.State10_MediationBoundWorkComplete, _loggerBase.InProgress);
        }
    }
}
