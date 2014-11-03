using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.TestUtilities;
using SageConnector.ViewModel;

namespace SageConnector.Test
{


    /// <summary>
    ///This is a test class for ConfigurationViewModelTest and is intended
    ///to contain all ConfigurationViewModelTest Unit Tests
    ///</summary>
    [TestClass]
    public class ConfigurationViewModelTest
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
        [ClassCleanup]
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
        public void TestVerifyTenantIdConnectionKey()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                ConfigurationViewModelManager cvmm = new ConfigurationViewModelManager();
                cvmm.FillList();

                string tenant = cvmm.CurrentConfiguration.CloudTenantId;
                Assert.IsFalse(cvmm.ValidateTenantIdUnique(tenant), "TenantId validation should fail");

                tenant = Guid.NewGuid().ToString();
                Assert.IsTrue(cvmm.ValidateTenantIdUnique(tenant), "TenantId validation should pass.");

                //create new model and set connection key to same as currentconneciton
                ConfigurationViewModel newModel = cvmm.CreateNewTenant(ConnectorPluginsViewModel.GetConnectorPlugins().Where(x => x.Id == "Mock").First());
                newModel.CompositeConnectionKey = cvmm.CurrentConfiguration.CompositeConnectionKey;
                Assert.IsFalse(cvmm.ValidateConnectionKeyUnique(newModel.CompositeConnectionKey), "ConnectionKey validation should fail");

                newModel.CloudPremiseKey = "Test";
                Assert.IsFalse(cvmm.ValidateConnectionKeyUnique(newModel.CompositeConnectionKey), "ConnectionKey validation should fail");

                newModel.CloudPremiseKey = cvmm.CurrentConfiguration.CloudPremiseKey;
                Assert.IsTrue(cvmm.ValidateTenantIdUnique(newModel.CompositeConnectionKey), "ConnectionKey validation should pass.");

                newModel.CloudTenantId = Guid.NewGuid().ToString();
                Assert.IsTrue(cvmm.ValidateTenantIdUnique(newModel.CompositeConnectionKey), "ConnectionKey validation should pass.");

                newModel.CloudTenantId = cvmm.CurrentConfiguration.CloudTenantId;
                newModel.CompositeConnectionKey = "blah-blah";
                Assert.IsTrue(cvmm.ValidateTenantIdUnique(newModel.CompositeConnectionKey), "ConnectionKey validation should pass.");
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod]
        public void TestDuplicateCompany()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                ConfigurationViewModelManager cvmm = new ConfigurationViewModelManager();
                cvmm.FillList();

                string tenant = cvmm.CurrentConfiguration.CloudTenantId;

                ConfigurationViewModel model = cvmm.CurrentConfiguration;

                List<string> errors = new List<string>();
                Assert.IsTrue(model.ValidateCompanyUnique(ref errors), "Should pass because the value has not changed.");
                Assert.AreEqual(0, errors.Count, "Should be no errors in list");

                model.BackOfficeCompanyName = "Duh Company";
                Assert.IsTrue(model.ValidateCompanyUnique(ref errors), "Company name should pass as being unique.");

                if (cvmm.Configurations.Count > 1)
                {
                    ConfigurationViewModel nextModel = cvmm.Configurations[cvmm.Configurations.Count - 1];
                    model.BackOfficeCompanyName = nextModel.BackOfficeCompanyName;
                    errors.Clear();

                    Assert.IsFalse(model.ValidateCompanyUnique(ref errors), "Company unique should fail");
                    Assert.AreNotEqual(0, errors.Count, "Should be errors in the list");
                }

                model = null;
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod]
        public void TestValidateRequiredFields()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                ConfigurationViewModelManager cvmm = new ConfigurationViewModelManager();
                cvmm.FillList();
                ConfigurationViewModel target = cvmm.CurrentConfiguration;

                //TODO: are there any other cases that should be tested here? Removed case checking back office as name as it is no longer required.            
                List<string> msgs = new List<string>();

                target.CloudTenantId = "";
                msgs.Clear();
                Assert.IsFalse(target.ValidateRequiredFields(ref msgs), "Validate required fields should have complained about required fields");
                Assert.AreEqual(1, msgs.Count, "Should be one message returned");
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        /// <summary>
        ///A test for saving Back office Company name
        ///</summary>
        [TestMethod]
        public void TestSaveBOCompanyNameConfiguration()
        {
            string testCompanyName = "BackOffice2";
            string valueTested = "Back office company name";

            Action<ConfigurationViewModel, string> setValue = (ConfigurationViewModel model, string value) => { model.BackOfficeCompanyName = value; };
            Func<ConfigurationViewModel, string> getValue = (ConfigurationViewModel model) => { return model.BackOfficeCompanyName; };

            DoConfigurationViewModelTest(testCompanyName, valueTested, setValue, getValue);
        }


        /// <summary>
        ///A test for saving cloud company name
        ///</summary>
        [TestMethod]
        public void TestSaveCloudCompanyNameConfiguration()
        {
            string testCloudCompany = "TestCloudCompanyName";
            string valueTested = "Cloud Company Name";

            Action<ConfigurationViewModel, string> setValue = (ConfigurationViewModel model, string value) => { model.CloudCompanyName = value; };
            Func<ConfigurationViewModel, string> getValue = (ConfigurationViewModel model) => { return model.CloudCompanyName; };

            DoConfigurationViewModelTest(testCloudCompany, valueTested, setValue, getValue);
        }


        /// <summary>
        ///A test for saving cloud company URL
        ///</summary>
        [TestMethod]
        public void TestSaveCloudCompanyURLConfiguration()
        {
            string testCloudCompanyURL = "TestCloudCompanyURL";
            string valueTested = "Cloud Company URL";

            Action<ConfigurationViewModel, string> setValue = (ConfigurationViewModel model, string value) => { model.CloudCompanyUrl = value; };
            Func<ConfigurationViewModel, string> getValue = (ConfigurationViewModel model) => { return model.CloudCompanyUrl; };

            DoConfigurationViewModelTest(testCloudCompanyURL, valueTested, setValue, getValue);
        }


        /// <summary>
        ///A test for saving Tenant ID
        ///</summary>
        [TestMethod]
        public void TestSaveTenantIDConfiguration()
        {
            string testCloudTenantID = "TestCloudTenantID";
            string valueTested = "Tenant ID";

            Action<ConfigurationViewModel, string> setValue = (ConfigurationViewModel model, string value) => { model.CloudTenantId = value; };
            Func<ConfigurationViewModel, string> getValue = (ConfigurationViewModel model) => { return model.CloudTenantId; };

            DoConfigurationViewModelTest(testCloudTenantID, valueTested, setValue, getValue);
        }


        /// <summary>
        ///A test for saving Premise Key
        ///</summary>
        [TestMethod]
        public void TestSavePremiseKeyConfiguration()
        {
            string testPremiseKey = "TestPremiseKey";
            string valueTested = "Premise Key";

            Action<ConfigurationViewModel, string> setValue = (ConfigurationViewModel model, string value) => { model.CloudPremiseKey = value; };
            Func<ConfigurationViewModel, string> getValue = (ConfigurationViewModel model) => { return model.CloudPremiseKey; };

            DoConfigurationViewModelTest(testPremiseKey, valueTested, setValue, getValue);
        }


        /// <summary>
        ///A test for saving Back office connection enabled flag
        ///</summary>
        [TestMethod]
        public void TestSaveBOConnectionEnabledConfiguration()
        {
            bool testBOConnectionEnabled = false;
            string valueTested = "Back Office connection enabled";

            Action<ConfigurationViewModel, bool> setValue = (ConfigurationViewModel model, bool value) => { model.BackOfficeConnectionEnabledToReceive = value; };
            Func<ConfigurationViewModel, bool> getValue = (ConfigurationViewModel model) => { return model.BackOfficeConnectionEnabledToReceive; };

            DoConfigurationViewModelTest(testBOConnectionEnabled, valueTested, setValue, getValue);
        }



        /// <summary>
        ///A test for saving cloud connection enabled to receive flag
        ///</summary>
        [TestMethod]
        public void TestSaveCloudConnectionEnabledToReceiveConfiguration()
        {
            bool testCloudConnectionEnabled = false;
            string valueTested = "Cloud connection enabled To Receive";

            Action<ConfigurationViewModel, bool> setValue = (ConfigurationViewModel model, bool value) => { model.CloudConnectionEnabledToReceive = value; };
            Func<ConfigurationViewModel, bool> getValue = (ConfigurationViewModel model) => { return model.CloudConnectionEnabledToReceive; };

            DoConfigurationViewModelTest(testCloudConnectionEnabled, valueTested, setValue, getValue);
        }

        /// <summary>
        ///A test for saving cloud connection enabled to send flag
        ///</summary>
        [TestMethod]
        public void TestSaveCloudConnectionEnabledToSendConfiguration()
        {
            bool testCloudConnectionEnabled = false;
            string valueTested = "Cloud connection enabled to SEnd";

            Action<ConfigurationViewModel, bool> setValue = (ConfigurationViewModel model, bool value) => { model.CloudConnectionEnabledToSend = value; };
            Func<ConfigurationViewModel, bool> getValue = (ConfigurationViewModel model) => { return model.CloudConnectionEnabledToSend; };

            DoConfigurationViewModelTest(testCloudConnectionEnabled, valueTested, setValue, getValue);
        }


        /// <summary>
        ///Test composite connection key
        ///</summary>
        [TestMethod]
        public void TestCompositeConnectionKey()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            string expected;
            expected = "TenantID:PremiseKey";

            ConfigurationViewModel target = new ConfigurationViewModel();
            target.CloudPremiseKey = "PremiseKey";
            target.CloudTenantId = "TenantID";

            Assert.AreEqual(expected, target.CompositeConnectionKey, "Connection Key was not generated correctly");
        }

        /// <summary>
        ///Test empty composite connection key
        ///</summary>
        [TestMethod]
        public void TestEmptyCompositeConnectionKey()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            string expected = "";

            ConfigurationViewModel target = new ConfigurationViewModel();
            target.CloudPremiseKey = "";
            target.CloudTenantId = "";

            Assert.AreEqual(expected, target.CompositeConnectionKey, "Empty connection key was not generated correctly");
        }

        /// <summary>
        ///Test TenantID with '-' composite connection key
        ///</summary>
        [TestMethod]
        public void TestCompositeConnectionKeyWithDash()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            string expected = "Tenant-ID:PremiseKey";

            ConfigurationViewModel target = new ConfigurationViewModel();
            target.CloudPremiseKey = "PremiseKey";
            target.CloudTenantId = "Tenant-ID";

            Assert.AreEqual(expected, target.CompositeConnectionKey, "Connection Key not generated when TenantID contains a dash");
        }


        /// <summary>
        ///Test getting Premise Key
        ///</summary>
        [TestMethod]
        public void TestGetCloudPremiseKey()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            string actual;
            string expected = "PremiseKey";

            ConfigurationViewModel target = new ConfigurationViewModel();
            target.CompositeConnectionKey = "TenantID:PremiseKey";
            actual = target.CloudPremiseKey;

            Assert.AreEqual(expected, actual, "Premise key not retrieved correctly from CompositeConnectionKey");
        }

        /// <summary>
        ///Test getting Tenant ID
        ///</summary>
        [TestMethod]
        public void TestGetCloudTenantID()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            string actual;
            string expected = "TenantID";

            ConfigurationViewModel target = new ConfigurationViewModel();
            target.CompositeConnectionKey = "TenantID:PremiseKey";
            actual = target.CloudTenantId;

            Assert.AreEqual(expected, actual, "Tenant ID not retrieved correctly from CompositeConnectionKey");
        }

        #region Helper Methods
        /// <summary>
        ///Helper function for Save tests
        ///</summary>
        public void DoConfigurationViewModelTest<T>(T testValue, string valueNameTested, Action<ConfigurationViewModel, T> setValue, Func<ConfigurationViewModel, T> getValue)
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);

            ConfigurationViewModelManager cvmm = new ConfigurationViewModelManager();
            cvmm.FillList();
            ConfigurationViewModel target = cvmm.CurrentConfiguration;
            Assert.AreNotEqual(testValue, getValue(target), valueNameTested + " must be different than default");

            setValue(target, testValue);
            target.Save();

            cvmm = null;
            cvmm = new ConfigurationViewModelManager();
            cvmm.FillList();
            target = cvmm.CurrentConfiguration;

            ConfigurationViewModel savedCVM = null;
            for(int index = 0; index < cvmm.Configurations.Count; index ++)
            {
                savedCVM = cvmm.Configurations[index];
                string testConfig = testValue.ToString();
                string savedConfig = (getValue(savedCVM)).ToString();
                if (testConfig == savedConfig)
                    break;
            }

            Assert.IsNotNull(savedCVM, "failed to save " + valueNameTested);
            Assert.AreEqual(testValue, getValue(savedCVM), valueNameTested + " should be the same as what was saved");
        }
        #endregion
    }
}
