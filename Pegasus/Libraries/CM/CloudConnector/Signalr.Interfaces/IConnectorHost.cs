using System;

namespace Sage.Connector.Signalr.Interfaces
{
    /// <summary>
    /// Delegate type for obtaining the connector key based on the specified id.
    /// </summary>
    /// <param name="connectorId"></param>
    /// <returns></returns>
    public delegate string GetConnectorKeyMethod(Guid connectorId);

    /// <summary>
    /// Interface for connector hosting of SignalR hubs.
    /// </summary>
    public interface IConnectorHost : IDisposable
    {
        /// <summary>
        /// The connector controller instance.
        /// </summary>
        IConnectorController Controller { get; }

        /// <summary>
        /// The Owin app instance.
        /// </summary>
        IDisposable OwinApp { get; }

        /// <summary>
        /// The actual SignalR endpoint which is derived from the base address.
        /// </summary>
        string EndpointAddress { get; }

        /// <summary>
        /// The url endpoint for the SignalR notifications.
        /// </summary>
        string BaseAddress { get; }
    }
}
