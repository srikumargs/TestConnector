using System.Threading;
using Sage.Connector.DomainContracts.BackOffice;

namespace Sage.Connector.ProcessExecution.Addin
{
    /// <summary>
    /// 
    /// </summary>
    internal class PluginSessionContext : ISessionContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        public PluginSessionContext(ILogging logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// 
        /// </summary>
        public ILogging Logger { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CancellationToken CancellationToken { get; private set; }
    }
}
