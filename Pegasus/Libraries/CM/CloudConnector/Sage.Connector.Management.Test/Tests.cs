using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sage.Connector.Management.Test
{
    [TestClass]
    public class Tests
    {
        /// <summary>
        /// test register connection. Can only be used manually as it needs a mock cloud.
        /// </summary>
        //[TestMethod]
        //[Ignore]
        //public void RegistrationTest()
        //{
        //    ConfigurationHelpers.RegisterConnection();
        //    Assert.IsTrue(true);
        //}

        [TestMethod]
        [Ignore]
        public void BackOfficePluginDownloadTest()
        {
            //move this to a test that has the service running.
            string backOfficeId = "Mock2";
            string autoUpdateProductId = "Sage.NA.SBS.Pegasus.Addins";
            string autoUpdateVersion = "1.0";
            string autoUpdateComponentBaseName = "Discovery.";
            ConfigurationHelpers.DownloadBackOfficePlugin(backOfficeId, autoUpdateProductId, autoUpdateVersion, autoUpdateComponentBaseName);

        }

        [TestMethod]
        [Ignore]
        public void TenantListTest()
        {
            var result = ConfigurationHelpers.GetTenantList("sageIdToken", ConfigurationHelpers.GetDefaultCloudSiteUri().ToString());
            Assert.IsNotNull(result);
        }
    }
}
