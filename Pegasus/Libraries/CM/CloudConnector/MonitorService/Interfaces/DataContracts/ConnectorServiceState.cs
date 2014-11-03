using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sage.Connector.Common;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.LinkedSource;
using HostingFxInterfaces = Sage.CRE.HostingFramework.Interfaces;

namespace Sage.Connector.MonitorService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ConnectorServiceStateContract")]
    public sealed class ConnectorServiceState : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectorServiceConnectivityStatus"></param>
        /// <param name="connectorState"></param>
        /// <param name="monitorServiceFileVersion"></param>
        /// <param name="serviceInfos"></param>
        public ConnectorServiceState(
            ConnectorServiceConnectivityStatus connectorServiceConnectivityStatus,
            ConnectorState connectorState,
            String monitorServiceFileVersion,
            IEnumerable<HostingFxInterfaces.ServiceInfo> serviceInfos)
        {
            ConnectorServiceConnectivityStatus = connectorServiceConnectivityStatus;
            ConnectorState = connectorState;
            MonitorServiceFileVersion = monitorServiceFileVersion;
            ServiceInfos = (serviceInfos != null) ? serviceInfos.ToArray() : null;
        }

        /// <summary>
        /// Initializes a new instance of the ConnectorServiceState class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public ConnectorServiceState(ConnectorServiceState source, IEnumerable<PropertyTuple> propertyTuples)
        {
            ConnectorServiceConnectivityStatus = source.ConnectorServiceConnectivityStatus;
            ConnectorState = source.ConnectorState;
            MonitorServiceFileVersion = source.MonitorServiceFileVersion;
            ServiceInfos = source.ServiceInfos;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(ConnectorServiceState));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ConnectorServiceConnectivityStatus", IsRequired = true, Order = 0)]
        public ConnectorServiceConnectivityStatus ConnectorServiceConnectivityStatus { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ConnectorState", IsRequired = true, Order = 1)]
        public ConnectorState ConnectorState { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "MonitorServiceFileVersion", IsRequired = true, Order = 2)]
        public String MonitorServiceFileVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ServiceInfos", IsRequired = true, Order = 3)]
        public HostingFxInterfaces.ServiceInfo[] ServiceInfos { get; private set; }

        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
