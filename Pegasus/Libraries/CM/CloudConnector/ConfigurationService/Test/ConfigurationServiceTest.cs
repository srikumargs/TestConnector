using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Common;
using Sage.Connector.ConfigurationService.Proxy;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Documents;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.Queues;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Connector.TestUtilities;
using Sage.Connector.Utilities;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;

namespace Sage.Connector.ConfigurationService.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ConfigurationServiceTest
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
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
        }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        [TestCleanup]
        public void MyTestCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
        }
        #endregion

        /// <summary>
        /// Simple test of proxy connectivity
        /// </summary>
        [TestMethod]
        public void TestServiceProxy()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    Assert.IsNotNull(proxy, "Failed to retrieve configuration proxy.");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod]
        public void TestLogEntryEntries()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    var logEntries = proxy.GetLogEntries().Where(x => x.User == Environment.UserName);
                    int previousLogEntryCount = logEntries.Count();

                    LogEntryRecordFactory factory = LogEntryRecordFactory.Create();
                    LogEntryRecord entry = factory.CreateNewEntry();
                    entry.User = "TestUser";
                    entry.Type = "LogEntry";
                    entry.Machine = "ThisMachine";
                    entry.DateTime = DateTime.UtcNow;
                    entry.SourceTypeName = "SourceTypeName";
                    entry.SourceMemberName = "SourceMemberName";
                    entry.ProcessId = 1;
                    entry.AppDomainId = 2;
                    entry.ThreadId = 3;
                    entry.ObjectId = 4;
                    entry.Description = "Description";
                    entry.CloudTenantId = "tenantId";
                    entry.CloudRequestId = Guid.Empty;
                    factory.AddEntry(entry);
                    Guid newEntryGuid = entry.Id;

                    logEntries = proxy.GetLogEntries().Where(x => x.User == Environment.UserName);
                    Assert.AreEqual(previousLogEntryCount + 1, logEntries.Count(), "Entry count should have increased by one.");
                    LogEntryRecord retrievedEntry = factory.GetEntry(newEntryGuid);
                    Assert.IsNotNull(retrievedEntry, "New entry was not found");
                    Assert.AreEqual(entry.User, retrievedEntry.User, "User value should be the same.");
                    Assert.AreEqual(entry.Type, retrievedEntry.Type, "Type value should be the same.");
                    Assert.AreEqual(entry.Machine, retrievedEntry.Machine, "Machine value should be the same.");
                    Assert.AreEqual(entry.DateTime.ToShortDateString(), retrievedEntry.DateTime.ToShortDateString(), "DateTime value should be the same.");
                    Assert.AreEqual(entry.SourceTypeName, retrievedEntry.SourceTypeName, "SourceTypeName value should be the same.");
                    Assert.AreEqual(entry.SourceMemberName, retrievedEntry.SourceMemberName, "SourceMemberName value should be the same.");
                    Assert.AreEqual(entry.AppDomainId, retrievedEntry.AppDomainId, "AppDomainId value should be the same.");
                    Assert.AreEqual(entry.ProcessId, retrievedEntry.ProcessId, "ProcessId value should be the same.");
                    Assert.AreEqual(entry.ThreadId, retrievedEntry.ThreadId, "ThreadId value should be the same.");
                    Assert.AreEqual(entry.ObjectId, retrievedEntry.ObjectId, "ObjectId value should be the same.");
                    Assert.AreEqual(entry.Description, retrievedEntry.Description, "User value should be the same.");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        [TestMethod]
        public void TestLogEntryFactory()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    LogEntryRecord[] logEntries = proxy.GetLogEntries();
                    LogEntryRecordFactory factory = LogEntryRecordFactory.Create();
                    int previousLogEntryCount = logEntries.Length;

                    LogEntryRecord entry = factory.CreateNewEntry();
                    entry.User = "TestUser";
                    entry.Type = "LogEntry";
                    entry.Machine = "ThisMachine";
                    entry.DateTime = DateTime.UtcNow;
                    entry.SourceTypeName = "SourceTypeName";
                    entry.SourceMemberName = "SourceMemberName";
                    entry.ProcessId = 0;
                    entry.AppDomainId = 0;
                    entry.ThreadId = 0;
                    entry.ObjectId = 0;
                    entry.Description = "Description";
                    entry.CloudTenantId = "tenantId";
                    factory.AddEntry(entry);
                    Guid newEntryGuid = entry.Id;

                    factory.DeleteEntry(newEntryGuid);

                    LogEntryRecord retrievedEntry = factory.GetEntry(newEntryGuid);
                    Assert.IsNull(retrievedEntry, "GetEntry should have returned null");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }
        }

        private void AddDummyLogEntry(LogEntryRecordFactory factory, out Int32 count)
        {
            count = 0;

            LogEntryRecord entry = factory.CreateNewEntry();
            entry.CloudRequestId = Guid.Empty;
            entry.CloudTenantId = "";
            entry.User = Environment.UserName;
            entry.Type = "LogEntry";
            entry.Machine = "ThisMachine";
            entry.DateTime = DateTime.UtcNow;
            entry.SourceTypeName = "SourceTypeName";
            entry.SourceMemberName = "SourceMemberName";
            entry.ProcessId = 0;
            entry.AppDomainId = 0;
            entry.ThreadId = 0;
            entry.ObjectId = 0;
            entry.Description = "Description";
            factory.AddEntry(entry);

            var logEntries = factory.GetEntries();
            count = logEntries.Where(x=>x.User == Environment.UserName).Count();
        }

        
        [TestMethod]
        public void TestLogEntryCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                var factory = LogEntryRecordFactory.Create();
                factory.RetentionPolicyThreshold = new TimeSpan(1, 0, 0);

                int countBaseline = factory.GetEntries().Where(x => x.User == Environment.UserName).Count();

                Int32 tempCount = 0;
                Int32 logsToAdd = 100;
                for (Int32 i = 0; i < logsToAdd; i++)
                {
                    AddDummyLogEntry(factory, out tempCount);
                }

                Assert.AreEqual(countBaseline + logsToAdd, tempCount, String.Format("Entry count should be baseline ({0}) + logsToAdd ({1}).", countBaseline, logsToAdd));

                TimeSpan tinyThreshold = new TimeSpan(0, 0, 10);
                Thread.Sleep(tinyThreshold);

                AddDummyLogEntry(factory, out tempCount);

                Assert.AreEqual(countBaseline + logsToAdd + 1, tempCount, String.Format("Entry count should be baseline ({0}) + logsToAdd ({1}) + 1.", countBaseline, logsToAdd));

                factory.RetentionPolicyThreshold = tinyThreshold;
                Thread.Sleep(tinyThreshold);

                AddDummyLogEntry(factory, out tempCount);

                Assert.AreEqual(tempCount, 1, "All baselines should be gone.");
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }
        }

        private static void AddDummyActivityEntry(ActivityEntryRecordFactory factory, out Int32 count)
        {
            var entry = factory.CreateNewEntry();
            entry.CloudRequestId = Guid.NewGuid();
            entry.CloudTenantId = "tenantId";
            entry.CloudRequestType = "type";
            entry.CloudRequestRetryCount = 0;
            entry.CloudRequestRequestingUser = "user";
            entry.CloudRequestCreatedTimestampUtc = DateTime.UtcNow;
            factory.AddEntry(entry);

            var logEntries = factory.GetEntries();
            count = logEntries.Length;
        }

        [TestMethod]
        public void TestActivityEntryCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            using (var logger = new SimpleTraceLogger())
            {
                var factory = ActivityEntryRecordFactory.Create(logger);
                factory.RetentionPolicyThreshold = new TimeSpan(1, 0, 0);

                int countBaseline = factory.GetEntries().Length;

                Int32 tempCount = 0;
                Int32 logsToAdd = 100;
                for (Int32 i = 0; i < logsToAdd; i++)
                {
                    AddDummyActivityEntry(factory, out tempCount);
                }

                Assert.AreEqual(countBaseline + logsToAdd, tempCount, String.Format("Entry count should be baseline ({0}) + logsToAdd ({1}).", countBaseline, logsToAdd));

                TimeSpan tinyThreshold = new TimeSpan(0, 0, 10);
                Thread.Sleep(tinyThreshold);

                AddDummyActivityEntry(factory, out tempCount);

                Assert.AreEqual(countBaseline + logsToAdd + 1, tempCount, String.Format("Entry count should be baseline ({0}) + logsToAdd ({1}) + 1.", countBaseline, logsToAdd));

                factory.RetentionPolicyThreshold = tinyThreshold;
                Thread.Sleep(tinyThreshold);

                AddDummyActivityEntry(factory, out tempCount);

                Assert.AreEqual(tempCount, 1, "All baselines should be gone.");
            }
        }


        [TestMethod]
        public void TestQueueLogAccess()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                using (var logger = new SimpleTraceLogger())
                using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    QueueEntryRecord[] queueEntries = proxy.GetQueueEntries();
                    QueueEntryRecordFactory factory = QueueEntryRecordFactory.Create(logger);
                    queueEntries = proxy.GetQueueEntries();
                    int previousEntryCount = queueEntries.Length;

                    QueueEntryRecord entry = factory.CreateNewEntry();
                    entry.User = "TestUser";
                    entry.Type = "Inbox";
                    entry.Machine = "ThisMachine";
                    entry.DateTimeUtc = DateTime.UtcNow;
                    entry.Content = "Payload";

                    Guid returnGuid = InsertQueueItem(entry);

                    queueEntries = null;
                    queueEntries = proxy.GetQueueEntries();
                    Assert.IsNotNull(queueEntries, "Proxy did not return queue entries.");
                    Assert.AreEqual(previousEntryCount + 1, queueEntries.Length, "Queue entries should have grown by only one.");

                    QueueEntryRecord newEntry = factory.GetEntry(returnGuid);
                    Assert.AreEqual(entry.User, newEntry.User, "Entry's User property doesn't match.");
                    Assert.AreEqual(entry.Type, newEntry.Type, "Entry's Type property doesn't match.");
                    Assert.AreEqual(entry.Machine, newEntry.Machine, "Entry's Machine property doesn't match.");
                    Assert.AreEqual(entry.DateTimeUtc.ToLongDateString(), newEntry.DateTimeUtc.ToLongDateString(), "Entry's DateTime property doesn't match.");
                    Assert.AreEqual(entry.Content, newEntry.Content, "Entry's Content property doesn't match.");

                    factory.DeleteEntry(returnGuid);
                    queueEntries = null;
                    queueEntries = proxy.GetQueueEntries();
                    Assert.AreEqual(previousEntryCount, queueEntries.Length, "Queue entries should be back to original count.");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        /// <summary>
        /// Test that when we manually corrupt the database
        /// A repair is attempted, fails, and the state service is notified
        /// Note: Marked as ignore because corrupting the db and restoring it is
        /// Potentially dangerous.  May not want to run this with every build
        /// </summary>
        [TestMethod, Ignore]
        public void TestConfigurationDatabaseHardCorruption()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                // Filenames for db backup/restore purposes
                string databaseFilename = "ConfigurationStore.sdf";
                string databaseFileDirectory = ConnectorServiceUtils.InstanceApplicationDataFolder;
                string sdfFilePath = Path.Combine(
                    databaseFileDirectory,
                    databaseFilename);
                string sdfBackupFilePath = Path.Combine(
                    databaseFileDirectory,
                    String.Format("{0}_{1}", DateTime.UtcNow.ToFileTimeUtc(), databaseFilename));
                string recoveryBackupDirectory = Path.Combine(
                    databaseFileDirectory,
                    ConnectorRegistryUtils.DatabaseRecoveryBackupDirectory);

                // Make sure that the sdf file exists
                if (!File.Exists(sdfFilePath))
                {
                    throw new ArgumentException(String.Format("Could not find database file '{0}'", sdfFilePath));
                }

                // Make sure there are no hard db corruptions already
                Assert.AreEqual(0, GetSubsystemHealthMessageCountForDBHardCorruption(),
                    "Database already has a hard corruption");

                try
                {
                    using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        // Create a new configuration
                        PremiseConfigurationRecord ipc = proxy.CreateNewConfiguration();
                        ipc.ConnectorPluginId = "Mock";
                        ipc.BackOfficeProductName = "Mock Back Office Product";

                        Assert.IsNotNull(ipc, "Failed to create a new configuration");

                        // Make sure the backup dir exists
                        // First back up the existing database
                        if (!Directory.Exists(recoveryBackupDirectory))
                        {
                            // Create the missing directory
                            Directory.CreateDirectory(recoveryBackupDirectory);
                        }

                        // Count the current number of config db backups in the recovery backup folder
                        string[] backupFiles = Directory.GetFiles(recoveryBackupDirectory, databaseFilename, SearchOption.AllDirectories);
                        int initialBackupFilesCount = (backupFiles == null) ? 0 : backupFiles.Count();

                        // Backup and hard corrupt the database
                        DatabaseRepairUtils.HardCorruptDatabase(sdfFilePath, sdfBackupFilePath);

                        // Try to add the new configuration
                        try
                        {
                            proxy.AddConfiguration(ipc);
                            Assert.Fail("Expected hard corruption scenario to throw an exception.");
                        }
                        catch (Exception)
                        {
                            // This is a hard corruption, so the exception is re-thrown
                        }

                        // Make sure that a new backup was created
                        backupFiles = Directory.GetFiles(recoveryBackupDirectory, databaseFilename, SearchOption.AllDirectories);
                        Assert.IsNotNull(backupFiles,
                            "There should be at least one backup file after the first hard db corruption was detected");
                        Assert.AreEqual(initialBackupFilesCount + 1, backupFiles.Count(),
                            "There should be exactly one new backup file after the first hard bd corruption was detected");

                        // Make sure that the state service was notified of the hard database corruption
                        Assert.AreEqual(1, GetSubsystemHealthMessageCountForDBHardCorruption(),
                            "Sate service did not get notified of the hard corruption");

                        // Try to add the new configuration again
                        try
                        {
                            proxy.AddConfiguration(ipc);
                            Assert.Fail("Expected existing hard corruption scenario to throw an exception again.");
                        }
                        catch (Exception)
                        {
                            // This is a hard corruption, so the exception is re-thrown
                        }

                        // Make sure that no new backups were created
                        backupFiles = Directory.GetFiles(recoveryBackupDirectory, databaseFilename, SearchOption.AllDirectories);
                        Assert.IsNotNull(backupFiles,
                            "There should be at least one backup file after the second hard db corruption was detected");
                        Assert.AreEqual(initialBackupFilesCount + 1, backupFiles.Count(),
                           "There should be exactly one new backup file after the second hard bd corruption was detected");

                        // Make sure that the state service still only has the one hard database corruption notification
                        Assert.AreEqual(1, GetSubsystemHealthMessageCountForDBHardCorruption(),
                            "Sate service should have exactly one notification of the hard corruption");
                    }
                }
                finally
                {
                    // Restore the original, non-corrupt configuration database file!
                    if (File.Exists(sdfBackupFilePath))
                    {
                        File.Copy(sdfBackupFilePath, sdfFilePath, true);
                        File.Delete(sdfBackupFilePath);
                    }

                    // Just make sure that our database is no longer corrupt!
                    using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        // Create a new configuration
                        PremiseConfigurationRecord ipc = proxy.CreateNewConfiguration();
                        Assert.IsNotNull(ipc, "Restoring non-corrupt database: Failed to create a new configuration");

                        // Set the ID
                        ipc.ConnectorPluginId = "Mock";
                        ipc.BackOfficeProductName = "Mock Back Office Product";

                        ipc.CloudTenantId = "unit test tenant";

                        // Add the new configuration
                        proxy.AddConfiguration(ipc);

                        // Retrieve it
                        PremiseConfigurationRecord ipc2 = proxy.GetConfiguration(ipc.CloudTenantId);
                        Assert.IsNotNull(ipc2, "Restoring non-corrupt database: Unable to add/retrieve new configuration");
                    }
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        /// <summary>
        /// Test of configuration CRUD
        /// </summary>
        [TestMethod]
        public void TestConfigurationCRUD()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    // Create a new configuration
                    PremiseConfigurationRecord ipc = proxy.CreateNewConfiguration();
                    Assert.IsNotNull(ipc, "Failed to create a new configuration");

                    // Set some properties
                    ipc.ConnectorPluginId = "Mock";
                    ipc.BackOfficeProductName = "Mock Back Office Product";

                    ipc.SiteAddress = "http://localhost";
                    ipc.PremiseAgent = "unit test premise agent";
                    ipc.CloudTenantId = "unit test tenant";
                    ipc.CloudPremiseKey = "unit test premise key";

                    ipc.MinCommunicationFailureRetryInterval = 1000;
                    ipc.MaxCommunicationFailureRetryInterval = 2000;

                    // Add it
                    proxy.AddConfiguration(ipc);

                    // Retreive it directly
                    PremiseConfigurationRecord ipc2 = proxy.GetConfiguration(ipc.CloudTenantId);
                    
                    Assert.AreEqual(ipc.SiteAddress, ipc2.SiteAddress);
                    Assert.AreEqual(ipc.PremiseAgent, ipc2.PremiseAgent);
                    Assert.AreEqual(ipc.CloudTenantId, ipc2.CloudTenantId);
                    Assert.AreEqual(ipc.CloudPremiseKey, ipc2.CloudPremiseKey);
                    
                    Assert.AreEqual(ipc.MinCommunicationFailureRetryInterval, ipc2.MinCommunicationFailureRetryInterval);
                    Assert.AreEqual(ipc.MaxCommunicationFailureRetryInterval, ipc2.MaxCommunicationFailureRetryInterval);

                    // Update it
                    
                    ipc2.SiteAddress = ConnectorRegistryUtils.SiteAddress;
                    ipc2.PremiseAgent = "unit test updated premise agent";
                    ipc2.CloudTenantId = "unit test updated tenant";
                    ipc2.CloudPremiseKey = "unit test updated premise key";
                    ipc2.MinCommunicationFailureRetryInterval = 2000;
                    ipc2.MaxCommunicationFailureRetryInterval = 3000;
                    proxy.UpdateConfiguration(ipc2);

                    // Retrieve it again
                    ipc = proxy.GetConfiguration(ipc2.CloudTenantId);
                    Assert.AreEqual(ipc.SiteAddress, ipc2.SiteAddress);
                    Assert.AreEqual(ipc.PremiseAgent, ipc2.PremiseAgent);
                    Assert.AreEqual(ipc.CloudTenantId, ipc2.CloudTenantId);
                    Assert.AreEqual(ipc.CloudPremiseKey, ipc2.CloudPremiseKey);
                    Assert.AreEqual(ipc.MinCommunicationFailureRetryInterval, ipc2.MinCommunicationFailureRetryInterval);
                    Assert.AreEqual(ipc.MaxCommunicationFailureRetryInterval, ipc2.MaxCommunicationFailureRetryInterval);

                    // Retrieve the entire collection and find ours...
                    bool found = false;
                    PremiseConfigurationRecord[] ipcs = proxy.GetConfigurations();
                    foreach (PremiseConfigurationRecord ipc3 in ipcs)
                    {
                        if (ipc3.CloudTenantId == ipc.CloudTenantId)
                        {
                            found = true;
                            break;
                        }
                    }
                    Assert.IsTrue(found, "Saved configuration not found in collection.");

                    // Delete our tenant
                    proxy.DeleteConfiguration(ipc.CloudTenantId);

                    // Retrieve the entire collection and _not_ find ours...
                    found = false;
                    ipcs = proxy.GetConfigurations();
                    foreach (PremiseConfigurationRecord ipc3 in ipcs)
                    {
                        if (ipc3.CloudTenantId == ipc.CloudTenantId)
                        {
                            found = true;
                            break;
                        }
                    }
                    Assert.IsFalse(found, "Saved configuration should _not_ be found in collection.");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        private Guid InsertQueueItem(QueueEntryRecord item)
        {
            using (var logger = new SimpleTraceLogger())
            {
                QueueEntryRecordFactory qFactory = QueueEntryRecordFactory.Create(logger);
                return qFactory.AddEntry(item);
            }
        }

        private static void CreateSmallFile(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                writer.Write("Test string");
                writer.Flush();
            }
        }

        private sealed class Subscriber
        {
            public Subscriber()
            {
                ConfigurationAddedOccurrences = new List<String>();
                ConfigurationUpdatedOccurrences = new List<String>();
                ConfigurationDeletedOccurrences = new List<String>();
            }

            public void ConfigurationAdded(String cloudTenantId)
            { ConfigurationAddedOccurrences.Add(cloudTenantId); }

            public void ConfigurationUpdated(String cloudTenantId)
            { ConfigurationUpdatedOccurrences.Add(cloudTenantId); }

            public void ConfigurationDeleted(String cloudTenantId)
            { ConfigurationDeletedOccurrences.Add(cloudTenantId); }

            public void Clear()
            {
                ConfigurationAddedOccurrences.Clear();
                ConfigurationUpdatedOccurrences.Clear();
                ConfigurationDeletedOccurrences.Clear();
            }

            public List<String> ConfigurationAddedOccurrences
            { get; set; }

            public List<String> ConfigurationUpdatedOccurrences
            { get; set; }

            public List<String> ConfigurationDeletedOccurrences
            { get; set; }
        }

        //SA is being an idiot about subscriptionServiceProxy not getting disposed in some cases. So suppress it.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), TestMethod]
        public void TestServiceProxyWithSubscription()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                Subscriber subscriber = new Subscriber();
                NotificationCallbackInstanceHelper callbackInstance = new NotificationCallbackInstanceHelper();
                NotificationSubscriptionServiceProxy subscriptionServiceProxy = null;
                try
                {
                    subscriptionServiceProxy = NotificationSubscriptionServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber, callbackInstance);

                    Assert.IsNotNull(subscriptionServiceProxy, "Failed to retrieve notification subscription proxy.");

                    subscriptionServiceProxy.Connect();
                    callbackInstance.SubscribeConfigurationAdded(subscriptionServiceProxy, subscriber.ConfigurationAdded);
                    callbackInstance.SubscribeConfigurationUpdated(subscriptionServiceProxy, subscriber.ConfigurationUpdated);
                    callbackInstance.SubscribeConfigurationDeleted(subscriptionServiceProxy, subscriber.ConfigurationDeleted);

                    using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        Assert.IsNotNull(proxy, "Failed to retrieve configuration proxy.");

                        var config = proxy.CreateNewConfiguration();
                        config.ConnectorPluginId = "Mock";
                        config.BackOfficeProductName = "Mock Back Office Product";
                        config.CloudTenantId = Guid.NewGuid().ToString();
                        proxy.AddConfiguration(config);
                        config.CloudTenantId = Guid.NewGuid().ToString();
                        proxy.AddConfiguration(config);

                        proxy.UpdateConfiguration(config);
                        proxy.UpdateConfiguration(config);
                        proxy.UpdateConfiguration(config);

                        proxy.DeleteConfiguration(config.CloudTenantId);

                        System.Threading.Thread.Sleep(5000);
                        Assert.AreEqual(2, subscriber.ConfigurationAddedOccurrences.Count);
                        Assert.AreEqual(3, subscriber.ConfigurationUpdatedOccurrences.Count);
                        Assert.AreEqual(1, subscriber.ConfigurationDeletedOccurrences.Count);
                    }

                    callbackInstance.Unsubscribe(subscriptionServiceProxy);
                    subscriptionServiceProxy.Disconnect();
                    subscriptionServiceProxy.Close();
                    subscriptionServiceProxy = null;
                }
                finally
                {
                    if (subscriptionServiceProxy != null)
                    {
                        subscriptionServiceProxy.Abort();
                    }
                    callbackInstance = null;
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }
        }

        /// <summary>
        /// Tests out all aspects of cleanup that should take place when a tenant is deleted
        /// Including document storage and queues
        /// </summary>
        [TestMethod]
        public void TestServiceDeleteTenantCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                using (var proxy = ConfigurationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    Assert.IsNotNull(proxy, "Failed to retrieve configuration proxy.");

                    string fakeResponse = "Fake response string";
                    string fakeResponseType = "Fake response type string";

                    // Create a new tenant config
                    var config = proxy.CreateNewConfiguration();
                    config.ConnectorPluginId = "Mock";
                    config.BackOfficeProductName = "Mock Back Office Product";
                    config.CloudTenantId = Guid.NewGuid().ToString();
                    proxy.AddConfiguration(config);

                    using (var qm = new QueueManager())
                    {
                        Assert.IsNotNull(qm, "Failed to create queue manager");

                        // Add a fake response to the outbound queue for this tenant
                        // And vierify that it made it there
                        qm.AddMessageToOutput(fakeResponse, fakeResponseType, QueueContext.FakeTenantInstance(config.CloudTenantId));
                        string[] peekMessages = qm.PeekMessagesFromOutput(1, config.CloudTenantId);
                        Assert.IsTrue(peekMessages.Length == 1,
                            "Could not add a response message for the tenant prior to deletion");
                        Assert.AreEqual(fakeResponse, peekMessages[0],
                            "Retrieved message should have matched the test response string");

                        // Create a document for the tenant we are about to delete
                        DocumentManager dm = new DocumentManager();
                        string generatedPath = dm.GetFilePathLocation(config.CloudTenantId, Guid.NewGuid(), "TXT");
                        CreateSmallFile(generatedPath);
                        Assert.IsTrue(File.Exists(generatedPath),
                            "Test file for tenant not created successfully");

                        // Delete the tenant and allow time for processing
                        proxy.DeleteConfiguration(config.CloudTenantId);
                        System.Threading.Thread.Sleep(5000);

                        // Make sure the outbound queue is clear
                        StorageQueueMessage isqm = qm.GetMessageFromOutput(config.CloudTenantId);
                        Assert.IsNull(isqm, "Output queue for tenant should be empty after tenant delete");

                        // Make sure the document no longer exists
                        Assert.IsFalse(Directory.Exists(generatedPath),
                            "Document directory for tenant should no longer exist after tenant is deleted");

                        // Try to add another fake response to the outbound queue for this tenant
                        qm.AddMessageToOutput(fakeResponse, fakeResponseType, QueueContext.FakeTenantInstance(config.CloudTenantId));

                        // Ensure that the response never made it
                        isqm = qm.GetMessageFromOutput(config.CloudTenantId);
                        Assert.IsNull(isqm);

                        // Create a new request
                        LoopBackRequest reportListRequest = new LoopBackRequest(
                            Guid.NewGuid(),
                            DateTime.UtcNow,
                            0, 0, "");

                        // Add request to inbound queue for the deleted tenant and allow time for processing
                        String requestAsString =
                            Utils.JsonSerialize(new RequestWrapper(new ActivityTrackingContext(Guid.NewGuid(), config.CloudTenantId, reportListRequest.Id, reportListRequest.GetType().FullName), Utils.JsonSerialize(reportListRequest)));
                        qm.AddMessageToInput(requestAsString, reportListRequest.GetType().FullName, QueueContext.FakeTenantInstance(config.CloudTenantId));
                        System.Threading.Thread.Sleep(5000);

                        // Make sure the request never made it to the outbound queue
                        isqm = qm.GetMessageFromOutput(config.CloudTenantId);
                        Assert.IsNull(isqm);
                    }
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        /// <summary>
        /// Gets the number of state service messages for hard db corruption
        /// </summary>
        /// <returns></returns>
        private int GetSubsystemHealthMessageCountForDBHardCorruption()
        {
            using (var stateServiceProxy = StateServiceProxyFactory.CreateFromCatalog(
                        "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                ConnectorState connectorState = stateServiceProxy.GetConnectorState();
                IEnumerable<SubsystemHealthMessage> messages = 
                    connectorState.SubsystemHealthMessages.Where(x => x.Subsystem == Subsystem.DatabaseHardCorruption);
                return (messages == null) ? 0 : messages.Count();
            }
        }


        private Dictionary<String, Boolean> _PCReadResult = new Dictionary<string, bool>();
        private void PCReadAfterSleep(string InstanceId, string TenantId, int SleepMillis)
        {
            Thread.Sleep(SleepMillis);
            using (var configServiceProxy = ConfigurationServiceProxyFactory.CreateFromCatalog(
                        "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                _PCReadResult[InstanceId] = (null != configServiceProxy.GetConfiguration(TenantId));
            }
        }

        private void PCWriteAfterSleep(string TenantId, int SleepMillis)
        {
            Thread.Sleep(SleepMillis);
            using (var configServiceProxy = ConfigurationServiceProxyFactory.CreateFromCatalog(
                        "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                PremiseConfigurationRecord pcr = configServiceProxy.CreateNewConfiguration();
                pcr.ConnectorPluginId = "Mock";
                pcr.BackOfficeProductName = "Mock Back Office Product";
                pcr.CloudTenantId = TenantId;
                configServiceProxy.AddConfiguration(pcr);
            }
        }


        /// <summary>
        /// Tests read responsiveness of the configuration service.
        /// The added tenant must be readable after one second. (It can be faster).
        /// </summary>
        [TestMethod, Ignore]
        public void TestConfigAddAndRead()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                string newTenant = Guid.NewGuid().ToString();

                int iWriteMillis = 100;

                Task.Factory.StartNew(() => PCReadAfterSleep("ReadBeforeTick", newTenant, iWriteMillis - 90));
                Task.Factory.StartNew(() => PCWriteAfterSleep(newTenant, iWriteMillis));
                Task.Factory.StartNew(() => PCReadAfterSleep("Read1MsAfterWrite", newTenant, iWriteMillis + 1));
                Task.Factory.StartNew(() => PCReadAfterSleep("Read10MsAfterTick", newTenant, iWriteMillis + 10));
                Task.Factory.StartNew(() => PCReadAfterSleep("Read100MsAfterTick", newTenant, iWriteMillis + 100));
                Task.Factory.StartNew(() => PCReadAfterSleep("Read1sAfterTick", newTenant, iWriteMillis + 1000));
                Task.Factory.StartNew(() => PCReadAfterSleep("Read1.5sAfterTick", newTenant, iWriteMillis + 1500));

                Thread.Sleep(2000);

                Assert.IsTrue(_PCReadResult["Read1.5sAfterTick"]);
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }
    }
}