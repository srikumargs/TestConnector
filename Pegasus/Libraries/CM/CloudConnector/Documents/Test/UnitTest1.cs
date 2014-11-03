using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.TestUtilities;

namespace Sage.Connector.Documents.Test
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        private TestContext testContextInstance;

        private static String UnitTestTenant = "UNITTEST";

        private static void CreateMBFile(string filePath)
        {
            int megabyte = 1024 * 1024;
            char[] buffer = new char[megabyte];

            for (int i = 0; i < buffer.Length; i++)
            {
                // just to vary the output
                if (i % 3 == 0)
                    buffer[i] = '_';
                else if (i % 2 == 0)
                    buffer[i] = '1';
                else
                    buffer[i] = '0';
            }

            using (var writer = new System.IO.StreamWriter(filePath))
            {
                writer.Write(buffer);
                writer.Flush();
            }
        }

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
            DocumentManager dm = new DocumentManager();
            dm.DeleteTenant(UnitTestTenant);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestPathGenerator()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            DocumentManager dm = new DocumentManager();
            Guid requestId = Guid.NewGuid();
            string generatedPath = dm.GetFilePathLocation(UnitTestTenant, requestId, "TXT");
            string fileName = Path.GetFileName(generatedPath);
            Assert.AreEqual(requestId + ".TXT", fileName);
            string directoryName = Path.GetDirectoryName(generatedPath);
            Assert.IsTrue(directoryName.EndsWith(UnitTestTenant));
        }

        /// <summary>
        /// Tests no-op for file sent.
        /// </summary>
        [TestMethod]
        public void TestNoOpCopy()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            DocumentManager dm = new DocumentManager();
            Guid requestId = Guid.NewGuid();
            string generatedPath = dm.GetFilePathLocation(UnitTestTenant, requestId, "TXT");
            CreateMBFile(generatedPath);
            Assert.IsTrue(File.Exists(generatedPath));

            DocumentManager.FileSentForUnitTesting(UnitTestTenant, generatedPath, DocumentManager.SentDocumentCleanupPolicy.None, 0, 0);
            Assert.IsTrue(File.Exists(generatedPath));

            string sentPath = Path.Combine(DocumentManager.SentFolderForUnitTesting(UnitTestTenant), requestId + ".TXT");
            Assert.IsFalse(File.Exists(sentPath));
        }

        /// <summary>
        /// Tests always delete for file sent.
        /// </summary>
        [TestMethod]
        public void TestSentDeletion()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            DocumentManager dm = new DocumentManager();
            Guid requestId = Guid.NewGuid();
            string generatedPath = dm.GetFilePathLocation(UnitTestTenant, requestId, "TXT");
            CreateMBFile(generatedPath);
            Assert.IsTrue(File.Exists(generatedPath));

            DocumentManager.FileSentForUnitTesting(UnitTestTenant, generatedPath, DocumentManager.SentDocumentCleanupPolicy.Immediate, 0, 0);
            Assert.IsFalse(File.Exists(generatedPath));

            string sentPath = Path.Combine(DocumentManager.SentFolderForUnitTesting(UnitTestTenant), requestId + ".TXT");
            Assert.IsFalse(File.Exists(sentPath));
        }

        /// <summary>
        /// Tests no sent file management
        /// </summary>
        [TestMethod]
        public void TestSentCopy()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            DocumentManager dm = new DocumentManager();
            Guid requestId = Guid.NewGuid();
            string generatedPath = dm.GetFilePathLocation(UnitTestTenant, requestId, "TXT");
            CreateMBFile(generatedPath);
            Assert.IsTrue(File.Exists(generatedPath));

            DocumentManager.FileSentForUnitTesting(UnitTestTenant, generatedPath, DocumentManager.SentDocumentCleanupPolicy.Never, 0, 0);
            Assert.IsFalse(File.Exists(generatedPath));

            string sentPath = Path.Combine(DocumentManager.SentFolderForUnitTesting(UnitTestTenant), requestId + ".TXT");
            Assert.IsTrue(File.Exists(sentPath));
        }

        /// <summary>
        /// Tests always delete for file sent.
        /// </summary>
        [TestMethod]
        public void TestDayOldManagement()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            DocumentManager dm = new DocumentManager();
            Guid requestId = Guid.NewGuid();
            string generatedPath1 = dm.GetFilePathLocation(UnitTestTenant, requestId, "TXT");
            CreateMBFile(generatedPath1);
            Assert.IsTrue(File.Exists(generatedPath1));

            Guid requestId2 = Guid.NewGuid();
            string generatedPath2 = dm.GetFilePathLocation(UnitTestTenant, requestId2, "TXT");
            CreateMBFile(generatedPath2);
            Assert.IsTrue(File.Exists(generatedPath2));

            DocumentManager.FileSentForUnitTesting(UnitTestTenant, generatedPath1, DocumentManager.SentDocumentCleanupPolicy.DaysOld, 1, 0);
            // File 1 should have moved to sent
            Assert.IsFalse(File.Exists(generatedPath1));
            string savedpath1 = Path.Combine(DocumentManager.SentFolderForUnitTesting(UnitTestTenant), requestId + ".TXT");
            Assert.IsTrue(File.Exists(savedpath1));

            // Make path1 two days old
            DateTime path1FileCreationDate = File.GetCreationTimeUtc(savedpath1).AddDays(-2);
            File.SetCreationTimeUtc(savedpath1, path1FileCreationDate);

            DocumentManager.FileSentForUnitTesting(UnitTestTenant, generatedPath2, DocumentManager.SentDocumentCleanupPolicy.DaysOld, 1, 0);
            // File 2 should have moved to sent
            Assert.IsFalse(File.Exists(generatedPath2));
            string savedpath2 = Path.Combine(DocumentManager.SentFolderForUnitTesting(UnitTestTenant), requestId2 + ".TXT");
            Assert.IsTrue(File.Exists(savedpath2));
            // File 1 should have been deleted (too old)
            Assert.IsFalse(File.Exists(savedpath1));
        }

        /// <summary>
        /// Tests always delete for file sent.
        /// </summary>
        [TestMethod]
        public void TestSizeManagement()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            DocumentManager dm = new DocumentManager();
            Guid requestId = Guid.NewGuid();
            string generatedPath1 = dm.GetFilePathLocation(UnitTestTenant, requestId, "TXT");
            CreateMBFile(generatedPath1);
            Assert.IsTrue(File.Exists(generatedPath1));

            Guid requestId2 = Guid.NewGuid();
            string generatedPath2 = dm.GetFilePathLocation(UnitTestTenant, requestId2, "TXT");
            CreateMBFile(generatedPath2);
            Assert.IsTrue(File.Exists(generatedPath2));

            DocumentManager.FileSentForUnitTesting(UnitTestTenant, generatedPath1, DocumentManager.SentDocumentCleanupPolicy.DirectoryMegabyteSize, 0, 1);
            // File 1 should have moved to sent
            Assert.IsFalse(File.Exists(generatedPath1));
            string sentPath1 = Path.Combine(DocumentManager.SentFolderForUnitTesting(UnitTestTenant), requestId + ".TXT");
            Assert.IsTrue(File.Exists(sentPath1));

            DocumentManager.FileSentForUnitTesting(UnitTestTenant, generatedPath2, DocumentManager.SentDocumentCleanupPolicy.DirectoryMegabyteSize, 0, 1);
            // File 2 should have moved to sent
            Assert.IsFalse(File.Exists(generatedPath2));
            string sentPath2 = Path.Combine(DocumentManager.SentFolderForUnitTesting(UnitTestTenant), requestId2 + ".TXT");
            Assert.IsTrue(File.Exists(sentPath2));

            // File 1 should have been deleted (over capacity)
            Assert.IsFalse(File.Exists(sentPath1));
        }

        /// <summary>
        /// Signal for thread to continue to run
        /// </summary>
        private bool _stopped = false;

        /// <summary>
        /// Threaded file creation and 'sending'
        /// </summary>
        private void CreateAndSendFileWorker()
        {
            while (!_stopped)
            {
                DocumentManager dm = new DocumentManager();
                string generatedPath = dm.GetFilePathLocation(UnitTestTenant, Guid.NewGuid(), "TXT");
                CreateMBFile(generatedPath);

                Random rnd = new Random();
                Thread.Sleep(rnd.Next(1000));

                DocumentManager.FileSentForUnitTesting(UnitTestTenant, generatedPath, DocumentManager.SentDocumentCleanupPolicy.DirectoryMegabyteSize, 0, 2);
            }
        }


        /// <summary>
        /// Tests threaded (synchronous create / sent management)
        /// </summary>
        /// THIS TESTS RUNS FOR 30 SECONDS AND SHOULD RUN WITHOUT EXCEPTIONS
        [TestMethod, Ignore]
        public void TestSynchronousFileCreateAndSend()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            Thread thread1 = new Thread(CreateAndSendFileWorker);
            Thread thread2 = new Thread(CreateAndSendFileWorker);
            Thread thread3 = new Thread(CreateAndSendFileWorker);
            Thread thread4 = new Thread(CreateAndSendFileWorker);
            Thread thread5 = new Thread(CreateAndSendFileWorker);

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();
            thread5.Start();

            // Let the threads run for a while
            Thread.Sleep(30000);

             _stopped = true;

            // Give time for the threads to shut themselves down
            Thread.Sleep(1500);

            if (thread1.IsAlive)
            {
                thread1.Abort();
            }
            if (thread2.IsAlive)
            {
                thread2.Abort();
            }
            if (thread3.IsAlive)
            {
                thread3.Abort();
            }
            if (thread4.IsAlive)
            {
                thread4.Abort();
            }
            if (thread5.IsAlive)
            {
                thread5.Abort();
            }
        }
    }
}
