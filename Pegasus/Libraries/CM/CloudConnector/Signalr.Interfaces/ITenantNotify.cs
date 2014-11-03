using System;

namespace Sage.Connector.Signalr.Interfaces
{
    /// <summary>
    /// Interface for handling outbox notfications specfic to a given tenant
    /// </summary>
    public interface ITenantNotify
    {
        /// <summary>
        /// The SignalR connection id associated with the connector/tenant instance.
        /// </summary>
        string ClientId { get; }

        /// <summary>
        /// The connector id that owns the tenant id.
        /// </summary>
        Guid ConnectorId { get; }

        /// <summary>
        /// The tenant id.
        /// </summary>
        Guid TenantId { get; }

        /// <summary>
        /// The count of notifications yet to be signalled to the client. 
        /// </summary>
        int NotificationCount { get; }

        /// <summary>
        /// Called when the client acknowledges the reciept of the notification count.
        /// </summary>
        /// <param name="count">The number of notifications that the client is acknowledging.</param>
        void AcknowledgeNotification(int count);

        /// <summary>
        /// Called by the controller to queue an outbox notification with the connector/tenant dispatcher.
        /// </summary>
        /// <param name="count">Count of notifications to add. Defaults to one.</param>
        void AddNotification(int count = 1);

        /// <summary>
        /// Called by the controller to obtain the number of outbox notifications for a given connector/tenant.
        /// </summary>
        /// <param name="count">The count of notifications which will be filled in upon return</param>
        /// <returns>True if there are notifications to send to the connector/tennant client.</returns>
        bool GetNotification(out int count);
    }
}
