using System;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.MonitorService.Interfaces.DataContracts;
using HostingFxInterfaces = Sage.CRE.HostingFramework.Interfaces;

namespace ConnectorServiceMonitor.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ServiceStatus
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="monitorServiceStatus"></param>
        /// <param name="connectorServiceState"></param>
        /// <param name="requests"></param>
        public ServiceStatus(String host, Int32 port, HostingFxInterfaces.Status? monitorServiceStatus, ConnectorServiceState connectorServiceState, RequestState[] requests)
        {
            Host = host;
            Port = port;
            MonitorServiceStatus = monitorServiceStatus;
            ConnectorServiceState = connectorServiceState;
            ConnectorServiceStatus = (connectorServiceState != null) ? connectorServiceState.ConnectorServiceConnectivityStatus : ConnectorServiceConnectivityStatus.None;
            Requests = requests;
        }

        /// <summary>
        /// 
        /// </summary>
        public String Host { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Port { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public HostingFxInterfaces.Status? MonitorServiceStatus { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ConnectorServiceState ConnectorServiceState { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ConnectorServiceConnectivityStatus ConnectorServiceStatus { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public RequestState[] Requests { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            String result = "Offline";
            if (MonitorServiceStatus.HasValue)
            {
                result = String.Format("{0} | {1}", MonitorServiceStatus.Value, ConnectorServiceStatus); 

            }
            return result;
        }
    }
}
