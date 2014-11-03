using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Common;
using Sage.Connector.Management;
using Sage.Connector.SageCloudService;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Connector.TestUtilities;
using SageConnector.ViewModel;
using CloudUtils = Sage.Connector.ConnectorServiceCommon.CloudUtils;

namespace SageConnector.Test
{
    /// <summary>
    ///This is a test class for the SageConstructionAnywhereConnector 
    ///</summary>
    [TestClass]
    public class CloudConnector_IntegratedTest
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

        [TestMethod]
        public void TestConnectorViewModel()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                string test = ConnectorViewModel.MainFormTitle;
                Assert.IsFalse(string.IsNullOrEmpty(test), "MainFormTitle should not be empty");
                Assert.IsTrue(test.Contains("Connector"), "Value should contain 'Connector'");

                test = ConnectorViewModel.ProductName;
                Assert.IsFalse(string.IsNullOrEmpty(test), "ProductName should not be empty");
                Assert.IsTrue(test.Contains("Connector"), "Value should contain 'Connector'");

                test = ConnectorViewModel.PluggedInProductNameConnectionDetails;
                Assert.IsFalse(string.IsNullOrEmpty(test), "PluggedInProductNameConnectionDetails should not be empty");
                Assert.IsTrue(test.Contains("connection details"), "Value should contain 'Connection Details'");

                test = ConnectorViewModel.ServiceDisplayName;
                Assert.IsFalse(string.IsNullOrEmpty(test), "ServiceDisplayName should not be empty");

                using (var logger = new SimpleTraceLogger())
                {
                    Assert.IsTrue(ConnectorViewModel.IsHostingFrameworkServiceReady(logger), "Hosting framework appears to be not running");
                }

                test = ConnectorViewModel.ApplicationVersion();
                Assert.IsFalse(string.IsNullOrEmpty(test), "ApplicationVersion should not return empty");

                const string showEndpointName = "SAGE_CONNECTOR_SHOW_ENDPOINT";
                string currentEndpointAddressSetting = Environment.GetEnvironmentVariable(showEndpointName);
                Environment.SetEnvironmentVariable(showEndpointName, "1");
                Assert.IsTrue(DeveloperFlags.ShowEndPointAddress(), "ShowEndPointAddress should be enabled");
                Environment.SetEnvironmentVariable(showEndpointName, "0");
                Assert.IsFalse(DeveloperFlags.ShowEndPointAddress(), "ShowEndPointAddress should be disabled");

                if (!string.IsNullOrEmpty(currentEndpointAddressSetting))
                    Environment.SetEnvironmentVariable(showEndpointName, currentEndpointAddressSetting);
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod, Ignore]
        public void TestConnectorViewModelHelpLinks()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                Uri testFormLink = ConnectorViewModel.MainFormHelpUri;
                Assert.IsTrue(RemoteFileExists(testFormLink.AbsoluteUri), "Help link was not found");

                testFormLink = ConnectorViewModel.DetailFormHelpUri;
                Assert.IsTrue(RemoteFileExists(testFormLink.AbsoluteUri), "Help link was not found");
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod]
        public void TestCreatePremiseConfig()
        {
            //TODO: why is a test that writes directly to the database here? Seems like this
            //should be with tests for the premise store.

            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            Guid id = Guid.NewGuid();
            
            string boCompanyUniqueId = "path";
            string premiseAgent = "premise agent";
            bool boEnabledToReceive = false;
            string tPremiseKey = "premise";
            string tTenant = "tenant";
            string tEndpoint = "endpoint";
            bool tEnabledToReceive = false;
            short sSentPolicy = 0;
            short sSentDays = 30;
            long lSentSize = 1024;
            string sSentName = "Sent";
            short sConcurrentBackOffice = 1;
            int iMinCommunicationFailureRetryInterval = 60000;
            int iMaxCommunicationFailureRetryInterval = 300000;
            bool tEnabledToSend = false;
            String sConnectorPluginId = "Mock";
            String backOfficeProductName = "Mock Back Office Product";
            String backOfficeCredentials = string.Empty;
            String backOfficeCredentialsDescription = string.Empty;
            String cloudTenantClaim = "CloudTenantClaim";

            //NOTE: Do not use constructor for PremiseConfiguration. The EF code generation in EF4.x "loses" constructor members.  So set members manually.
            //Thats why this is failing, Rather then constantly battle with EF to restore the missing members until we can find the root
            //add the below stub to the generated file.
            //An old version of the constructor version of this can be found in file history.

            /*
                    /// <summary>
                    /// Creates the premise configuration. 
                    /// </summary>
                    /// <returns></returns>
                    /// <remarks>manually added to work around code generation failures in EF4.x</remarks>
                    public static PremiseConfiguration CreatePremiseConfiguration()
                    {
                        PremiseConfiguration premiseConfiguration = new PremiseConfiguration();
                        return premiseConfiguration;
                    }
           */
            var testConfig = Sage.Connector.PremiseStore.ConfigurationStore.PremiseConfiguration.CreatePremiseConfiguration();
            testConfig.Id = id;

            testConfig.BackOfficeCompanyUniqueId = boCompanyUniqueId;
            testConfig.PremiseAgent = premiseAgent;
            testConfig.CloudTenantId = tTenant;
            testConfig.CloudPremiseKey = tPremiseKey;
            testConfig.SiteAddress = tEndpoint;
            testConfig.BackOfficeConnectionEnabledToReceive = boEnabledToReceive;
            testConfig.CloudConnectionEnabledToReceive = tEnabledToReceive;
            testConfig.CloudConnectionEnabledToSend = tEnabledToSend;
            testConfig.SentDocumentStoragePolicy = sSentPolicy;
            testConfig.SentDocumentStorageDays = sSentDays;
            testConfig.SentDocumentStorageMBs = lSentSize;
            testConfig.SentDocumentFolderName = sSentName;
            testConfig.BackOfficeAllowableConcurrentExecutions = sConcurrentBackOffice;
            testConfig.ConnectorPluginId = sConnectorPluginId;
            testConfig.BackOfficeProductName = backOfficeProductName;
            testConfig.BackOfficeConnectionCredentials = backOfficeCredentials;
            testConfig.BackOfficeConnectionCredentialsDescription    = backOfficeCredentialsDescription;

            //parameters out of order as these are the ones the EF constructor usually looses.
            testConfig.CloudTenantClaim = cloudTenantClaim;
            testConfig.MinCommunicationFailureRetryInterval = iMinCommunicationFailureRetryInterval;
            testConfig.MaxCommunicationFailureRetryInterval = iMaxCommunicationFailureRetryInterval;
            

            Assert.IsNotNull(testConfig, "Premise Configuration did not instantiate correctly");
            Assert.AreEqual(id, testConfig.Id, string.Format("Values should be equal: {0} - {1}", id, testConfig.Id));
            Assert.AreEqual(boCompanyUniqueId, testConfig.BackOfficeCompanyUniqueId, string.Format("Values should be equal: {0} - {1}", boCompanyUniqueId, testConfig.BackOfficeCompanyUniqueId));
            Assert.AreEqual(premiseAgent, testConfig.PremiseAgent, string.Format("Values should be equal: {0} - {1}", premiseAgent, testConfig.PremiseAgent));
            Assert.AreEqual(tTenant, testConfig.CloudTenantId, string.Format("Values should be equal: {0} - {1}", tTenant, testConfig.CloudTenantId));
            Assert.AreEqual(tPremiseKey, testConfig.CloudPremiseKey, string.Format("Values should be equal: {0} - {1}", tPremiseKey, testConfig.CloudPremiseKey));
            Assert.AreEqual(tEndpoint, testConfig.SiteAddress, string.Format("Values should be equal: {0} - {1}", tEndpoint, testConfig.SiteAddress));
            Assert.AreEqual(boEnabledToReceive, testConfig.BackOfficeConnectionEnabledToReceive, string.Format("Values should be equal: {0} - {1}", boEnabledToReceive, testConfig.BackOfficeConnectionEnabledToReceive));
            Assert.AreEqual(tEnabledToReceive, testConfig.CloudConnectionEnabledToReceive, string.Format("Values should be equal: {0} - {1}", tEnabledToReceive, testConfig.CloudConnectionEnabledToReceive));
            Assert.AreEqual(sSentPolicy, testConfig.SentDocumentStoragePolicy, string.Format("Values should be equal: {0} - {1}", sSentPolicy, testConfig.SentDocumentStoragePolicy));
            Assert.AreEqual(lSentSize, testConfig.SentDocumentStorageMBs, string.Format("Values should be equal: {0} - {1}", lSentSize, testConfig.SentDocumentStorageMBs));
            Assert.AreEqual(sSentName, testConfig.SentDocumentFolderName, string.Format("Values should be equal: {0} - {1}", sSentName, testConfig.SentDocumentFolderName));
            Assert.AreEqual(sConcurrentBackOffice, testConfig.BackOfficeAllowableConcurrentExecutions, string.Format("Values should be equal: {0} - {1}", sConcurrentBackOffice, testConfig.BackOfficeAllowableConcurrentExecutions));
            Assert.AreEqual(iMinCommunicationFailureRetryInterval, testConfig.MinCommunicationFailureRetryInterval, string.Format("Values should be equal: {0} - {1}", iMinCommunicationFailureRetryInterval, testConfig.MinCommunicationFailureRetryInterval));
            Assert.AreEqual(iMaxCommunicationFailureRetryInterval, testConfig.MaxCommunicationFailureRetryInterval, string.Format("Values should be equal: {0} - {1}", iMaxCommunicationFailureRetryInterval, testConfig.MaxCommunicationFailureRetryInterval));
            Assert.AreEqual(tEnabledToSend, testConfig.CloudConnectionEnabledToSend, string.Format("Values should be equal: {0} - {1}", tEnabledToSend, testConfig.CloudConnectionEnabledToSend));
            Assert.AreEqual(sConnectorPluginId, testConfig.ConnectorPluginId, string.Format("Values should be equal: {0} - {1}", sConnectorPluginId, testConfig.ConnectorPluginId));
        }

        [TestMethod]
        public void TestActiveConnection()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                StartUpMockCloud();

                // not using "using" for the raw channel, the channel can fault on Close() and if it does we should also call Abort()
                ChannelFactory<ICREMessageServiceInjection> csiFactory = null;
                try
                {
                    csiFactory = CreateMockMessageServiceInjectionChannelFactory();

                    string aTenantId = MockCloudService.TenantIds.First();
                    string aPremiseKey = MockCloudService.TenantPremiseKeys.First();
                    ConfigurationViewModelManager manager = new ConfigurationViewModelManager();
                    
                    ConfigurationViewModel model = manager.CreateNewTenant(ConnectorPluginsViewModel.GetConnectorPlugins().Where(x => x.Id == "Mock").First());

                    model.CloudEndpoint = MockCloudServiceHost.SiteAddress;
                    model.CloudTenantId = aTenantId;
                    model.CloudPremiseKey = aPremiseKey;

                    string[] cloudErrors = new string[] { };
                    string cloudCompanyName, cloudCompanyUrl;
                    TenantConnectivityStatus cloudStatus = VerifyCloudConnection(model, out cloudCompanyName, out cloudCompanyUrl, out cloudErrors);
                    bool testStatusCloud = cloudStatus == TenantConnectivityStatus.Normal;

                    string[] backOfficeErrors = new string[] { };
                    BackOfficeConnectivityStatus backOfficeStatus = VerifyBackOfficeConnection(model, out backOfficeErrors);
                    bool testStatusPremise = backOfficeStatus == BackOfficeConnectivityStatus.Normal;

                    ConnectorUtilities.UpdateConnectionStatusesInStateService(aTenantId, backOfficeStatus, cloudStatus);
                    model.RefreshConnectionStatuses();

                    Image premiseActiveImage = model.BackOfficeConnectionStatusImage;
                    Image cloudActiveImage = model.CloudConnectionStatusImage;

                    Assert.IsTrue(testStatusCloud, "Active status came back false.");
                    Assert.IsTrue(backOfficeErrors.Length > 0, "There should at least one error message returned from back office validate as it was never configured.");
                    Assert.IsFalse(testStatusPremise, "Incorrect status return for back office validation");
                    Assert.AreEqual(Color.Green, GetMostDominantRbgColor(cloudActiveImage), "Incorrect image returned for Active status.");

                    //no data supplied for BO connection so not sure that Red is correct status.
                    //it may want to be transparent.
                    Assert.AreEqual(Color.Red, GetMostDominantRbgColor(premiseActiveImage), "Incorrect image returned for Active status.");

                    model.CloudPremiseKey = "blah";
                    cloudStatus = VerifyCloudConnection(model, out cloudCompanyName, out cloudCompanyUrl, out cloudErrors);
                    testStatusCloud = cloudStatus == TenantConnectivityStatus.Normal;

                    ConnectorUtilities.UpdateConnectionStatusesInStateService(aTenantId, backOfficeStatus, cloudStatus);
                    model.RefreshConnectionStatuses();

                    cloudActiveImage = model.CloudConnectionStatusImage;

                    Assert.IsFalse(testStatusCloud, "Active status came back true.");
                    Assert.IsFalse(testStatusPremise, "Incorrect image return for Active status");
                    Assert.AreEqual(Color.Red, GetMostDominantRbgColor(cloudActiveImage), "Incorrect image returned for Active status.");
                    Assert.AreEqual(Color.Red, GetMostDominantRbgColor(premiseActiveImage), "Incorrect image returned for Active status.");
                }
                finally
                {
                    if (csiFactory != null)
                    {
                        csiFactory.Abort();
                        csiFactory = null;
                    }
                    ShutDownMockCloud();
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        private BackOfficeConnectivityStatus VerifyBackOfficeConnection(ConfigurationViewModel model, out string[] errors)
        {
            ValidateBackOfficeConnectionResponse response = null;
            using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                response = proxy.ValidateBackOfficeConnectionCredentialsAsString(model.ConnectorPluginId, model.BackOfficeConnectionCredentials);
            }

            return ConnectorUtilities.ProcessValidateBackOfficeConnectionResponse(response, out errors);
        }

        private TenantConnectivityStatus VerifyCloudConnection(
            ConfigurationViewModel model,
            out string cloudCompanyName,
            out string cloudCompanyUrl,
            out string[] errors)
        {
            ValidateTenantConnectionResponse response = null;
            using (var proxy = TenantValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                // Verify cloud connection via the state service
                response = proxy.ValidateTenantConnection(
                    model.CloudEndpoint, model.CloudTenantId, model.CloudPremiseKey, String.Empty);
            }

            return ConnectorUtilities.ProcessValidateTenantConnectionResponse(response, out cloudCompanyName, out cloudCompanyUrl, out errors);
        }

        [TestMethod]
        public void TestValidateConnectionKey()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                StartUpMockCloud();
                try
                {
                    ConfigurationViewModelManager manager = new ConfigurationViewModelManager();
                    Guid newGuid = Guid.NewGuid();

                    //This configuration should pass as new tenant.
                    ConfigurationViewModel model = manager.CreateNewTenant(ConnectorPluginsViewModel.GetConnectorPlugins().Where(x => x.Id == "Mock").First());
                    model.CloudTenantId = newGuid.ToString();
                    model.CloudPremiseKey = "Blah";
                    List<string> msgs = new List<string>();
                    Assert.IsTrue(model.ValidateTenantIdUnique(ref msgs), "New connection failed Tenant validation");
                    Assert.AreEqual(0, msgs.Count, "Should be no messages");

                    //Change tenant should fail
                    model.Save();
                    msgs.Clear();
                    model.CloudTenantId = "AnotherValue";
                    Assert.IsFalse(model.ValidateTenantIdUnique(ref msgs), "Change of tenant should fail.");
                    Assert.AreEqual(1, msgs.Count, "There should be one message in the list");

                    //Revert to original value
                    msgs.Clear();
                    model.CloudTenantId = model.OriginalTenantId;
                    model.CloudPremiseKey = model.OriginalConfigurationPremiseKey;
                    Assert.IsTrue(model.ValidateTenantIdUnique(ref msgs), "New connection failed Tenant validation");
                    Assert.AreEqual(0, msgs.Count, "Should be no messages");
                }
                finally
                {
                    ShutDownMockCloud();
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }


        #region Helper Methods


        private ChannelFactory<ICREMessageServiceInjection> CreateMockMessageServiceInjectionChannelFactory()
        {
            ChannelFactory<ICREMessageServiceInjection> csiFactory = null;
            try
            {
                string mockEndpointTemplate = "http://{0}:8002/MockCloudServiceInjection.svc";
                string mockEndpointURIString = String.Format(mockEndpointTemplate, Environment.MachineName);
                EndpointAddress address = new EndpointAddress(mockEndpointURIString);
                WSHttpBinding binding = CloudUtils.CreateCloudBinding(new Uri(mockEndpointURIString));

                csiFactory = new ChannelFactory<ICREMessageServiceInjection>(binding, address);
                csiFactory.Endpoint.Behaviors.Add(new WebHttpBehavior());
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

        //Returns the color Red, Green, or Blue that represents the most dominant color in the image passed in.
        private Color GetMostDominantRbgColor(Image img)
        {
            long green = 0;
            long red = 0;
            long blue = 0;
            long alpha = 0;
            using (var bmp = new Bitmap(img))
            {

                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color c = bmp.GetPixel(i, j);
                        alpha += c.A;
                        green += c.G;
                        red += c.R;
                        blue += c.B;
                    }
                }
            }

            //TODO: look at this logic in light of changes to color indicators..

            //Which color is more dominant
            Color retVal = Color.Green;
            long max = green;

            if (red > max)
            {
                max = red;
                retVal = Color.Red;
            }

            if (blue > max)
            {
                max = blue;
                retVal = Color.Blue;
            }

            if (alpha == 0)
            {
                retVal = Color.Transparent;
            }

            return retVal;
        }



        private void StartUpMockCloud()
        {
            MockCloudServiceHost.StartService();
        }

        private void ShutDownMockCloud()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);

            MockCloudServiceHost.StopService();
        }

        ///
        /// Checks the file exists or not.
        ///
        /// The URL of the remote file.
        /// True : If the file exits, False if file not exists
        private bool RemoteFileExists(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                //Returns TURE if the Status code == 200
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }


        #endregion
    }
}
