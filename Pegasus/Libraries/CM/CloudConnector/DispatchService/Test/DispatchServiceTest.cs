using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.DispatchService.Internal;
using Sage.Connector.DispatchService.Proxy;
using Sage.Connector.Logging;
using Sage.Connector.Queues;
using Sage.Connector.TestUtilities;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using CloudInterfaces = Sage.Connector.Cloud.Integration.Interfaces;

namespace Sage.Connector.DispatchService.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DispatchServiceTest
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

        /// <summary>
        /// TestBinderCatalogLoopBackDiscovery
        /// </summary>
        [TestMethod]
        public void TestBinderCatalogLoopBackDiscovery()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            TestLoopBackRetrieval();
        }

        /// <summary>
        /// TestBadBinderCatalogLookup
        /// </summary>
        [TestMethod]
        public void TestBadBinderCatalogLookup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            string sLoopBackMessageKind = "I AM NOT SUPPORTED";
            IBindable ib = UnitTestStaticInvocations.FindIBindable(sLoopBackMessageKind);
            Assert.IsNull(ib);
        }

        private static void TestLoopBackRetrieval()
        {
            string sLoopBackMessageKind = "Sage.Connector.Cloud.Integration.Interfaces.Requests.LoopBackRequest";
            IBindable ib = UnitTestStaticInvocations.FindIBindable(sLoopBackMessageKind);
            Assert.IsNotNull(ib);
            Assert.IsTrue(ib.SupportRequestKind(sLoopBackMessageKind));
        }

        /// <summary>
        /// TestThreadedBinderCatalogLoopBackDiscovery
        /// </summary>
        [TestMethod]
        public void TestThreadedBinderCatalogLoopBackDiscovery()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            int numThreads = 5;
            Task[] tasks = new Task[numThreads];

            for (int index = 0; index < numThreads; index++)
            {
                tasks[index] = Task.Factory.StartNew(() => TestLoopBackRetrieval());
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// A simple tester binder for testing invocations
        /// </summary>
        private class TestBinder : IBindable
        {
            public Boolean _invoked = false;
            public bool SupportRequestKind(string requestKind)
            {
                return true;
            }
            public ResponseWrapper InvokeWork(RequestWrapper requestWrapper, CancellationTokenSource cancellationTokenSource, long maxBlobSize)
            {
                _invoked = true;
                return new ResponseWrapper(
                    new ActivityTrackingContext(Guid.NewGuid(), String.Empty, Guid.Empty, String.Empty),
                    string.Empty,
                    Guid.NewGuid(),
                    String.Empty,
                    String.Empty,
                    null);
            }
        }

        /// <summary>
        /// TestBinderInvocation
        /// </summary>
        [TestMethod]
        public void TestBinderInvocation()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            TestBinder tb = new TestBinder();
            String tenantId = Guid.NewGuid().ToString();
            Guid requestId = Guid.NewGuid();
            RequestWrapper rw = new RequestWrapper(
                new ActivityTrackingContext(Guid.NewGuid(), tenantId, requestId, typeof (Request).FullName),
                Utils.JsonSerialize(new CloudInterfaces.Requests.Request(
                    requestId,
                    DateTime.UtcNow,
                    0, 0, "requestingUser", "projectName", "requestSummary")));

            Assert.IsFalse(tb._invoked);
            UnitTestStaticInvocations.TestInvoke(tb, rw);
            Thread.Sleep(1000);
            Assert.IsTrue(tb._invoked);
        }

        /// <summary>
        /// TestBinderLocator
        /// </summary>
        [TestMethod]
        public void TestBinderLocator()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            using (var qm = new QueueManager())
            {
                string tenantId = TestUtils.CannedTenantIds[0].ToString();

                // Create the request
                Request request = new LoopBackRequest(Guid.NewGuid(), DateTime.UtcNow, 0, 0, "requestingUser");
                RequestWrapper requestWrapper = new RequestWrapper(
                    new ActivityTrackingContext(Guid.NewGuid(), tenantId, request.Id, request.GetType().FullName),
                    Utils.JsonSerialize(request));
                String sInputMessage = Utils.JsonSerialize(requestWrapper);
                qm.AddMessageToInput(sInputMessage, request.GetType().FullName, QueueContext.FakeTenantInstance(tenantId));
                StorageQueueMessage sqm = qm.GetMessageFromInput(tenantId);
                string messageId = sqm.Id;
                qm.RestoreQueueMessage(messageId);
                using (var lm = new LogManager())
                {
                    Assert.IsTrue(UnitTestStaticInvocations.Locator(tenantId, messageId, qm, lm));
                }
            }
        }

        /// <summary>
        /// Invokes cancellation of a specific work
        /// </summary>
        [TestMethod]
        public void TestCancelWork()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                using (var dsp = DispatchServiceProxyFactory.CreateFromCatalog("localhost",
                                                                               ConnectorServiceUtils.
                                                                                   CatalogServicePortNumber))
                {
                    dsp.CancelWork(TestUtils.CannedTenantIds[0], "BLAH");
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }


        /// <summary>
        /// Retrieves list of in progress work
        /// </summary>
        [TestMethod]
        public void TestInProgressWorkList()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureRunning);
            try
            {
                using (var dsp = DispatchServiceProxyFactory.CreateFromCatalog("localhost",
                                                                               ConnectorServiceUtils.
                                                                                   CatalogServicePortNumber))
                {
                    dsp.InProgressWork(TestUtils.CannedTenantIds[0]);
                }
            }
            finally
            {
                TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
            }

        }

        private bool ElementFound(IEnumerable<RequestWrapper> requestWraps, Guid requestIdentifier)
        {
            foreach (var element in requestWraps)
            {
                if (element.ActivityTrackingContext.RequestId == requestIdentifier)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Excercises in progress list
        /// </summary>
        [TestMethod]
        public void TestBinderQueueInProgressList()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            using (BinderQueue bq = new BinderQueue(TestUtils.CannedTenantIds[0]))
            {
                String binderQueueIdentifier = new Guid().ToString();
                String dispatchIdentifier = new Guid().ToString();
                Guid requestGuid = new Guid();
                Request request = new Request(requestGuid, DateTime.UtcNow, 0, 0, "UnitTest", "ProjectName", "RequestSummary");
                ActivityTrackingContext atc = new ActivityTrackingContext(Guid.NewGuid(), TestUtils.CannedTenantIds[0],
                                                                          requestGuid, request.GetType().FullName);
                RequestWrapper rw = new RequestWrapper(atc, Utils.JsonSerialize(request));
                BinderQueueElement bqe = new BinderQueueElement(binderQueueIdentifier, dispatchIdentifier, null, rw);

                bq.Enqueue(bqe);
                Assert.IsTrue(ElementFound(bq.InProgressWork, requestGuid));

                bq.Dequeue();
                Assert.IsTrue(ElementFound(bq.InProgressWork, requestGuid));

                bq.Delete(binderQueueIdentifier);
                Assert.IsFalse(ElementFound(bq.InProgressWork, requestGuid));
            }
        }

        /// <summary>
        /// Excercises queue retrieval
        /// </summary>
        [TestMethod]
        public void TestBinderQueueRetrieve()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            using (BinderQueue bq = new BinderQueue(TestUtils.CannedTenantIds[0])) 
            {
                String binderQueueIdentifier = new Guid().ToString();
                String dispatchIdentifier = new Guid().ToString();
                Guid requestGuid = new Guid();
                Request request = new Request(requestGuid, DateTime.UtcNow, 0, 0, "UnitTest", "ProjectName", "RequestSummary");
                ActivityTrackingContext atc = new ActivityTrackingContext(Guid.NewGuid(), TestUtils.CannedTenantIds[0],
                                                                            requestGuid, request.GetType().FullName);
                RequestWrapper rw = new RequestWrapper(atc, Utils.JsonSerialize(request));
                BinderQueueElement bqe = new BinderQueueElement(binderQueueIdentifier, dispatchIdentifier, null, rw);

                bq.Enqueue(bqe);
                Assert.IsNotNull(bq.Retrieve(binderQueueIdentifier));

                bq.Dequeue();
                Assert.IsNotNull(bq.Retrieve(binderQueueIdentifier));

                bq.Delete(binderQueueIdentifier);
                Assert.IsNull(bq.Retrieve(binderQueueIdentifier));
            }
        }

        /// <summary>
        /// Excercises queue deletion
        /// </summary>
        [TestMethod]
        public void TestBinderQueueDeletion()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.ResetAndEnsureStopped);

            using (BinderQueue bq = new BinderQueue(TestUtils.CannedTenantIds[0]))
            {
                String binderQueueIdentifier = new Guid().ToString();
                String dispatchIdentifier = new Guid().ToString();
                Guid requestGuid = new Guid();
                Request request = new Request(requestGuid, DateTime.UtcNow, 0, 0, "UnitTest", "ProjectName", "RequestSummary");
                ActivityTrackingContext atc = new ActivityTrackingContext(Guid.NewGuid(), TestUtils.CannedTenantIds[0],
                                                                          requestGuid, request.GetType().FullName);
                RequestWrapper rw = new RequestWrapper(atc, Utils.JsonSerialize(request));
                BinderQueueElement bqe = new BinderQueueElement(binderQueueIdentifier, dispatchIdentifier, null, rw);

                bq.Enqueue(bqe);
                Assert.IsNotNull(bq.Retrieve(binderQueueIdentifier));
                bq.Delete(binderQueueIdentifier);
                Assert.IsNull(bq.Retrieve(binderQueueIdentifier));

                bq.Enqueue(bqe);
                bq.Dequeue();
                Assert.IsNotNull(bq.Retrieve(binderQueueIdentifier));

                bq.Delete(binderQueueIdentifier);
                Assert.IsNull(bq.Retrieve(binderQueueIdentifier));
            }
        }
    }
}
