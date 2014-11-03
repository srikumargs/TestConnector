using System;
using System.Collections.Generic;

namespace Sage.Connector.Signalr.Interfaces
{
    /// <summary>
    /// Enumerated state that the connector client can be in.
    /// </summary>
    public enum ClientState
    {
        /// <summary>
        /// Disconnected state
        /// </summary>
        Disconnected = 0,

        /// <summary>
        /// Connecting state
        /// </summary>
        Connecting = 1,

        /// <summary>
        /// Connnected state
        /// </summary>
        Connected = 2,

        /// <summary>
        /// Reconnecting state
        /// </summary>
        Reconnecting = 3,

        /// <summary>
        /// Unexpected disconnect state
        /// </summary>
        UnexpectedDisconnect = 4,
    }

    /// <summary>
    /// Custom event type needed for interface event definition.
    /// </summary>
    public class ClientStateArgs : EventArgs
    {
        private readonly ClientState _state;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="state">The state that the client is currently in.</param>
        public ClientStateArgs(ClientState state)
        {
            _state = state;
        }

        /// <summary>
        /// The new state of the client connection.
        /// </summary>
        public ClientState State
        {
            get
            {
                return _state;
            }
        }
    }

    /// <summary>
    /// Custom event type needed for interface event definition.
    /// </summary>
    public class ClientNotifyArgs : EventArgs
    {
        private readonly Guid _tenantId;
        private readonly int _count;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="tenantId">The tenant id that the notification count is associated with.</param>
        /// <param name="count">The count of notifications.</param>
        public ClientNotifyArgs(Guid tenantId, int count)
        {
            _tenantId = tenantId;
            _count = count;
        }

        /// <summary>
        /// The tenant id that the notification count is associated with.
        /// </summary>
        public Guid TenantId
        {
            get
            {
                return _tenantId;
            }
        }

        /// <summary>
        /// The count of notifications.
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
        }
    }

    /// <summary>
    /// Notification handler event delegate type.
    /// </summary>
    /// <param name="sender">The object firing the event.</param>
    /// <param name="e">The client notification event args.</param>
    [Serializable]
    public delegate void NotifyHandler(object sender, ClientNotifyArgs e);

    /// <summary>
    /// State handler event delegate type.
    /// </summary>
    /// <param name="sender">The object firing the event.</param>
    /// <param name="e">The client state event args.</param>
    [Serializable]
    public delegate void StateHandler(object sender, ClientStateArgs e);

    /// <summary>
    /// Interface for connector client which will handle tenant subscription and notifications.
    /// </summary>
    public interface IConnectorClient : IDisposable
    {
        /// <summary>
        /// Method to allow for connecting to the SignalR endpoint.
        /// </summary>
        void Startup();

        /// <summary>
        /// Method to allow for disconnecting from the SignalR endpoint.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Returns the current client state
        /// </summary>
        ClientState State { get; }

        /// <summary>
        /// The event that gets fired when a SignalR connection state changes.
        /// </summary>
        event StateHandler OnStateChange;

        /// <summary>
        /// Event that will be fired when a notification occurs.
        /// </summary>
        event NotifyHandler OnNotification;

        /// <summary>
        /// The connector id which identifies the connector client.
        /// </summary>
        Guid ConnectorId { get; }

        /// <summary>
        /// The base url address used for connecting to the SignalR endpoint.
        /// </summary>
        string BaseAddress { get; }

        /// <summary>
        /// Subscribes a SignalR client for tenant notifications.
        /// </summary>9
        /// <param name="tenantId">The Tenant id to recieve notifications for.</param>
        /// <returns>True if the client successfully subscribed to the tenant id, otherwise false.</returns>
        bool TenantSubscribe(Guid tenantId);

        /// <summary>
        /// Unsubscribes a SignalR client from recieving tenant notifications.
        /// </summary>
        /// <param name="tenantId">The Tenant id to stop recieve notifications for.</param>
        /// <returns>True if the client successfully unsubscribed from the tenant id, otherwise false.</returns>
        bool TenantUnsubscribe(Guid tenantId);

        /// <summary>
        /// Returns an enumerable list of tenant id's that the client is subscribed to.
        /// </summary>
        /// <returns>An enumerable list of tenant guids associated with the calling client.</returns>
        IEnumerable<Guid> GetSubscribedTenants();
    }
}
