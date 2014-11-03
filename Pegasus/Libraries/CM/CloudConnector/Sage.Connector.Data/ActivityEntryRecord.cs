using System;
using LogStore = Sage.Connector.PremiseStore.LogStore;

namespace Sage.Connector.Data
{
    /// <summary>
    /// TODO:  v1ok - evaluate consolidation of ActivityEntryStatus (an internal implementation detail of the Connector service) with the RequestStatus
    /// parallel to it located in Sage.CRE.CloudConnector.Commmon (which is used to surface request status information to the Monitor tray app)
    /// </summary>
    public enum ActivityEntryStatus
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        InProgress,

        /// <summary>
        /// 
        /// </summary>
        InProgressBindableWorkProcessing,

        /// <summary>
        /// 
        /// </summary>
        InProgressMediationBoundWorkProcessing,

        /// <summary>
        /// 
        /// </summary>
        CompletedWithSuccessResponse,

        /// <summary>
        /// 
        /// </summary>
        CompletedWithErrorResponse,

        /// <summary>
        /// 
        /// </summary>
        CompletedWithCancelResponse


    }

    /// <summary>
    /// TODO:  v1ok - evaluate consolidation of ActivityEntryRecord (an internal implementation detail of the Connector service) with the RequestState 
    /// parallel to it located in Sage.CRE.CloudConnector.Commmon (which is used to surface request status information to the Monitor tray app)
    /// </summary>
    [Serializable]
    public class ActivityEntryRecord
    {
        internal ActivityEntryRecord(LogStore.ActivityEntry activityEntry)
        { _activityEntry = activityEntry; }

        internal LogStore.ActivityEntry GetInternalActivityEntry()
        { return _activityEntry; }

        /// <summary>
        /// 
        /// </summary>
        public Guid Id
        {
            get { return _activityEntry.Id; }
            set { _activityEntry.Id = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String CloudTenantId
        {
            get { return _activityEntry.CloudTenantId; }
            set { _activityEntry.CloudTenantId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid CloudRequestId
        {
            get { return _activityEntry.CloudRequestId; }
            set { _activityEntry.CloudRequestId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CloudRequestCreatedTimestampUtc
        {
            get { return _activityEntry.CloudRequestCreatedTimestampUtc; }
            set { _activityEntry.CloudRequestCreatedTimestampUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64 CloudRequestRetryCount
        {
            get { return _activityEntry.CloudRequestRetryCount; }
            set { _activityEntry.CloudRequestRetryCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String CloudRequestType
        {
            get { return _activityEntry.CloudRequestType; }
            set { _activityEntry.CloudRequestType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String CloudRequestInnerType
        {
            get { return _activityEntry.CloudRequestInnerType; }
            set { _activityEntry.CloudRequestInnerType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String CloudRequestRequestingUser
        {
            get { return _activityEntry.CloudRequestRequestingUser; }
            set { _activityEntry.CloudRequestRequestingUser = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is system request.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is system request; otherwise, <c>false</c>.
        /// </value>
        public bool IsSystemRequest
        {
            get { return _activityEntry.IsSystemRequest; }
            set { _activityEntry.IsSystemRequest = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State1DateTimeUtc
        {
            get { return _activityEntry.State1DateTimeUtc; }
            set { _activityEntry.State1DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State2DateTimeUtc
        {
            get { return _activityEntry.State2DateTimeUtc; }
            set { _activityEntry.State2DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State3DateTimeUtc
        {
            get { return _activityEntry.State3DateTimeUtc; }
            set { _activityEntry.State3DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State4DateTimeUtc
        {
            get { return _activityEntry.State4DateTimeUtc; }
            set { _activityEntry.State4DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State5DateTimeUtc
        {
            get { return _activityEntry.State5DateTimeUtc; }
            set { _activityEntry.State5DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State6DateTimeUtc
        {
            get { return _activityEntry.State6DateTimeUtc; }
            set { _activityEntry.State6DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State7DateTimeUtc
        {
            get { return _activityEntry.State7DateTimeUtc; }
            set { _activityEntry.State7DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State8DateTimeUtc
        {
            get { return _activityEntry.State8DateTimeUtc; }
            set { _activityEntry.State8DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State9DateTimeUtc
        {
            get { return _activityEntry.State9DateTimeUtc; }
            set { _activityEntry.State9DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State10DateTimeUtc
        {
            get { return _activityEntry.State10DateTimeUtc; }
            set { _activityEntry.State10DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State11DateTimeUtc
        {
            get { return _activityEntry.State11DateTimeUtc; }
            set { _activityEntry.State11DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State12DateTimeUtc
        {
            get { return _activityEntry.State12DateTimeUtc; }
            set { _activityEntry.State12DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State13DateTimeUtc
        {
            get { return _activityEntry.State13DateTimeUtc; }
            set { _activityEntry.State13DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State14DateTimeUtc
        {
            get { return _activityEntry.State14DateTimeUtc; }
            set { _activityEntry.State14DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State15DateTimeUtc
        {
            get { return _activityEntry.State15DateTimeUtc; }
            set { _activityEntry.State15DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State16DateTimeUtc
        {
            get { return _activityEntry.State16DateTimeUtc; }
            set { _activityEntry.State16DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? State17DateTimeUtc
        {
            get { return _activityEntry.State17DateTimeUtc; }
            set { _activityEntry.State17DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTimeUtc
        {
            get { return _activityEntry.DateTimeUtc; }
            set { _activityEntry.DateTimeUtc = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ActivityEntryStatus Status
        {
            get { return (ActivityEntryStatus)_activityEntry.Status; }
            set { _activityEntry.Status = (Int32)value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String CloudProjectName
        {
            get { return _activityEntry.CloudProjectName; }
            set { _activityEntry.CloudProjectName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String CloudRequestSummary
        {
            get { return _activityEntry.CloudRequestSummary; }
            set { _activityEntry.CloudRequestSummary = value; }
        }

        private readonly LogStore.ActivityEntry _activityEntry;
    }
}
