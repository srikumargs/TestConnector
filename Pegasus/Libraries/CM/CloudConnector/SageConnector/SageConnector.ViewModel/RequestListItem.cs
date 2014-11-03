using System;
using System.Drawing;
using Sage.Connector.Common.DataContracts;

namespace SageConnector.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestListItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityTrackingContextId"></param>
        /// <param name="requestId"></param>
        /// <param name="tenantName"></param>
        /// <param name="backofficeName"></param>
        /// <param name="status"></param>
        /// <param name="requestingUser"></param>
        /// <param name="timeRequested"></param>
        /// <param name="requestType"></param>
        /// <param name="requestProjectName"></param>
        /// <param name="requestSummary"></param>
        public RequestListItem(
            string activityTrackingContextId,
            string requestId, string tenantName,
            string backofficeName, RequestStatus status,
            string requestingUser, DateTime timeRequested,
            string requestType,
            string requestProjectName,
            string requestSummary)
        {
            ActivityTrackingContextId = activityTrackingContextId;
            RequestId = requestId;
            TenantName = tenantName;
            BackofficeName = backofficeName;
            Status = status;
            RequestingUser = requestingUser;
            TimeRequested = timeRequested;
            RequestType = requestType;
            RequestProjectName = requestProjectName;
            RequestSummary = requestSummary;
        }
        /// <summary>
        /// 
        /// </summary>
        public string RequestSummary { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string RequestProjectName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActivityTrackingContextId { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string RequestId { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string TenantName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string BackofficeName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        private RequestStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Image RequestStatusImage
        {
            get
            {
                return Status == RequestStatus.InProgressBindableWorkProcessing
                        ? ResourcesViewModel.Running
                        : ResourcesViewModel.Blank;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RequestingUser { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime TimeRequested
        {
            get { return _timeRequested; }
            set
            {
                _timeRequested = new DateTime(value.Ticks, DateTimeKind.Utc);
                _timeRequested = _timeRequested.ToLocalTime();
            }
        }
        private DateTime _timeRequested;

        /// <summary>
        /// 
        /// </summary>
        public string TimeElapsedString
        {
            get { return DateTime.Now.Subtract(TimeRequested).ToString(@"dd\.hh\:mm\:ss"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RequestType
        {
            get { return _requestType; }
            private set { _requestType = value; }
        }
        private string _requestType;
    }
}
