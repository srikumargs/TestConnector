using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Common;
using Sage.Connector.MonitorService.Interfaces.DataContracts;
using Sage.Connector.MonitorService.Proxy;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.TestUtilities;
using Sage.CRE.HostingFramework.Interfaces;

namespace Sage.Connector.MonitorService.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MonitorServiceTest
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
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
        }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestConnectorServiceStateConstructor()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            ConnectorServiceConnectivityStatus connectivityStatus1 = ConnectorServiceConnectivityStatus.ServiceNotRegistered;
            ConnectorState connectorState1 = DataContractStaticDataClass.GetConnectorState("1");
            string monitorServiceFileVersion1 = "Version 1";
            List<ServiceInfo> serviceInfos1 = new List<ServiceInfo>();

            ConnectorServiceState state = new ConnectorServiceState(connectivityStatus1, connectorState1,
                                          monitorServiceFileVersion1, serviceInfos1);

            List<PropertyTuple> properties = new List<PropertyTuple>();
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.ConnectorState), DataContractStaticDataClass.GetConnectorState("2")));

            ConnectorServiceState state2 = new ConnectorServiceState(state,properties);

            PropertyComparisonResults results = new PropertyComparisonResults();
            bool testResult = DataObjectComparisonUtil.AreObjectsEqual(state, state2, ref results, true,new string[]{"ExtensionData"});
            Assert.IsFalse(testResult, "Data comparison should fail.");

            properties.Clear();
            results.Clear();
            properties.Add(new PropertyTuple(state.PropertyInfo(x=>x.ConnectorState),state.ConnectorState));
            ConnectorServiceState state3 = new ConnectorServiceState(state2,properties);
            testResult = DataObjectComparisonUtil.AreObjectsEqual(state, state3,ref results,true,new string[]{"ExtensionData"});
            Assert.IsTrue(testResult,"Objects should now match");
        }

        /// <summary>
        /// Simple test of proxy connectivity
        /// </summary>
        [TestMethod]
        public void TestServiceProxy()
        {
            // make sure everything is shutdown
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.ResetAndEnsureStopped);

            // spin up only the monitor service; as a service
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StartHostingFx);

            try
            {
                using (var proxy = MonitorServiceProxyFactory.CreateFromCatalog("localhost", ConnectorMonitorServiceUtils.CatalogServicePortNumber))
                {
                    Assert.IsNotNull(proxy, "Failed to retrieve monitor service proxy.");

                    var state = proxy.GetConnectorServiceState();
                    Assert.IsNotNull(state, "Connector service state is null");
                    Assert.AreEqual(state.ConnectorServiceConnectivityStatus, ConnectorServiceConnectivityStatus.ServiceNotRunning, "Service state is expected to be 'ServiceNotRunning' but wasn't");

                    // spin up the connector services; force running it as a service so that the Monitor Service can properly report that the Connector Windows Service is running
                    TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StartHostingFx | TestUtils.UnitTestSetupFlags.ForceRunAsService);
                    state = proxy.GetConnectorServiceState();
                    Assert.IsNotNull(state, "Connector service state is null");
                    Assert.AreEqual(state.ConnectorServiceConnectivityStatus, ConnectorServiceConnectivityStatus.Connected, "Service state is expected to be 'Connected' but wasn't");

                    Assert.IsNotNull(state.ConnectorState, "ConnectorState is null");
                    Assert.IsNotNull(state.ConnectorState.Uptime, "Uptime is null");
                    Assert.IsNotNull(state.ConnectorState.FileVersion, "ConnectorFileVersion is null");
                    Assert.AreNotEqual(state.ConnectorState.FileVersion, String.Empty, "ConnectorFileVersion is empty");

                    var newerState = proxy.GetConnectorServiceState();
                    Assert.IsNotNull(newerState.ConnectorState, "ConnectorState is null");
                    Assert.IsNotNull(newerState.ConnectorState.Uptime, "Uptime is null");
                    Assert.IsTrue(state.ConnectorState.Uptime < newerState.ConnectorState.Uptime, "Newer uptime is not greater than older one");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx | TestUtils.UnitTestSetupFlags.ForceRunAsService);
            }
        }

        [TestMethod]
        public void TestMonitorServiceConnectorServiceState()
        {
            string fileVersion = DataContractStaticDataClass.FileVersion1;

            ConnectorState state = DataContractStaticDataClass.GetConnectorState("1");
            ConnectorState state2 = DataContractStaticDataClass.GetConnectorState("2");

            ServiceInfo sInfo = new ServiceInfo(
                    DataContractStaticDataClass.name1,
                    DataContractStaticDataClass.description1,
                    DataContractStaticDataClass.uniqueId1,
                    new List<Uri> { new Uri("http://www.blah.com") });

            ConnectorServiceState stateService = new ConnectorServiceState(
                ConnectorServiceConnectivityStatus.Connected,
                state,
                fileVersion,
                new List<ServiceInfo>{sInfo});

            //Test change in MonitorServiceFileVersion
            PropertyTuple property = new PropertyTuple(stateService.PropertyInfo(x =>x.MonitorServiceFileVersion), "NotRight");
            ConnectorServiceState stateService2 = new ConnectorServiceState(stateService, new List<PropertyTuple>{property});

            PropertyComparisonResults results = new PropertyComparisonResults();
            bool testResult = DataObjectComparisonUtil.AreObjectsEqual(stateService, stateService2, ref results, true,
                                                                   new string[] {"MonitorServiceFileVersion", "ExtensionData"});
            Assert.IsTrue(testResult, "Everything other than the ignore list should be equal");
            
            results.Clear();
            testResult = DataObjectComparisonUtil.AreObjectsEqual(stateService, stateService2, ref results, true,
                                                                   new string[] {"ExtensionData"});
            Assert.AreEqual(1, results.Failures.Count, "Should only be one error");
            Assert.IsFalse(testResult, "MonitorServiceFileVersion should make this fail");

            //Test change in ConnectorServiceConnectivityStatus
            property = new PropertyTuple(stateService.PropertyInfo(x => x.ConnectorServiceConnectivityStatus), ConnectorServiceConnectivityStatus.ServiceNotRunning);
            ConnectorServiceState stateService3 = new ConnectorServiceState(stateService, new List<PropertyTuple> { property });

            results.Clear();
            testResult = DataObjectComparisonUtil.AreObjectsEqual(stateService, stateService3, ref results, true,
                                                                   new string[] { "ConnectorServiceConnectivityStatus", "ExtensionData" });
            Assert.IsTrue(testResult, "Everything other than the ignore list should be equal");

            results.Clear();
            testResult = DataObjectComparisonUtil.AreObjectsEqual(stateService, stateService3, ref results, true,
                                                                   new string[] { "ExtensionData" });
            Assert.AreEqual(1, results.Failures.Count, "Should only be one error");
            Assert.IsFalse(testResult, "ConnectorServiceConnectivityStatus should make this fail");


            //Test change in ConnectorState
            property = new PropertyTuple(stateService.PropertyInfo(x => x.ConnectorState), state2);
            ConnectorServiceState stateService4 = new ConnectorServiceState(stateService, new List<PropertyTuple> { property });

            results.Clear();
            testResult = DataObjectComparisonUtil.AreObjectsEqual(stateService, stateService4, ref results, true,
                                                                   new string[] { "ConnectorState", "ExtensionData" });
            Assert.IsTrue(testResult, "Everything other than the ignore list should be equal");

            results.Clear();
            testResult = DataObjectComparisonUtil.AreObjectsEqual(stateService, stateService4, ref results, true,
                                                                   new string[] { "ExtensionData" });
            Assert.IsTrue(results.Failures.Count >= 1, "Should only be one error");
            Assert.IsFalse(testResult, "ConnectorState should make this fail");
        }

    }
}