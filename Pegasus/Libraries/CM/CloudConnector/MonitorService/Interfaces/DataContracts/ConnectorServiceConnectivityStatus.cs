using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.MonitorService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ConnectorServiceConnectivityStatusContract")]
    public enum ConnectorServiceConnectivityStatus
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        ServiceNotRegistered,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        ServiceNotRunning,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        ServiceNotReady,

        /// <summary>
        /// The server is currently accessible
        /// </summary>
        [EnumMember]
        Connected,
               
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        ConnectivityError

    }
}
