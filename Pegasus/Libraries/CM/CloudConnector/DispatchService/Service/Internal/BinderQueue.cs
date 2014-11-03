using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.Queues;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Diagnostics;
using Convert = System.Convert;

namespace Sage.Connector.DispatchService.Internal
{
    /// <summary>
    /// An in-memory queue to manage a tenant's collection of in-progress work
    /// </summary>
    internal sealed class BinderQueue : IDisposable
    {
        /// <summary>
        /// Class to store information about and instance of in process elements
        /// </summary>
        private class InProcessElement
        {
            /// <summary>
            /// Do not allow default construction
            /// </summary>
            private InProcessElement() {}

            /// <summary>
            /// Constructor taking the dequeued element
            /// </summary>
            /// <param name="element"></param>
            public InProcessElement(BinderQueueElement element)
            {
                ArgumentValidator.ValidateNonNullReference(element, "element", _myTypeName + ".ctor()");

                Element = element;
                DequeuedTimestamp = DateTime.UtcNow;
            }

            /// <summary>
            /// The time stamp for when this element was dequeued
            /// </summary>
            public DateTime DequeuedTimestamp { get; private set; }

            /// <summary>
            /// The element that was dequeued
            /// </summary>
            public BinderQueueElement Element { get; private set; }
        }

        /// <summary>
        /// Do not allow construction without the tenant identifier
        /// </summary>
        private BinderQueue(){}

        /// <summary>
        /// Constructor so we can test out the cleanup process
        /// By overriding the defaults
        /// </summary>
        /// <param name="cleanupProcessInterval"></param>
        /// <param name="cleanupThreshold"></param>
        /// <param name="tenantId"></param>
        public BinderQueue(
            double cleanupProcessInterval,
            TimeSpan cleanupThreshold,
            string tenantId)
        {
            // Override the cleanup process defaults
            _cleanupProcessInterval = cleanupProcessInterval;
            _cleanupThreshold = cleanupThreshold;

            // Set the tenant ID
            TenantId = tenantId;

            // Setup and start the cleanup timer for old in process messages
            SetupCleanupProcess();
        }

        /// <summary>
        /// Public constructor with tenant ID specified
        /// </summary>
        /// <param name="tenantId"></param>
        public BinderQueue(string tenantId)
        {
            // Set the tenant ID
            TenantId = tenantId;

            // Setup and start the cleanup timer for old in process messages
            SetupCleanupProcess();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~BinderQueue()
        {
            Dispose(false);
        }

        #region Public Methods

        /// <summary>
        /// Returns the item priority value with the highest priority (lowest value) from the queue.
        /// If the queue is empty then (-1) will be returned.
        /// </summary>
        /// <returns>The priority value of the next item on the queue.</returns>
        public int PeekPriority()
        {
            lock (_internalDataLock)
            {
                return _internalBinderQueue.PeekPriority();
            }
        }

        /// <summary>
        /// Adds the supplied element to the bottom of the queue
        /// (should be mutex protected to prevent race conditions)
        /// </summary>
        /// <param name="bqe"></param>
        public void Enqueue(BinderQueueElement bqe)
        {
            lock (_internalDataLock)
            {
                if (bqe != null)
                {
                    _internalBinderQueue.Enqueue(bqe, GetPriority(bqe), GetCreatedTimeStamp(bqe));

                    using (var lm = new LogManager())
                    {
                        lm.AdvanceActivityState(this, bqe.RequestWrap.ActivityTrackingContext, ActivityState.State4_EnqueueTenantBinderRequest, ActivityEntryStatus.InProgress);
                    }
                }
            }

            try
            {
                // Notify subscribers
                using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.NotifyBinderElementEnqueued(TenantId, bqe.Identifier);
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteErrorForRequest(this, bqe.RequestWrap.ActivityTrackingContext, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Failed to notify of work addition: " + ex.Message);
            }
        }

        /// <summary>
        /// Dequeues the next element from the internal queue based on the scheduling limits. This simplifies
        /// the multiple locking that would be done if the work was performed in the scheduler.
        /// </summary>
        /// <param name="scheduler">The scheduler to obtain the execution limits from.</param>
        /// <returns>The binder element scheduled for work, or null if no more work (currently allowed).</returns>
        public BinderQueueElement DequeueForInvoke(BinderScheduler scheduler)
        {
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            lock (_internalDataLock)
            {
                if (_internalBinderQueue.Count == 0) return null;

                var nextPriority = _internalBinderQueue.PeekPriority();
                var execReserved = false;

                if (nextPriority == 0)
                {
                    var realtimeCount = _inProcessElements.Count(item => item.Element.ExecutionType == ExecutionTypeValue.HighPriority);

                    if (realtimeCount < scheduler.NumberOfAllowableReservedExecutions)
                    {
                        execReserved = true;
                    }
                }

                if (!execReserved)
                {
                    var normalCount = _inProcessElements.Count(item => item.Element.ExecutionType == ExecutionTypeValue.Normal);

                    if (normalCount >= scheduler.NumberOfAllowableConcurrentExecutions)
                    {
                        return null;
                    }
                }

                var element = _internalBinderQueue.Dequeue();
                
                element.SetExecution(execReserved);
                _inProcessElements.Add(new InProcessElement(element));

                return element;
            }
        }

        /// <summary>
        /// Retrieves the top element from the queue.  The queue element is
        /// hidden from further retrieval until either explictly deleted
        /// or restored
        /// (should be mutex protected to prevent race conditions)
        /// </summary>
        /// <returns></returns>
        public BinderQueueElement Dequeue()
        {
            try
            {
                lock (_internalDataLock)
                {
                    // Dequeue the element
                    var element = _internalBinderQueue.Dequeue();

                    // Add to the internal in process collection
                    _inProcessElements.Add(new InProcessElement(element));

                    return element;
                }
            }
            catch (InvalidOperationException)
            {
                // Queue is empty, return null
                return null;
            }
        }

        /// <summary>
        /// Transfers the given element into the cancel list
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        public bool TransferElementToInCancel(string elementId)
        {
            lock (_internalDataLock)
            {
                // Don't care where it came from.
                BinderQueueElement bqe = Retrieve(elementId);
                if (null != bqe)
                {
                    Delete(elementId);

                    // CTS Disposed by Delete
                    bqe.CancelTokenSource = null;

                    _cancellingElements.Add(bqe);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Given an identifier, the queue element is removed from the collection
        /// Should support external subscription notification of queue element deletion
        /// (should be mutex protected to prevent race conditions)
        /// </summary>
        /// <param name="elementId"></param>
        public void Delete(string elementId)
        {
            lock (_internalDataLock)
            {
                if (_internalBinderQueue.Any(item => item.Value.Identifier == elementId))
                {
                    var filtered = _internalBinderQueue.Where((item) => item.Value.Identifier != elementId);

                    _internalBinderQueue = new PriorityTimeStampQueue<BinderQueueElement>(filtered);
                }

                // Remove the element from the inprocess list
                RemoveInProcessElement(elementId);

                // Remove the element from the cancel list
                _cancellingElements.RemoveAll(element => element.Identifier == elementId);
            }

            try
            {
                // Notify subscribers
                using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.NotifyBinderElementDeleted(TenantId, elementId);
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Failed to notify of work deletion: " + ex.Message);
            }
        }

        /// <summary>
        /// Given an identifier, the queue element is 'restored' from hidden and
        /// enabled for subsequent dequeue operations
        /// Should support external subscription notification of queue element restoration
        /// (should be mutex protected to prevent race conditions)
        /// </summary>
        /// <param name="elementId"></param>
        public void Restore(string elementId)
        {
            lock (_internalDataLock)
            {
                // Don't care where it comes from
                BinderQueueElement bqe = Retrieve(elementId);
                if (null != bqe)
                {
                    // Get rid of it everywhere
                    Delete(elementId);

                    // CTS disposed by Delete
                    bqe.CancelTokenSource = null;

                    // Enqueue the matching element back on the binder queue
                    _internalBinderQueue.Enqueue(bqe, 0, GetCreatedTimeStamp(bqe));
                }
            }

            try
            {
                // Notify subscribers
                using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.NotifyBinderElementRestored(TenantId, elementId);
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Failed to notify of work retry: " + ex.Message);
            }
        }

        /// <summary>
        /// Given an identifier, the queue element is retrieved from the
        /// in-process queue
        /// </summary>
        /// <param name="elementId"></param>
        public BinderQueueElement Retrieve(string elementId)
        {
            lock (_internalDataLock)
            {
                foreach (var element in _internalBinderQueue)
                {
                    var bqe = element.Value;

                    if ((null != bqe) && (bqe.Identifier == elementId))
                    {
                        return bqe;
                    }
                }

                // Look in the 'in-process'
                InProcessElement ipe = FindInProcessElement(elementId);
                if (null != ipe)
                {
                    return ipe.Element;
                }

                // Finally, look in the 'cancelled'
                return _cancellingElements.FirstOrDefault(element => element.Identifier == elementId);
            }
        }

        /// <summary>
        /// Given an identifier, the queue element is retrieved from the
        /// queues via the activity tracking context identifier
        /// </summary>
        /// <param name="activityTrackingContextId"></param>
        public BinderQueueElement RetrieveElementByActivityTrackingContextId(string activityTrackingContextId)
        {
            lock (_internalDataLock)
            {
                foreach (var element in _internalBinderQueue)
                {
                    var bqe = element.Value;

                    if ((null != bqe) &&
                        (null != bqe.RequestWrap) &&
                        (null != bqe.RequestWrap.ActivityTrackingContext) &&
                        (bqe.RequestWrap.ActivityTrackingContext.Id.ToString() == activityTrackingContextId))
                    {
                        return bqe;
                    }
                }

                foreach (var element in _inProcessElements)
                {
                    if ((null != element.Element) &&
                        (null != element.Element.RequestWrap) &&
                        (null != element.Element.RequestWrap.ActivityTrackingContext) &&
                        (element.Element.RequestWrap.ActivityTrackingContext.Id.ToString() == activityTrackingContextId))
                    {
                        return element.Element;
                    }
                }

                foreach (var element in _cancellingElements)
                {
                    if ((null != element.RequestWrap) &&
                        (null != element.RequestWrap.ActivityTrackingContext) &&
                        (element.RequestWrap.ActivityTrackingContext.Id.ToString() == activityTrackingContextId))
                    {
                        return element;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Given an identifier, the lement is retrieved from the
        /// in-process queue and the task is updated
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="cancelTokenSource"></param>
        public void SetElementCancelTokenSource(string elementId, CancellationTokenSource cancelTokenSource)
        {
            lock (_internalDataLock)
            {
                // Only in process elements can be directly retrieved
                // Find the in process element
                InProcessElement matchingInProcessElement = FindInProcessElement(elementId);
                if (null != matchingInProcessElement)
                    matchingInProcessElement.Element.CancelTokenSource = cancelTokenSource;
            }
        }

        /// <summary>
        /// Iterates through memory collection and restores all queue elements
        /// to the persistant tenant dispatch queue.
        /// </summary>
        public void RestoreAllElementsToTenantDispatchQueue()
        {
            try
            {
                using (var qm = new QueueManager())
                {
                    lock (_internalDataLock)
                    {
                        // Restore the in process collection
                        foreach (InProcessElement inProcessElement in _inProcessElements)
                        {
                            qm.RestoreQueueMessage(inProcessElement.Element.DispatchIdentifier);
                            CancellationTokenSource cts = inProcessElement.Element.CancelTokenSource;
                            if (null != cts)
                            {
                                inProcessElement.Element.CancelTokenSource = null;
                                cts.Cancel();
                                cts.Dispose();
                                cts = null;
                            }
                        }

                        // Clear the in process element list
                        _inProcessElements.Clear();

                        // Restore the binder queue               
                        foreach (var element in _internalBinderQueue)
                        {
                            var binderElement = element.Value;

                            if (binderElement != null)
                            {
                                qm.RestoreQueueMessage(binderElement.DispatchIdentifier);
                            }
                        }
                        // Clear the binder queue
                        _internalBinderQueue.Clear();


                        // Restore the cancel list
                        foreach (BinderQueueElement cancelElement in _cancellingElements)
                        {
                            qm.RestoreQueueMessage(cancelElement.DispatchIdentifier);
                        }
                        // Clear the cancel list
                        _cancellingElements.Clear();

                        SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.Queues);

                    }
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteCriticalWithEventLogging(this, "Binder Queue", "Error encountered restoring elements to the dispatch queue; exception: {0}", ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.Queues, ex.ExceptionAsString(), "Failed to replace elements from the workers to the inbox: " + ex.Message);
            }
        }

        /// <summary>
        /// The tenant ID for this binder queue
        /// </summary>
        public string TenantId
        {
            get { return _tenantId; }
            private set { _tenantId = value;  }
        }

        /// <summary>
        /// The current count of elements in the queue
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get 
            {
                lock (_internalDataLock)
                {
                    return _internalBinderQueue.Count;
                }
            }
        }

        /// <summary>
        /// Return a snapshot of inprocess elements
        /// </summary>
        public IEnumerable<BinderQueueElement> InProcessElements
        {
            get
            {
                List<BinderQueueElement> retList = new List<BinderQueueElement>();
                lock (_internalDataLock)
                {
                    foreach (var inProcessElement in _inProcessElements)
                    {
                        retList.Add(inProcessElement.Element);
                    }
                }

                return retList;
            }
        }

        /// <summary>
        /// Returns the count of queued and in progress work items.
        /// </summary>
        /// <returns></returns>
        public int InProgressWorkCount()
        {
            lock (_internalDataLock)
            {
                return _internalBinderQueue.Count + _inProcessElements.Count;
            }
        }

        /// <summary>
        /// Returns the snapshot list of in progress work
        /// </summary>
        public IEnumerable<RequestWrapper> InProgressWork
        {
            get
            {
                List<RequestWrapper> retList = new List<RequestWrapper>();

                lock (_internalDataLock)
                {
                    foreach (var enqueuedElement in _internalBinderQueue)
                    {
                        var bqe = enqueuedElement.Value;
                        if (null != bqe)
                            retList.Add(bqe.RequestWrap);
                    }
                    foreach (var inProgressElement in _inProcessElements)
                    {
                        retList.Add(inProgressElement.Element.RequestWrap);
                    }
                }

                return retList;
            }
        }

        /// <summary>
        /// The current count of elements in process
        /// </summary>
        public int InProcessCount
        {
            get 
            {
                lock (_internalDataLock)
                {
                    return _inProcessElements.Count;
                }
            }
        }

        /// <summary>
        /// Requisite cleanup of resource.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Private dispose that allows for cleanup of task and cancellation token.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_cleanupProcess != null)
                {
                    _cleanupProcess.Dispose();
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Find an in process element given the identifier
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        private InProcessElement FindInProcessElement(string elementId)
        {
            // Obtain a lock if one was not already
            lock (_internalDataLock)
            {
                // Find the element with a matching identifier
                InProcessElement matchingInProcessElement =
                    _inProcessElements.FirstOrDefault(inProcessElement => inProcessElement.Element.Identifier.Equals(elementId));

                if (matchingInProcessElement != null)
                {
                    // Match found, return the element
                    return matchingInProcessElement;
                }

                return null;
            }
        }

        /// <summary>
        /// Safely remove an element from the in-process list
        /// (with proper cleanup)
        /// </summary>
        /// <param name="elementId"></param>
        private void RemoveInProcessElement(string elementId)
        {
            lock (_internalDataLock)
            {
                // Find the element with a matching identifier
                InProcessElement matchingInProcessElement = FindInProcessElement(elementId);
                if (matchingInProcessElement != null)
                {
                    if ((null != matchingInProcessElement.Element) &&
                        (null != matchingInProcessElement.Element.CancelTokenSource))
                    {
                        matchingInProcessElement.Element.CancelTokenSource.Cancel();
                        matchingInProcessElement.Element.CancelTokenSource.Dispose();
                        matchingInProcessElement.Element.CancelTokenSource = null;
                    }

                    _inProcessElements.RemoveAll(element => element.Element.Identifier == elementId);
                }
            }
        }

        /// <summary>
        /// Coordinates the execution of the method to clean up in process elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CleanupTimerHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Timers.Timer timer = (System.Timers.Timer)sender;

            try
            {
                // Disable timer for debug purposes, in case someone
                // Sets the interval to be very short
                timer.Enabled = false;

                // Call the actual cleanup method
                CleanupInProcessCollection();
            }
            finally
            {
                // Re-enable when complete
                timer.Enabled = true;
            }
        }

        //TODO: JSB VALIDATE 0 Priority issues.
        /// <summary>
        /// Gets the priority value from the request that is wrapped in the binder queue element.
        /// </summary>
        /// <param name="element">The binder queue element.</param>
        /// <returns>The request priority, or zero if request, request wrapper, or element is null.</returns>
        private int GetPriority(BinderQueueElement element)
        {
            if (element == null) return 0;
            if (element.RequestWrap == null) return 0;

            var request = Utils.JsonDeserialize<Request>(element.RequestWrap.RequestPayload);
            return (request == null) ? 100 : Convert.ToInt32(request.Priority);
        }

        /// <summary>
        /// Gets the Creation Time stamp value from the request that is wrapped in the binder queue element.
        /// </summary>
        /// <param name="element">The binder queue element.</param>
        /// <returns>The request priority, or zero if request, request wrapper, or element is null.</returns>
        private DateTime? GetCreatedTimeStamp(BinderQueueElement element)
        {
            if (element == null) return null;
            if (element.RequestWrap == null) return null;

            var request = Utils.JsonDeserialize<Request>(element.RequestWrap.RequestPayload);
            return ((request == null) ? null : (DateTime?) request.CreatedTimestampUtc);
        }


        /// <summary>
        /// Performs the cleanup of in process elements
        /// Based on age greater than a certain threshold
        /// </summary>
        private void CleanupInProcessCollection()
        {
            // Set the cleanup timestamp threshold
            // Should be a pretty large value since requests can potentially take a long time
            DateTime cleanupThreshold = DateTime.UtcNow - _cleanupThreshold;

            // Create a list of in process elements to clean up based on age
            List<InProcessElement> cleanupElements = new List<InProcessElement>();
            lock (_internalDataLock)
            {
                _inProcessElements.ForEach(inProcessElement =>
                    {
                        if (inProcessElement.DequeuedTimestamp < cleanupThreshold)
                        {
                            cleanupElements.Add(inProcessElement);
                        }
                    });
            }
            
            cleanupElements.ForEach(cleanupElement =>
                                    BinderInvoker.CancelWork(
                                        cleanupElement.Element.RequestWrap.ActivityTrackingContext.Id.ToString(), this, true));
        }

        /// <summary>
        /// Set up and kick off the cleanup process for old in process messages
        /// Note from MSDN on threading with this type of timer:
        ///     System.Timers.Timer class will, by default, call your timer event handler 
        ///     on a worker thread obtained from the common language runtime (CLR) thread pool.
        /// </summary>
        private void SetupCleanupProcess()
        {
            _cleanupProcess = new System.Timers.Timer();
            _cleanupProcess.Elapsed += new System.Timers.ElapsedEventHandler(CleanupTimerHandler);
            _cleanupProcess.Interval = _cleanupProcessInterval;
            _cleanupProcess.Start();
        }

        #endregion


        #region Private Members

        /// <summary>
        /// Name of this class for error text purposes
        /// </summary>
        private static readonly String _myTypeName = typeof(BinderQueue).FullName;

        /// <summary>
        /// A mutex for locking BOTH the internal queue and in process list
        /// </summary>
        private static object _internalDataLock = new object();

        /// <summary>
        /// The internal collection of "hidden" elements that are currently in process
        /// </summary>
        private List<InProcessElement> _inProcessElements = new List<InProcessElement>();

        /// <summary>
        /// The internal collection of "hidden" elements being cancelled
        /// </summary>
        private List<BinderQueueElement> _cancellingElements = new List<BinderQueueElement>(); 

        /// <summary>
        /// The queue of tenant messages that are ready to be processed
        /// </summary>
        private PriorityTimeStampQueue<BinderQueueElement> _internalBinderQueue = new PriorityTimeStampQueue<BinderQueueElement>();

        /// <summary>
        /// The tenant identifier for this binder queue
        /// </summary>
        private string _tenantId;

        /// <summary>
        /// Periodic cleanup check to make sure no messages are stuck on the in process queue forever
        /// </summary>
        private System.Timers.Timer _cleanupProcess;

        /// <summary>
        /// Flag for checking disposed status.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Interval for running the cleanup process
        /// Left as not read only so internally we can test changing this value
        /// </summary>
        private readonly double _cleanupProcessInterval = TimeSpan.FromHours(1).TotalMilliseconds;

        /// <summary>
        /// The threshold for cleaning up old in process messages
        /// Left as not read only so internally we can test changing this value
        /// </summary>
        private readonly TimeSpan _cleanupThreshold = TimeSpan.FromDays(3);

        #endregion
    }
}
