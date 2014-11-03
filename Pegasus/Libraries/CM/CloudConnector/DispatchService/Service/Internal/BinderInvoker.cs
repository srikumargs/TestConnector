using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.Queues;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Diagnostics;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;

namespace Sage.Connector.DispatchService.Internal
{
    /// <summary>
    /// Threaded invoker of bindable work
    /// </summary>
    internal static class BinderInvoker
    {
        /// <summary>
        /// Type name for error purposes
        /// </summary>
        private static readonly String _myTypeName = typeof(BinderInvoker).FullName;

        #region Private Completion Processing

        /// <summary>
        /// An execution yield no output, create an error response
        /// </summary>
        /// <returns></returns>
        private static string EmptyResultError(BinderQueueElement binderQueueElement)
        {
            if ((null != binderQueueElement) &&
                (null != binderQueueElement.RequestWrap) &&
                (null != binderQueueElement.RequestWrap.ActivityTrackingContext))
            {
                var request = Utils.JsonDeserialize<Request>(binderQueueElement.RequestWrap.RequestPayload);
                ErrorResponse errorResponse = ErrorResponseUtils.GenerateRetryErrorResponse(
                    binderQueueElement.RequestWrap.ActivityTrackingContext.RequestId,
                    "Execution completed but without proper response.",
                    string.Empty,
                    (null != request) ? request.RetryCount : 0,
                    null);
                return Utils.JsonSerialize(new ResponseWrapper(binderQueueElement.RequestWrap, errorResponse, null));
            }
            return String.Empty;
        }

        /// <summary>
        /// Invoke when bindable work is completed
        /// (enqueues result to tenant outbox, notifies controller for completion)
        /// </summary>
        /// <param name="binderQueueElement"></param>
        /// <param name="responseWrapper"></param>
        private static void BindableWorkFinishCleanup(BinderQueueElement binderQueueElement, ResponseWrapper responseWrapper)
        {
            // Serialize the response, whether a valid response or an error response
            string result = Utils.JsonSerialize(responseWrapper);

            // Log the fact that that we're finishing up
            using (var lm = new LogManager())
            {
                lm.WriteInfo(null, "Bound work completed; adding results to outbox; BinderQueueElement.Identifier={0}", binderQueueElement.Identifier);
            }

            // Track whether the below retryable action succeeds
            bool addToOutputSucceeded = false;

            try
            {
                if (String.IsNullOrEmpty(result))
                {
                    result = EmptyResultError(binderQueueElement);
                }
                using (var qm = new QueueManager())
                {
                    var id = responseWrapper.ActivityTrackingContext.RequestId.ToString();

                    // Add the result to the output queue
                    qm.AddMessageToOutput(result, responseWrapper.ResponseType, new QueueContext(binderQueueElement.RequestWrap.ActivityTrackingContext, ActivityState.State14_EnqueueTenantOutboxResponse, ActivityEntryStatus.InProgress), id, true);
                    SubsystemHealthHelper.ClearSubsystemHealthIssues(Subsystem.Queues);

                    addToOutputSucceeded = true;
                }
            }
            catch (Exception ex)
            {
                // Log errors
                using (var lm = new LogManager())
                {
                    // Log the exception
                    lm.WriteCriticalForRequestWithEventLogging(
                        null,
                        binderQueueElement.RequestWrap.ActivityTrackingContext,
                        "Dispatch Binding",
                        "Outbox enqueue failure; failed to add execution result to the outbox. Exception: {0}",
                        ex.ExceptionAsString());

                    // Log the fact that this message will be dropped
                    lm.WriteCriticalForRequestWithEventLogging(
                        null,
                        binderQueueElement.RequestWrap.ActivityTrackingContext,
                        "Dispatch Binding",
                        "Exceeded max retries, message will be dropped!");

                    SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.Queues, ex.ExceptionAsString(), "Work response dropped due to outbox failure: " + ex.Message);

                    // Force a new thread context
                    // For W2K3, we have noticed signature exception with
                    // the underlying DLLs that cannot be cleared up
                    if (ex is System.Data.EntityException)
                    {
                        try
                        {
                            if ((null != binderQueueElement.RequestWrap) &&
                                (null != binderQueueElement.RequestWrap.ActivityTrackingContext))
                                using (
                                    var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost",
                                                                                                  ConnectorServiceUtils.
                                                                                                      CatalogServicePortNumber)
                                    )
                                {
                                    proxy.NotifyTenantRestart(binderQueueElement.RequestWrap.ActivityTrackingContext.TenantId);
                                }
                        }
                        catch (Exception)
                        {
                            // Best attempt notification to spawn restarts
                        }
                    }
                }
            }

            // Only do notification if we succeeded above
            if (addToOutputSucceeded)
            {
                try
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteInfo(null, "Notifying coordinator of bound work completion; BinderQueueElement.Identifier={0}", binderQueueElement.Identifier);
                    }

                    // Notify subscribers
                    using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        proxy.NotifyBinderElementCompleted(binderQueueElement.RequestWrap.ActivityTrackingContext.TenantId, binderQueueElement.Identifier);
                    }
                }
                catch (Exception ex)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteErrorForRequest(null, binderQueueElement.RequestWrap.ActivityTrackingContext, ex.ExceptionAsString());
                    }
                    SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Failed to notify work completion: " + ex.Message);
                }
            }
        }

        #endregion

        #region Private Threaded Invocation Worker

        /// <summary>
        /// Exception handler for task building and execution
        /// </summary>
        /// <param name="binderQueueElement"></param>
        /// <param name="failedActionString"></param>
        /// <param name="ex"></param>
        private static void LogAndQueueErrorResponse(BinderQueueElement binderQueueElement, String failedActionString, Exception ex)
        {
            using (var lm = new LogManager())
            {
                lm.WriteError(null, ex.ExceptionAsString());
            }

            if ((null != binderQueueElement) &&
                (null != binderQueueElement.RequestWrap) &&
                (null != binderQueueElement.RequestWrap.ActivityTrackingContext))
            {
                var request = Utils.JsonDeserialize<Request>(binderQueueElement.RequestWrap.RequestPayload);
                ErrorResponse errorResponse = ErrorResponseUtils.GenerateRetryErrorResponse(
                    binderQueueElement.RequestWrap.ActivityTrackingContext.RequestId,
                    failedActionString,
                    string.Empty,
                    (null != request) ? request.RetryCount : 0,
                    null);
                ResponseWrapper responseWrapper = new ResponseWrapper(binderQueueElement.RequestWrap, errorResponse, null);
                BindableWorkFinishCleanup(binderQueueElement, responseWrapper);
            }
            using (var lm = new LogManager())
            {
                if ((null != binderQueueElement) &&
                    (null != binderQueueElement.RequestWrap) &&
                    (null != binderQueueElement.RequestWrap.ActivityTrackingContext))
                {
                    failedActionString += " on request '" + binderQueueElement.RequestWrap.ActivityTrackingContext.RequestId + "'";
                }
                lm.WriteErrorForRequest(null, binderQueueElement.RequestWrap.ActivityTrackingContext, "{0}; exception: {1}", failedActionString, ex.ExceptionAsString());
            }
        }

        /// <summary>
        /// Synchronous invocation of bindable work
        /// </summary>
        /// <param name="binderQueueElement"></param>
        /// <param name="cancelTokenSource"></param>
        private static void DoWork(BinderQueueElement binderQueueElement, CancellationTokenSource cancelTokenSource)
        {
            // Only log if the system variable SAGE_CONNECTOR_LOG_REQUEST_RESPONSE is set to 1
            String logRequestResponse = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_LOG_REQUEST_RESPONSE", EnvironmentVariableTarget.Machine);
            Boolean bLogRequestResponse = (!String.IsNullOrEmpty(logRequestResponse) && logRequestResponse == "1");

            using (new StackTraceContext(null, "binderQueueElement.Identifier={0}", binderQueueElement.Identifier))
            {
                try
                {
                    IBindable ib = binderQueueElement.Bindable;
                    RequestWrapper rw = binderQueueElement.RequestWrap;

                    using (var lm = new LogManager())
                    {
                        lm.AdvanceActivityState(null, rw.ActivityTrackingContext, ActivityState.State6_InvokingBindableWork, ActivityEntryStatus.InProgressBindableWorkProcessing);
                        lm.WriteInfoForRequest(null, rw.ActivityTrackingContext, "Invoking " + ib.GetType().FullName + " on request '" + rw.ActivityTrackingContext.RequestId + "'");
                        if (bLogRequestResponse)
                        {
                            //note have to use a "{0}" here or the format code will cause the json to now log correctly.
                            lm.WriteInfoForRequest(null, rw.ActivityTrackingContext, "{0}", Utils.JsonSerialize(rw));
                        }
                        


                        ResponseWrapper response = ib.InvokeWork(rw, cancelTokenSource, binderQueueElement.MaxBlobSize);

                        lm.AdvanceActivityState(null, rw.ActivityTrackingContext, ActivityState.State13_BindableWorkComplete, ActivityEntryStatus.InProgress);

                        if (cancelTokenSource.IsCancellationRequested)
                        {
                            // Don't process the cancellation response (potentially null),
                            // when the service is restarted, the work will be automatically
                            // retried.
                            return;
                        }

                        lm.WriteInfoForRequest(null, rw.ActivityTrackingContext, "Invocation on request '" + rw.ActivityTrackingContext.RequestId + "' completed with response of type " + response.ResponseType);
                        if (bLogRequestResponse)
                        {
                            //note have to use a "{0}" here or the format code will cause the json to now log correctly.
                            lm.WriteInfoForRequest(null, rw.ActivityTrackingContext, "{0}", Utils.JsonSerialize(response));
                        }
                        

                        BindableWorkFinishCleanup(binderQueueElement, response);
                    }


                }
                catch (Exception ex)
                {
                    LogAndQueueErrorResponse(binderQueueElement, "Failed to execute work", ex);
                }
            }
        }

        #endregion

        /// <summary>
        /// Spawns a new thread to invoke binder work
        /// (should return after successful instantiation of thread;
        /// thread success and failure must be managed by invocation)
        /// </summary>
        /// <param name="binderQueueElement"></param>
        /// <param name="binderQueue"></param>
        /// <param name="cancelTokenSource"></param>
        public static void InvokeBinder(BinderQueueElement binderQueueElement, BinderQueue binderQueue, CancellationTokenSource cancelTokenSource)
        {
            ArgumentValidator.ValidateNonNullReference(binderQueueElement, "Binder Queue Element", _myTypeName + ".InvokeBinder()");
            ArgumentValidator.ValidateNonNullReference(binderQueueElement.Bindable, "Bindable Interface", _myTypeName + ".InvokeBinder()");
            ArgumentValidator.ValidateNonNullReference(binderQueueElement.RequestWrap, "Request Wrapper", _myTypeName + ".InvokeBinder()");
            ArgumentValidator.ValidateNonNullReference(binderQueueElement.RequestWrap.ActivityTrackingContext, "ActivityTrackingContext", _myTypeName + ".InvokeBinder()");
            ArgumentValidator.ValidateNonNullReference(binderQueue, "Binder Queue", _myTypeName + ".InvokeBinder()");
            ArgumentValidator.ValidateNonNullReference(cancelTokenSource, "Cancel Token Source", _myTypeName + ".InvokeBinder()");

            using (new StackTraceContext(null, "binderQueueElement.Identifier={0}", binderQueueElement.Identifier))
            {
                try
                {
                    TaskCreationOptions options = (binderQueueElement.Priority == 0)
                        ? TaskCreationOptions.None
                        : TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness;

                    Task.Factory.StartNew(
                        () => DoWork(binderQueueElement, cancelTokenSource), options);
                    binderQueue.SetElementCancelTokenSource(binderQueueElement.Identifier, cancelTokenSource);
                }
                catch (Exception ex)
                {
                    LogAndQueueErrorResponse(binderQueueElement, "Failed to start binding task", ex);
                }
            }
        }

        /// <summary>
        /// Cancels work for a binderQueueElement
        /// </summary>
        /// <param name="binderQueueElement"></param>
        /// <param name="systemCancelled"></param>
        private static void CancelWork(BinderQueueElement binderQueueElement, Boolean systemCancelled)
        {
            ArgumentValidator.ValidateNonNullReference(binderQueueElement, "Binder Queue Element",
                                                       _myTypeName + ".CancelWork()");

            using (new StackTraceContext(null, "binderQueueElement.Identifier={0}", binderQueueElement.Identifier))
            {
                if ((null != binderQueueElement.RequestWrap) &&
                    (null != binderQueueElement.RequestWrap.ActivityTrackingContext))
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteWarningForRequest(
                            null,
                            binderQueueElement.RequestWrap.ActivityTrackingContext,
                            "Request '" + binderQueueElement.RequestWrap.ActivityTrackingContext.RequestId + "' cancelled.");
                    }

                    // Create a cancel error response
                    ErrorResponse errorResponse = ErrorResponseUtils.GenerateErrorResponse(
                        binderQueueElement.RequestWrap.ActivityTrackingContext.RequestId,
                        systemCancelled ? Resource1.SystemCancelWorkActionString : Resource1.CancelWorkActionString,
                        systemCancelled ? Resource1.SystemCancelWorkActionString : Resource1.CancelWorkActionString,
                        ErrorResponseAction.Cancel);

                    ResponseWrapper responseWrapper =
                        new ResponseWrapper(binderQueueElement.RequestWrap, errorResponse, null);

                    BindableWorkFinishCleanup(binderQueueElement, responseWrapper);
                }
            }
        }

        /// <summary>
        /// Cancel all in-process binder queue work
        /// </summary>
        /// <param name="binderQueue"></param>
        public static void CancelWork(BinderQueue binderQueue)
        {
            ArgumentValidator.ValidateNonNullReference(binderQueue, "Binder Queue", _myTypeName + ".CancelWork()");

            IEnumerable<RequestWrapper> rws = binderQueue.InProgressWork;
            foreach (RequestWrapper rw in rws)
            {
                CancelWork(rw.ActivityTrackingContext.Id.ToString(), binderQueue);
            }
        }

        /// <summary>
        /// Cancels the specified binder queue work
        /// </summary>
        /// <param name="activityTrackingContextId"></param>
        /// <param name="binderQueue"></param>
        /// <param name="systemCancelled"></param>
        public static void CancelWork(String activityTrackingContextId, BinderQueue binderQueue, Boolean systemCancelled = false)
        {
            ArgumentValidator.ValidateNonNullReference(activityTrackingContextId, "Activity Tracking Context Identifier", _myTypeName + ".CancelWork()");
            ArgumentValidator.ValidateNonNullReference(binderQueue, "Binder Queue", _myTypeName + ".CancelWork()");

            BinderQueueElement bqe = binderQueue.RetrieveElementByActivityTrackingContextId(activityTrackingContextId);
            if ((null != bqe) &&
                (binderQueue.TransferElementToInCancel(bqe.Identifier)))
            {
                try
                {
                    CancelWork(bqe, systemCancelled);
                }
                catch (Exception ex)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCritical(null, "Exception encountered canceling work, restoring back to dispatch: {0}", ex.ToString());
                    }
                    binderQueue.Restore(bqe.Identifier);
                }
            }
            else
            {
                using (var lm = new LogManager())
                {
                    lm.WriteWarning(null, "Unable to cancel work. Activity was not found: {0}", activityTrackingContextId);

                    // 'Cancel' orphaned activity as appropriate
                    lm.CancelOrphanedInProgressActivity(null, activityTrackingContextId);
                }
            }
        }
    }
}
