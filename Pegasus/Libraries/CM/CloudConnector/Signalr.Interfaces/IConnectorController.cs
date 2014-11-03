using System;
using System.Collections.Generic;

namespace Sage.Connector.Signalr.Interfaces
{
    /// <summary>
    /// Interface for connector SignalR controller. 
    /// </summary>
    public interface IConnectorController : IDisposable
    {
        /// <summary>
        /// Adds the client and connector id to the client users dictionary.
        /// </summary>
        /// <param name="clientId">The SignalR client identifier.</param>
        /// <param name="connectorId">The connector id associated with the connection</param>
        void AddClient(string clientId, Guid connectorId);

        /// <summary>
        /// Removes the client and connector id from the client users dictionary, also unsubscribes all
        /// tenants associated with the client.
        /// </summary>
        /// <param name="clientId">The SignalR client identifier.</param>
        void RemoveClient(string clientId);

        /// <summary>
        /// Acknowledges the notification count for a specific tenant.
        /// </summary>
        /// <param name="tenantId">The tenant id to acknowledge the notification counts for.</param>
        /// <param name="clientId">The SignalR client id assoicated with the request.</param>
        /// <param name="count">The count of notifications to acknowledge.</param>
        void AcknowledgeNotification(Guid tenantId, string clientId, int count);

        /// <summary>
        /// Allows a client to subscribe to notifications for a specific tenant.
        /// </summary>
        /// <param name="tenantId">The tenant id to subscribe to.</param>
        /// <param name="clientId">The SignalR client id associated with the request.</param>
        /// <returns>True if the client successfully subscribed to the tenant id, otherwise false.</returns>
        bool TenantSubscribe(Guid tenantId, string clientId);

        /// <summary>
        /// Allows a client to unsubscribe to notifications for a specific tenant.
        /// </summary>
        /// <param name="tenantId">>The tenant id to unsubscribe from.</param>
        /// <param name="clientId">The SignalR client id associated with the request.</param>
        /// <returns>True if the client successfully unsubscribed from the tenant id, otherwise false.</returns>
        bool TenantUnsubscribe(Guid tenantId, string clientId);

        /// <summary>
        /// Removes all tenant id subscribtions that are associated with the client id.
        /// </summary>
        /// <param name="clientId">The SignalR client id assoicated with the request.</param>
        void UnsubscribeAll(string clientId);

        /// <summary>
        /// Increments the notification count for the Tenant id by the specified count
        /// </summary>
        /// <param name="tenantId">The tenant id to have its notification count updated.</param>
        /// <param name="count">The count to increment by.</param>
        void AddNotification(Guid tenantId, int count = 1);

        /// <summary>
        /// Gets the TenantNotify instance associated with the specified tenant id.
        /// </summary>
        /// <param name="tenantId">The tenant id to obtain the instance for.</param>
        /// <returns>The TenantNotify instance, otherwise null if the tenant id is not subscribed to.</returns>
        ITenantNotify GetTenant(Guid tenantId);

        /// <summary>
        /// Returns a list of tenant Guids for the active tenant subscriptions.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Guid> GetTenantKeys();
    }
}
