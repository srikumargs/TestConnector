using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sage.Connector.NotificationService.Interfaces
{
    /// <summary>
    /// The supported types of queues
    /// </summary>
    public enum QueueTypes
    {
        /// <summary>
        /// No QueueTypes (default value automatically initialized by runtime)
        /// </summary>
        None = 0,

        /// <summary>
        /// Incoming messages from cloud to premise
        /// </summary>
        Inbox,

        /// <summary>
        /// Outgoing messages from premise to cloud
        /// </summary>
        Outbox,

        /// <summary>
        /// Outgoing messages staged for imminent cloud delivery
        /// </summary>
        OutboxPendingDeletion
    }
}
