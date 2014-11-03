using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.TestUtilities;
using SageConnector.ViewModel;

namespace SageConnector.Test
{
    /// <summary>
    ///This is a test class for ConfigurationViewModelManagerTest and is intended
    ///to contain all ConfigurationViewModelManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConfigurationViewModelManagerTest
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
        public void TestDeleteConfiguration()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                ConfigurationViewModelManager target = new ConfigurationViewModelManager();
                target.FillList();
                int tenantCount = target.Configurations.Count;

                //Add new tenant and persist.
                Guid tenant = Guid.NewGuid();
                ConfigurationViewModel newModel = target.CreateNewTenant(ConnectorPluginsViewModel.GetConnectorPlugins().Where(x => x.Id == "Mock").First());
                newModel.CloudTenantId = tenant.ToString();
                newModel.CloudPremiseKey = "Test-Test";
                newModel.Save();

                target.FillList();
                Assert.AreEqual(tenantCount + 1, target.Configurations.Count, "Configuration count should have increased by one");
                target = null;
                newModel = null;

                //Delete tenant by tenantId
                target = new ConfigurationViewModelManager();
                target.FillList();
                target.SetCurrent(tenant.ToString());
                newModel = target.CurrentConfiguration;
                Assert.IsNotNull(newModel, "Did not find new configuration");

                target.DeleteConfiguration(newModel);
                Assert.IsNull(target.CurrentConfiguration, "CurrentConfiguration should be null");
                target.FillList();
                Assert.AreEqual(tenantCount, target.Configurations.Count, "Configuration count should be back to original");
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod]
        public void TestAddNewConfiguration()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                ConfigurationViewModelManager target = new ConfigurationViewModelManager();
                target.FillList();
                int currentCount = target.Configurations.Count;

                string name = "name";
                string connectionKey = "connection:key";

                ConfigurationViewModel newTenant = target.CreateNewTenant(ConnectorPluginsViewModel.GetConnectorPlugins().Where(x => x.Id == "Mock").First());
                newTenant.BackOfficeCompanyName = name;
                newTenant.CompositeConnectionKey = connectionKey;

                target = null;
                newTenant.Save();

                //Verify saved tenant
                target = new ConfigurationViewModelManager();
                target.FillList();
                Assert.AreEqual(currentCount + 1, target.Configurations.Count, "The Config count should have increased by one.");
                target.SetCurrent("connection");//tenant portion of the connection key.
                Assert.IsNotNull(target.CurrentConfiguration, "CurrentConfiguration returned null");
                Assert.AreEqual("connection", target.CurrentConfiguration.CloudTenantId);
                Assert.AreEqual("key", target.CurrentConfiguration.CloudPremiseKey);
                Assert.AreEqual(name, target.CurrentConfiguration.BackOfficeCompanyName, "Back office company name did not match");
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        /// <summary>
        ///A test for FillList
        ///</summary>
        [TestMethod()]
        public void TestFillList()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                //Code currently assumes that after fill list there is always at least one item in the configurations list.

                ConfigurationViewModelManager target = new ConfigurationViewModelManager();
                int vmCount = target.Configurations.Count;
                Assert.AreEqual(vmCount, 0, "Configuration View Model Manger unexpectedly has items in its Configurations list");

                target.FillList();
                vmCount = target.Configurations.Count;
                Assert.AreNotEqual(vmCount, 0, "Configuration View Model Manger unexpectedly does not have items in its Configurations list");
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod()]
        public void TestSetCurrentConfig()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                //Add some configurations
                Dictionary<int, string> newConfigs = TestUtils.CreateNewPremiseStoreConfigurations(3);

                //Run test with multi configurations
                ValidateSetCurrentConfig();

                //Clean up configs
                TestUtils.DeletePremiseStoreConfigurations(newConfigs);
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }



        #region Helper Methods

        private void ValidateSetCurrentConfig()
        {
            ConfigurationViewModelManager target = new ConfigurationViewModelManager();
            target.FillList();

            if (target.Configurations.Count <= 0)
            {
                Assert.Inconclusive("Cannot test, there are no configurations available in the list to set as current.");
                return;
            }
            //Get a random configuration
            int configCount = target.Configurations.Count;
            Random rand = new Random(DateTime.Now.Millisecond);
            int randItem = rand.Next(0, configCount);

            ConfigurationViewModel config = target.Configurations[randItem];

            //set the current configuration.
            string selectedconfigurationId = config.CloudTenantId;
            target.SetCurrent(selectedconfigurationId);

            Assert.AreEqual(selectedconfigurationId, target.CurrentConfiguration.CloudTenantId, "Current configuration was not set correctly");
            Assert.AreEqual(configCount, target.Configurations.Count, "Configuration count has changed during test.");

            //Make sure all are unique
            List<string> tenants = new List<string>();
            foreach (ConfigurationViewModel vm in target.Configurations)
                if (!tenants.Contains(vm.CloudTenantId))
                    tenants.Add(vm.CloudTenantId);

            Assert.AreEqual(tenants.Count, target.Configurations.Count, "There is a duplicate tenant in the configuration list.");
        }

        #endregion
    }
}
