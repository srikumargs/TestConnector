using System;
using System.ServiceModel;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.MonitorService.Interfaces;
using Sage.Connector.MonitorService.Interfaces.DataContracts;

namespace Sage.Connector.MonitorService.Proxy.Internal
{
    internal sealed class RawMonitorServiceProxy : ClientBase<IMonitorService>, IMonitorService
    {
        /// <summary>
        /// 
        /// </summary>
        public RawMonitorServiceProxy()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public RawMonitorServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawMonitorServiceProxy(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawMonitorServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public RawMonitorServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        { }

        #region IMonitorService Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConnectorServiceState GetConnectorServiceState()
        {
            return base.Channel.GetConnectorServiceState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recentEntriesThreshold"></param>
        /// <returns></returns>
        public RequestState[] GetRecentAndInProgressRequestsState(TimeSpan recentEntriesThreshold)
        { return base.Channel.GetRecentAndInProgressRequestsState(recentEntriesThreshold); }

        #endregion
    }
}
