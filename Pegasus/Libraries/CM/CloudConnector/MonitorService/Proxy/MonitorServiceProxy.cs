using System;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.MonitorService.Interfaces;
using Sage.Connector.MonitorService.Interfaces.DataContracts;
using Sage.ServiceModel;

namespace Sage.Connector.MonitorService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MonitorServiceProxy : RetryClientBase<IMonitorService>, IMonitorService
    {
        /// <summary>
        /// 
        /// </summary>
        public MonitorServiceProxy(RetryClientBase<IMonitorService>.CreationFunction rawProxyCreationFunction)
            : base(rawProxyCreationFunction)
        { }

        #region IMonitorService Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConnectorServiceState GetConnectorServiceState()
        {
            return (ConnectorServiceState)RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.GetConnectorServiceState();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recentEntriesThreshold"></param>
        /// <returns></returns>
        public RequestState[] GetRecentAndInProgressRequestsState(TimeSpan recentEntriesThreshold)
        {
            return (RequestState[])RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.GetRecentAndInProgressRequestsState(recentEntriesThreshold);
            });
        }


        #endregion
    }
}
