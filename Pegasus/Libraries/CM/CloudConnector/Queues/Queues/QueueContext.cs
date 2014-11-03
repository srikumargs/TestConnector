using System;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Logging;

namespace Sage.Connector.Queues
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class QueueContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackingContext"></param>
        /// <param name="newState"></param>
        /// <param name="newStatus"></param>
        public QueueContext(ActivityTrackingContext trackingContext, ActivityState newState, ActivityEntryStatus newStatus)
        {
            ActivityTrackingContext = trackingContext;
            NewState = newState;
            NewStatus = newStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        public static QueueContext FakeInstance
        { get { return _fakeInstance; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static QueueContext FakeTenantInstance(String tenantId)
        { return new QueueContext(new ActivityTrackingContext(Guid.Empty, tenantId, Guid.Empty, String.Empty), ActivityState.None, ActivityEntryStatus.None); }

        /// <summary>
        /// 
        /// </summary>
        public ActivityTrackingContext ActivityTrackingContext { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityState NewState { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityEntryStatus NewStatus { get; private set; } 

        private readonly static QueueContext _fakeInstance = new QueueContext(new ActivityTrackingContext(Guid.Empty, Guid.NewGuid().ToString(), Guid.Empty, String.Empty), ActivityState.None, ActivityEntryStatus.None);
    }
}
