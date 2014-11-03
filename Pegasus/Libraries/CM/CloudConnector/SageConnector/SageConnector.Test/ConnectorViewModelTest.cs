using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Common;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Connector.TestUtilities;
using SageConnector.ViewModel;

namespace SageConnector.Test
{
    /// <summary>
    /// Summary description for ConnectorViewModelTest
    /// </summary>
    [TestClass]
    public class ConnectorViewModelTest
    {
        public ConnectorViewModelTest() { }

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
        public void TestValidateBackOfficeAdmin()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            string backOfficeId = "Mock";
            string adminUser = "admin";
            string password = "1";
            string[] errors = new string[] { };

            var response = GetCompanyConnectionManagementCredentialsNeeded(backOfficeId, out errors);
            IDictionary<string, string> credentials = response.CurrentValues;

            credentials["UserId"] = adminUser;
            credentials["Password"] = password;

            Assert.IsTrue(ValidateBackOfficeAdmin(backOfficeId, credentials, out errors),
                string.Format("User should have validated - known good user and password{0}{1}",
                Environment.NewLine, errors));

            adminUser = "duh";
            credentials["UserId"] = adminUser;
            Assert.IsFalse(ValidateBackOfficeAdmin(backOfficeId, credentials, out errors),
                string.Format("User validation should have failed - bad user name{0}{1}",
                Environment.NewLine, errors));

            //This test will fail when implemented
            adminUser = "admin";
            password = "duh";
            credentials["UserId"] = adminUser;
            credentials["Password"] = password;
            Assert.IsFalse(ValidateBackOfficeAdmin(backOfficeId, credentials, out errors),
                string.Format("User validation should have failed - bad user password{0}{1}",
                Environment.NewLine, errors));
        }

        private Boolean ValidateBackOfficeAdmin(String backOfficeId, IDictionary<string, string> credentials, out string[] errors)
        {
            ValidateBackOfficeAdminCredentialsResponse response = null;

            using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                response = proxy.ValidateBackOfficeAdminCredentials(backOfficeId, credentials);
            }

            return ConnectorViewModel.ProcessValidateBackOfficeAdminCredentialsResponse(response, out errors);
        }

        private ManagementCredentialsNeededResponse GetCompanyConnectionManagementCredentialsNeeded(String backOfficeId, out string[] errors)
        {
            ManagementCredentialsNeededResponse response = null;

            using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                response = proxy.GetManagementCredentialsNeeded(backOfficeId);
                //TODO: value up errors out
                errors = null;
            }

            return response;
        }
    }
}
