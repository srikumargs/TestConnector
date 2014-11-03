using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.TestUtilities;

namespace Sage.Connector.Utilities.Test
{
    [TestClass]
    public class ProcessKillerTest
    {
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


        [TestMethod]
        public void LaunchAndKillNotepad()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            using (Process notePadProcess = new Process())
            {
                notePadProcess.StartInfo.FileName = "notepad.exe";
                notePadProcess.Start();

                ProcessKiller.KillProcessTree(notePadProcess.Id);
                Thread.Sleep(100);

                Assert.IsTrue(notePadProcess.HasExited);
            }
        }
    }
}
