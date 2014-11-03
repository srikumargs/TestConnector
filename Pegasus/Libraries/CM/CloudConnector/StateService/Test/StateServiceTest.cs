using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Common;
using Sage.Connector.ConfigurationService.Proxy;
using Sage.Connector.Data;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Connector.TestUtilities;

namespace Sage.Connector.StateService.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class StateServiceTest
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
                using (var proxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    Assert.IsNotNull(proxy, "Failed to retrieve state service proxy.");

                    var state = proxy.GetConnectorState();
                    Assert.IsNotNull(state, "Connector service state is null");
                    Assert.IsNotNull(state.Uptime, "Uptime is null");
                    Assert.IsNotNull(state.FileVersion, "ConnectorFileVersion is null");
                    Assert.AreNotEqual(state.FileVersion, String.Empty, "ConnectorFileVersion is empty");

                    var newerState = proxy.GetConnectorState();
                    Assert.IsTrue(state.Uptime < newerState.Uptime, "Newer uptime is not greater than older one");

                    var configServiceHealthMessagesCount = proxy.GetConnectorState().SubsystemHealthMessages.Where(x => x.Subsystem == Subsystem.ConfigurationService).Count();
                    Int32 numberOfMessagesToAdd = 10;
                    for (Int32 i = 0; i < numberOfMessagesToAdd; i++)
                    {
                        proxy.RaiseSubsystemHealthIssue(new SubsystemHealthMessage(Subsystem.ConfigurationService, String.Format("rawMessage {0}", i), String.Format("userFacingMessage {0}", i), DateTime.UtcNow, null));
                    }
                    var newConfigServiceHealthMessagesCount = proxy.GetConnectorState().SubsystemHealthMessages.Where(x => x.Subsystem == Subsystem.ConfigurationService).Count();

                    Assert.AreEqual(configServiceHealthMessagesCount + numberOfMessagesToAdd, newConfigServiceHealthMessagesCount, "Expected health messages count not changed");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        /// <summary>
        /// Simple test of proxy connectivity
        /// </summary>
        [TestMethod]
        public void TestIntegratedConnectionState()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                PremiseConfigurationRecord pcr = null;
                using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    pcr = proxy.GetConfiguration(TestUtils.CannedTenantIds[0]);
                    Assert.IsNotNull(pcr, "Failed to retrieve config.");
                    pcr.BackOfficeConnectionEnabledToReceive = false;
                    pcr.CloudConnectionEnabledToReceive = false;
                    pcr.CloudConnectionEnabledToSend = false;
                    proxy.UpdateConfiguration(pcr);
                }

                using (var proxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    Assert.IsNotNull(proxy, "Failed to retrieve state service proxy.");

                    var unitTestBackOfficeProductPluginInfo = new BackOfficePluginInformation("Mock", "Mock Back Office Product", "1.0.0", "3.0.0", "4.0.0","id","version","baseName",false,"x86");
                    IntegratedConnectionState unitTestState = new IntegratedConnectionState(
                        pcr.CloudTenantId,
                        "Unit Test Tenant",
                        new Uri("http://www.sage.com"),
                        IntegrationEnabledStatus.BackOfficeProcessing | IntegrationEnabledStatus.CloudGetRequests | IntegrationEnabledStatus.CloudPutResponses,
                        TenantConnectivityStatus.None,
                        BackOfficeConnectivityStatus.None,
                        DateTime.UtcNow,
                        DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                        DateTime.UtcNow.Subtract(TimeSpan.FromDays(2)),
                        0, 0, 0, 0,
                        "company",
                        unitTestBackOfficeProductPluginInfo);

                    // Test set and retrieval
                    proxy.UpdateTenantConnectivityStatus(pcr.CloudTenantId, unitTestState.TenantConnectivityStatus);
                    proxy.UpdateIntegratedConnectionState(pcr.CloudTenantId, unitTestState.TenantName, unitTestState.BackOfficeCompanyName, unitTestState.TenantUri, unitTestState.IntegrationEnabledStatus, unitTestBackOfficeProductPluginInfo);
                    proxy.UpdateLastAttemptedCommunicationWithCloud(pcr.CloudTenantId, unitTestState.LastAttemptedCommunicationWithCloud);
                    proxy.UpdateLastSuccessfulCommunicationWithCloud(pcr.CloudTenantId, unitTestState.LastSuccessfulCommunicationWithCloud);
                    proxy.UpdateNextScheduledCommunicationWithCloud(pcr.CloudTenantId, unitTestState.NextScheduledCommunicationWithCloud);
                    IntegratedConnectionState retrievedState = proxy.GetConnectorState().IntegratedConnectionStates.SingleOrDefault(x => x.TenantId == pcr.CloudTenantId);

                    AssertRetrievedStateMatches(unitTestState, retrievedState);

                    proxy.IncrementRequestsReceivedCount(unitTestState.TenantId, 10);
                    proxy.IncrementNonErrorResponsesSentCount(unitTestState.TenantId, 4);
                    proxy.IncrementErrorResponsesSentCount(unitTestState.TenantId, 3);
                    proxy.AdjustRequestsInProgressCount(unitTestState.TenantId, 2);

                    IntegratedConnectionState newRetrievedState = proxy.GetConnectorState().IntegratedConnectionStates.SingleOrDefault(x => x.TenantId == unitTestState.TenantId);
                    Assert.AreEqual(newRetrievedState.RequestsReceivedCount, retrievedState.RequestsReceivedCount + 10, "Requests counts did not match.");
                    Assert.AreEqual(newRetrievedState.NonErrorResponsesSentCount, retrievedState.NonErrorResponsesSentCount + 4, "Non-error response counts did not match.");
                    Assert.AreEqual(newRetrievedState.ErrorResponsesSentCount, retrievedState.ErrorResponsesSentCount + 3, "Error response counts did not match.");
                    Assert.AreEqual(newRetrievedState.RequestsInProgressCount, retrievedState.RequestsInProgressCount + 2, "In progress counts did not match.");


                    // Test collection
                    IntegratedConnectionState[] retrievedStates = proxy.GetConnectorState().IntegratedConnectionStates;
                    bool bUnitTestStateFound = false;
                    foreach (IntegratedConnectionState state in retrievedStates)
                    {
                        if (state.TenantId == unitTestState.TenantId)
                        {
                            bUnitTestStateFound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(bUnitTestStateFound, "Failed to find tenant in tenant states.");

                    // Test update
                    var updatedUnitTestBackOfficeProductPluginInfo = new BackOfficePluginInformation("Mock", "Mock Back Office Product", "5.0.0", "7.0.0", "8.0.0", "id", "version", "baseName", false, "x86");
                    IntegratedConnectionState updatedTestState = new IntegratedConnectionState(
                        unitTestState.TenantId,
                        "Unit Test Tenant 2",
                        new Uri("http://www.sage2.com"),
                        IntegrationEnabledStatus.BackOfficeProcessing,
                        TenantConnectivityStatus.LocalNetworkUnavailable,
                        BackOfficeConnectivityStatus.None,
                        DateTime.UtcNow,
                        DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                        DateTime.UtcNow.Add(TimeSpan.FromDays(2)),
                        0, 0, 0, 0,
                        "company2",
                        updatedUnitTestBackOfficeProductPluginInfo);

                    // Test set and retrieval
                    proxy.UpdateTenantConnectivityStatus(updatedTestState.TenantId, updatedTestState.TenantConnectivityStatus);
                    proxy.UpdateIntegratedConnectionState(updatedTestState.TenantId, updatedTestState.TenantName, updatedTestState.BackOfficeCompanyName, updatedTestState.TenantUri, updatedTestState.IntegrationEnabledStatus, updatedUnitTestBackOfficeProductPluginInfo);
                    proxy.UpdateLastAttemptedCommunicationWithCloud(updatedTestState.TenantId, updatedTestState.LastAttemptedCommunicationWithCloud);
                    proxy.UpdateLastSuccessfulCommunicationWithCloud(updatedTestState.TenantId, updatedTestState.LastSuccessfulCommunicationWithCloud);
                    proxy.UpdateNextScheduledCommunicationWithCloud(updatedTestState.TenantId, updatedTestState.NextScheduledCommunicationWithCloud);
                    retrievedState = proxy.GetConnectorState().IntegratedConnectionStates.SingleOrDefault(x => x.TenantId == unitTestState.TenantId);

                    AssertRetrievedStateMatches(updatedTestState, retrievedState);

                    proxy.IncrementRequestsReceivedCount(updatedTestState.TenantId, 20);
                    proxy.IncrementNonErrorResponsesSentCount(updatedTestState.TenantId, 14);
                    proxy.IncrementErrorResponsesSentCount(updatedTestState.TenantId, 13);
                    proxy.AdjustRequestsInProgressCount(updatedTestState.TenantId, 12);

                    IntegratedConnectionState newUpdatedRetrievedState = proxy.GetConnectorState().IntegratedConnectionStates.SingleOrDefault(x => x.TenantId == unitTestState.TenantId);
                    Assert.AreEqual(newUpdatedRetrievedState.RequestsReceivedCount, retrievedState.RequestsReceivedCount + 20, "Requests counts did not match.");
                    Assert.AreEqual(newUpdatedRetrievedState.NonErrorResponsesSentCount, retrievedState.NonErrorResponsesSentCount + 14, "Non-error response counts did not match.");
                    Assert.AreEqual(newUpdatedRetrievedState.ErrorResponsesSentCount, retrievedState.ErrorResponsesSentCount + 13, "Error response counts did not match.");
                    Assert.AreEqual(newUpdatedRetrievedState.RequestsInProgressCount, retrievedState.RequestsInProgressCount + 12, "In progress counts did not match.");


                    // Test removal
                    proxy.RemoveIntegratedConnectionState(updatedTestState.TenantId);
                    retrievedStates = proxy.GetConnectorState().IntegratedConnectionStates;
                    bUnitTestStateFound = false;
                    foreach (IntegratedConnectionState state in retrievedStates)
                    {
                        if (state.TenantId == unitTestState.TenantId)
                        {
                            bUnitTestStateFound = true;
                            break;
                        }
                    }
                    Assert.IsFalse(bUnitTestStateFound, "Found deleted tenant in tenant states.");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod]
        public void Test_BackOfficePluginInformation()
        {
            string integrationInterfacesVersion = "InterfaceVersion1";
            string productPluginFileVersion = "FileVersion1";
            string productVerison = "ProductVersion1";
            string autoUpdateProductId = "autoUpdateProductId1";
            string autoUpdateProductVersion = "autoUpdateProductVersion1";
            string autoUpdateComponentBaseName = "autoUpdateComponentBaseName1";
            bool runAsUserRequried = false;
            string platform = "platform1";

            var backOfficePluginInformation = new BackOfficePluginInformation(
                "Mock", "Mock Back Office Product", integrationInterfacesVersion, productPluginFileVersion, productVerison,
                autoUpdateProductId, autoUpdateProductVersion, autoUpdateComponentBaseName, runAsUserRequried, platform);
            Assert.AreEqual(integrationInterfacesVersion, backOfficePluginInformation.IntegrationInterfaceVersion, "property value does not match");
            Assert.AreEqual(productPluginFileVersion, backOfficePluginInformation.ProductPluginFileVersion, "property value does not match");
            Assert.AreEqual(productVerison, backOfficePluginInformation.ProductVersion, "property value does not match");

            Assert.AreEqual(autoUpdateProductId, backOfficePluginInformation.AutoUpdateProductId, "property value does not match");
            Assert.AreEqual(autoUpdateProductVersion, backOfficePluginInformation.AutoUpdateProductVersion, "property value does not match");
            Assert.AreEqual(autoUpdateComponentBaseName, backOfficePluginInformation.AutoUpdateComponentBaseName, "property value does not match");
            Assert.AreEqual(runAsUserRequried, backOfficePluginInformation.RunAsUserRequried, "property value does not match");
            Assert.AreEqual(platform, backOfficePluginInformation.Platform, "property value does not match");



            List<PropertyTuple> properties = new List<PropertyTuple>();
            properties.Add(new PropertyTuple(backOfficePluginInformation.PropertyInfo(x => x.IntegrationInterfaceVersion), integrationInterfacesVersion.Replace("1","2")));
            properties.Add(new PropertyTuple(backOfficePluginInformation.PropertyInfo(x => x.ProductPluginFileVersion), productPluginFileVersion.Replace("1", "2")));
            properties.Add(new PropertyTuple(backOfficePluginInformation.PropertyInfo(x => x.ProductVersion), productVerison.Replace("1", "2")));

            properties.Add(new PropertyTuple(backOfficePluginInformation.PropertyInfo(x => x.AutoUpdateProductId), autoUpdateProductId.Replace("1", "2")));
            properties.Add(new PropertyTuple(backOfficePluginInformation.PropertyInfo(x => x.AutoUpdateProductVersion), autoUpdateProductVersion.Replace("1", "2")));
            properties.Add(new PropertyTuple(backOfficePluginInformation.PropertyInfo(x => x.AutoUpdateComponentBaseName), autoUpdateComponentBaseName.Replace("1", "2")));
            properties.Add(new PropertyTuple(backOfficePluginInformation.PropertyInfo(x => x.Platform), platform.Replace("1", "2")));
        }




        [TestMethod]
        public void Test_BackOfficeConnectionDataContract()
        {
            string newValue = "info";
            BackOfficeConnection boc = new BackOfficeConnection("connection information", "displayable connection information", "name");

            List<PropertyTuple> properties = new List<PropertyTuple>();
            properties.Add(new PropertyTuple(boc.PropertyInfo(x => x.ConnectionInformation), newValue));
            properties.Add(new PropertyTuple(boc.PropertyInfo(x => x.DisplayableConnectionInformation), newValue));
            properties.Add(new PropertyTuple(boc.PropertyInfo(x => x.Name), newValue));

            BackOfficeConnection boc2 = new BackOfficeConnection(boc, properties);
            Assert.AreEqual(newValue, boc2.Name);
            Assert.AreEqual(newValue, boc2.ConnectionInformation);
            Assert.AreEqual(newValue, boc2.DisplayableConnectionInformation);
        }

        [TestMethod]
        public void Test_ValidateTenantConnectionResponse()
        {
            string name = "Google";
            Uri uri = new Uri("http://www.google.com");
            var tenantConnectivityStatus = TenantConnectivityStatus.CloudUnavailable;

            var response = new ValidateTenantConnectionResponse(name, uri, tenantConnectivityStatus);

            Assert.AreEqual(name, response.Name, "Property does not match");
            Assert.AreEqual(uri.AbsoluteUri, response.SiteAddress.AbsoluteUri, "Property does not match");
            Assert.AreEqual(tenantConnectivityStatus, response.TenantConnectivityStatus, string.Format("Response should be {0}.", tenantConnectivityStatus.ToString()));

            name = "Yahoo";
            uri = new Uri("http://www.yahoo.com");
            tenantConnectivityStatus = TenantConnectivityStatus.InternetConnectionUnavailable;

            List<PropertyTuple> properties = new List<PropertyTuple>();
            properties.Add(new PropertyTuple(response.PropertyInfo(x => x.Name), name));
            properties.Add(new PropertyTuple(response.PropertyInfo(x => x.SiteAddress), uri));
            properties.Add(new PropertyTuple(response.PropertyInfo(x => x.TenantConnectivityStatus), tenantConnectivityStatus));

            var response2 = new ValidateTenantConnectionResponse(response, properties);

            Assert.AreEqual(name, response2.Name, "Property does not match");
            Assert.AreEqual(uri.AbsoluteUri, response2.SiteAddress.AbsoluteUri, "Property does not match");
            Assert.AreEqual(tenantConnectivityStatus, response2.TenantConnectivityStatus, string.Format("Response should be {0}.", tenantConnectivityStatus.ToString()));
        }

        [TestMethod]
        public void Test_ValidateBackOfficeConnectionResponse()
        {
            int numUserMessages = 5;
            int numRawMessages = 8;

            List<string> userFacingMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numUserMessages));
            List<string> rawErrorMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numRawMessages));
            BackOfficeConnectivityStatus connectionStatus = BackOfficeConnectivityStatus.Incompatible;

            //TODO: JSB add info for new paramaters
            var response = new ValidateBackOfficeConnectionResponse(connectionStatus, null, null, null, userFacingMessages, rawErrorMessages);

            Assert.AreEqual(numUserMessages, response.UserFacingMessages.Length, "Number of messages does not match");
            Assert.AreEqual(numRawMessages, response.RawErrorMessage.Length, "Number of messages does not match");
            Assert.AreEqual(connectionStatus, response.BackOfficeConnectivityStatus, string.Format("Response should be {0}.",connectionStatus.ToString()));

            for (int i = 0; i < userFacingMessages.Count; i++)
            {
                Assert.AreEqual(userFacingMessages[i], response.UserFacingMessages[i], "User facing message have been altered");
            }
            for (int i = 0; i < rawErrorMessages.Count; i++)
            {
                Assert.AreEqual(rawErrorMessages[i], response.RawErrorMessage[i], "Raw error message have been altered");
            }

            numUserMessages = 3;
            numRawMessages = 23;
            connectionStatus = BackOfficeConnectivityStatus.ConnectivityBroken;
            userFacingMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numUserMessages));
            rawErrorMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numRawMessages));

            List<PropertyTuple> properties = new List<PropertyTuple>();
            properties.Add(new PropertyTuple(response.PropertyInfo(x => x.UserFacingMessages), userFacingMessages.ToArray()));
            properties.Add(new PropertyTuple(response.PropertyInfo(x => x.RawErrorMessage), rawErrorMessages.ToArray()));
            properties.Add(new PropertyTuple(response.PropertyInfo(x => x.BackOfficeConnectivityStatus), connectionStatus));

            var response2 = new ValidateBackOfficeConnectionResponse(response, properties);
            for (int i = 0; i < userFacingMessages.Count; i++)
            {
                Assert.AreEqual(userFacingMessages[i], response2.UserFacingMessages[i], "User facing message have been altered");
            }
            for (int i = 0; i < rawErrorMessages.Count; i++)
            {
                Assert.AreEqual(rawErrorMessages[i], response2.RawErrorMessage[i], "Raw error message have been altered");
            }

            Assert.IsNotNull(response2.RawErrorMessage, "Constructor should have created a blank List");
            Assert.IsNotNull(response2.UserFacingMessages, "Constructor should have created a blank List");
            Assert.AreEqual(connectionStatus, response2.BackOfficeConnectivityStatus, "Is valid response incorrect.");
        }

        [TestMethod]
        public void Test_ValidateBackOfficeAdminCredentialsResponse()
        {
            int numUserMessages = 5;
            int numRawMessages = 8;

            List<string> userFacingMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numUserMessages));
            List<string> rawErrorMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numRawMessages));
            bool isValidConnection = false;

            var response = new ValidateBackOfficeAdminCredentialsResponse(isValidConnection,userFacingMessages, rawErrorMessages);

            Assert.AreEqual(numUserMessages, response.UserFacingMessages.Length, "Number of messages does not match");
            Assert.AreEqual(numRawMessages, response.RawErrorMessage.Length, "Number of messages does not match");
            Assert.AreEqual(isValidConnection, response.IsValid, "Is valid response incorrect.");

            for (int i = 0; i < userFacingMessages.Count; i++)
            {
                Assert.AreEqual(userFacingMessages[i], response.UserFacingMessages[i], "User facing message have been altered");
            }
            for (int i = 0; i < rawErrorMessages.Count; i++)
            {
                Assert.AreEqual(rawErrorMessages[i], response.RawErrorMessage[i], "Raw error message have been altered");
            }

            numUserMessages = 3;
            numRawMessages = 23;
            isValidConnection = true;
            userFacingMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numUserMessages));
            rawErrorMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numRawMessages));

            List<PropertyTuple> properties = new List<PropertyTuple>();
            properties.Add(new PropertyTuple(response.PropertyInfo(x => x.UserFacingMessages), userFacingMessages.ToArray()));
            properties.Add(new PropertyTuple(response.PropertyInfo(x => x.RawErrorMessage), rawErrorMessages.ToArray()));
            properties.Add(new PropertyTuple(response.PropertyInfo(x => x.IsValid), isValidConnection));

            var response2 = new ValidateBackOfficeAdminCredentialsResponse(response, properties);
            for (int i = 0; i < userFacingMessages.Count; i++)
            {
                Assert.AreEqual(userFacingMessages[i], response2.UserFacingMessages[i], "User facing message have been altered");
            }
            for (int i = 0; i < rawErrorMessages.Count; i++)
            {
                Assert.AreEqual(rawErrorMessages[i], response2.RawErrorMessage[i], "Raw error message have been altered");
            }

            Assert.IsNotNull(response2.RawErrorMessage, "Constructor should have created a blank List");
            Assert.IsNotNull(response2.UserFacingMessages, "Constructor should have created a blank List");
            Assert.AreEqual(isValidConnection, response2.IsValid, "Is valid response incorrect.");
        }

        [TestMethod]
        public void Test_BackOfficeConnectionsForCredentialsResponse()
        {
            int numUserMessages = 5;
            int numRawMessages = 8;

            List<string> userFacingMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numUserMessages));
            List<string> rawErrorMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numRawMessages));
            List<BackOfficeConnection> bocs = new List<BackOfficeConnection>{new BackOfficeConnection("connection information", "displayable connection information", "name")};
            var response = new BackOfficeConnectionsForCredentialsResponse(bocs, userFacingMessages, rawErrorMessages);

            Assert.AreEqual(numUserMessages, response.UserFacingMessages.Length, "Number of messages does not match");
            Assert.AreEqual(numRawMessages, response.RawErrorMessage.Length, "Number of messages does not match");

            PropertyComparisonResults results = new PropertyComparisonResults();
            Assert.IsTrue(
                DataObjectComparisonUtil.AreObjectsEqual(bocs.ToArray(), response.BackOfficeConnections, ref results, true, new string[] { })
                , "BackofficeConnection objects do not match");

            for (int i = 0; i < userFacingMessages.Count; i++)
            {
                Assert.AreEqual(userFacingMessages[i], response.UserFacingMessages[i], "User facing message have been altered");
            }
            for (int i = 0; i < rawErrorMessages.Count; i++)
            {
                Assert.AreEqual(rawErrorMessages[i], response.RawErrorMessage[i], "Raw error message have been altered");
            }

            numUserMessages = 3;
            numRawMessages = 23;
            userFacingMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numUserMessages));
            rawErrorMessages = new List<string>(DataContractStaticDataClass.GetListOfStrings(numRawMessages));

            List<PropertyTuple> properties = new List<PropertyTuple>();
            properties.Add(new PropertyTuple(response.PropertyInfo(x => x.UserFacingMessages), userFacingMessages.ToArray()));
            properties.Add(new PropertyTuple(response.PropertyInfo(x => x.RawErrorMessage), rawErrorMessages.ToArray()));

            var response2 = new BackOfficeConnectionsForCredentialsResponse(response, properties);
            for (int i = 0; i < userFacingMessages.Count; i++)
            {
                Assert.AreEqual(userFacingMessages[i], response2.UserFacingMessages[i], "User facing message have been altered");
            }
            for (int i = 0; i < rawErrorMessages.Count; i++)
            {
                Assert.AreEqual(rawErrorMessages[i], response2.RawErrorMessage[i], "Raw error message have been altered");
            }

            var response3 = new BackOfficeConnectionsForCredentialsResponse(bocs, null, null);
            Assert.IsNotNull(response3.RawErrorMessage, "Constructor should have created a blank List");
            Assert.IsNotNull(response3.UserFacingMessages, "Constructor should have created a blank List");
        }

        [TestMethod]
        public void Test_ValidateTenantConnection()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                using (var proxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    Assert.IsNotNull(proxy, "Failed to retrieve state service proxy.");

                    var state = proxy.GetConnectorState();
                    Assert.IsNotNull(state, "Connector service state is null");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }


        }

        [TestMethod]
        public void Test_ConnectorState()
        {
            ConnectorState state = DataContractStaticDataClass.GetConnectorState("1");

            List<PropertyTuple> properties = new List<PropertyTuple>();
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.CloudInterfaceVersion), state.CloudInterfaceVersion + "_done"));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.ConnectorBackOfficeIntegrationInterfaceVersion), state.ConnectorBackOfficeIntegrationInterfaceVersion + "_done"));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.ConnectorMinimumBackOfficeIntegrationInterfaceVersion), state.ConnectorMinimumBackOfficeIntegrationInterfaceVersion + "_done"));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.FileVersion), state.FileVersion + "_done"));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.ProductCode), state.ProductCode + "_done"));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.ProductName), state.ProductName + "_done"));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.ProductVersion), state.ProductVersion + "_done"));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.MaxUptimeBeforeRestart), null));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.SubsystemHealthMessages), null));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.CloudConnectivityStatus), CloudConnectivityStatus.None));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.ConnectorUpdateStatus), ConnectorUpdateStatus.None));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.RestartMode), RestartMode.None));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.TimeToBlackoutEnd), new TimeSpan(0)));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.MaxUptimeBeforeRestart), new TimeSpan(0)));
            properties.Add(new PropertyTuple(state.PropertyInfo(x => x.ConnectorUpdateInfo), DataContractStaticDataClass.GetGenericUpdateInfo("_done")));

            ConnectorState state2 = new ConnectorState(state, properties);

            Assert.IsTrue(state2.CloudInterfaceVersion.EndsWith("done"), "Property did not update correctly");
            Assert.IsTrue(state2.ConnectorBackOfficeIntegrationInterfaceVersion.EndsWith("done"), "Property did not update correctly");
            Assert.IsTrue(state2.ConnectorMinimumBackOfficeIntegrationInterfaceVersion.EndsWith("done"), "Property did not update correctly");
            Assert.IsTrue(state2.FileVersion.EndsWith("done"), "Property did not update correctly");
            Assert.IsTrue(state2.ProductCode.EndsWith("done"), "Property did not update correctly");
            Assert.IsTrue(state2.ProductName.EndsWith("done"), "Property did not update correctly");
            Assert.IsTrue(state2.ProductVersion.EndsWith("done"), "Property did not update correctly");
            Assert.IsNull(state2.SubsystemHealthMessages, "Value should convert null to empty array");
            Assert.IsTrue(state2.CloudConnectivityStatus.ToString().Contains("None"), "Property should have a state of None");
            Assert.IsTrue(state2.RestartMode.ToString().Contains("None"), "Property should have a state of None");
            Assert.IsTrue(state2.ConnectorUpdateStatus.ToString().Contains("None"), "Property should have a state of None");
            Assert.AreEqual(0, state2.TimeToBlackoutEnd.Value.TotalMilliseconds,"Property should have a zero value.");
            Assert.AreEqual(0, state2.MaxUptimeBeforeRestart.Value.TotalMilliseconds, "Property should have a zero value.");
        }

        private static void AssertRetrievedStateMatches(IntegratedConnectionState unitTestState, IntegratedConnectionState retrievedState)
        {
            Assert.IsNotNull(retrievedState, "Failed to retrieved tenant.");
            Assert.AreEqual(unitTestState.TenantId, retrievedState.TenantId, "Retrieved tenant id did not match.");
            Assert.AreEqual(unitTestState.TenantName, retrievedState.TenantName, "Retrieved tenant name did not match.");
            Assert.AreEqual(unitTestState.BackOfficeCompanyName, retrievedState.BackOfficeCompanyName, "Retrieved back office company name did not match.");
            Assert.AreEqual(unitTestState.TenantUri, retrievedState.TenantUri, "Retrieved tenant uri did not match.");
            Assert.AreEqual(unitTestState.IntegrationEnabledStatus, retrievedState.IntegrationEnabledStatus, "Integration enabled status did not match.");

            Assert.AreEqual(unitTestState.LastAttemptedCommunicationWithCloud, retrievedState.LastAttemptedCommunicationWithCloud, "Last attempt times did not match.");
            Assert.AreEqual(unitTestState.LastSuccessfulCommunicationWithCloud, retrievedState.LastSuccessfulCommunicationWithCloud, "Last success times did not match.");
            Assert.AreEqual(unitTestState.NextScheduledCommunicationWithCloud, retrievedState.NextScheduledCommunicationWithCloud, "Next times did not match.");

            Assert.AreEqual(unitTestState.TenantConnectivityStatus, retrievedState.TenantConnectivityStatus, "Tenant connectivity status did not match.");
            Assert.AreEqual(unitTestState.BackOfficePluginInformation.ProductVersion, retrievedState.BackOfficePluginInformation.ProductVersion, "Back office product versions did not match.");
            Assert.AreEqual(unitTestState.BackOfficePluginInformation.IntegrationInterfaceVersion, retrievedState.BackOfficePluginInformation.IntegrationInterfaceVersion, "Back office integration versions did not match.");
            Assert.AreEqual(unitTestState.BackOfficePluginInformation.ProductPluginFileVersion, retrievedState.BackOfficePluginInformation.ProductPluginFileVersion, "Back office plugin versions did not match.");
        }
    }
}