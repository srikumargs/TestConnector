using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Sage.Connector.Signalr.Common;
using Sage.Connector.Signalr.Interfaces;

namespace Sage.Connector.Signalr.Controller
{
    /// <summary>
    /// Signalr hub for the common connector.
    /// </summary>
    [Authorize]
    [HubName(ControllerCommon.HubName)]
    public class ConnectorHub : Hub, IConnectorHub
    {
        private readonly ConnectorController _controller;

        /// <summary>
        /// Default contructor
        /// </summary>
        public ConnectorHub() : this(ConnectorController.Instance) { }

        /// <summary>
        /// Overloaded constructor that takes an instance of a connector controller. 
        /// </summary>
        /// <param name="controller">The connector controller instance to be used by the hub.</param>
        public ConnectorHub(ConnectorController controller)
        {
            _controller = controller;
        }

        /// <summary>
        /// Gets the connector id from the authenticated principal identity
        /// </summary>
        /// <param name="connectorId">The connector id obtained from the prinipal identity</param>
        /// <returns>True if the connector id was obtained, false otherwise.</returns>
        private bool GetUserConnectorId(out Guid connectorId)
        {
            connectorId = Guid.Empty;

            if ((Context.User == null) || !Context.User.Identity.IsAuthenticated) return false;

            return Guid.TryParse(Context.User.Identity.Name, out connectorId);
        }

        /// <summary>
        /// Allows the SignalR client to acknowledge notification counts.
        /// </summary>
        /// <param name="tenantId">The tenant id to acknowledge notifications for.</param>
        /// <param name="count">The notification count to acknowledge.</param>
        public void AcknowledgeNotification(Guid tenantId, int count)
        {
            _controller.AcknowledgeNotification(tenantId, Context.ConnectionId, count);
        }

        /// <summary>
        /// Subscribes a SignalR client for tenant notifications.
        /// </summary>
        /// <param name="tenantId">The Tenant id to recieve notifications for.</param>
        /// <returns>True if the client successfully subscribed to the tenant id, otherwise false.</returns>
        public bool TenantSubscribe(Guid tenantId)
        {
            return _controller.TenantSubscribe(tenantId, Context.ConnectionId);
        }

        /// <summary>
        /// Unsubscribes a SignalR client from recieving tenant notifications.
        /// </summary>
        /// <param name="tenantId">The Tenant id to stop recieve notifications for.</param>
        /// <returns>True if the client successfully unsubscribed from the tenant id, otherwise false.</returns>
        public bool TenantUnsubscribe(Guid tenantId)
        {
            return _controller.TenantUnsubscribe(tenantId, Context.ConnectionId);
        }

        /// <summary>
        /// Unsubscribes all tenant notifications assoicated with the calling client id.
        /// </summary>
        public void UnsubscribeAll()
        {
            _controller.UnsubscribeAll(Context.ConnectionId);
        }

        /// <summary>
        /// Returns an enumerable list of tenant id's that the client is subscribed to.
        /// </summary>
        /// <returns>An enumerable list of tenant guids associated with the calling client.</returns>
        public IEnumerable<Guid> GetSubscribedTenants()
        {
            return _controller.GetSubscribedTenants(Context.ConnectionId);
        }

        /// <summary>
        /// Handles the connect for a connector client instance.
        /// </summary>
        /// <returns>The asynchronous operation.</returns>
        public override Task OnConnected()
        {
            Guid connectorId;

            if (GetUserConnectorId(out connectorId))
            {
                _controller.AddClient(Context.ConnectionId, connectorId);
            }

            return base.OnConnected();
        }

        /// <summary>
        /// Handles the reconnect for a connector client instance.
        /// </summary>
        /// <returns>The asynchronous operation.</returns>
        public override Task OnReconnected()
        {
            Guid connectorId;

            if (GetUserConnectorId(out connectorId))
            {
                _controller.AddClient(Context.ConnectionId, connectorId);
            }

            return base.OnConnected();
        }

        /// <summary>
        /// Handles the disconnect for a connector client instance.
        /// </summary>
        /// <returns>The asynchronous operation.</returns>
        public override Task OnDisconnected()
        {
            _controller.RemoveClient(Context.ConnectionId);

            return base.OnDisconnected();
        }
    }
}
