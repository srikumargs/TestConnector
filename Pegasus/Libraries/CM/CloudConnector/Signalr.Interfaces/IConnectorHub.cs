using System;
using System.Collections.Generic;

namespace Sage.Connector.Signalr.Interfaces
{
    /// <summary>
    /// Interface for connector SignalR hub.
    /// </summary>
    public interface IConnectorHub
    {
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
        bool TenantUnsubscribe(Guid tenantId);

        /// <summary>
        /// Unsubscribes all tenant notifications assoicated with the calling client id.
        /// </summary>
        /// <returns>True if the client successfully unsubscribed from the tenant id, otherwise false.</returns>
        void UnsubscribeAll();

        /// <summary>
        /// Returns an enumerable list of tenant id's that the client is subscribed to.
        /// </summary>
        /// <returns>An enumerable list of tenant guids associated with the calling client.</returns>
        IEnumerable<Guid> GetSubscribedTenants();

        /// <summary>
        /// Acknowledges the notification count for a specific tenant. 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        void AcknowledgeNotification(Guid tenantId, int count);
    }
}
