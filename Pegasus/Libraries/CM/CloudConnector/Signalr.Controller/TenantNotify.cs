using System;
using Sage.Connector.Signalr.Common;
using Sage.Connector.Signalr.Interfaces;

namespace Sage.Connector.Signalr.Controller
{
    /// <summary>
    /// Implementation for the ITenanatNotify interface, which will be responsible for handling the flow of notification counts.
    /// </summary>
    public class TenantNotify : ITenantNotify
    {
        private readonly string _clientId;
        private readonly Guid _tenantId;
        private readonly Guid _connectorId;
        private readonly Object _lock = new Object();
        private DateTime _pendingResend = DateTime.UtcNow;
        private volatile int _notificationCount;
        private int _rendingRetry;
        private int _pendingCount;
        private bool _isPendingAck;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="clientId">The SignalR client id to be associated with</param>
        /// <param name="connectorId">The connector id that owns/is associated with the tenant id.</param>
        /// <param name="tenantId">The Tenant id to be associated with. </param>
        public TenantNotify(string clientId, Guid connectorId, Guid tenantId)
        {
            _clientId = clientId;
            _connectorId = connectorId;
            _tenantId = tenantId;
        }

        /// <summary>
        /// The SignalR client id.
        /// </summary>
        public string ClientId 
        { 
            get
            {
                return _clientId;
            }
        }

        /// <summary>
        /// The connector id that owns the tenant id.
        /// </summary>
        public Guid ConnectorId
        {
            get
            {
                return _connectorId;
            }
        }

        /// <summary>
        /// The Tenant id.
        /// </summary>
        public Guid TenantId
        {
            get
            {
                return _tenantId;
            }
        }

        /// <summary>
        /// The count of notifications yet to be signalled to the client. 
        /// </summary>
        public int NotificationCount
        {
            get
            {
                return _notificationCount;
            }
        }

        /// <summary>
        /// Acknowledges the notification count send by the client. This is done by first adjusting any
        /// pending acknowledgement, and the adjusting the current count.
        /// </summary>
        /// <param name="count">The number of notifications to acknowledge.</param>
        public void AcknowledgeNotification(int count)
        {
            lock (_lock)
            {
                if (_isPendingAck)
                {
                    _isPendingAck = false;

                    count = (count < _pendingCount) ? 0 : count - _pendingCount;

                    _pendingCount = 0;
                    _rendingRetry = 0;
                }

                _notificationCount = (count > _notificationCount) ? 0 : _notificationCount - count;
            }
        }

        /// <summary>
        /// Increments the notification count for the Tenant id by the specified count
        /// </summary>
        /// <param name="count">Count of notifications to add. Defaults to one.</param>
        public void AddNotification(int count = 1)
        {
            lock (_lock)
            {
                _notificationCount += count;
            }
        }

        /// <summary>
        /// Returns the number of notifications that have not yet been acknowledged by the client. 
        /// </summary>
        /// <param name="count">The count of notifications to be filled in upon return.</param>
        /// <returns>True if there are notificaations for the client, otherwise false.</returns>
        public bool GetNotification(out int count)
        {
            count = 0;

            lock (_lock)
            {
                if (_isPendingAck)
                {
                    if (DateTime.UtcNow >= _pendingResend)
                    {
                        if (_rendingRetry >= NotificationCommon.MaxRetries)
                        {
                            _pendingCount = 0;
                            _rendingRetry = 0;
                            _isPendingAck = false;
                        }
                        else
                        {
                            _pendingResend = DateTime.UtcNow.Add(NotificationCommon.RetryTime);
                            _rendingRetry++;
                            count = _pendingCount;
                        }
                    }

                    return (count > 0);
                }

                if (_notificationCount > 0)
                {
                    _pendingCount = _notificationCount;
                    _notificationCount = 0;
                    _rendingRetry = 1;
                    _pendingResend = DateTime.UtcNow.Add(NotificationCommon.RetryTime);
                    _isPendingAck = true;

                    count = _pendingCount;
                }

                return (count > 0);
            }
        }
    }
}
