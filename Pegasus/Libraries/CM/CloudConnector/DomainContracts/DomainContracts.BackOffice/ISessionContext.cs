using System.Threading;

namespace Sage.Connector.DomainContracts.BackOffice
{
    /// <summary>
    /// Provides services needed for the session. These Include access to a logging interface and access to a Cancellation Toke
    /// </summary>
    public interface ISessionContext
    {
        /// <summary>
        /// Logger to allow log entries to be added the connector log.
        /// </summary>
        ILogging Logger { get; }

        /// <summary>
        /// CancellationToken connected to the connector core. Will triggered if the core requests a cancel.
        /// </summary>
        CancellationToken CancellationToken { get; }
    }
}
