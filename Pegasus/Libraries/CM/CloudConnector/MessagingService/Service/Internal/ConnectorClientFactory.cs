using System;
using System.Collections.Concurrent;
using System.Linq;
using Sage.Connector.Common;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.Signalr.Client;
using Sage.Connector.Signalr.Interfaces;

namespace Sage.Connector.MessagingService.Internal
{
    internal static class ConnectorClientFactory
    {
        private class ConnectorKeyHolder
        {
            public string GetConnectorKey(Guid connectorId)
            {
                return connectorId.ToString("N");
            }
        }

        /// <summary>
        /// Factory to manage creating/sharing connector clients
        /// </summary>
        /// <param name="connectorId"></param>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        static public IConnectorClient CreateConnectorClient(
            Guid connectorId,
            string baseAddress)
        {
            if (!_connectorClients.ContainsKey(baseAddress))
            {
                var keyHolder = new ConnectorKeyHolder();
                var connectorClient = new ConnectorClient(
                    connectorId,
                    keyHolder.GetConnectorKey,
                    baseAddress);

                connectorClient.OnNotification += ConnectorClientOnOnNotification;
                connectorClient.OnStateChange += ConnectorClientOnOnStateChange;
                _connectorClients[baseAddress] = connectorClient;
            }

            return _connectorClients[baseAddress];
        }

        /// <summary>
        /// Assistive disposal of created connector clients
        /// </summary>
        /// <param name="baseAddress"></param>
        static public void DisposeEmptyConnectorClient(string baseAddress)
        {
            DisposeConnectorClient(baseAddress, false);
        }

        /// <summary>
        /// Assistive shutdown of all connector clients
        /// </summary>
        static public void Shutdown()
        {
            foreach (var connectorClient in _connectorClients)
            {
                DisposeConnectorClient(connectorClient.Value.BaseAddress, true);
            }
        }


        static private void ConnectorClientOnOnNotification(object sender, ClientNotifyArgs clientNotifyArgs)
        {
            try
            {
                using (
                    var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost",
                                                                                  ConnectorServiceUtils.
                                                                                      CatalogServicePortNumber)
                    )
                {
                    proxy.NotifyRequestMessageAvailable(clientNotifyArgs.TenantId.ToString(), Guid.Empty);
                }
            }
            catch (Exception)
            {
                // Best attempt notification to message availability
            }
        }

        static private void ConnectorClientOnOnStateChange(object sender, ClientStateArgs clientStateArgs)
        {
            if (clientStateArgs.State == ClientState.UnexpectedDisconnect)
            {
                try
                {
                    using (
                        var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost",
                            ConnectorServiceUtils.
                                CatalogServicePortNumber)
                        )
                    {
                        proxy.NotifyRestoreRequestMessageAvailableConnections();
                    }
                }
                catch (Exception)
                {
                    // Best attempt notification to message availability
                }
            }
        }

        static private void DisposeConnectorClient(string baseAddress, bool forceDispose)
        {
            if (!_connectorClients.ContainsKey(baseAddress))
                return;

            var connectorClient = _connectorClients[baseAddress];
            if (forceDispose || !connectorClient.GetSubscribedTenants().Any())
            {
                connectorClient.OnNotification -= ConnectorClientOnOnNotification;
                connectorClient.OnStateChange -= ConnectorClientOnOnStateChange;
                connectorClient.Shutdown();
                _connectorClients.TryRemove(baseAddress, out connectorClient);
                connectorClient.Dispose();
            }
        }

        static readonly private ConcurrentDictionary<String, IConnectorClient> _connectorClients =
            new ConcurrentDictionary<string, IConnectorClient>();
    }
}
