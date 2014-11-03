using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sage.Connector.Common;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.DispatchService.Proxy;
using Sage.Connector.StateService.Proxy;

namespace SageConnector.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionRequestListViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        public ConnectionRequestListViewModel(string tenantId)
        {
            TenantId = tenantId;
            InitializeData();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string TenantId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool RefreshingRequests { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public RequestListItemCollection Requests
        {
            get
            {
                if(_requests ==null)
                    _requests = new RequestListItemCollection();
                return _requests;
            }
            set
            {
                _requests = value;
                NotifyPropertyChanged("Requests");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void RefreshRequestList(PropertyDescriptor sProperty = null, ListSortDirection sDirection = ListSortDirection.Descending)
        {
            GetPendingRequests(sProperty, sDirection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityTrackingId"></param>
        /// <returns></returns>
        public bool CancelRequest(string activityTrackingId)
        {
            List<string> list = new List<string>();
            list.Add(activityTrackingId);
            return CancelRequest(list);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityTrackingIds"></param>
        /// <returns></returns>
        public bool CancelRequest(IEnumerable<string> activityTrackingIds)
        {
            bool success = true;
            try
            {
                using (
                    var proxy = DispatchServiceProxyFactory.CreateFromCatalog("localhost",
                                                                              ConnectorServiceUtils.
                                                                                  CatalogServicePortNumber))
                {
                    foreach (var id in activityTrackingIds)
                    {
                        proxy.CancelWork(TenantId, id);
                    }
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
                success = false;
            }
            return success;
        }

        private RequestListItemCollection _requests = null;
        
        private void InitializeData()
        {
            GetPendingRequests();
        }

        private void GetPendingRequests(PropertyDescriptor sortedProperty = null, ListSortDirection sortDirection = ListSortDirection.Descending)
        {
            RefreshingRequests = true;
            try
            {
                RequestListItemCollection tempList = new RequestListItemCollection();

                List<RequestState> activityData = GetActivityData();

                // If the activity appears in the 'active' list, add them to our collection
                foreach (var activity in activityData)
                    if ((!activity.State8DateTimeUtc.HasValue) &&
                        (activity.CloudTenantId == TenantId))
                        tempList.Add(new RequestListItem(
                                            activity.Id.ToString(),
                                            activity.CloudRequestId.ToString(),
                                            activity.TenantName,
                                            activity.BackOfficeCompanyName,
                                            activity.RequestStatus,
                                            activity.CloudRequestRequestingUser,
                                            activity.DateTimeUtc,
                                            activity.CloudRequestType,
                                            activity.CloudProjectName,
                                            activity.CloudRequestSummary));
                
                // Assign the collection
                Requests = tempList;
                if(sortedProperty!= null)
                    Requests.ApplySort(sortedProperty, sortDirection);
                Requests.ResetBindings();
            }
            finally
            {
                RefreshingRequests = false;
            }
        }

        private List<RequestState> GetActivityData()
        {
            List<RequestState> requestState = new List<RequestState>();
            try
            {
                using (var proxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    requestState.AddRange(proxy.GetRecentAndInProgressRequestsState(new TimeSpan(0,0,0,0)));
                }

            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            return requestState;
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// PropertyChanged event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

    }

}
