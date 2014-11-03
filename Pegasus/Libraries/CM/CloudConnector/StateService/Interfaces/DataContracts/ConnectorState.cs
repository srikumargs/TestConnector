using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sage.Connector.Common;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ConnectorStateContract")]
    public sealed class ConnectorState : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileVersion"></param>
        /// <param name="productCode"></param>
        /// <param name="productName"></param>
        /// <param name="productVersion"></param>
        /// <param name="cloudInterfaceVersion"></param>
        /// <param name="backOfficeInterfaceVersion"></param>
        /// <param name="connectorMinimumBackOfficeIntegrationInterfaceVersion"></param>
        /// <param name="systemDateTimeUtc"></param>
        /// <param name="uptime"></param>
        /// <param name="maxUptimeBeforeRestart"></param>
        /// <param name="restartMode"></param>
        /// <param name="timeToBlackoutEnd"></param>
        /// <param name="connectorUpdateStatus"></param>
        /// <param name="connectorUpdateInfo"></param>
        /// <param name="subsystemHealthMessages"></param>
        /// <param name="integratedConnectionStates"></param>
        public ConnectorState(
            String fileVersion,
            String productCode,
            String productName,
            String productVersion,
            String cloudInterfaceVersion,
            String backOfficeInterfaceVersion,
            String connectorMinimumBackOfficeIntegrationInterfaceVersion,
            DateTime systemDateTimeUtc,
            TimeSpan uptime,
            TimeSpan? maxUptimeBeforeRestart,
            RestartMode restartMode,
            TimeSpan? timeToBlackoutEnd,
            ConnectorUpdateStatus connectorUpdateStatus,
            UpdateInfo connectorUpdateInfo,
            IEnumerable<SubsystemHealthMessage> subsystemHealthMessages,
            IEnumerable<IntegratedConnectionState> integratedConnectionStates)
        {
            FileVersion = fileVersion;
            ProductCode = productCode;
            ProductName = productName;
            ProductVersion = productVersion;
            CloudInterfaceVersion = cloudInterfaceVersion;
            ConnectorBackOfficeIntegrationInterfaceVersion = backOfficeInterfaceVersion;

            ConnectorMinimumBackOfficeIntegrationInterfaceVersion = connectorMinimumBackOfficeIntegrationInterfaceVersion;

            SystemDateTimeUtc = systemDateTimeUtc;
            Uptime = uptime;
            MaxUptimeBeforeRestart = maxUptimeBeforeRestart;
            RestartMode = restartMode;
            TimeToBlackoutEnd = timeToBlackoutEnd;
            ConnectorUpdateStatus = connectorUpdateStatus;
            ConnectorUpdateInfo = connectorUpdateInfo;
            SubsystemHealthMessages = subsystemHealthMessages.ToArray();
            IntegratedConnectionStates = integratedConnectionStates.ToArray();
            CloudConnectivityStatus = ComputeCloudConnectivityStatus();
        }

        /// <summary>
        /// Initializes a new instance of the ConnectorServiceState class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public ConnectorState(ConnectorState source, IEnumerable<PropertyTuple> propertyTuples)
        {
            FileVersion = source.FileVersion;
            ProductCode = source.ProductCode;
            ProductName = source.ProductName;
            ProductVersion = source.ProductVersion;
            CloudInterfaceVersion = source.CloudInterfaceVersion;
            ConnectorBackOfficeIntegrationInterfaceVersion = source.ConnectorBackOfficeIntegrationInterfaceVersion;
            ConnectorMinimumBackOfficeIntegrationInterfaceVersion = source.ConnectorMinimumBackOfficeIntegrationInterfaceVersion;
            SystemDateTimeUtc = source.SystemDateTimeUtc;
            Uptime = source.Uptime;
            MaxUptimeBeforeRestart = source.MaxUptimeBeforeRestart;
            RestartMode = source.RestartMode;
            TimeToBlackoutEnd = source.TimeToBlackoutEnd;
            ConnectorUpdateStatus = source.ConnectorUpdateStatus;
            ConnectorUpdateInfo = source.ConnectorUpdateInfo;
            SubsystemHealthMessages = source.SubsystemHealthMessages.ToArray();
            IntegratedConnectionStates = source.IntegratedConnectionStates.ToArray();
            CloudConnectivityStatus = ComputeCloudConnectivityStatus();
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(ConnectorState));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "FileVersion", IsRequired = true, Order = 0)]
        public String FileVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ProductCode", IsRequired = true, Order = 1)]
        public String ProductCode { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ProductName", IsRequired = true, Order = 2)]
        public String ProductName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ProductVersion", IsRequired = true, Order = 3)]
        public String ProductVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CloudInterfaceVersion", IsRequired = true, Order = 4)]
        public String CloudInterfaceVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ConnectorBackOfficeIntegrationInterfaceVersion", IsRequired = true, Order = 5)]
        public String ConnectorBackOfficeIntegrationInterfaceVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ConnectorMinimumBackOfficeIntegrationInterfaceVersion", IsRequired = true, Order = 6)]
        public String ConnectorMinimumBackOfficeIntegrationInterfaceVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "SystemDateTimeUtc", IsRequired = true, Order = 7)]
        public DateTime SystemDateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "Uptime", IsRequired = true, Order = 8)]
        public TimeSpan Uptime { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "MaxUptimeBeforeRestart", IsRequired = true, Order = 9)]
        public TimeSpan? MaxUptimeBeforeRestart { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "RestartMode", IsRequired = true, Order = 10)]
        public RestartMode RestartMode { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "TimeToBlackoutEnd", IsRequired = true, Order = 11)]
        public TimeSpan? TimeToBlackoutEnd { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ConnectorUpdateStatus", IsRequired = true, Order = 12)]
        public ConnectorUpdateStatus ConnectorUpdateStatus { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ConnectorUpdateInfo", IsRequired = true, Order = 13)]
        public UpdateInfo ConnectorUpdateInfo { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "SubsystemHealthMessages", IsRequired = true, Order = 14)]
        public SubsystemHealthMessage[] SubsystemHealthMessages { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "IntegratedConnectionStates", IsRequired = true, Order = 15)]
        public IntegratedConnectionState[] IntegratedConnectionStates { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CloudConnectivityStatus", IsRequired = true, Order = 16)]
        public CloudConnectivityStatus CloudConnectivityStatus { get; private set; }

        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        private CloudConnectivityStatus ComputeCloudConnectivityStatus()
        {
            var result = CloudConnectivityStatus.Normal;

            // TODO:  blackout?

            if (IntegratedConnectionStates != null && IntegratedConnectionStates.Length > 0)
            {
                foreach (object state in IntegratedConnectionStates)
                {
                    var integratedConnectionState = (state as IntegratedConnectionState);
                    var tenantConnectivityStatusValue = integratedConnectionState.TenantConnectivityStatus;
                    var tenantConnectivityStatusAsCloudConnectivityStatus = CloudConnectivityStatus.None;
                    switch (tenantConnectivityStatusValue)
                    {
                        case TenantConnectivityStatus.None:
                            tenantConnectivityStatusAsCloudConnectivityStatus = CloudConnectivityStatus.None;
                            break;
                        case TenantConnectivityStatus.LocalNetworkUnavailable:
                            tenantConnectivityStatusAsCloudConnectivityStatus = CloudConnectivityStatus.LocalNetworkUnavailable;
                            break;
                        case TenantConnectivityStatus.InternetConnectionUnavailable:
                            tenantConnectivityStatusAsCloudConnectivityStatus = CloudConnectivityStatus.InternetConnectionUnavailable;
                            break;
                        case TenantConnectivityStatus.CloudUnavailable:
                            tenantConnectivityStatusAsCloudConnectivityStatus = CloudConnectivityStatus.CloudUnavailable;
                            break;
                        case TenantConnectivityStatus.GatewayServiceUnavailable:
                            tenantConnectivityStatusAsCloudConnectivityStatus = CloudConnectivityStatus.GatewayServiceUnavailable;
                            break;
                        case TenantConnectivityStatus.CommunicationFailure:
                            tenantConnectivityStatusAsCloudConnectivityStatus = CloudConnectivityStatus.CommunicationFailure;
                            break;
                        case TenantConnectivityStatus.IncompatibleClient:
                        case TenantConnectivityStatus.InvalidConnectionInformation:
                        case TenantConnectivityStatus.Reconfigure:
                        case TenantConnectivityStatus.TenantDisabled:
                        case TenantConnectivityStatus.Normal:
                            tenantConnectivityStatusAsCloudConnectivityStatus = CloudConnectivityStatus.Normal;
                            break;
                        //default:
                        //    using (var lm = new LogManager())
                        //    {
                        //        lm.WriteError(null, "Unknown result: {0}", tenantConnectionStatusValue.ToString());
                        //    }
                        //    break;
                    }

                    switch (tenantConnectivityStatusAsCloudConnectivityStatus)
                    {
                        case CloudConnectivityStatus.None:
                        //case CloudConnectivityStatus.Blackout:
                        case CloudConnectivityStatus.LocalNetworkUnavailable:
                        case CloudConnectivityStatus.InternetConnectionUnavailable:
                        case CloudConnectivityStatus.CloudUnavailable:
                        case CloudConnectivityStatus.GatewayServiceUnavailable:
                        case CloudConnectivityStatus.CommunicationFailure:
                            if (result > tenantConnectivityStatusAsCloudConnectivityStatus)
                            {
                                // lower the aggregate result down to the current value at best
                                result = tenantConnectivityStatusAsCloudConnectivityStatus;
                            }
                            break;
                        case CloudConnectivityStatus.Normal:
                            // do nothing, we are Normal by default
                            break;
                        //default:
                        //    using (var lm = new LogManager())
                        //    {
                        //        lm.WriteError(null, "Unknown result: {0}", tenantConnectionStatusValue.ToString());
                        //    }
                        //    break;
                    }
                }
            }
            else
            {
                result = CloudConnectivityStatus.None;
            }

            return result;
        }
    }
}
