using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Common;
using Sage.Connector.ConfigurationService.Proxy;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Queues;
using Sage.Connector.SageCloudService;
using Sage.Connector.TestUtilities;
using Sage.Connector.Cloud.Integration.Interfaces;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Connector.LinkedSource;
using CloudInterfaceUtils = Sage.Connector.Cloud.Integration.Interfaces.Utils;
using Mock = Sage.Connector.SageCloudService;

namespace Sage.Connector.IntegrationTest
{
    /// <summary>
    /// Summary description for IntegrationTestUsingHostingFramework
    /// </summary>
    [TestClass]
    public class HostingFrameworkIntegrationTest
    {
        #region Properties & Local Variables

        private const Int32 HOSTING_FRAMEWORK_SERVICE_TIME_LIMIT = 5 * 60 * 1000;
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

        #endregion

        #region Additional test attributes

        /// <summary>
        /// startup and teardown the MockCloudServiceHost for each test, so that it is in a known state  
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            MockCloudServiceHost.StartService();
        }



        /// <summary>
        /// startup and teardown the MockCloudServiceHost for each test, so that it is in a known state  
        /// </summary>
        [TestCleanup]
        public void MyTestCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            MockCloudServiceHost.StopService();
        }
            
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Constructs a sample loopback message
        /// </summary>
        /// <returns></returns>
        private static Request CreateLoopBackRequest()
        { return new LoopBackRequest(Guid.NewGuid(), DateTime.UtcNow, 0, 0, "requestingUser"); }

        /// <summary>
        /// Create a simple LoopBackRequest and serializes it
        /// </summary>
        /// <returns></returns>
        private static string CreateLoopBackRequestString()
        { return ConnectorServiceCommon.Utils.JsonSerialize(CreateLoopBackRequest()); }

        #endregion

        #region Tests

        [TestMethod]
        public void TestStartAndStopHostingFrameworkService()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                if (TestUtils.RunHostingFxAsService())
                {
                    using (var service = new System.ServiceProcess.ServiceController(ConnectorServiceUtils.ServiceName))
                    {
                        Assert.AreEqual(System.ServiceProcess.ServiceControllerStatus.Running, service.Status, "The Sage Hosting Framework Service is not running.");
                    }

                    TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx);

                    using (var service = new System.ServiceProcess.ServiceController(ConnectorServiceUtils.ServiceName))
                    {
                        DateTime start = DateTime.UtcNow;
                        while (service.Status != System.ServiceProcess.ServiceControllerStatus.Stopped &&
                            (DateTime.UtcNow - start) < TimeSpan.FromMilliseconds(HOSTING_FRAMEWORK_SERVICE_TIME_LIMIT))
                        {
                            System.Threading.Thread.Sleep(1000);
                            service.Refresh();
                        }

                        service.Refresh();
                        Assert.AreEqual(System.ServiceProcess.ServiceControllerStatus.Stopped, service.Status, "The Sage Hosting Framework Service is not stopped.");
                    }
                }
                else
                {
                    // if not running as a windows servce, then the most we can do is check the service ready mutex

                    Assert.IsTrue(ConnectorServiceUtils.IsServiceReady(), "The Sage Hosting Framework Service is not running.");

                    TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx);

                    Assert.IsFalse(ConnectorServiceUtils.IsServiceProcessRunning(), "The Sage Hosting Framework Service is not stopped.");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }
        }

        [TestMethod]
        public void TestHostingFrameworkGetReportListFullCircle_MultiTenant()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                // not using "using" for the raw channel, the channel can fault on Close() and if it does we should also call Abort()
                ChannelFactory<ICREMessageServiceInjection> csiFactory = null;
                try
                {
                    csiFactory = CreateMockMessageServiceInjectionChannelFactory();
                    Guid testGuid1 = Guid.NewGuid();
                    var request = new LoopBackRequest(testGuid1, DateTime.UtcNow, 0, 0, "");
                    ICREMessageServiceInjection client = csiFactory.CreateChannel();
                    Assert.IsTrue(client.ExternalAddToPremiseMessage(
                        MockCloudService.TenantIds.ToArray()[0],
                        request));
                    TraceUtils.WriteLine("Injected {1} ReportListRequest with Id: {0}", testGuid1, MockCloudService.TenantIds.ToArray()[0]);


                    Guid testGuid2 = Guid.NewGuid();
                    request = new LoopBackRequest(testGuid2, DateTime.UtcNow, 0, 0, "");
                    client = csiFactory.CreateChannel();
                    Assert.IsTrue(client.ExternalAddToPremiseMessage(
                        MockCloudService.TenantIds.ToArray()[1],
                        request));
                    TraceUtils.WriteLine("Injected {1} ReportListRequest with Id: {0}", testGuid2, MockCloudService.TenantIds.ToArray()[1]);


                    // Setup a timeout with two minutes (Note: Timeout must be greater than client's max polling frequency)
                    DateTime startUtc = DateTime.UtcNow;
                    TimeSpan timeoutTimeSpan = new TimeSpan(0, 2, 0);

                    // Wait for the response. Poll the CloudService until we get the response or we timeout
                    bool responseReceived1 = false;
                    bool responseReceived2 = false;
                    while ((DateTime.UtcNow - startUtc) < timeoutTimeSpan && (!responseReceived1 || !responseReceived2))
                    {
                        IEnumerable<Response> respMessages1 = client.ExternalPeekInboxMessage(
                            MockCloudService.TenantIds.ToArray()[0]);
                        foreach (var message in respMessages1)
                        {
                            if (null != message)
                            {
                                TraceUtils.WriteLine("Receive MessageKind: {0} with RequestMessageId: {1}", message.GetType().FullName, message.RequestId);
                                if (message.GetType() == typeof(LoopBackRequestResponse))
                                {
                                    responseReceived1 = true;
                                    Assert.AreEqual(testGuid1, message.RequestId, "ReportListResponse message received but RequestId is not correct!");
                                    break;
                                }
                            }
                        }

                        IEnumerable<Response> respMessages2 = client.ExternalPeekInboxMessage(
                            MockCloudService.TenantIds.ToArray()[1]);
                        foreach (var message in respMessages2)
                        {
                            if (null != message)
                            {
                                TraceUtils.WriteLine("Receive MessageKind: {0} with RequestMessageId: {1}", message.GetType().FullName, message.RequestId);
                                if (message.GetType() == typeof(LoopBackRequestResponse))
                                {
                                    responseReceived2 = true;
                                    Assert.AreEqual(testGuid2, message.RequestId, "ReportListResponse message received but RequestMessageId is not correct!");
                                    break;
                                }
                            }
                        }


                        if (!responseReceived1 || !responseReceived2)
                        {
                            System.Threading.Thread.Sleep(10000);
                        }
                    }
                    Assert.IsTrue(responseReceived1, String.Format("No ReportListResponse message received for tenant {0}!",MockCloudService.TenantIds.ToArray()[0]));
                    Assert.IsTrue(responseReceived2, String.Format("No ReportListResponse message received for tenant {0}!",MockCloudService.TenantIds.ToArray()[1]));

                    csiFactory.Close();
                    csiFactory = null;
                }
                finally
                {
                    if (csiFactory != null)
                    {
                        csiFactory.Abort();
                        csiFactory = null;
                    }
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

       
        [TestMethod]
        public void TestHostingFrameworkGetHealthCheckFullCircle()
        {
            // Create the request
            var healthCheckRequest = new HealthCheckRequest(
                Guid.NewGuid(), DateTime.UtcNow, 0, 0, "requestingUser");

            // Standard processing
            ProcessStandardRequestResponse<HealthCheckRequestResponse>(healthCheckRequest);
        }


        /// <summary>
        /// Check a couple of update site service info requests in a row
        /// </summary>
        [TestMethod]
        public void TestHostingFrameworkMultipleUpdateSiteServiceInfoFullCircle()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                // not using "using" for the raw channel, the channel can fault on Close() and if it does we should also call Abort()
                ChannelFactory<ICREMessageServiceInjection> csiFactory = null;
                int numRequests = 2;
                try
                {
                    // Get the tenant ID for this test, and its current gateway service info
                    String tenantId = MockCloudService.TenantIds.ToArray()[0];
                    ServiceInfo gatewayServiceInfo = MockCloudService.RetrieveGatewayServiceInfo(tenantId);

                    // Create the injector client
                    csiFactory = CreateMockMessageServiceInjectionChannelFactory();
                    ICREMessageServiceInjection client = csiFactory.CreateChannel();

                    // Create the test guids and send the requests
                    List<Guid> testGuids = new List<Guid>();
                    for (int i = 0; i < numRequests; ++i)
                    {
                        // Create/add the next test request guid
                        Guid testGuid = Guid.NewGuid();
                        testGuids.Add(testGuid);

                        // Create a bad URI for the first test only
                        string mockServiceUriString = (i == 0)
                            ? String.Concat(MockCloudServiceHost.SiteAddress, "x")
                            : MockCloudServiceHost.SiteAddress;

                        // Create an update site service info request using the same gateway service info
                        var updateServiceInfoRequest = new UpdateSiteServiceInfoRequest(
                            testGuid, DateTime.UtcNow, 0, 0, "requestingUser", new Uri(MockCloudServiceHost.SiteAddress));

                        Assert.IsTrue(client.ExternalAddToPremiseMessage(tenantId, updateServiceInfoRequest),
                            String.Format("Failed to inject UpdateSiteServiceInfoRequest # {0}", i + 1));
                        TraceUtils.WriteLine("Injected UpdateSiteServiceInfoRequest with Id: {0}", testGuid);
                    }

                    // Setup a timeout with two minutes (Note: Timeout must be greater than client's max polling frequency)
                    DateTime startUtc = DateTime.UtcNow;
                    TimeSpan timeoutTimeSpan = new TimeSpan(0, 4, 0);

                    // Wait for the response. Poll the CloudService until we get the expected number of responses or we timeout
                    int numResponsesReceived = 0;
                    while ((DateTime.UtcNow - startUtc) < timeoutTimeSpan && (numResponsesReceived < numRequests))
                    {
                        IEnumerable<Response> respMessages = client.ExternalPeekInboxMessage(
                            MockCloudService.TenantIds.ToArray()[0]);
                        foreach (var message in respMessages)
                        {
                            if (null != message)
                            {
                                TraceUtils.WriteLine("Receive MessageKind: {0} with RequestMessageId: {1}", message.GetType().FullName, message.RequestId);
                                if (message.GetType() == typeof(UpdateSiteServiceInfoRequestResponse))
                                {
                                    Assert.IsTrue(testGuids.Contains(message.RequestId),
                                        "UpdateSiteServiceInfoRequestResponse message received but RequestMessageId does not match any!");

                                    // Check if ALL responses were received
                                    if (++numResponsesReceived == numRequests)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (numResponsesReceived != numRequests)
                        {
                            System.Threading.Thread.Sleep(10000);
                        }
                    }

                    Assert.AreEqual(numRequests, numResponsesReceived,
                        "Not all expected UpdateSiteServiceInfoRequestResponse messages received!");

                    csiFactory.Close();
                    csiFactory = null;
                }
                finally
                {
                    if (csiFactory != null)
                    {
                        csiFactory.Abort();
                        csiFactory = null;
                    }
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }


        [TestMethod]
        public void TestHostingFrameworkGetReportListFullCircle()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                // not using "using" for the raw channel, the channel can fault on Close() and if it does we should also call Abort()
                ChannelFactory<ICREMessageServiceInjection> csiFactory = null;
                try
                {
                    csiFactory = CreateMockMessageServiceInjectionChannelFactory();
                    Guid testGuid = Guid.NewGuid();
                    var reportListRequest = new LoopBackRequest(testGuid, DateTime.UtcNow, 0, 0, "");
                    ICREMessageServiceInjection client = csiFactory.CreateChannel();
                    Assert.IsTrue(client.ExternalAddToPremiseMessage(
                        MockCloudService.TenantIds.ToArray()[0],
                        reportListRequest));

                    TraceUtils.WriteLine("Injected ReportListRequest with Id: {0}", testGuid);

                    // Setup a timeout with two minutes (Note: Timeout must be greater than client's max polling frequency)
                    DateTime startUtc = DateTime.UtcNow;
                    TimeSpan timeoutTimeSpan = new TimeSpan(0, 2, 0);

                    // Wait for the response. Poll the CloudService until we get the response or we timeout
                    bool responseReceived = false;
                    while ((DateTime.UtcNow - startUtc) < timeoutTimeSpan && !responseReceived)
                    {
                        IEnumerable<Response> respMessages = client.ExternalPeekInboxMessage(
                            MockCloudService.TenantIds.ToArray()[0]);
                        foreach (var message in respMessages)
                        {
                            if (null != message)
                            {
                                TraceUtils.WriteLine("Receive MessageKind: {0} with RequestMessageId: {1}", message.GetType().FullName, message.RequestId);
                                if (message.GetType() == typeof(LoopBackRequestResponse))
                                {
                                    responseReceived = true;
                                    Assert.IsTrue(message.RequestId == testGuid,
                                        "ReportListResponse message received but RequestMessageId is invalid!");
                                    break;
                                }
                            }
                        }

                        if (!responseReceived)
                        {
                            System.Threading.Thread.Sleep(10000);
                        }
                    }
                    Assert.IsTrue(responseReceived, "No ReportListResponse message received!");

                    csiFactory.Close();
                    csiFactory = null;
                }
                finally
                {
                    if (csiFactory != null)
                    {
                        csiFactory.Abort();
                        csiFactory = null;
                    }
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod]
        public void TestHostingFrameworkGetMultipleReportLists()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                // not using "using" for the raw channel, the channel can fault on Close() and if it does we should also call Abort()
                ChannelFactory<ICREMessageServiceInjection> csiFactory = null;
                try
                {
                    csiFactory = CreateMockMessageServiceInjectionChannelFactory();
                    Guid testGuid = Guid.NewGuid();
                    var reportListRequest = new LoopBackRequest(testGuid, DateTime.UtcNow, 0, 0, "");
                    ICREMessageServiceInjection client = csiFactory.CreateChannel();

                    int numReportLists = 7;
                    for (int i = 0; i < numReportLists; i++)
                    {
                        Assert.IsTrue(client.ExternalAddToPremiseMessage(
                            MockCloudService.TenantIds.ToArray()[0],
                            reportListRequest));
                    }

                    TraceUtils.WriteLine("Injected {0} ReportListRequests with Id: {1}", numReportLists, testGuid);

                    // Setup a timeout (Note: Timeout must be greater than client's max polling frequency)
                    DateTime startUtc = DateTime.UtcNow;
                    TimeSpan timeoutTimeSpan = new TimeSpan(0, 4, 0);

                    // Wait for the response. Poll the CloudService until we get all responses or we timeout
                    int numReportListsReceived = 0;
                    int maxNumReportListsReceived = 0;
                    while ((DateTime.UtcNow - startUtc) < timeoutTimeSpan && numReportListsReceived < numReportLists)
                    {
                        IEnumerable<Response> respMessages = client.ExternalPeekInboxMessage(
                            MockCloudService.TenantIds.ToArray()[0]);

                        if (respMessages.Count() > maxNumReportListsReceived)
                            maxNumReportListsReceived = respMessages.Count();

                        foreach (var message in respMessages)
                        {
                            if (message.GetType() == typeof(LoopBackRequestResponse))
                            {
                                numReportListsReceived++;
                                Assert.IsTrue(message.RequestId == testGuid,
                                    "ReportListResponse message received but RequestMessageId is invalid!");
                            }
                        }

                        if (numReportListsReceived < numReportLists)
                        {
                            // Reset the numReportListsReceived since ExternalPeekMessage doesn't clear the inbox
                            numReportListsReceived = 0;
                            System.Threading.Thread.Sleep(10000);
                        }
                    }
                    Assert.IsTrue(numReportListsReceived == numReportLists,
                        string.Format("numReportListsReceived ({0}) != numReportListsReceived requested ({1})",
                            numReportListsReceived, numReportLists));

                    csiFactory.Close();
                    csiFactory = null;
                }
                finally
                {
                    if (csiFactory != null)
                    {
                        csiFactory.Abort();
                        csiFactory = null;
                    }
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod]
        public void TestGetLogEntriesAfterGetMultipleReportLists()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                // not using "using" for the raw channel, the channel can fault on Close() and if it does we should also call Abort()
                ChannelFactory<ICREMessageServiceInjection> csiFactory = null;
                try
                {
                    csiFactory = CreateMockMessageServiceInjectionChannelFactory();
                    Guid testGuid = Guid.NewGuid();
                    var reportListRequest = new LoopBackRequest(testGuid, DateTime.UtcNow, 0, 0, "");
                    ICREMessageServiceInjection client = csiFactory.CreateChannel();

                    int numReportLists = 5;
                    for (int i = 0; i < numReportLists; i++)
                    {
                        Assert.IsTrue(client.ExternalAddToPremiseMessage(
                            MockCloudService.TenantIds.ToArray()[0],
                            reportListRequest));
                    }

                    TraceUtils.WriteLine("Injected {0} ReportListRequests with Id: {1}", numReportLists, testGuid);

                    // Setup a timeout (Note: Timeout must be greater than client's max polling frequency)
                    DateTime startUtc = DateTime.UtcNow;
                    TimeSpan timeoutTimeSpan = new TimeSpan(0, 4, 0);

                    // Wait for the response. Poll the CloudService until we get all responses or we timeout
                    int numReportListsReceived = 0;
                    int maxNumReportListsReceived = 0;
                    while ((DateTime.UtcNow - startUtc) < timeoutTimeSpan && numReportListsReceived < numReportLists)
                    {
                        IEnumerable<Response> respMessages = client.ExternalPeekInboxMessage(
                            MockCloudService.TenantIds.ToArray()[0]);

                        if (respMessages.Count() > maxNumReportListsReceived)
                            maxNumReportListsReceived = respMessages.Count();

                        foreach (var message in respMessages)
                        {
                            if (message.GetType() == typeof(LoopBackRequestResponse))
                            {
                                numReportListsReceived++;
                                Assert.IsTrue(message.RequestId == testGuid,
                                    "ReportListResponse message received but RequestMessageId is invalid!");
                            }
                        }

                        if (numReportListsReceived < numReportLists)
                        {
                            // Reset the numReportListsReceived since ExternalPeekMessage doesn't clear the inbox
                            numReportListsReceived = 0;
                            System.Threading.Thread.Sleep(10000);
                        }
                    }
                    Assert.IsTrue(numReportListsReceived == numReportLists,
                        string.Format("numReportListsReceived ({0}) != numReportListsReceived requested ({1})",
                            numReportListsReceived, numReportLists));

                    // Get LogEntries
                    using (var proxy =
                        ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        TraceUtils.WriteLine(String.Format(@"*** ConfigWizard entered GetLogEntries at at {0}",
                            DateTime.Now.ToString(@"M/d/yyyy hh:mm:ss tt")));
                        LogEntryRecord[] le = proxy.GetLogEntries();
                        TraceUtils.WriteLine(String.Format(@"*** ConfigWizard: Got {0} LogEntries from ConfigService",
                            le.Length));
                        Assert.IsTrue(le.Length > 0, "No LogEntries retrieved!");
                    }

                    csiFactory.Close();
                    csiFactory = null;
                }
                finally
                {
                    if (csiFactory != null)
                    {
                        csiFactory.Abort();
                        csiFactory = null;
                    }
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        /// <summary>
        /// Injects a premise loopback message and expect same for cloud message (eventually)
        /// </summary>
        [TestMethod]
        public void TestHostingFrameworkLoopbackTest()
        {
            // Create the request
            Request loopBackRequest =
                ConnectorServiceCommon.Utils.JsonDeserialize<LoopBackRequest>(CreateLoopBackRequestString());

            // Standard processing
            ProcessStandardRequestResponse<LoopBackRequestResponse>(loopBackRequest);
        }

        /// <summary>
        /// Injects multiple loopback messages and expect same for cloud message (eventually)
        /// </summary>
        [TestMethod]
        public void TestHostingFrameworkMultiLoopbackTest()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                // not using "using" for the raw channel, the channel can fault on Close() and if it does we should also call Abort()
                ChannelFactory<ICREMessageServiceInjection> csiFactory = null;
                try
                {
                    csiFactory = CreateMockMessageServiceInjectionChannelFactory();
                    ICREMessageServiceInjection client = csiFactory.CreateChannel();

                    int numLoopbacks = 10;
                    for (int i = 0; i < numLoopbacks; i++)
                    {
                        Assert.IsTrue(client.ExternalAddToPremiseMessage(
                            MockCloudService.TenantIds.ToArray()[0],
                            ConnectorServiceCommon.Utils.JsonDeserialize<LoopBackRequest>(CreateLoopBackRequestString())));
                    }

                    // Setup a timeout with four minutes (Note: Timeout must be greater than client's max polling frequency)
                    DateTime startUtc = DateTime.UtcNow;
                    TimeSpan timeoutTimeSpan = new TimeSpan(0, 4, 0);

                    // Wait for the response. Poll the CloudService until we get the response or we timeout
                    int numLoopbacksReceived = 0;
                    int maxNumLoopbacksReceived = 0;
                    while ((DateTime.UtcNow - startUtc) < timeoutTimeSpan && numLoopbacksReceived < numLoopbacks)
                    {
                        IEnumerable<Response> respMessages = client.ExternalPeekInboxMessage(
                            MockCloudService.TenantIds.ToArray()[0]);

                        if (respMessages.Count() > maxNumLoopbacksReceived)
                            maxNumLoopbacksReceived = respMessages.Count();

                        foreach (var message in respMessages)
                        {
                            if (message.GetType() == typeof(LoopBackRequestResponse))
                            {
                                numLoopbacksReceived++;
                            }
                        }

                        if (numLoopbacksReceived < numLoopbacks)
                        {
                            // Reset the numLoopbacksReceived since ExternalPeekMessage doesn't clear the inbox
                            numLoopbacksReceived = 0;
                            System.Threading.Thread.Sleep(10000);
                        }
                    }
                    // TODO: Not sure why we receive more than the numLoopbacks since we clear the outbox.
                    Assert.IsTrue(numLoopbacksReceived == numLoopbacks,
                        string.Format("numLoopbacksReceived ({0}) != numLoopbacks requested ({1})",
                            maxNumLoopbacksReceived, numLoopbacks));

                    csiFactory.Close();
                    csiFactory = null;
                }
                finally
                {
                    if (csiFactory != null)
                    {
                        csiFactory.Abort();
                        csiFactory = null;
                    }
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod]
        public void TestMultipleTenantConfigurations()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    TraceUtils.WriteLine("Deleting existing configs...");

                    var configs = proxy.GetConfigurations();
                    foreach (var config in configs)
                    {
                        proxy.DeleteConfiguration(config.CloudTenantId);
                    }


                    var mockTenantIds = MockCloudService.TenantIds.ToArray();
                    var mockPremiseKeys =MockCloudService.TenantPremiseKeys.ToArray();

                    Int32 count = Math.Min(10, mockTenantIds.Count());

                    TraceUtils.WriteLine("Creating {0} disabled configurations...", count);

                    for (Int32 i = 0; i < count; i++)
                    {
                        var configuration = proxy.CreateNewConfiguration();
                        TestUtils.SetConfigValuesForMockCloud(mockTenantIds[i], mockPremiseKeys[i], configuration);
                        configuration.CloudConnectionEnabledToReceive = false;
                        configuration.CloudConnectionEnabledToSend = false;
                        configuration.BackOfficeConnectionEnabledToReceive = false;
                        configuration.BackOfficeCompanyName = String.Format("Company {0}", i);

                        proxy.AddConfiguration(configuration);
                    }

                    TraceUtils.WriteLine("Enabling {0} disabled configurations...", count);

                    for (Int32 i = 0; i < count; i++)
                    {
                        var configuration = proxy.GetConfiguration(mockTenantIds[i]);
                        configuration.CloudConnectionEnabledToReceive = true;
                        configuration.CloudConnectionEnabledToSend = true;
                        configuration.BackOfficeConnectionEnabledToReceive = true;
                        proxy.UpdateConfiguration(configuration);
                    }
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        private ChannelFactory<ICREMessageServiceInjection> CreateMockMessageServiceInjectionChannelFactory()
        {
            ChannelFactory<ICREMessageServiceInjection> csiFactory = null;
            try
            {
                string mockEndpointTemplate = "http://{0}:8002/MockCloudServiceInjection.svc";
                string mockEndpoingURIString = String.Format(mockEndpointTemplate, Environment.MachineName);
                EndpointAddress address = new EndpointAddress(mockEndpoingURIString);
                var binding = CloudUtils.CreateCloudBinding(new Uri(mockEndpoingURIString));

                csiFactory = new ChannelFactory<ICREMessageServiceInjection>(binding, address);
            }
            catch
            {
                if (csiFactory != null)
                {
                    ((IDisposable)csiFactory).Dispose();
                }
                throw;
            }
            return csiFactory;
        }

        /// <summary>
        /// Injects a loopback when the cloud is disabled
        /// and expect cloud delivery when cloud re-enabled
        /// </summary>
        [TestMethod]
        public void TestHostingFrameworkDelayedLoopbackTest()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try{

                using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    // Disable the cloud connection
                    PremiseConfigurationRecord pcr = proxy.GetConfiguration(MockCloudService.TenantIds.ToArray()[0]);
                    pcr.CloudConnectionEnabledToReceive = false;
                    pcr.CloudConnectionEnabledToSend = false;
                    proxy.UpdateConfiguration(pcr);

                    // Inject a loopback response to the tenant's outbox
                    LoopBackRequestResponse innerResponse = new LoopBackRequestResponse(
                       Guid.NewGuid(),
                       Guid.NewGuid(),
                       DateTime.UtcNow);
                    ResponseWrapper response = new ResponseWrapper(
                        new ActivityTrackingContext(Guid.NewGuid(), String.Empty, Guid.Empty, String.Empty),
                        string.Empty,
                        innerResponse.Id,
                        innerResponse.GetType().FullName,
                        Utils.JsonSerialize(innerResponse),
                        null);
                    String sResponse = ConnectorServiceCommon.Utils.JsonSerialize(response);

                    using (var qm = new QueueManager())
                    {
                        qm.AddMessageToOutput(sResponse, innerResponse.GetType().FullName,  QueueContext.FakeTenantInstance(MockCloudService.TenantIds.ToArray()[0]));
                    }

                    // Re-enable the cloud connection
                    pcr = proxy.GetConfiguration(MockCloudService.TenantIds.ToArray()[0]);
                    pcr.CloudConnectionEnabledToReceive = true;
                    pcr.CloudConnectionEnabledToSend = true;
                    proxy.UpdateConfiguration(pcr);

                    // not using "using" for the raw channel, the channel can fault on Close() and if it does we should also call Abort()
                    ChannelFactory<ICREMessageServiceInjection> csiFactory = null;
                    try
                    {
                        csiFactory = CreateMockMessageServiceInjectionChannelFactory();
                        ICREMessageServiceInjection client = csiFactory.CreateChannel();

                        // Setup a timeout for a minute
                        DateTime startUtc = DateTime.UtcNow;
                        TimeSpan timeoutTimeSpan = new TimeSpan(0, 1, 0);

                        // Wait for the response. Poll the CloudService until we get the response or we timeout
                        bool loopbackReceived = false;
                        while ((DateTime.UtcNow - startUtc) < timeoutTimeSpan && !loopbackReceived)
                        {
                            IEnumerable<Response> respMessages = client.ExternalPeekInboxMessage(
                                MockCloudService.TenantIds.ToArray()[0]);
                            foreach (var message in respMessages)
                            {
                                if (message.GetType() == typeof(LoopBackRequestResponse))
                                {
                                    loopbackReceived = true;
                                    break;
                                }
                            }

                            if (!loopbackReceived)
                            {
                                System.Threading.Thread.Sleep(10000);
                            }
                        }
                        Assert.IsTrue(loopbackReceived);

                        csiFactory.Close();
                        csiFactory = null;
                    }
                    finally
                    {
                        if (csiFactory != null)
                        {
                            csiFactory.Abort();
                            csiFactory = null;
                        }
                    }
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }
        #endregion


        #region Private Methods

        /// <summary>
        /// If you're just testing a standard single request/response
        /// Use this method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        private void ProcessStandardRequestResponse<T>(Request request)
            where T : Response
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                // Get the request type name for errors, etc.
                string requestTypeName = request.GetType().FullName;

                // Not using "using" for the raw channel, the channel can fault on Close() 
                // And if it does we should also call Abort()
                ChannelFactory<ICREMessageServiceInjection> csiFactory = null;
                try
                {
                    // Create message injection client
                    csiFactory = CreateMockMessageServiceInjectionChannelFactory();
                    ICREMessageServiceInjection client = csiFactory.CreateChannel();

                    // Inject the request for a test tenant
                    Assert.IsTrue(client.ExternalAddToPremiseMessage(
                        MockCloudService.TenantIds.ToArray()[0], request));
                    TraceUtils.WriteLine("Injected {0} with Id: {1}", requestTypeName, request.Id);

                    // Setup a timeout with two minutes 
                    // Note: Timeout must be greater than client's max polling frequency
                    DateTime startUtc = DateTime.UtcNow;
                    TimeSpan timeoutTimeSpan = new TimeSpan(0, 2, 0);

                    // Wait for the response
                    // Poll the CloudService until we get the response or we timeout
                    bool responseReceived = false;
                    while ((DateTime.UtcNow - startUtc) < timeoutTimeSpan && !responseReceived)
                    {
                        IEnumerable<Response> respMessages = client.ExternalPeekInboxMessage(
                            MockCloudService.TenantIds.ToArray()[0]);
                        foreach (var message in respMessages)
                        {
                            if (null != message)
                            {
                                TraceUtils.WriteLine("Receive MessageKind: {0} with RequestMessageId: {1}", message.GetType().FullName, message.RequestId);
                                if (message.GetType() == typeof(T))
                                {
                                    responseReceived = true;
                                    Assert.IsTrue(message.RequestId == request.Id,
                                        String.Format("{0} message received but RequestMessageId is invalid!", requestTypeName));
                                    break;
                                }
                            }
                        }

                        if (!responseReceived)
                        {
                            System.Threading.Thread.Sleep(10000);
                        }
                    }

                    // Check that we got the expected response
                    Assert.IsTrue(responseReceived,
                        String.Format("No {0} message received!", requestTypeName));

                    csiFactory.Close();
                    csiFactory = null;
                }
                finally
                {
                    if (csiFactory != null)
                    {
                        csiFactory.Abort();
                        csiFactory = null;
                    }
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        #endregion
    }
}
