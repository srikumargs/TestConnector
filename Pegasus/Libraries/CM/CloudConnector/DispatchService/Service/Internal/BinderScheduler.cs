using System;
using System.Threading;
using Sage.Connector.Common;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Diagnostics;

namespace Sage.Connector.DispatchService.Internal
{
    /// <summary>
    /// Manages scheduler policy for binder work invocation
    /// </summary>
    internal sealed class BinderScheduler
    {
        private PremiseConfigurationRecord _pcr;

        private static readonly String _myTypeName = typeof(BinderScheduler).FullName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pcr"></param>
        public BinderScheduler(PremiseConfigurationRecord pcr)
        {
            _pcr = pcr;
        }

        /// <summary>
        /// The number of synchronously allowed execution reserved for priority zero items for a tenant.
        /// </summary>
        /// <remarks>
        /// The reserved count will be determined by getting the count of concurrent executions allowed, and 
        /// dividing this number by two. The minimum value will be one, and the maximum will be capped to five.
        /// Only priority level zero items will be allowed to run using the reserved count.
        /// </remarks>
        public int NumberOfAllowableReservedExecutions
        {
            get
            {
                return Math.Min(Math.Max((NumberOfAllowableConcurrentExecutions / 2), 1), 5);
            }
        }


        /// <summary>
        /// The number of synchronously allowed execution for a tenant
        /// </summary>
        public int NumberOfAllowableConcurrentExecutions
        {
            get
            {
                return _pcr.BackOfficeConnectionEnabledToReceive ?
                    _pcr.BackOfficeAllowableConcurrentExecutions : 0;
            }
        }

        /// <summary>
        /// Invokes the next unit of work (if still within execution policy limits)
        /// </summary>
        /// <param name="binderQueue"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void InvokeTopBinderElement(BinderQueue binderQueue)
        {
            try
            {
                BinderQueueElement bqe = binderQueue.DequeueForInvoke(this);

                if (bqe == null) return;

                using (var lm = new LogManager())
                {
                    lm.AdvanceActivityState(this, bqe.RequestWrap.ActivityTrackingContext, ActivityState.State5_DequeueTenantBinderRequest, ActivityEntryStatus.InProgress);
                    lm.WriteInfo(this, "BinderScheduler: within allowable execution; invoking top binder message");
                }
                BinderInvoker.InvokeBinder(bqe, binderQueue, new CancellationTokenSource());
                SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.DispatchService);
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, "Error encountered invoking top binder queue element; exception: {0}", ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.DispatchService, ex.ExceptionAsString(), "Failed to invoke dispatched work: " + ex.Message);
            }
        }

        /// <summary>
        /// The configuration for the tenant has been updated
        /// </summary>
        /// <param name="updatedPcr"></param>
        /// <param name="binderQueue"></param>
        public void ConfigurationUpdated(PremiseConfigurationRecord updatedPcr, BinderQueue binderQueue)
        {
            ArgumentValidator.ValidateNonNullReference(binderQueue, "updatedPCR", _myTypeName + ".ConfigurationUpdated");
            ArgumentValidator.ValidateNonNullReference(binderQueue, "binderQueue", _myTypeName + ".ConfigurationUpdated");

            _pcr = updatedPcr;
            InvokeTopBinderElement(binderQueue);
        }

        /// <summary>
        /// A message has been added or restored to the tenant's binder queue
        /// </summary>
        /// <param name="binderQueue"></param>
        public void BinderMessageEnqueued(BinderQueue binderQueue)
        {
            ArgumentValidator.ValidateNonNullReference(binderQueue, "binderQueue", _myTypeName + ".BinderMessageEnqueued");
            InvokeTopBinderElement(binderQueue);
        }

        /// <summary>
        /// A message has been deleted from the tenant binder's queue
        /// </summary>
        /// <param name="binderQueue"></param>
        public void BinderMessageDeleted(BinderQueue binderQueue)
        {
            ArgumentValidator.ValidateNonNullReference(binderQueue, "binderQueue", _myTypeName + ".BinderMessageDeleted");
            InvokeTopBinderElement(binderQueue);
        }
    }
}
