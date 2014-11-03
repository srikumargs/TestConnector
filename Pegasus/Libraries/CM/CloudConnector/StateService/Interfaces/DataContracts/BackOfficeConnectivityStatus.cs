using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "BackOfficeConnectivityStatusContract")]
    public enum BackOfficeConnectivityStatus
    {
        /// <summary>
        /// No BackOfficeConnectionStatus (default value automatically initialized by runtime)
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Incompatible=1,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        ConnectivityBroken=2,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Normal=3
    }
}
