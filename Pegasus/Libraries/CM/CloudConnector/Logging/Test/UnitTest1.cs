using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.TestUtilities;

namespace Sage.Connector.Logging.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
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

        private void DoTestLogging(LogManager service)
        {
            service.WriteCritical(this, null);
            service.WriteCritical(this, "Hello");
            service.WriteCritical(this, null);
            service.WriteCritical(this, "Hello");

            service.WriteError(this, null);
            service.WriteError(this, "Bye");
            service.WriteError(this, null);
            service.WriteError(this, "Bye");

            service.WriteWarning(this, null);
            service.WriteWarning(this, "Bye");
            service.WriteWarning(this, null);
            service.WriteWarning(this, "Bye");

            service.WriteCritical(this, null);
            service.WriteCritical(this, "Foo");
            service.WriteCritical(this, null);
            service.WriteCritical(this, "Foo");

            service.WriteInfo(this, null);
            service.WriteInfo(this, "Unit");
            service.WriteInfo(this, null);
            service.WriteInfo(this, "Unit");

            service.WriteInfoWithEventLogging(this, "Unit", "Test");
        }

        [TestMethod]
        public void TestLogging_NonHosted()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);
            using (var logManager = new LogManager())
            {
                DoTestLogging(logManager);
            }
        }

        [TestMethod]
        public void TestLogging_Hosted()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                using (var lm = new LogManager())
                {
                    DoTestLogging(lm);
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }
    }
}
