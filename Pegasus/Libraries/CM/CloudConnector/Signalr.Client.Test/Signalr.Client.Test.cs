using System;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Signalr.Client;
using Sage.Connector.Signalr.Controller;
using Sage.Connector.Signalr.Interfaces;

namespace Sage.Connector.Signalr.Client.Test
{
    /// <summary>
    /// Test class for SignalR client
    /// </summary>
    [TestClass]
    public sealed class SignalrClientTest : IDisposable
    {
        private static ConnectorHost _host;
        private const string Address = "http://localhost:8888/testing";
        private const string Key = "Test";
        private const string BadKey = "test";
        private readonly Guid _tenantId1 = Guid.NewGuid();
        private readonly Guid _tenantId2 = Guid.NewGuid();
        private readonly Guid _connectorId = Guid.NewGuid();
        private bool _disposed;

        private static string GetBadConnectorKey(Guid connectorId)
        {
            return BadKey;
        }

        private static string GetConnectorKey(Guid connectorId)
        {
            return Key;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SignalrClientTest()
        {
            Dispose(false);
        }

        /// <summary>
        /// Cleanup method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {

            }

            _disposed = true;
        }

        /// <summary>
        /// Test method
        /// </summary>
        [ClassCleanup]
        public static void Cleanup()
        {
            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [ClassInitialize]
        public static void Startup(TestContext context)
        {
            try
            {
                // _host = new ConnectorHost(Address, GetConnectorKey);
            }
            catch (Exception)
            {
                _host = null;

                throw;
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_Constructor()
        {
            using (var client = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
               client.Shutdown(); 
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_Shutdown()
        {
            using (var client = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                client.Shutdown();
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_Startup_Good()
        {
            using (var client = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                client.Startup();
                client.Shutdown();
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ConnectorClient_Startup_Bad()
        {
            using (var client = new ConnectorClient(_connectorId, GetBadConnectorKey, Address))
            {
                client.Startup();
                client.Shutdown();
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_State()
        {
            using (var client = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                Assert.IsTrue(client.State == ClientState.Disconnected);

                client.Startup();

                Assert.IsTrue(client.State == ClientState.Connected);

                client.Shutdown();

                Assert.IsTrue(client.State == ClientState.Disconnected);
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_BaseAddress()
        {
            using (var client = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                Assert.IsTrue(client.BaseAddress.Equals(Address));
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_ConnectorId()
        {
            using (var client = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                Assert.IsTrue(client.ConnectorId.Equals(_connectorId));
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_Subscribe()
        {
            using (var client = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                client.Startup();

                try
                {
                    Assert.IsTrue(client.TenantSubscribe(_tenantId1));
                    Assert.IsTrue(client.TenantSubscribe(_tenantId2));

                    var list = client.GetSubscribedTenants();

                    Assert.IsTrue(list.Count() == 2);
                }
                finally 
                {
                    client.Shutdown();
                }
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_SubscribeFail()
        {
            using (var client1 = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                using (var client2 = new ConnectorClient(Guid.NewGuid(), GetConnectorKey, Address))
                {
                    client1.Startup();

                    try
                    {
                        client2.Startup();

                        try
                        {
                            Assert.IsTrue(client1.TenantSubscribe(_tenantId1));
                            Assert.IsFalse(client2.TenantSubscribe(_tenantId1));
                        }
                        finally
                        {
                            client2.Shutdown();
                        }
                    }
                    finally
                    {
                        client1.Shutdown();
                    }
                }
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_UnsubscribeFail()
        {
            using (var client1 = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                using (var client2 = new ConnectorClient(Guid.NewGuid(), GetConnectorKey, Address))
                {
                    client1.Startup();

                    try
                    {
                        client2.Startup();

                        try
                        {
                            Assert.IsTrue(client1.TenantSubscribe(_tenantId1));
                            Assert.IsFalse(client2.TenantUnsubscribe(_tenantId1));
                        }
                        finally
                        {
                            client2.Shutdown();
                        }
                    }
                    finally
                    {
                        client1.Shutdown();
                    }
                }
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_Unsubscribe()
        {
            using (var client = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                client.Startup();

                try
                {
                    Assert.IsTrue(client.TenantSubscribe(_tenantId1));
                    Assert.IsTrue(client.TenantUnsubscribe(_tenantId1));

                    var list = client.GetSubscribedTenants();

                    Assert.IsTrue(!list.Any());
                }
                finally
                {
                    client.Shutdown();
                }
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_GetSubscribedTenants()
        {
            using (var client = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                client.Startup();

                try
                {
                    Assert.IsTrue(client.TenantSubscribe(_tenantId1));

                    var list = client.GetSubscribedTenants();

                    var enumerable = list as Guid[] ?? list.ToArray();

                    Assert.IsTrue(enumerable.Any());
                    Assert.IsTrue(enumerable.FirstOrDefault().Equals(_tenantId1));
                }
                finally
                {
                    client.Shutdown();
                }
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_OnStateChange()
        {
            var states = new ConcurrentBag<ClientStateArgs>();

            using (var client = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                var signal = new EventWaitHandle(false, EventResetMode.ManualReset);
                var signalEvent = signal;

                client.OnStateChange += ((sender, args) =>
                {
                    states.Add(args);

                    if (args.State.Equals(ClientState.Disconnected))
                    {
                        signalEvent.Set();
                    }
                });

                client.Startup();
                client.Shutdown();

                signal.WaitOne();
                signal.Dispose();

                Assert.IsTrue(states.Count == 3);

                Assert.IsTrue(states.Any(arg => arg.State.Equals(ClientState.Connecting)));
                Assert.IsTrue(states.Any(arg => arg.State.Equals(ClientState.Connected)));
                Assert.IsTrue(states.Any(arg => arg.State.Equals(ClientState.Disconnected)));
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        [TestMethod]
        public void ConnectorClient_OnNotification()
        {
            var notifications = new ConcurrentBag<ClientNotifyArgs>();

            using (var client = new ConnectorClient(_connectorId, GetConnectorKey, Address))
            {
                var signal = new EventWaitHandle(false, EventResetMode.ManualReset);
                var signalEvent = signal;

                client.OnNotification += ((sender, args) =>
                {
                    notifications.Add(args);
                    signalEvent.Set();
                });

                client.Startup();

                Assert.IsTrue(client.TenantSubscribe(_tenantId1));

                _host.Controller.AddNotification(_tenantId1, 5);

                signal.WaitOne();
                signal.Dispose();

                client.Shutdown();

                Assert.IsTrue(notifications.Count == 1);

                var enumerable = notifications.ToArray();

                Assert.IsTrue(enumerable[0].TenantId.Equals(_tenantId1));
                Assert.IsTrue(enumerable[0].Count == 5);
            }
        }
    }
}
