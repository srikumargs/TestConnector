using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.TestUtilities;
using SageConnectorConfiguration.ViewModel;

namespace SageConnectorConfiguration.Test
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

        // Things to write unit tests for (should test each view model by calling Initialize first)
        //      - WindowsAccountSelectorViewModel -> ValidateUser() (remember to set up the xml file first)
        //      - InstallViewModel -> InstallButtonClick() (if connector not installed)
        //      - ConfigureViewModel -> ConfigureButtonClick() (set up the ConfigureViewModel context properly)
        //      - ConfigureTenantViewModel -> ValidateBOConnection() (make sure BO installed, pick Sage300CRE, set BackofficeAdminName, BackofficeAdminPassword, return bool should be true)
        //          and ConfigureTenantButtonClick()

        [TestMethod]
        [Ignore]
        public void TestWindowsAccountSelectorViewModel()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                // Things to write unit tests for (should test each view model by calling Initialize first)
                //      - WindowsAccountSelectorViewModel -> ValidateUser() (remember to set up the xml file first)
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }
        }

        [TestMethod]
        [Ignore]
        public void TestInstallViewModel()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                // Things to write unit tests for (should test each view model by calling Initialize first)
                //      - InstallViewModel -> InstallButtonClick() (if connector not installed)
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }
        }

        [TestMethod]
        [Ignore]
        public void TestConfigureViewModel()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                // Things to write unit tests for (should test each view model by calling Initialize first)
                //      - ConfigureViewModel -> ConfigureButtonClick() (set up the ConfigureViewModel context properly)
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }
        }

        [TestMethod]
        [Ignore]
        public void TestConfigureTenantViewModel()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                // Things to write unit tests for (should test each view model by calling Initialize first)
                //      - ConfigureTenantViewModel -> ValidateBOConnection() (make sure BO installed, pick Sage300CRE, set BackofficeAdminName, BackofficeAdminPassword, return bool should be true)
                //          and ConfigureTenantButtonClick()
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }
        }
    }
}
