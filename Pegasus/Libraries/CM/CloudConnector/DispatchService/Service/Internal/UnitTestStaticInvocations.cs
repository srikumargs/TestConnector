using System;
using System.Threading;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Logging;
using Sage.Connector.Queues;

namespace Sage.Connector.DispatchService
{
    /// <summary>
    /// Expose internal static methods for public unit testing
    /// </summary>
    public static class UnitTestStaticInvocations
    {
        /// <summary>
        /// Finds IBindable interface for a given messageType
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public static IBindable FindIBindable(String messageType)
        {
            return Internal.BindableCatalog.FindIBindable(messageType, null);
        }

        /// <summary>
        /// Exercises the invoker
        /// </summary>
        /// <param name="bindable"></param>
        /// <param name="requestWrapper"></param>
        public static void TestInvoke(IBindable bindable, RequestWrapper requestWrapper)
        {
            Internal.BinderQueueElement bqe = new Internal.BinderQueueElement(
                Guid.NewGuid().ToString(), // binder id
                Guid.NewGuid().ToString(), // dispatch id
                bindable,
                requestWrapper);
            string sTenantId = Guid.NewGuid().ToString();

            using (var cts = new CancellationTokenSource())
            {
                using (var bq = new Internal.BinderQueue(sTenantId))
                {
                    bq.Enqueue(bqe);
                    Internal.BinderInvoker.InvokeBinder(bqe, bq, cts);
                }
            }
        }

        /// <summary>
        /// Locator (tests whether an element is added to the binder queue)
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="messageId"></param>
        /// <param name="queueManager"></param>
        /// <param name="logManager"></param>
        public static bool Locator(string tenantId, string messageId, QueueManager queueManager, LogManager logManager)
        {
            using (var bq = new Internal.BinderQueue(tenantId))
            {
                int oldBinderCount = bq.Count;
                Internal.BinderLocator.QueueMessageForBinding(tenantId, messageId, 1000, queueManager, bq, logManager);
                return bq.Count > oldBinderCount;
            }
        }
    }
}
