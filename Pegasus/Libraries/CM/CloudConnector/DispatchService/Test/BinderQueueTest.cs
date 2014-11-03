using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.DispatchService.Internal;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.Queues;
using Sage.Connector.TestUtilities;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;

namespace Sage.Connector.DispatchService.Test
{
    [TestClass]
    public class BinderQueueTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// Information about and functionality for the current test run.
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


        #region Tests
        
        /// <summary>
        /// Test the fundamentals: enqueue, dequeue, delete and restore
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), TestMethod]
        public void TestBinderQueueingSingleTenant()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            // Create the binder queue
            string tenantId = Guid.NewGuid().ToString();
            BinderQueue binderQueue = new BinderQueue(tenantId);
            Assert.AreEqual(0, binderQueue.Count, "Initial binder queue count should be zero");

            // Enqueue and Dequeue elements
            const int numElements = 15;
            List<BinderQueueElement> enqueuedList;
            List<BinderQueueElement> dequeuedList;
            PerformEnqueueAndDequeue(numElements, numElements, binderQueue, out enqueuedList, out dequeuedList);

            // Check the enqueue and dequeue counts
            Assert.AreEqual(0, binderQueue.Count, "All binder queue elements should have been dequeued");
            Assert.AreEqual(enqueuedList.Count, dequeuedList.Count, "Enqueued and Dequeued lists should have the same number of elements");
            Assert.AreEqual(dequeuedList.Count, binderQueue.InProcessCount, "Dequeued list count and in process count should be the same");

            // Make sure the lists are in the same order
            for (int index = 0; index < enqueuedList.Count; ++index)
            {
                Assert.AreEqual(enqueuedList[index].Identifier, dequeuedList[index].Identifier,
                    String.Format("Element {0} mismatch: Enqueued and Dequeued lists should be in the same order", index.ToString()));
            }

            // Test both a successfull and an unsuccessful retrieve
            // Use a valid ID
            BinderQueueElement retrievedElement = binderQueue.Retrieve(dequeuedList.First().Identifier);
            Assert.IsNotNull(retrievedElement, "Retrieved element with valid Id should not be null");
            Assert.AreEqual(retrievedElement.Identifier, dequeuedList.First().Identifier,
                "Retrieved an element with the wrong identifier");

            // Use an invalid ID
            retrievedElement = binderQueue.Retrieve(Guid.NewGuid().ToString());
            Assert.IsNull(retrievedElement, "Retrieve element with an invalid Id should have been null");

            // Delete half of the in process elements
            const int numElementsToDelete = numElements / 2;
            for (int index = 0; index < numElementsToDelete; ++index)
            {
                binderQueue.Delete(dequeuedList[index].Identifier);
            }
            Assert.AreEqual(numElements - numElementsToDelete, binderQueue.InProcessCount,
                "In process cound should have gone down by the number of elements we deleted");

            // Restore the rest of the elements
            for (int index = numElementsToDelete; index < numElements; ++index)
            {
                binderQueue.Restore(dequeuedList[index].Identifier);
            }
            Assert.AreEqual(0, binderQueue.InProcessCount,
                "In process count should be zero after the remaining elements were restored");
            Assert.AreEqual(numElements - numElementsToDelete, binderQueue.Count,
                "The binder queue count should have gone up by the number of elements that were restored");
        }

        /// <summary>
        /// Test that deleting and restoring binder elements causes notifications
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), TestMethod]
        public void TestBinderQueueNotifications()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {

                // Setup for subscriptions
                Subscriber subscriber = new Subscriber();
                NotificationCallbackInstanceHelper callbackInstance = new NotificationCallbackInstanceHelper();
                NotificationSubscriptionServiceProxy subscriptionServiceProxy = null;

                try
                {
                    // Setup the subscription service
                    subscriptionServiceProxy = NotificationSubscriptionServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber, callbackInstance);
                    Assert.IsNotNull(subscriptionServiceProxy, "Failed to retrieve notification subscription proxy.");

                    // Setup the callbacks for the binder delete/restore/enqueue notifications
                    subscriptionServiceProxy.Connect();
                    callbackInstance.SubscribeBinderElementDeleted(subscriptionServiceProxy, subscriber.BinderElementDeleted);
                    callbackInstance.SubscribeBinderElementRestored(subscriptionServiceProxy, subscriber.BinderElementRestored);
                    callbackInstance.SubscribeBinderElementEnqueued(subscriptionServiceProxy, subscriber.BinderElementEnqueued);

                    // Create the binder queue
                    string tenantId = Guid.NewGuid().ToString();
                    BinderQueue binderQueue = new BinderQueue(tenantId);
                    Assert.AreEqual(0, binderQueue.Count, "Initial binder queue count should be zero");

                    // Enqueue and Dequeue elements
                    const int numElements = 15;
                    List<BinderQueueElement> enqueuedList;
                    List<BinderQueueElement> dequeuedList;
                    PerformEnqueueAndDequeue(numElements, numElements, binderQueue, out enqueuedList, out dequeuedList);

                    // Delete half of the in process elements
                    const int numElementsToDelete = numElements / 2;
                    for (int index = 0; index < numElementsToDelete; ++index)
                    {
                        binderQueue.Delete(dequeuedList[index].Identifier);
                    }

                    // Restore the rest of the elements
                    for (int index = numElementsToDelete; index < numElements; ++index)
                    {
                        binderQueue.Restore(dequeuedList[index].Identifier);
                    }

                    // Check that the callbacks were hit the expected number of times
                    System.Threading.Thread.Sleep(60 * 1000);
                    // Delete is hit for both delete and restore.
                    Assert.AreEqual(numElements, subscriber.BinderElementDeletedOccurrences.Count,
                        "Callback for binder element deleted notification not hit the expected number of times");
                    Assert.AreEqual(numElements - numElementsToDelete, subscriber.BinderElementRestoredOccurrences.Count,
                        "Callback for binder element restored notification not hit the expected number of times");
                    Assert.AreEqual(numElements, subscriber.BinderElementEnqueuedOccurrences.Count,
                        "Callback for binder element enqueued notification not hit the expected number of times");

                    // Cleanup
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), TestMethod]
        public void TestBinderQueueTimedCleanupProcess()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            // Setup
            const int cleanupProcessInterval = 1000;
            TimeSpan cleanupThreshold = TimeSpan.FromSeconds(1);
            const int waitTimeForCleanupProcessToRun = cleanupProcessInterval * 5;

            // Create the binder queue with overridden cleanup process values
            string tenantId = Guid.NewGuid().ToString();
            BinderQueue binderQueue = new BinderQueue(cleanupProcessInterval, cleanupThreshold, tenantId);
            Assert.AreEqual(0, binderQueue.Count, "Initial binder queue count should be zero");

            // Enqueue and Dequeue elements
            const int numElements = 15;
            List<BinderQueueElement> enqueuedList;
            List<BinderQueueElement> dequeuedList;
            PerformEnqueueAndDequeue(numElements, numElements, binderQueue, out enqueuedList, out dequeuedList);

            // Wait for the cleanup process to run, then check
            System.Threading.Thread.Sleep(waitTimeForCleanupProcessToRun);
            // NOTE: Requires notifications to delete the element from the binder queue
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), TestMethod]
        public void TestBinderQueueRestoreToDispatch()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            // Create the binder queue
            string tenantId = TestUtils.CannedTenantIds[0].ToString();
            BinderQueue binderQueue = new BinderQueue(tenantId);
            Assert.AreEqual(0, binderQueue.Count, "Initial binder queue count should be zero");

            // Enqueue and Dequeue some of the elements
            // We want elements both in the binder queue and in process list for this test
            const int numElements = 15;
            const int numElementsToDequeue = numElements / 2;
            List<BinderQueueElement> enqueuedList;
            List<BinderQueueElement> dequeuedList;

            using (var qm = new QueueManager())
            {
                PerformEnqueueAndDequeue(numElements, numElementsToDequeue, binderQueue, qm, out enqueuedList, out dequeuedList);

                // Check the enqueue and dequeue counts
                Assert.AreEqual(numElementsToDequeue, binderQueue.InProcessCount,
                    "In process element count should match the number of elements dequeued");
                Assert.AreEqual(numElements - numElementsToDequeue, binderQueue.Count,
                    "Binder element count should match the number of original elements minus the number dequeued");

                // Check that the dispatch queue is initially empty
                StorageQueueMessage isqm = qm.GetMessageFromInput(tenantId);
                Assert.IsNull(isqm, "Queue was not initially empty");

                // Restore ALL binder queue elements
                binderQueue.RestoreAllElementsToTenantDispatchQueue();

                // Check the binder queue
                Assert.AreEqual(0, binderQueue.InProcessCount, "In process element list should be empty");
                Assert.AreEqual(0, binderQueue.Count, "Binder queue should be empty");

                // Prep the original request Ids to check against
                List<Guid> originalRequestIds = new List<Guid>();
                enqueuedList.ForEach(enqueuedElement => originalRequestIds.Add(enqueuedElement.RequestWrap.ActivityTrackingContext.RequestId));

                // Check the dispatch queue
                for (int count = 0; count < numElements; ++count)
                {
                    // Get the message we expect was restored
                    isqm = qm.GetMessageFromInput(tenantId);
                    Assert.IsNotNull(isqm, "Did not find the expected number of elements restored to dispatch queue");
                    
                    // Inspect the actual request wrapper
                    RequestWrapper requestWrapper = Utils.JsonDeserialize<RequestWrapper>(isqm.Payload);
                    Assert.AreEqual(requestWrapper.ActivityTrackingContext.TenantId, tenantId,
                        "Restored message is not for the correct tenant");
                    Assert.IsTrue(originalRequestIds.Contains(requestWrapper.ActivityTrackingContext.RequestId),
                        "Could not find the restored request Id in the list of original request Ids");
                }
            }
        }

        /// <summary>
        /// For ActionConstruction
        /// </summary>
        public void DoNothing()
        {
        }


        /// <summary>
        /// Excercises cancel token setting
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), TestMethod]
        public void TestCancelTokenSetting()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            // Create the binder queue
            string tenantId = TestUtils.CannedTenantIds[0].ToString();
            BinderQueue binderQueue = new BinderQueue(tenantId);
            List<BinderQueueElement> enqueuedList;
            List<BinderQueueElement> dequeuedList;

            PerformEnqueueAndDequeue(1, 1, binderQueue, out enqueuedList, out dequeuedList);

            using (var cts = new CancellationTokenSource())
            {
                binderQueue.SetElementCancelTokenSource(dequeuedList[0].Identifier, cts);
                BinderQueueElement testBQE = binderQueue.Retrieve(dequeuedList[0].Identifier);
                Assert.IsNotNull(testBQE.CancelTokenSource);
                binderQueue.SetElementCancelTokenSource(dequeuedList[0].Identifier, null);
                Assert.IsNull(testBQE.CancelTokenSource);
            }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Enqueue and Dequeue the specified number of elements for the binder queue
        /// </summary>
        /// <param name="numElements"></param>
        /// <param name="numElementsToDequeue"></param>
        /// <param name="binderQueue"></param>
        /// <param name="enqueuedList"></param>
        /// <param name="dequeuedList"></param>
        private void PerformEnqueueAndDequeue(
            int numElements, 
            int numElementsToDequeue,
            BinderQueue binderQueue,
            out List<BinderQueueElement> enqueuedList,
            out List<BinderQueueElement> dequeuedList)
        {
            // Create a list of elements
            enqueuedList = CreateBinderQueueElements(binderQueue.TenantId, numElements);

            // Enqueue all the messages
            enqueuedList.ForEach(element => binderQueue.Enqueue(element));
            Assert.AreEqual(numElements, binderQueue.Count, "Binder queue does not have the expected number of elements");

            // Dequeue all the elements into a list
            dequeuedList = new List<BinderQueueElement>();
            for (int count = 0; count < numElementsToDequeue; ++count)
            {
                BinderQueueElement dequeuedElement = binderQueue.Dequeue();
                if (dequeuedElement == null) { break; }
                dequeuedList.Add(dequeuedElement);
            }
        }

        /// <summary>
        /// Enqueue and Dequeue the specified number of elements for the binder queue
        /// </summary>
        /// <param name="numElements"></param>
        /// <param name="numElementsToDequeue"></param>
        /// <param name="binderQueue"></param>
        /// <param name="queueManager"></param>
        /// <param name="enqueuedList"></param>
        /// <param name="dequeuedList"></param>
        private void PerformEnqueueAndDequeue(
            int numElements,
            int numElementsToDequeue,
            BinderQueue binderQueue,
            QueueManager queueManager,
            out List<BinderQueueElement> enqueuedList,
            out List<BinderQueueElement> dequeuedList)
        {
            // Create a list of elements
            List<BinderQueueElement> creationList = CreateBinderQueueElements(binderQueue.TenantId, numElements);
            List<BinderQueueElement> queuedList = new List<BinderQueueElement>();

            // First enqueue and dequeue them into the official inbox queue
            Action<BinderQueueElement> enqueue = new Action<BinderQueueElement>((bqe) =>
            {
                string sInputElement = Utils.JsonSerialize(bqe.RequestWrap);
                queueManager.AddMessageToInput(sInputElement, bqe.RequestWrap.ActivityTrackingContext.RequestType, QueueContext.FakeTenantInstance(bqe.RequestWrap.ActivityTrackingContext.TenantId));
                StorageQueueMessage sqm = queueManager.GetMessageFromInput(bqe.RequestWrap.ActivityTrackingContext.TenantId);
                queuedList.Add(new BinderQueueElement(bqe.Identifier, sqm.Id, bqe.Bindable, bqe.RequestWrap));
            });
            creationList.ForEach(element => enqueue(element));
            enqueuedList = queuedList;

            // Enqueue all the messages
            enqueuedList.ForEach(element => binderQueue.Enqueue(element));
            Assert.AreEqual(numElements, binderQueue.Count, "Binder queue does not have the expected number of elements");

            // Dequeue all the elements into a list
            dequeuedList = new List<BinderQueueElement>();
            for (int count = 0; count < numElementsToDequeue; ++count)
            {
                BinderQueueElement dequeuedElement = binderQueue.Dequeue();
                if (dequeuedElement == null) { break; }
                dequeuedList.Add(dequeuedElement);
            }
        }

        /// <summary>
        /// Create the specified number of binder queue elements for the specified tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="numElements"></param>
        /// <returns></returns>
        private List<BinderQueueElement> CreateBinderQueueElements(string tenantId, int numElements)
        {
            List<BinderQueueElement> binderQueueElementList = new List<BinderQueueElement>();

            if (numElements > 0)
            {
                for (int index = 0; index < numElements; ++index)
                {

                    // Create the request
                    Request request = new LoopBackRequest(Guid.NewGuid(), DateTime.UtcNow, 0, 0, "requestingUser");
                    RequestWrapper requestWrapper = new RequestWrapper(
                        new ActivityTrackingContext(Guid.NewGuid(), tenantId, request.Id, request.GetType().FullName),
                        Utils.JsonSerialize(request));

                    // Create the binder queue element
                    BinderQueueElement element = new BinderQueueElement(
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString(),
                        FindIBindable("LoopBack"),
                        requestWrapper);

                    // Add it to the list
                    binderQueueElementList.Add(element);
                }
            }

            return binderQueueElementList;
        }

        /// <summary>
        /// Find the IBindable for the message type provided
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        private IBindable FindIBindable(String messageType)
        {
            return Internal.BindableCatalog.FindIBindable(messageType, null);
        }

        /// <summary>
        /// Class to provide handlers for subscription testing 
        /// </summary>
        private sealed class Subscriber
        {
            public Subscriber()
            {
                BinderElementDeletedOccurrences = new List<Tuple<String,String>>();
                BinderElementRestoredOccurrences = new List<Tuple<String, String>>();
                BinderElementEnqueuedOccurrences = new List<Tuple<String, String>>();
            }

            public void Clear()
            {
                BinderElementDeletedOccurrences.Clear();
                BinderElementRestoredOccurrences.Clear();
                BinderElementEnqueuedOccurrences.Clear();
            }

            #region Callback Methods
            
            public void BinderElementDeleted(String tenantId, String elementId)
            { BinderElementDeletedOccurrences.Add(new Tuple<String,String>(tenantId, elementId)); }

            public void BinderElementRestored(String tenantId, String elementId)
            { BinderElementRestoredOccurrences.Add(new Tuple<String,String>(tenantId, elementId)); }

            public void BinderElementEnqueued(String tenantId, String elementId)
            { BinderElementEnqueuedOccurrences.Add(new Tuple<String, String>(tenantId, elementId)); }

            #endregion


            #region Properties

            public List<Tuple<String, String>> BinderElementDeletedOccurrences
            { get; set; }

            public List<Tuple<String, String>> BinderElementRestoredOccurrences
            { get; set; }

            public List<Tuple<String, String>> BinderElementEnqueuedOccurrences
            { get; set; }

            #endregion
        }

        #endregion
    }
}
