using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.SageCloudService;
using Sage.Connector.TestUtilities;

namespace Sage.Connector.Utilities.Test
{
    [TestClass]
    public class InternetConnectivityStatusHelperTest
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
        ///A test for GetReportList
        ///</summary>
        ///<remarks>
        ///Tests the "full" get report list stack from manager on down to mock.
        ///</remarks>
        [TestMethod()]
        public void TestGetGatewayServiceUri()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            try
            {
                MockCloudServiceHost.StartService();
                Uri uri = new Uri(MockCloudServiceHost.SiteAddress);
                var result = InternetConnectivityStatusHelper.GetGatewayServiceUri(uri);
                Assert.IsNotNull(result, "Result was null");
                Assert.AreEqual(result.Result, GetGatewayServiceUriResult.Success);

                MockCloudServiceHost.StopService();

                result = InternetConnectivityStatusHelper.GetGatewayServiceUri(uri);
                Assert.IsNotNull(result, "Result was null");
                Assert.AreEqual(result.Result, GetGatewayServiceUriResult.Step13_GatewayUriCreated);

                Uri badUri = new UriBuilder("X" + uri.Scheme, uri.Host, uri.Port, uri.PathAndQuery).Uri;
                result = InternetConnectivityStatusHelper.GetGatewayServiceUri(badUri);
                Assert.IsNotNull(result, "Result was null");
                Assert.AreEqual(result.Result, GetGatewayServiceUriResult.Step1i_MicrosoftNCSISuccess);
            }
            finally
            {
                MockCloudServiceHost.StopService();
            }
        }
    }
}
