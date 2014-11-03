using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Common;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.TestUtilities;

namespace Sage.Connector.NotificationService.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class NotificationServiceTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
        }
        #endregion

        /// <summary>
        /// Simple test of proxy connectivity
        /// </summary>
        [TestMethod]
        public void TestServiceProxy()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    Assert.IsNotNull(proxy, "Failed to retrieve notification proxy.");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        private sealed class Subscriber
        {
            public Subscriber()
            {
                ConfigurationAddedOccurrences = new List<String>();
                ConfigurationUpdatedOccurrences = new List<String>();
                ConfigurationDeletedOccurrences = new List<String>();
            }

            public void ConfigurationAdded(String cloudTenantId)
            { ConfigurationAddedOccurrences.Add(cloudTenantId); }

            public void ConfigurationUpdated(String cloudTenantId)
            { ConfigurationUpdatedOccurrences.Add(cloudTenantId); }

            public void ConfigurationDeleted(String cloudTenantId)
            { ConfigurationDeletedOccurrences.Add(cloudTenantId); }

            public void Clear()
            {
                ConfigurationAddedOccurrences.Clear();
                ConfigurationUpdatedOccurrences.Clear();
                ConfigurationDeletedOccurrences.Clear();
            }

            public List<String> ConfigurationAddedOccurrences
            { get; set; }

            public List<String> ConfigurationUpdatedOccurrences
            { get; set; }

            public List<String> ConfigurationDeletedOccurrences
            { get; set; }
        }

        //suppressed complaint about subscriptionServiceProxy not being disposed on all paths after making sure it is
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), TestMethod]
        public void TestServiceProxyWithSubscription()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                Subscriber subscriber = new Subscriber();
                NotificationCallbackInstanceHelper callbackInstance = new NotificationCallbackInstanceHelper();
                NotificationSubscriptionServiceProxy subscriptionServiceProxy = null;

                try
                {
                    subscriptionServiceProxy = NotificationSubscriptionServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber, callbackInstance);
                    Assert.IsNotNull(subscriptionServiceProxy, "Failed to retrieve notification subscription proxy.");

                    subscriptionServiceProxy.Connect();
                    callbackInstance.SubscribeConfigurationAdded(subscriptionServiceProxy, subscriber.ConfigurationAdded);
                    callbackInstance.SubscribeConfigurationUpdated(subscriptionServiceProxy, subscriber.ConfigurationUpdated);
                    callbackInstance.SubscribeConfigurationDeleted(subscriptionServiceProxy, subscriber.ConfigurationDeleted);

                    using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        Assert.IsNotNull(proxy, "Failed to retrieve notification proxy.");
                        proxy.NotifyConfigurationAdded(Guid.NewGuid().ToString());
                        proxy.NotifyConfigurationAdded(Guid.NewGuid().ToString());
                        proxy.NotifyConfigurationUpdated(Guid.NewGuid().ToString());
                        proxy.NotifyConfigurationUpdated(Guid.NewGuid().ToString());
                        proxy.NotifyConfigurationUpdated(Guid.NewGuid().ToString());
                        proxy.NotifyConfigurationDeleted(Guid.NewGuid().ToString());

                        System.Threading.Thread.Sleep(5000);
                        Assert.AreEqual(2, subscriber.ConfigurationAddedOccurrences.Count);
                        Assert.AreEqual(3, subscriber.ConfigurationUpdatedOccurrences.Count);
                        Assert.AreEqual(1, subscriber.ConfigurationDeletedOccurrences.Count);
                    }

                    callbackInstance.Unsubscribe(subscriptionServiceProxy);
                    subscriptionServiceProxy.Disconnect();
                    subscriptionServiceProxy.Close();
                    subscriptionServiceProxy = null;
                }
                finally
                {
                    if (subscriptionServiceProxy != null)
                    {
                        subscriptionServiceProxy.Abort();
                    }
                    callbackInstance = null;
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }
    }
}
