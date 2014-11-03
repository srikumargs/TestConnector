using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Queues;
using Sage.Connector.TestUtilities;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using System.Threading.Tasks;

namespace Sage.Connector.Queues.Test
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
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx);
        }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Basic test of enqueue and dequeue retrieves equivalent contents
        /// </summary>
        [TestMethod]
        public void TestBasicStorageQueueTests()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            StorageQueueFactory factory = new StorageQueueFactory();
            StorageQueue isq = factory.GetQueue("UNITTEST");
            StorageQueueMessage sqm = new StorageQueueMessage("UnitTest", "UnitTestPayloadType", "UnitTestPayload");

            isq.Enqueue(sqm, QueueContext.FakeInstance);

            StorageQueueMessage retVal = isq.Dequeue();

            Assert.IsFalse(string.IsNullOrEmpty(retVal.Id));
            Assert.IsTrue(retVal.Payload.Equals("UnitTestPayload"));
        }

        /// <summary>
        /// Tests that enqueue of string autogenerates necessary queue id
        /// </summary>
        [TestMethod]
        public void TestAutoGenId()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            StorageQueueFactory factory = new StorageQueueFactory();
            StorageQueue isq = factory.GetQueue("UNITTEST");

            isq.Enqueue("UnitTestPayload", "UnitTestPayloadType", QueueContext.FakeInstance);

            StorageQueueMessage retVal = isq.Dequeue();

            Assert.IsFalse(string.IsNullOrEmpty(retVal.Id));
            Assert.IsTrue(retVal.Payload.Equals("UnitTestPayload"));
        }

        /// <summary>
        /// Tests that enqueue of message keeps supplied queue id
        /// </summary>
        [TestMethod]
        public void TestPreserveId()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            StorageQueueFactory factory = new StorageQueueFactory();
            StorageQueue isq = factory.GetQueue("UNITTEST");
            Guid myGuid = Guid.NewGuid();
            StorageQueueMessage sqm = new StorageQueueMessage(
                myGuid.ToString(),
                "UnitTestPayloadType",
                "UnitTestPayload");

            isq.Enqueue(sqm, QueueContext.FakeInstance);

            StorageQueueMessage retVal = isq.Dequeue();

            Assert.IsTrue(myGuid.ToString().Equals(retVal.Id));
            Assert.IsTrue(retVal.Payload.Equals("UnitTestPayload"));
        }

        /// <summary>
        /// Tests retrieveal of specific queue entry
        /// </summary>
        [TestMethod]
        public void TestRetrieveById()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            StorageQueueFactory factory = new StorageQueueFactory();
            StorageQueue isq = factory.GetQueue("UNITTEST") as StorageQueue;
            Guid myGuid = Guid.NewGuid();
            StorageQueueMessage sqm = new StorageQueueMessage(
                myGuid.ToString(),
                "UnitTestPayloadType",
                "UnitTestPayload");

            int beforeCount = isq.Count;
            isq.Enqueue(sqm, QueueContext.FakeInstance);
            int afterCount = isq.Count;
            Assert.AreEqual(beforeCount + 1, afterCount, "Storage Queue count should have increased by one.");

            StorageQueueMessage retVal = isq.GetQueueElement(myGuid);

            Assert.IsTrue(myGuid.ToString().Equals(retVal.Id));
            Assert.IsTrue(retVal.Payload.Equals("UnitTestPayload"));
            beforeCount = afterCount;
            isq.Delete(sqm);
            afterCount = isq.Count;
            Assert.AreEqual(beforeCount - 1, afterCount, "Storage Queue count should have decreased by one.");
        }

        /// <summary>
        /// Tests type rename
        /// </summary>
        [TestMethod]
        public void TestRenameQueueType()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            StorageQueueFactory factory = new StorageQueueFactory();
            StorageQueue isq = factory.GetQueue("UNITTEST") as StorageQueue;
            Guid myGuid = Guid.NewGuid();
            StorageQueueMessage sqm = new StorageQueueMessage(
                myGuid.ToString(),
                "UnitTestPayloadType",
                "UnitTestPayload");

            isq.Enqueue(sqm, QueueContext.FakeInstance);
            isq.RenameQueueType("UNITTEST2");
            StorageQueueMessage retVal = isq.Dequeue();
            Assert.IsTrue((null == retVal) || (retVal.Id != myGuid.ToString()));

            isq = factory.GetQueue("UNITTEST2") as StorageQueue;
            retVal = isq.Dequeue();
            Assert.IsTrue((null != retVal) && (retVal.Id == myGuid.ToString()));
        }


        /// <summary>
        /// Tests that queues can store more than 4000 characters
        /// </summary>
        [TestMethod]
        public void TestLargePayload()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            StorageQueueFactory factory = new StorageQueueFactory();
            StorageQueue isq = factory.GetQueue("UNITTEST");

            // 2^10 * sPayload.Length = 15,360 characters
            string sPayload = "UnitTestPayload";
            for (int i = 0; i < 10; i++)
            {
                sPayload += sPayload;
            }
            isq.Enqueue(sPayload, "UnitTestPayloadType", QueueContext.FakeInstance);
            StorageQueueMessage retVal = isq.Dequeue();
            Assert.IsTrue(retVal.Payload.Equals(sPayload));
        }

        /// <summary>
        /// Tests that queue can dehydrate and hydrate a ReportDescriptorListRequestResponse
        /// </summary>
        [TestMethod]
        public void TestEnqueueReportDescriptorListRequestResponse()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            StorageQueueFactory factory = new StorageQueueFactory();
            StorageQueue isq = factory.GetQueue("UNITTEST");

            var parameter = new
                CloudContracts.StringReportParam(
                    "name",
                    "",
                    false,
                     "premiseMetadata",
                    "",
                    10);

            var report = new
                CloudContracts.ReportDescriptor(
                    "premiseUniqueIdentity",
                    "name",
                    "description",
                    "category",
                    "applicationName",
                    "menuPath",
                    "path",
                    new CloudContracts.ReportParam[] { parameter },
                    new CloudContracts.SystemFilterParam[] { }
                    );

            Response response = new
                LoopBackRequestResponse(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow);
            string serializedResponse = Utils.JsonSerialize(new ResponseWrapper(new ActivityTrackingContext(Guid.NewGuid(), String.Empty, Guid.Empty, String.Empty), string.Empty, response.Id, response.GetType().FullName, Utils.JsonSerialize(response), null));

            isq.Enqueue(serializedResponse, response.GetType().FullName, QueueContext.FakeInstance);
            StorageQueueMessage retVal = isq.Dequeue();
            isq.Delete(retVal);

            Assert.IsFalse(string.IsNullOrEmpty(retVal.Payload));
            Assert.IsTrue(retVal.Payload.Equals(serializedResponse));

            var responsePayload = Utils.JsonDeserialize<ResponseWrapper>(retVal.Payload).ResponsePayload;
            var deserializedResponse = Utils.JsonDeserialize<LoopBackRequestResponse>(responsePayload);

            Assert.IsNotNull(deserializedResponse);
            Assert.AreEqual(response.Id, deserializedResponse.Id);
        }

        [TestMethod]
        public void TestAddMessageToInbox()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            String tenant = TestUtils.CannedTenantIds[0];
            Guid msgId = Guid.NewGuid();

            var request = new LoopBackRequest(msgId, DateTime.UtcNow, 0, 0, "");
            string sRequest = Utils.JsonSerialize(new RequestWrapper(new ActivityTrackingContext(Guid.NewGuid(), tenant.ToString(), request.Id, request.GetType().FullName), Utils.JsonSerialize(request)));

            //Test empty inbox
            using (var qm = new QueueManager())
            {
                StorageQueueMessage getOnEmpty = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNull(getOnEmpty, "GetMessageFromInput should have returned null.");
            }

            //Test adding and pulling from inbox
            using (var qm = new QueueManager())
            {
                qm.AddMessageToInput(sRequest, request.GetType().FullName, QueueContext.FakeTenantInstance(tenant.ToString()));
            }

            using (var qm = new QueueManager())
            {
                StorageQueueMessage isqm = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNotNull(isqm);
                string pulledMsg = isqm.Payload;
                RequestWrapper msgOffInbox = Utils.JsonDeserialize<RequestWrapper>(pulledMsg);
                Assert.AreEqual(tenant.ToString(), msgOffInbox.ActivityTrackingContext.TenantId, "Tenant id off inbox msg does not match original.");
                Assert.AreEqual(msgId, msgOffInbox.ActivityTrackingContext.RequestId, "Msg id off inbox msg does not match original.");
            }
        }

        /// <summary>
        /// Adds a message to the premise-cloud outbox
        /// </summary>
        [TestMethod]
        public void TestAddMessageToOutbox()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            var response = new
                LoopBackRequestResponse(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow);

            string sResponse =
                Utils.JsonSerialize(
                    new ResponseWrapper(
                        new ActivityTrackingContext(Guid.NewGuid(), String.Empty, Guid.Empty, String.Empty),
                        string.Empty,
                        response.Id,
                        response.GetType().FullName,
                        Utils.JsonSerialize(response),
                        null));

            using (var qm = new QueueManager())
            {
                qm.AddMessageToOutput(sResponse, response.GetType().FullName, QueueContext.FakeInstance);
            }
        }

        /// <summary>
        /// Excercies batch deletions
        /// </summary>
        [TestMethod]
        public void TestPeekMessageFromOutbox()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            String tenantId = TestUtils.CannedTenantIds[0].ToString();
            using (var qm = new QueueManager())
            {
                qm.AddMessageToOutput("OUTPUT1", "OUTPUTTYPE", QueueContext.FakeTenantInstance(tenantId));
                qm.AddMessageToOutput("OUTPUT2", "OUTPUTTYPE", QueueContext.FakeTenantInstance(tenantId));
                qm.AddMessageToOutput("OUTPUT3", "OUTPUTTYPE", QueueContext.FakeTenantInstance(tenantId));

                string[] messages = qm.PeekMessagesFromOutput(1, tenantId);
                Assert.IsTrue(messages.Length == 1);
                Assert.IsTrue(messages[0] == "OUTPUT1");

                qm.RestorePeekMessagesToOutput(tenantId);

                messages = qm.PeekMessagesFromOutput(10, tenantId);
                Assert.IsTrue(messages.Length == 3);

                qm.RemoveLastPeekMessagesFromOutput(tenantId);
                qm.RestorePeekMessagesToOutput(tenantId);

                messages = qm.PeekMessagesFromOutput(10, tenantId);
                Assert.IsTrue(messages.Length == 0);
            }
        }

        /// <summary>
        /// Tests timeout hiding, re-availability, and deletion
        /// </summary>
        [TestMethod]
        public void TestBasicExpirationHiding()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            StorageQueueFactory factory = new StorageQueueFactory();
            StorageQueue isq = factory.GetQueue("UNITTEST");
            string sId = Guid.NewGuid().ToString();
            StorageQueueMessage sqm = new StorageQueueMessage(sId, "UnitTestPayloadType", "UnitTestPayload");

            // Lower the timeout to speed up the unit test
            int dBaseProcessingInterval = 1000;
            int dSleepTimeout = dBaseProcessingInterval + 500;
            isq.DequeueProcessingExpiration = dBaseProcessingInterval;

            isq.Enqueue(sqm, QueueContext.FakeInstance);
            StorageQueueMessage response = isq.Dequeue();
            Assert.IsNotNull(response);
            Assert.AreEqual(sId, response.Id);
            Assert.AreEqual("UnitTestPayload", response.Payload);

            // Response should now be hidden from further retrieval
            response = isq.Dequeue();
            Assert.IsNull(response);

            // Wait for the timeout
            System.Threading.Thread.Sleep(dSleepTimeout);

            // Response should now be re-available
            response = isq.Dequeue();
            Assert.IsNotNull(response);
            Assert.AreEqual(sId, response.Id);
            Assert.AreEqual("UnitTestPayload", response.Payload);
            
            // Delete the response
            isq.Delete(response);

            // Response should gone before timeout
            response = isq.Dequeue();
            Assert.IsNull(response);
            
            // Wait for the timeout
            System.Threading.Thread.Sleep(dSleepTimeout);

            // Response should be gone after timeout
            response = isq.Dequeue();
            Assert.IsNull(response);
        }

        /// <summary>
        /// Excercise official inbox message consumption (delete)
        /// message cancelation (restore)
        /// message expiration
        /// </summary>
        [TestMethod]
        public void TestMessageHiding()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            String tenant = TestUtils.CannedTenantIds[0];
            Guid msgId = Guid.NewGuid();

            var request = new LoopBackRequest(msgId, DateTime.UtcNow, 0, 0, "");
            string sRequest = Utils.JsonSerialize(new RequestWrapper(new ActivityTrackingContext(Guid.NewGuid(), tenant.ToString(), Guid.Empty, String.Empty), Utils.JsonSerialize(request)));

            // Lower the timeout to speed up the unit test
            int dBaseProcessingInterval = 1000;
            int dSleepTimeout = dBaseProcessingInterval + 500;

            //Test normal consumption
            using (var qm = new QueueManager())
            {
                // Initial empty
                StorageQueueMessage isqm = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNull(isqm, "Queue was not initially empty");
                // Initial load and retrieval
                qm.AddMessageToInput(sRequest, request.GetType().FullName, QueueContext.FakeTenantInstance(tenant.ToString()));
                isqm = qm.GetMessageFromInput(dBaseProcessingInterval, tenant.ToString());
                Assert.IsNotNull(isqm, "Fail to retrieve just injected message");
                // Consumption
                qm.RemoveSpecificMessage(isqm.Id);
                // Now empty
                isqm = qm.GetMessageFromInput(dBaseProcessingInterval, tenant.ToString());
                Assert.IsNull(isqm, "Message deletion failed");
                System.Threading.Thread.Sleep(dSleepTimeout);
                // Stays empty
                isqm = qm.GetMessageFromInput(dBaseProcessingInterval, tenant.ToString());
                Assert.IsNull(isqm, "Delayed message deletion failed");
            }

            //Test message cancelation
            using (var qm = new QueueManager())
            {
                // Initial empty
                StorageQueueMessage isqm = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNull(isqm, "Queue was not initially empty");
                // Initial load and retrieval
                qm.AddMessageToInput(sRequest, request.GetType().FullName, QueueContext.FakeTenantInstance(tenant.ToString()));
                isqm = qm.GetMessageFromInput(dBaseProcessingInterval, tenant.ToString());
                Assert.IsNotNull(isqm, "Fail to retrieve just injected message");
                // Cancellation
                qm.RestoreQueueMessage(isqm.Id);
                // Re-available
                isqm = qm.GetMessageFromInput(dBaseProcessingInterval, tenant.ToString());
                Assert.IsNotNull(isqm, "Message restoration failed");
                // Delete it
                qm.RemoveSpecificMessage(isqm.Id);
                System.Threading.Thread.Sleep(dSleepTimeout);
                // Stays empty
                isqm = qm.GetMessageFromInput(dBaseProcessingInterval, tenant.ToString());
                Assert.IsNull(isqm, "Delayed message deletion failed");
            }

            //Test message expiration
            using (var qm = new QueueManager())
            {
                // Initial empty
                StorageQueueMessage isqm = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNull(isqm, "Queue was not initially empty");
                // Initial load and retrieval
                qm.AddMessageToInput(sRequest, request.GetType().FullName, QueueContext.FakeTenantInstance(tenant.ToString()));
                isqm = qm.GetMessageFromInput(dBaseProcessingInterval, tenant.ToString());
                Assert.IsNotNull(isqm, "Fail to retrieve just injected message");
                // Message now hidden
                isqm = qm.GetMessageFromInput(dBaseProcessingInterval, tenant.ToString());
                Assert.IsNull(isqm, "Fail to hide retrieved message");
                // Wait for expiration
                System.Threading.Thread.Sleep(dSleepTimeout);
                // Re-available
                isqm = qm.GetMessageFromInput(dBaseProcessingInterval, tenant.ToString());
                Assert.IsNotNull(isqm, "Message restoration failed");
                // Delete it
                qm.RemoveSpecificMessage(isqm.Id);
                System.Threading.Thread.Sleep(dSleepTimeout);
                // Stays empty
                isqm = qm.GetMessageFromInput(dBaseProcessingInterval, tenant.ToString());
                Assert.IsNull(isqm, "Delayed message deletion failed");
            }
        }

        /// <summary>
        /// Retrieve a message, restore all messages, re-retrieve the message
        /// </summary>
        [TestMethod]
        public void TestMessageRestoration()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            String tenant = TestUtils.CannedTenantIds[0];
            Guid msgId = Guid.NewGuid();

            var request = new LoopBackRequest(msgId, DateTime.UtcNow, 0, 0, "");
            string sRequest = Utils.JsonSerialize(new RequestWrapper(new ActivityTrackingContext(Guid.NewGuid(), tenant.ToString(), request.Id, request.GetType().FullName), Utils.JsonSerialize(request)));

            using (var qm = new QueueManager())
            {
                // Initial empty
                StorageQueueMessage isqm = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNull(isqm, "Queue was not initially empty");
                // Initial load and retrieval
                qm.AddMessageToInput(sRequest, request.GetType().FullName, QueueContext.FakeTenantInstance(tenant.ToString()));
                isqm = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNotNull(isqm, "Fail to retrieve just injected message");
                // Now empty
                isqm = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNull(isqm, "Message deletion failed");
                // Restore all
                qm.RestoreAllProcessingMessages();
                // Now back
                isqm = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNotNull(isqm, "Delayed message deletion failed");
            }
        }

        /// <summary>
        /// Create, retrieve, restore, and delete a message worker
        /// </summary>
        private void CreateRetrieveRestoreDeleteMessage()
        {
            String tenant = TestUtils.CannedTenantIds[0];
            Guid msgId = Guid.NewGuid();

            var request = new LoopBackRequest(msgId, DateTime.UtcNow, 0, 0, "");
            string sRequest = Utils.JsonSerialize(new RequestWrapper(new ActivityTrackingContext(Guid.NewGuid(), tenant.ToString(), request.Id, request.GetType().FullName), Utils.JsonSerialize(request)));

            using (var qm = new QueueManager())
            {
                // Load and retrieve (may not be the same if concurrent execution!)
                qm.AddMessageToInput(sRequest, request.GetType().FullName, QueueContext.FakeTenantInstance(tenant.ToString()));
                StorageQueueMessage isqm = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNotNull(isqm, "Fail to retrieve after addition");
                
                // Restore and retrieve (may not be the same if concurrent execution!)
                qm.RestoreQueueMessage(isqm.Id);
                isqm = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNotNull(isqm, "Failed to retrieve after restoration");

                // Now delete
                bool bResult = qm.RemoveSpecificMessage(isqm.Id);
                Assert.IsTrue(bResult, "Failed to delete after retrieval");
            }
        }

        /// <summary>
        /// Retrieve a message, restore all messages, re-retrieve the message
        /// </summary>
        [TestMethod, Ignore]
        public void TestMassConcurrentQueueOperations()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            uint iConcurrentExecutions = 100;
            Task[] tasks = new Task[iConcurrentExecutions];
            while (iConcurrentExecutions-- > 0)
            {
                tasks[iConcurrentExecutions] = Task.Factory.StartNew(() => CreateRetrieveRestoreDeleteMessage());
            }
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Retrieve a message, restore all messages, re-retrieve the message
        /// </summary>
        [TestMethod]
        public void TestTenantDeletions()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            String tenant = TestUtils.CannedTenantIds[0];
            Guid msgId = Guid.NewGuid();

            using (var qm = new QueueManager())
            {
                // Initial empty - Input
                StorageQueueMessage isqm = qm.GetMessageFromInput(tenant);
                Assert.IsNull(isqm, "Queue was not initially empty");
                // Initial load and retrieval
                qm.AddMessageToInput("ABC", "Type", QueueContext.FakeTenantInstance(tenant));
                isqm = qm.GetMessageFromInput(tenant.ToString());
                Assert.IsNotNull(isqm, "Fail to retrieve just injected message");
                string sFirstInputId = isqm.Id;
                // Now empty from 'normal' retrieval
                isqm = qm.GetMessageFromInput(tenant);
                Assert.IsNull(isqm, "Message hiding failed");
                // But, still there
                var sFirstMessage = qm.GetSpecificMessage(sFirstInputId);
                Assert.IsFalse(null != sFirstMessage, "Message retrieval failed");
                // Inject another input message
                qm.AddMessageToInput("DEF", "Type", QueueContext.FakeTenantInstance(tenant));


                // Initial empty - output
                isqm = qm.GetMessageFromOutput(tenant);
                Assert.IsNull(isqm, "Queue was not initially empty");
                // Initial load and retrieval
                qm.AddMessageToOutput("GHI", "Type", QueueContext.FakeTenantInstance(tenant));
                isqm = qm.GetMessageFromOutput(tenant);
                Assert.IsNotNull(isqm, "Fail to retrieve just injected message");
                string sFirstOuputId = isqm.Id;
                // Now empty from 'normal' retrieval
                isqm = qm.GetMessageFromOutput(tenant);
                Assert.IsNull(isqm, "Message hiding failed");
                // But, still there
                sFirstMessage = qm.GetSpecificMessage(sFirstOuputId);
                Assert.IsFalse(null != sFirstMessage, "Message retrieval failed");
                // Inject another output message
                qm.AddMessageToInput("JKL", "Type", QueueContext.FakeTenantInstance(tenant));

                // Remove all messages for the tenant
                qm.DeleteTenantMessages(tenant);
                // Normal retrieval is nothing
                isqm = qm.GetMessageFromInput(tenant);
                Assert.IsNull(isqm, "Input queue was not cleared");
                isqm = qm.GetMessageFromOutput(tenant);
                Assert.IsNull(isqm, "Output queue was not cleared");
                // Specific retrieval is also nothing
                sFirstMessage = qm.GetSpecificMessage(sFirstInputId);
                Assert.IsTrue(null != sFirstMessage, "Input queue was not cleared");
                sFirstMessage = qm.GetSpecificMessage(sFirstOuputId);
                Assert.IsTrue(null != sFirstMessage, "Output queue was not cleared");
            }
        }
    }
}
