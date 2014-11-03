using System;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.ProcessExecution.Interfaces;
using Sage.Connector.ProcessExecution.Interfaces.Events;
using CLog = Sage.Connector.Logging;

namespace Sage.Connector.ProcessExecution.RequestActivator
{

    /// <summary>
    /// Application Object is designed to fire events to the Domain Mediator process execution
    /// to notify of cancellation or any other communication. 
    /// </summary>
    public class AppObject : IApp
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AppNotificationEventArgs> AppNotification;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ILogging GetLogger()
        {
            return new LoggingAdapter();
        }

        // ReSharper disable once UnusedMember.Local
        /// <summary>
        /// Fire the 
        /// </summary>
        public void FireEvent()
        {
            if (AppNotification != null)
            {
                AppNotification.Invoke(this, new ConnectorNotificationEventArgs());
            }
        }
    }

    internal class LoggingAdapter : ILogging
    {
        private readonly CLog.LogManager _logManager;
        public LoggingAdapter ()
        {
            _logManager = new CLog.LogManager();
        }

        public void WriteVerbose(string caller, string message)
        {
            //Note that we do not currently have verbose level setup in log manager
            //write these at info so we do not lose if this is written.
            _logManager.WriteInfo(this, message, null);
        }

        public void WriteInfo(string caller, string message)
        {
            _logManager.WriteInfo(this,message,null);
        }

        public void WriteWarning(string caller, string message)
        {
            _logManager.WriteWarning(this, message, null);
        }

        public void WriteError(string caller, string message)
        {
            _logManager.WriteError(this, message, null);
        }

        public void WriteCriticalWithEventLogging(string caller, string source, string message)
        {
            _logManager.WriteCriticalWithEventLogging(this, string.Empty, message);
        }


        public void WriteCriticalForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            ActivityTrackingContext atc = new ActivityTrackingContext(trackingId, tenantId, requestId, string.Empty);
            _logManager.WriteCriticalForRequest(caller, atc, message);
        }

        public void WriteErrorForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            ActivityTrackingContext atc = new ActivityTrackingContext(trackingId, tenantId, requestId, string.Empty);
            _logManager.WriteErrorForRequest(caller, atc, message);
        }

        public void WriteWarningForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            ActivityTrackingContext atc = new ActivityTrackingContext(trackingId, tenantId, requestId, string.Empty);
            _logManager.WriteWarningForRequest(caller, atc, message);
        }

        public void WriteInfoForRequest(Object caller, Guid requestId, string tenantId, Guid trackingId, string message)
        {
            ActivityTrackingContext atc = new ActivityTrackingContext(trackingId, tenantId, requestId, string.Empty);
            _logManager.WriteInfoForRequest(caller, atc, message);
        }

        public void AdvanceActivityState(Object caller, Guid requestId, string tenantId, Guid trackingId, int newState, int newStatus)
        {
            ActivityTrackingContext atc = new ActivityTrackingContext(trackingId, tenantId, requestId, string.Empty);
            _logManager.AdvanceActivityState(caller, atc, (CLog.ActivityState)newState, (ActivityEntryStatus)newStatus);      
        }

    }
}
