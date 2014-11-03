using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Sage.Connector.Signalr.Common;
using Sage.Connector.Signalr.Interfaces;

namespace Sage.Connector.Signalr.Controller
{

    /// <summary>
    /// Static instance based controller for the client instances of the common connector hub.
    /// </summary>
    public sealed class ConnectorController : IConnectorController
    {
        #region Private Members

        private readonly static ConcurrentDictionary<string, Guid> _clientUsers = new ConcurrentDictionary<string, Guid>();
        private readonly ConcurrentDictionary<Guid, int> _offlineClients = new ConcurrentDictionary<Guid, int>();
        private readonly ConcurrentDictionary<Guid, ITenantNotify> _tenantClients = new ConcurrentDictionary<Guid, ITenantNotify>();
        private CancellationTokenSource _notificationCancel = new CancellationTokenSource();
        private Task _notificationTask;
        private bool _disposed;

        #endregion

        /// <summary>
        /// Singleton instance of the controller
        /// </summary>
        public static Lazy<ConnectorController> TheInstance = new Lazy<ConnectorController>(() => new ConnectorController(GlobalHost.ConnectionManager.GetHubContext<ConnectorHub>().Clients));

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="clients">The hub connection context.</param>
        private ConnectorController(IHubConnectionContext clients)
        {
            Clients = clients;

            try
            {
                _notificationTask = new Task(NotificationHandler, _notificationCancel.Token);
                _notificationTask.Start();
            }
            catch (Exception)
            {
                Dispose(true);

                throw;
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ConnectorController()
        {
            Dispose(false);
        }

        /// <summary>
        /// Public dispose method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Private dispose that allows for cleanup of task and cancellation token.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_notificationTask != null)
                    {
                        _notificationCancel.Cancel();
                        _notificationTask.Wait();
                        
                        _notificationCancel.Dispose();
                        _notificationCancel = null;

                        _notificationTask.Dispose();
                        _notificationTask = null;
                    }
                }
                _disposed = true;
            }                   
        }

        /// <summary>
        /// The hub connection context.
        /// </summary>
        private IHubConnectionContext Clients
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the singleton instance of the (hub) controller.
        /// </summary>
        public static ConnectorController Instance
        {
            get
            {
                return TheInstance.Value;
            }
        }

        /// <summary>
        /// Task loop for dispatching notifications to SignalR clients.
        /// </summary>
        private void NotificationHandler()
        {
            while (!_notificationCancel.IsCancellationRequested)
            {
                foreach (var kvp in _tenantClients)
                {
                    int count;

                    if (kvp.Value.GetNotification(out count))
                    {
                        Clients.Client(kvp.Value.ClientId).OnNotification(kvp.Value.TenantId, count);
                    }
                }

                Thread.Sleep(NotificationCommon.DispatchFrequency);
            }
        }

        /// <summary>
        /// Adds the client and connector id to the client users dictionary.
        /// </summary>
        /// <param name="clientId">The SignalR client identifier.</param>
        /// <param name="connectorId">The connector id associated with the connection</param>
        public void AddClient(string clientId, Guid connectorId)
        {
            _clientUsers.AddOrUpdate(clientId, connectorId, (key, oldValue) => connectorId);
        }

        /// <summary>
        /// Removes the client and connector id from the client users dictionary, also unsubscribes all
        /// tenants associated with the client.
        /// </summary>
        /// <param name="clientId">The SignalR client identifier.</param>
        public void RemoveClient(string clientId)
        {
            Guid connectorId;

            _clientUsers.TryRemove(clientId, out connectorId);

            UnsubscribeAll(clientId);
        }

        /// <summary>
        /// Increments the notification count for the Tenant id by the specified count
        /// </summary>
        /// <param name="tenantId">The tenant id to increment the count for.</param>
        /// <param name="count">The count to increment by.</param>
        public void AddNotification(Guid tenantId, int count = 1)
        {
            ITenantNotify client;

            if (_tenantClients.TryGetValue(tenantId, out client))
            {
                client.AddNotification(count);
            }
            else
            {
                _offlineClients.AddOrUpdate(tenantId, count, (key, oldValue) => oldValue + count);
            }
        }

        /// <summary>
        /// Acknowledges the notification count for a specific tenant.
        /// </summary>
        /// <param name="tenantId">The tenant id to acknowledge the notification counts for.</param>
        /// <param name="clientId">The SignalR client id assoicated with the request.</param>
        /// <param name="count">The count of notifications to acknowledge.</param>
        public void AcknowledgeNotification(Guid tenantId, string clientId, int count)
        {
            ITenantNotify client;

            if (!_tenantClients.TryGetValue(tenantId, out client)) return;

            if (client.ClientId.Equals(clientId))
            {
                client.AcknowledgeNotification(count);
            }
        }

        /// <summary>
        /// Allows a client to subscribe to notifications for a specific tenant.
        /// </summary>
        /// <param name="tenantId">The tenant id to subscribe to.</param>
        /// <param name="clientId">The SignalR client id associated with the request.</param>
        /// <returns>True if the client successfully subscribed to the tenant id, otherwise false.</returns>
        public bool TenantSubscribe(Guid tenantId, string clientId)
        {
            ITenantNotify client;
            Guid connectorId;

            if (_tenantClients.TryGetValue(tenantId, out client))
            {
                return client.ClientId.Equals(clientId);
            }

            if (!_clientUsers.TryGetValue(clientId, out connectorId)) return false;

            client = new TenantNotify(clientId, connectorId, tenantId);

            if (!_tenantClients.TryAdd(tenantId, client)) return false;

            int count;

            if (_offlineClients.TryRemove(tenantId, out count))
            {
                client.AddNotification(count);
            }

            return true;
        }

        /// <summary>
        /// Allows a client to unsubscribe to notifications for a specific tenant.
        /// </summary>
        /// <param name="tenantId">>The tenant id to unsubscribe from.</param>
        /// <param name="clientId">The SignalR client id associated with the request.</param>
        /// <returns>True if the client successfully unsubscribed from the tenant id, otherwise false.</returns>
        public bool TenantUnsubscribe(Guid tenantId, string clientId)
        {
            ITenantNotify client;

            if (!_tenantClients.TryGetValue(tenantId, out client)) return true;

            if (!client.ClientId.Equals(clientId)) return false;

            var count = client.NotificationCount;

            _offlineClients.AddOrUpdate(tenantId, count, (key, oldValue) => oldValue + count);

            return _tenantClients.TryRemove(tenantId, out client);
        }

        /// <summary>
        /// Returns an enumerable list of tenant id's that the client is subscribed to.
        /// </summary>
        /// <returns>An enumerable list of tenant guids associated with the calling client.</returns>
        public IEnumerable<Guid> GetSubscribedTenants(string clientId)
        {
            return _tenantClients.Where(kvp => kvp.Value.ClientId.Equals(clientId)).Select(kvp => kvp.Key).ToList();
        }

        /// <summary>
        /// Removes all tenant id subscribtions that are associated with the client id.
        /// </summary>
        /// <param name="clientId">The SignalR client id assoicated with the request.</param>
        public void UnsubscribeAll(string clientId)
        {
            var keys = _tenantClients.Where(kvp => kvp.Value.ClientId.Equals(clientId)).Select(kvp => kvp.Key).ToArray();

            foreach (var tenantId in keys)
            {
                ITenantNotify client;

                if (_tenantClients.TryRemove(tenantId, out client))
                {
                    var count = client.NotificationCount;

                    _offlineClients.AddOrUpdate(tenantId, count, (key, oldValue) => oldValue + count);
                }
            }
        }

        /// <summary>
        /// Gets the TenantNotify instance associated with the specified tenant id.
        /// </summary>
        /// <param name="tenantId">The tenant id to obtain the instance for.</param>
        /// <returns>The TenantNotify instance, otherwise null if the tenant id is not subscribed to.</returns>
        public ITenantNotify GetTenant(Guid tenantId)
        {
            ITenantNotify client;

            return (_tenantClients.TryGetValue(tenantId, out client)) ? client : null;
        }

        /// <summary>
        /// Returns a list of tenant Guids for the active tenant subscriptions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Guid> GetTenantKeys()
        {
            return _tenantClients.Keys;
        }
    }
}
