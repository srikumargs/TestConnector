using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Diagnostics;

namespace Sage.Connector.Logging
{
    /// <summary>
    /// A logging service for logging significant service events
    /// </summary>
    [TraceListenerIgnoreType]
    public sealed class LogManager : IDisposable, ILogging
    {
        private enum LoggingTypes
        {
            /// <summary>
            /// A non-recoverable error
            /// </summary>
            Critical,
            /// <summary>
            /// A recoverable error
            /// </summary>
            Error,
            /// <summary>
            /// A warning
            /// </summary>
            Warning,
            /// <summary>
            /// Information
            /// </summary>
            Information,

            /// <summary>
            /// 
            /// </summary>
            Verbose
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        { }

        #region General log entries

        /// <summary>
        /// Writes a critical log entry
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteCritical(Object caller, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, LoggingTypes.Critical, messageToFormat, list);
        }

        /// <summary>
        /// Writes an error log entry
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteError(Object caller, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, LoggingTypes.Error, messageToFormat, list);
        }

        /// <summary>
        /// Writes a warning log entry
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteWarning(Object caller, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, LoggingTypes.Warning, messageToFormat, list);
        }

        /// <summary>
        /// Writes a info entry
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteInfo(Object caller, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, LoggingTypes.Information, messageToFormat, list);
        }

        /// <summary>
        /// Need this version for the message inspector logger delegate
        /// </summary>
        /// <param name="message"></param>
        public static void StaticWriteInfo(string message)
        {
            using (var lm = new LogManager())
            {
                lm.WriteInfo(null, message);
            }
        }

        #endregion

        #region Log entries for a request
        /// <summary>
        /// Writes a critical log entry
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="trackingContext"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteCriticalForRequest(Object caller, ActivityTrackingContext trackingContext, string messageToFormat, params object[] list)
        {
            ArgumentValidator.ValidateNonNullReference(trackingContext, "trackingContext", _myTypeName + ".WriteCriticalForRequest()");
            SaveNewLogStoreEntry(caller, trackingContext, LoggingTypes.Critical, messageToFormat, list);
        }

        /// <summary>
        /// Writes an error log entry
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="trackingContext"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteErrorForRequest(Object caller, ActivityTrackingContext trackingContext, string messageToFormat, params object[] list)
        {
            ArgumentValidator.ValidateNonNullReference(trackingContext, "trackingContext", _myTypeName + ".WriteErrorForRequest()");
            SaveNewLogStoreEntry(caller, trackingContext, LoggingTypes.Error, messageToFormat, list);
        }

        /// <summary>
        /// Writes a warning log entry
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="trackingContext"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteWarningForRequest(Object caller, ActivityTrackingContext trackingContext, string messageToFormat, params object[] list)
        {
            ArgumentValidator.ValidateNonNullReference(trackingContext, "trackingContext", _myTypeName + ".WriteWarningForRequest()");
            SaveNewLogStoreEntry(caller, trackingContext, LoggingTypes.Warning, messageToFormat, list);
        }

        /// <summary>
        /// Writes a info entry
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="trackingContext"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteInfoForRequest(Object caller, ActivityTrackingContext trackingContext, string messageToFormat, params object[] list)
        {
            ArgumentValidator.ValidateNonNullReference(trackingContext, "trackingContext", _myTypeName + ".WriteInfoForRequest()");
            SaveNewLogStoreEntry(caller, trackingContext, LoggingTypes.Information, messageToFormat, list);
        }
        #endregion

        #region Log entries with event logging

        /// <summary>
        /// Writes a critical log entry with event logging
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="source"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteCriticalWithEventLogging(Object caller, string source, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, LoggingTypes.Critical, messageToFormat, list);
            WriteWindowsEventLogEntry(source, LoggingTypes.Critical, messageToFormat, list);
        }

        /// <summary>
        /// Writes an error log entry with event logging
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="source"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteErrorWithEventLogging(Object caller, string source, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, LoggingTypes.Error, messageToFormat, list);
            WriteWindowsEventLogEntry(source, LoggingTypes.Error, messageToFormat, list);
        }

        /// <summary>
        /// Writes a warning log entry with event logging
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="source"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteWarningWithEventLogging(Object caller, string source, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, LoggingTypes.Warning, messageToFormat, list);
            WriteWindowsEventLogEntry(source, LoggingTypes.Warning, messageToFormat, list);
        }

        /// <summary>
        /// Writes a info entry with event logging
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="source"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteInfoWithEventLogging(Object caller, string source, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, LoggingTypes.Information, messageToFormat, list);
            WriteWindowsEventLogEntry(source, LoggingTypes.Information, messageToFormat, list);
        }

        /// <summary>
        /// Writes a critical log entry with event logging
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="trackingContext"></param>
        /// <param name="source"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteCriticalForRequestWithEventLogging(Object caller, ActivityTrackingContext trackingContext, string source, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, trackingContext, LoggingTypes.Critical, messageToFormat, list);
            WriteWindowsEventLogEntry(trackingContext, source, LoggingTypes.Critical, messageToFormat, list);
        }

        /// <summary>
        /// Writes an error log entry with event logging
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="trackingContext"></param>
        /// <param name="source"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteErrorForRequestWithEventLogging(Object caller, ActivityTrackingContext trackingContext, string source, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, trackingContext, LoggingTypes.Error, messageToFormat, list);
            WriteWindowsEventLogEntry(trackingContext, source, LoggingTypes.Error, messageToFormat, list);
        }

        /// <summary>
        /// Writes a warning log entry with event logging
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="trackingContext"></param>
        /// <param name="source"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteWarningForRequestWithEventLogging(Object caller, ActivityTrackingContext trackingContext, string source, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, trackingContext, LoggingTypes.Warning, messageToFormat, list);
            WriteWindowsEventLogEntry(trackingContext, source, LoggingTypes.Warning, messageToFormat, list);
        }

        /// <summary>
        /// Writes a info entry with event logging
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="trackingContext"></param>
        /// <param name="source"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public void WriteInfoForRequestWithEventLogging(Object caller, ActivityTrackingContext trackingContext, string source, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, trackingContext, LoggingTypes.Information, messageToFormat, list);
            WriteWindowsEventLogEntry(trackingContext, source, LoggingTypes.Information, messageToFormat, list);

        }
        #endregion

        #region ILogging
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        void ILogging.WriteError(Object caller, String messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, LoggingTypes.Error, messageToFormat, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        void ILogging.VerboseTrace(Object caller, String messageToFormat, params object[] list)
        {
            var message = ComposeMessage(caller, LoggingTypes.Verbose, false, _traceMessageComposerFlags, messageToFormat, list);
            TraceWriteLine(message.FullMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        void ILogging.InfoTrace(Object caller, String messageToFormat, params object[] list)
        {
            var message = ComposeMessage(caller, LoggingTypes.Information, false, _traceMessageComposerFlags, messageToFormat, list);
            TraceWriteLine(message.FullMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        void ILogging.WarningTrace(Object caller, String messageToFormat, params object[] list)
        {
            var message = ComposeMessage(caller, LoggingTypes.Warning, false, _traceMessageComposerFlags, messageToFormat, list);
            TraceWriteLine(message.FullMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        void ILogging.ErrorTrace(Object caller, String messageToFormat, params object[] list)
        {
            var message = ComposeMessage(caller, LoggingTypes.Error, false, _traceMessageComposerFlags, messageToFormat, list);
            TraceWriteLine(message.FullMessage);
        }
        #endregion

        #region Activity tracking
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="tenantId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActivityTrackingContext BeginActivityTracking(Object caller, String tenantId, Request request)
        {
            Guid resultId = Guid.Empty;

            try
            {
                ActivityEntryRecordFactory factory = ActivityEntryRecordFactory.Create(this);
                if (null != factory)
                {
                    var entry = factory.CreateNewEntry();
                    if (null != entry)
                    {
                        entry.CloudTenantId = tenantId;
                        entry.CloudRequestId = request.Id;
                        entry.CloudRequestRetryCount = request.RetryCount;
                        entry.CloudRequestType = RequestTypeMapper.RequestDisplayName(request);
                        entry.CloudRequestInnerType = RequestTypeMapper.RequestInnerTypeName(request);
                        entry.CloudRequestCreatedTimestampUtc = request.CreatedTimestampUtc;
                        entry.CloudRequestRequestingUser = request.RequestingUser;
                        entry.IsSystemRequest = false;
                        entry.State1DateTimeUtc = DateTime.UtcNow;
                        entry.State2DateTimeUtc = null;
                        entry.State3DateTimeUtc = null;
                        entry.State4DateTimeUtc = null;
                        entry.State5DateTimeUtc = null;
                        entry.State6DateTimeUtc = null;
                        entry.State7DateTimeUtc = null;
                        entry.State8DateTimeUtc = null;
                        entry.State9DateTimeUtc = null;
                        entry.State10DateTimeUtc = null;
                        entry.State11DateTimeUtc = null;
                        entry.State12DateTimeUtc = null;
                        entry.State13DateTimeUtc = null;
                        entry.State14DateTimeUtc = null;
                        entry.State15DateTimeUtc = null;
                        entry.State16DateTimeUtc = null;
                        entry.State17DateTimeUtc = null;
                        entry.Status = ActivityEntryStatus.InProgress;
                        entry.CloudProjectName = request.ProjectName;
                        entry.CloudRequestSummary = request.RequestSummary;
                        resultId = factory.AddEntry(entry);
                    }
                }
                ClearLoggingHealthIssues();
            }
            catch (Exception ex)
            {
                WriteError(this, ex.ExceptionAsString());
                RaiseLoggingHealthIssue(ex.ExceptionAsString(), "Activity tracking has failed.");
            }

            var result = new ActivityTrackingContext(resultId, tenantId, request.Id, request.GetType().FullName);
            WriteInfoForRequest(caller, result, String.Format("ACTIVITY: Beginning tracking {0} for new request {1}", resultId, request.Id));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="tenantId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActivityTrackingContext BeginActivityTracking(Object caller, String tenantId, WebAPIMessage request)
        {
            Guid resultId = Guid.Empty;

            try
            {
                ActivityEntryRecordFactory factory = ActivityEntryRecordFactory.Create(this);
                if (null != factory)
                {
                    var entry = factory.CreateNewEntry();
                    if (null != entry)
                    {
                        entry.CloudTenantId = tenantId;
                        entry.CloudRequestId = request.Id;
                        entry.CloudRequestRetryCount = 0;
                        entry.CloudRequestType = request.BodyType;
                        entry.CloudRequestInnerType = string.Empty;
                        entry.CloudRequestCreatedTimestampUtc = request.TimeStamp;
                        entry.CloudRequestRequestingUser = String.Empty;
                        entry.IsSystemRequest = false;
                        entry.State1DateTimeUtc = DateTime.UtcNow;
                        entry.State2DateTimeUtc = null;
                        entry.State3DateTimeUtc = null;
                        entry.State4DateTimeUtc = null;
                        entry.State5DateTimeUtc = null;
                        entry.State6DateTimeUtc = null;
                        entry.State7DateTimeUtc = null;
                        entry.State8DateTimeUtc = null;
                        entry.State9DateTimeUtc = null;
                        entry.State10DateTimeUtc = null;
                        entry.State11DateTimeUtc = null;
                        entry.State12DateTimeUtc = null;
                        entry.State13DateTimeUtc = null;
                        entry.State14DateTimeUtc = null;
                        entry.State15DateTimeUtc = null;
                        entry.State16DateTimeUtc = null;
                        entry.State17DateTimeUtc = null;
                        entry.Status = ActivityEntryStatus.InProgress;
                        entry.CloudProjectName = String.Empty;
                        entry.CloudRequestSummary = String.Empty;
                        resultId = factory.AddEntry(entry);
                    }
                }
                ClearLoggingHealthIssues();
            }
            catch (Exception ex)
            {
                WriteError(this, ex.ExceptionAsString());
                RaiseLoggingHealthIssue(ex.ExceptionAsString(), "Activity tracking has failed.");
            }

            var result = new ActivityTrackingContext(resultId, tenantId, request.Id, request.BodyType);
            WriteInfoForRequest(caller, result, String.Format("ACTIVITY: Beginning tracking {0} for new request {1}", resultId, request.Id));
            return result;
        }


        /// <summary>
        /// Begins the activity tracking internal system request.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="requestInnerType">Type of the request inner.</param>
        /// <returns></returns>
        public ActivityTrackingContext BeginActivityTrackingInternalSystemRequest(Object caller, String tenantId, 
            Guid requestId, string requestType, string requestInnerType)
        {
            Guid resultId = Guid.Empty;

            try
            {
                ActivityEntryRecordFactory factory = ActivityEntryRecordFactory.Create(this);
                if (null != factory)
                {
                    var entry = factory.CreateNewEntry();
                    if (null != entry)
                    {
                        DateTime now = DateTime.UtcNow;
                        entry.CloudTenantId = tenantId;
                        entry.CloudRequestId = requestId;
                        entry.CloudRequestRetryCount = 0;
                        entry.CloudRequestType = requestType;
                        entry.CloudRequestInnerType = requestInnerType;
                        entry.CloudRequestCreatedTimestampUtc = now;
                        entry.CloudRequestRequestingUser = String.Empty;
                        entry.IsSystemRequest = true;
                        entry.State1DateTimeUtc = now;
                        entry.State2DateTimeUtc = null;
                        entry.State3DateTimeUtc = null;
                        entry.State4DateTimeUtc = null;
                        entry.State5DateTimeUtc = null;
                        entry.State6DateTimeUtc = now;
                        entry.State7DateTimeUtc = null;
                        entry.State8DateTimeUtc = null;
                        entry.State9DateTimeUtc = null;
                        entry.State10DateTimeUtc = null;
                        entry.State11DateTimeUtc = null;
                        entry.State12DateTimeUtc = null;
                        entry.State13DateTimeUtc = null;
                        entry.State14DateTimeUtc = null;
                        entry.State15DateTimeUtc = null;
                        entry.State16DateTimeUtc = null;
                        entry.State17DateTimeUtc = null;
                        entry.Status = ActivityEntryStatus.InProgress;
                        entry.CloudProjectName = String.Empty;
                        entry.CloudRequestSummary = String.Empty;
                        resultId = factory.AddEntry(entry);
                    }
                }
                ClearLoggingHealthIssues();
            }
            catch (Exception ex)
            {
                WriteError(this, ex.ExceptionAsString());
                RaiseLoggingHealthIssue(ex.ExceptionAsString(), "Activity tracking has failed.");
            }
            
            string compositeRequstType = string.Format("System request: {0}-{1}", requestType, requestInnerType);
            var result = new ActivityTrackingContext(resultId, tenantId, requestId, compositeRequstType);

            WriteInfoForRequest(caller, result, String.Format("ACTIVITY: Beginning tracking {0} for new system request {1}", resultId, requestId));
            return result;
        }

        /// <summary>
        /// This will forcibly set the specified activity to 'completed/cancelled'
        /// (if it was in-progress and has no timing for being placed on the output queue)
        /// Intended as a force cleanup of orphaned activities when canceling an activity
        /// that could not be found on the active queues.
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="trackingContextId"></param>
        public void CancelOrphanedInProgressActivity(Object caller, string trackingContextId)
        {
            ArgumentValidator.ValidateNonNullReference(trackingContextId, "trackingContextId", _myTypeName + ".ResetActivityStatus()");

            try
            {
                ActivityEntryRecordFactory factory = ActivityEntryRecordFactory.Create(this);
                if (null != factory)
                {
                    var entry = factory.GetEntry(new Guid(trackingContextId));
                    if (null != entry)
                    {
                        if ((entry.Status == ActivityEntryStatus.InProgress) ||
                            (entry.Status == ActivityEntryStatus.InProgressBindableWorkProcessing) &&
                            !entry.State8DateTimeUtc.HasValue)
                        {
                            entry.Status = ActivityEntryStatus.CompletedWithCancelResponse;
                            factory.UpdateEntry(entry);

                            WriteInfoForRequest(caller,
                                                new ActivityTrackingContext(entry.Id, entry.CloudTenantId,
                                                                            entry.CloudRequestId, entry.CloudRequestType),
                                                String.Format(
                                                    "ACTIVITY: Orphaned in-progress activity; forcibly updating tracking {0} to cancel status.",
                                                    trackingContextId));
                            ClearLoggingHealthIssues();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                WriteError(this, ex.ExceptionAsString());
                RaiseLoggingHealthIssue(ex.ExceptionAsString(), "Activity tracking update has failed.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="trackingContext"></param>
        /// <param name="newState"></param>
        /// <param name="newStatus"></param>
        public void AdvanceActivityState(Object caller, ActivityTrackingContext trackingContext, ActivityState newState, ActivityEntryStatus newStatus)
        {
            ArgumentValidator.ValidateNonNullReference(trackingContext, "trackingContext", _myTypeName + ".AdvanceActivityState()");

            try
            {
                ActivityEntryRecordFactory factory = ActivityEntryRecordFactory.Create(this);
                if (null != factory)
                {
                    var entry = factory.GetEntry(trackingContext.Id);
                    if (null != entry)
                    {
                        switch (newState)
                        {
                            case ActivityState.None:
                                break;
                            case ActivityState.State1_RequestReceivedFromCloud:
                                entry.State1DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State2_EnqueueTenantInboxRequest:
                                entry.State2DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State3_DequeueTenantInboxRequest:
                                entry.State3DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State4_EnqueueTenantBinderRequest:
                                entry.State4DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State5_DequeueTenantBinderRequest:
                                entry.State5DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State6_InvokingBindableWork:
                                entry.State6DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State7_InvokingProcessExecution:
                                entry.State7DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State8_InvokingDomainMediation:
                                entry.State8DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State9_InvokingMediationBoundWork:
                                entry.State9DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State10_MediationBoundWorkComplete:
                                entry.State10DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State11_DomainMediationComplete:
                                entry.State11DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State12_ProcessExecutionComplete:
                                entry.State12DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State13_BindableWorkComplete:
                                entry.State13DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State14_EnqueueTenantOutboxResponse:
                                entry.State14DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State15_DequeueTenantOutboxResponse:
                                entry.State15DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State16_UploadsCompletedAndResponseFinalized:
                                entry.State16DateTimeUtc = DateTime.UtcNow;
                                break;
                            case ActivityState.State17_ResponseSentToCloud:
                                entry.State17DateTimeUtc = DateTime.UtcNow;
                                break;
                            default:
                                break;
                        }

                        entry.Status = newStatus;

                        factory.UpdateEntry(entry);

                        WriteInfoForRequest(caller, trackingContext, String.Format("ACTIVITY: Updating tracking {0} to new state: {1}", trackingContext.Id, newState.ToString()));
                        ClearLoggingHealthIssues();
                    }
                }

            }
            catch (Exception ex)
            {
                WriteError(this, ex.ExceptionAsString());
                RaiseLoggingHealthIssue(ex.ExceptionAsString(), "Activity tracking update has failed.");
            }
        }

        /// <summary>
        /// Sets the type of the activity secondary.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="trackingContext">The tracking context.</param>
        /// <param name="innerType">Type of the secondary.</param>
        public void SetActivityInnerType(Object caller, ActivityTrackingContext trackingContext, String innerType)
        {
             ArgumentValidator.ValidateNonNullReference(trackingContext, "trackingContext", _myTypeName + ".AdvanceActivityState()");

            try
            {
                ActivityEntryRecordFactory factory = ActivityEntryRecordFactory.Create(this);
                if (null != factory)
                {
                    var entry = factory.GetEntry(trackingContext.Id);
                    if (null != entry)
                    {
                        entry.CloudRequestInnerType = innerType;
                        factory.UpdateEntry(entry);
                        WriteInfoForRequest(caller, trackingContext, String.Format("ACTIVITY:  Updating tracking {0} to new inner type: {1}", trackingContext.Id, innerType));
                        ClearLoggingHealthIssues();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(this, ex.ExceptionAsString());
                RaiseLoggingHealthIssue(ex.ExceptionAsString(), "Activity tracking update has failed.");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        public void DeleteTenantActivity(string tenantId)
        {
            ArgumentValidator.ValidateNonNullReference(tenantId, "tenantId",
                                                       _myTypeName + ".DeleteTenantActivity()");

            try
            {
                ActivityEntryRecordFactory factory = ActivityEntryRecordFactory.Create(this);
                if (null != factory)
                {
                    factory.DeleteTenantEntry(tenantId);
                }
            }
            catch (Exception ex)
            {
                WriteError(this, ex.ExceptionAsString());
                RaiseLoggingHealthIssue(ex.ExceptionAsString(), "Tenant activity clearing has failed.");
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawMessage"></param>
        /// <param name="userFacingMessage"></param>
        public static void RaiseLoggingHealthIssue(String rawMessage, String userFacingMessage)
        {
            // Due to TCP Time Wait Consumption, no longer tracking logging subsystem health.
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ClearLoggingHealthIssues()
        {
            // Due to TCP Time Wait Consumption, no longer tracking logging subsystem health.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private static void TraceWriteLine(String message)
        { Trace.WriteLine(message); }

        /// <summary>
        /// Helper for creating and saving a new log entry
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="trackingContext"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="type"></param>
        /// <param name="list"></param>
        private void SaveNewLogStoreEntry(Object caller, ActivityTrackingContext trackingContext, LoggingTypes type, string messageToFormat, params object[] list)
        {
            ArgumentValidator.ValidateNonNullReference(trackingContext, "trackingContext", _myTypeName + ".SaveNewLogStoreEntry()");
            SaveNewLogStoreEntry(caller, trackingContext.Id, trackingContext.TenantId, trackingContext.RequestId, type, messageToFormat, list);
        }

        /// <summary>
        /// Helper for creating and saving a new log entry
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="activityId"></param>
        /// <param name="tenantId"></param>
        /// <param name="requestId"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="type"></param>
        /// <param name="list"></param>
        private void SaveNewLogStoreEntry(Object caller, Guid? activityId, string tenantId, Guid? requestId, LoggingTypes type, string messageToFormat, params object[] list)
        {
            messageToFormat = messageToFormat ?? string.Empty;
            tenantId = tenantId ?? string.Empty;

            try
            {
                LogEntryRecordFactory factory = LogEntryRecordFactory.Create();
                if (null != factory)
                {
                    var traceMessage = ComposeMessage(caller, type, true, TraceMessageComposer.Flags.None, messageToFormat, list);
                    TraceWriteLine(traceMessage.FullMessage);

                    // only if not verbose do we want to save the entry to the LogStore
                    if (type != LoggingTypes.Verbose)
                    {
                        LogEntryRecord log = factory.CreateNewEntry();
                        if (null != log)
                        {
                            log.Description = traceMessage.FullMessage;
                            log.SourceTypeName = traceMessage.TypeName;
                            log.SourceMemberName = traceMessage.MemberName;
                            log.AppDomainId = traceMessage.AppDomainId;
                            log.ProcessId = traceMessage.ProcessId;
                            log.ThreadId = traceMessage.ThreadId;
                            log.ObjectId = traceMessage.ObjectId;
                            log.Type = type.ToString();
                            log.CloudRequestId = requestId;
                            log.CloudTenantId = tenantId;

                            Guid newEntry = factory.AddEntry(activityId, log);
                            if ((null == newEntry) || (newEntry == Guid.Empty))
                            {
                                throw new Exception("Unable to add a new log entry.");
                            }
                        }
                        else
                        {
                            throw new Exception("Unable to create a new log entry.");
                        }
                    }
                }
                else
                {
                    throw new Exception("Unable to create a log factory.");
                }

                ClearLoggingHealthIssues();
            }
            catch (Exception ex)
            {
                String loggingError = String.Format("Error creating a log entry; exception: {0}", ex.ExceptionAsString());
                EventLogger.WriteMessage(ConnectorRegistryUtils.FullProductName, loggingError, MessageType.Error);
                RaiseLoggingHealthIssue(loggingError, loggingError);

                // Logging to the event logger due to logging failure
                String messageToWrite = messageToFormat;
                if (list != null)
                {
                    try
                    {
                        messageToWrite = String.Format(messageToWrite, list);
                    }
                    catch (FormatException)
                    {
                        // Catch any exception while trying to write a LogStore message and prevent it from changing the control
                        // flow of the code.  This can happed, for example, if the caller supplied an invalid
                        // messageToFormat (e.g., one that contains literal "{" and "}" characters that do not match up
                        // with replaceable parameters).
                        String formatExceptionMessage = String.Format(CultureInfo.InvariantCulture, "FormatException failure occurred during output trace message '{0}'", messageToWrite);
                        Trace.WriteLine(formatExceptionMessage);
                    }
                }
                WriteWindowsEventLogEntry(tenantId, requestId, "Logging", type, messageToWrite);
            }
        }

        /// <summary>
        /// Helper for creating and saving a new log entry
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="type"></param>
        /// <param name="list"></param>
        private void SaveNewLogStoreEntry(Object caller, LoggingTypes type, string messageToFormat, params object[] list)
        {
            SaveNewLogStoreEntry(caller, null, String.Empty, null, type, messageToFormat, list);
        }

        /// <summary>
        /// Helper for creating and saving a new log entry
        /// </summary>
        /// <param name="trackingContext"></param>
        /// <param name="source"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="type"></param>
        /// <param name="list"></param>
        private void WriteWindowsEventLogEntry(ActivityTrackingContext trackingContext, string source, LoggingTypes type, string messageToFormat, params object[] list)
        {
            ArgumentValidator.ValidateNonNullReference(trackingContext, "trackingContext", _myTypeName + ".WriteWindowsEventLogEntry()");
            WriteWindowsEventLogEntry(trackingContext.TenantId, trackingContext.RequestId, source, type, messageToFormat, list);
        }

        /// <summary>
        /// Helper for creating and saving a new log entry
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="requestId"></param>
        /// <param name="source"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="type"></param>
        /// <param name="list"></param>
        private void WriteWindowsEventLogEntry(string tenantId, Guid? requestId, string source, LoggingTypes type, string messageToFormat, params object[] list)
        {
            // next save verbose entries to the Windows Event Log
            if (type == LoggingTypes.Verbose)
            {
                return;
            }

            source = source ?? string.Empty;
            messageToFormat = messageToFormat ?? string.Empty;
            tenantId = tenantId ?? string.Empty;

            StringBuilder sb = new StringBuilder("While processing");
            if (requestId.HasValue)
            {
                sb.AppendFormat(" request '{0}'", requestId.Value);
            }
            if (!string.IsNullOrEmpty(tenantId))
            {
                sb.AppendFormat(" for tenant '{0}'", tenantId);
            }
            if (!string.IsNullOrEmpty(source))
            {
                sb.AppendFormat(" during {0}", source);
            }

            if (list == null)
            {
                sb.AppendFormat(": {0}", messageToFormat);
            }
            else
            {
                sb.Append(": ");
                sb.AppendFormat(messageToFormat, list);
            }

            MessageType msgType = MessageType.Information;
            switch (type)
            {
                case LoggingTypes.Error:
                case LoggingTypes.Critical:
                    msgType = MessageType.Error;
                    break;
                case LoggingTypes.Warning:
                    msgType = MessageType.Warning;
                    break;
                case LoggingTypes.Information:
                    msgType = MessageType.Information;
                    break;
                //case LoggingTypes.Verbose:
                //    break;
            }

            EventLogger.WriteMessage(ConnectorRegistryUtils.FullProductName, sb.ToString(), msgType);
        }

        /// <summary>
        /// Helper for creating and saving a new log entry
        /// </summary>
        /// <param name="source"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="type"></param>
        /// <param name="list"></param>
        private void WriteWindowsEventLogEntry(string source, LoggingTypes type, string messageToFormat, params object[] list)
        {
            WriteWindowsEventLogEntry(String.Empty, null, source, type, messageToFormat, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="loggingType"></param>
        /// <param name="includeStackTrace"></param>
        /// <param name="flags"></param>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static TraceMessage ComposeMessage(Object caller, LoggingTypes loggingType, Boolean includeStackTrace, TraceMessageComposer.Flags flags, String messageToFormat, params object[] list)
        {
            StackTrace stackTrace = null;

            // include the stack trace if we have any flags that require it, or if we have been explicitly told to do it
            if(includeStackTrace || 
                (flags & (TraceMessageComposer.Flags.ShowTypeName | TraceMessageComposer.Flags.ShowMemberName | TraceMessageComposer.Flags.ShowMemberSignature | TraceMessageComposer.Flags.ShowFileLocation | TraceMessageComposer.Flags.ShowAssemblyName)) != TraceMessageComposer.Flags.None)
            {
                stackTrace = new StackTrace((flags & TraceMessageComposer.Flags.ShowFileLocation) != TraceMessageComposer.Flags.None); // only need to capture the file name, line number, and column number if ShowFileLocation is set
            }

            ArrayList ignoreStackTraceTypeNames = new ArrayList();
            ignoreStackTraceTypeNames.Add(typeof(System.Diagnostics.Trace).FullName);
            ignoreStackTraceTypeNames.Add("System.Diagnostics.TraceInternal");
            ignoreStackTraceTypeNames.Add(typeof(LogManager).FullName);

            // OID: object identifier
            Int32 oid = 0;
            if (caller != null)
            {
                if (caller.GetType().GetCustomAttributes(typeof(TraceListenerIgnoreTypeAttribute), false).Length == 1)
                {
                    // If the caller instance that we have has the TraceListenerIgnoreType custom attribute,
                    // then we don't know what the correct object ID to report should be (because the type
                    // that we are going to report is not the type of the caller, but the type of some caller
                    // of the caller).  In order to avoid presenting misleading information in the trace we
                    // simply catch this condition and refuse to output an OID for this call (since we cannot
                    // determine what is the correct one).
                    oid = -1;
                }
                else
                {
                    // We have a caller object and it does not have TraceListenerIgnoreType custom attribute.
                    oid = caller.GetHashCode();
                }
            }

            String mc = String.Empty;
            switch(loggingType)
            {
                case LoggingTypes.Critical:
                    mc = "CRT";
                    break;
                case LoggingTypes.Error:
                    mc = "ERR";
                    break;
                case LoggingTypes.Warning:
                    mc = "WRN";
                    break;
                case LoggingTypes.Information:
                    mc = "INF";
                    break;
                case LoggingTypes.Verbose:
                    mc = "VRB";
                    break;
            }

            return TraceMessageComposer.ComposeMessage(ignoreStackTraceTypeNames, stackTrace, flags, oid, mc, messageToFormat, list);
        }
        #endregion

        private static readonly String _myTypeName = typeof(LogManager).FullName;
      
        private static readonly TraceMessageComposer.Flags _traceMessageComposerFlags = TraceMessageComposer.Flags.ShowTypeName | TraceMessageComposer.Flags.ShowMemberName | TraceMessageComposer.Flags.ShowMessageCategory | TraceMessageComposer.Flags.ShowAppDomainId | TraceMessageComposer.Flags.ShowObjectId | TraceMessageComposer.Flags.ShowThreadId | TraceMessageComposer.Flags.ShowProcessId;

    }
}
