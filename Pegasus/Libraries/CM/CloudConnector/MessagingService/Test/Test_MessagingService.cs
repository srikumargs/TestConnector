using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Common;
using Sage.Connector.Logging;
using Sage.Connector.MessagingService.Internal;
using Sage.Connector.MessagingService.Proxy;
using Sage.Connector.SageCloudService;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.TestUtilities;


namespace Sage.Connector.MessagingService.Test
{
    /// <summary>
    /// Summary description for Test_MessagingService
    /// </summary>
    [TestClass]
    public class Test_MessagingService
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

        [TestMethod]
        public void TestPremiseAgentHelper()
        {
            //Explicitly test the ValidateTenantConnection in Messaging service.

            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                Guid tenantGuid = new Guid("{5dc75a87-4688-4d45-b4ff-91d43db98072}");
                var pa = PremiseAgentHelper.GetPremiseAgent(tenantGuid.ToString());
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod]
        public void TestMessageService_ValidateTenantConnection()
        {
            //Explicitly test the ValidateTenantConnection in Messaging service.

            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                Guid tenantGuid = new Guid("{5dc75a87-4688-4d45-b4ff-91d43db98072}");
                string premiseId = "Test";
                string backofficeName = "BackOffice1";

                var result = new ValidateTenantConnectionResponse(String.Empty, null, TenantConnectivityStatus.None);
                try
                {
                    MockCloudServiceHost.StartService();
                    using (var proxy = MessagingServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        result = proxy.ValidateTenantConnection(MockCloudServiceHost.SiteAddress, tenantGuid.ToString(), premiseId, String.Empty);
                    }

                    Assert.IsNotNull(result.SiteAddress, "Site address should not be null.");
                    Assert.IsFalse(string.IsNullOrEmpty(result.SiteAddress.AbsoluteUri), "Site address should not be null.");
                    Assert.AreEqual(TenantConnectivityStatus.Normal, result.TenantConnectivityStatus, "Connectivity status is not reporting Normal.");
                    Assert.AreEqual(backofficeName.ToLower(), result.Name.ToLower(), "Backoffice name does not match.");

                    using (var proxy = MessagingServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        result = proxy.ValidateTenantConnection(MockCloudServiceHost.SiteAddress.Replace("https", "http"), tenantGuid.ToString(), premiseId, String.Empty);
                    }
                    Assert.IsNotNull(result.SiteAddress, "Site address should not be null.");
                    Assert.IsFalse(string.IsNullOrEmpty(result.SiteAddress.AbsoluteUri), "Site address should not be null.");
                    Assert.AreEqual(TenantConnectivityStatus.Normal, result.TenantConnectivityStatus, "Connectivity status is not reporting Normal.");
                    Assert.AreEqual(backofficeName.ToLower(), result.Name.ToLower(), "Backoffice name does not match.");
                }
                catch (Exception ex)
                {
                    // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                    // to send error to client
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(this, ex.ExceptionAsString());
                    }
                }
                finally
                {
                    MockCloudServiceHost.StopService();
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }
    }
}
