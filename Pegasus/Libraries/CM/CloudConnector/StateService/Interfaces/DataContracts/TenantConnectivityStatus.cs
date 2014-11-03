using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Note we have a particular sensitivity to the specific enum values in this enumeration callers may want
    /// to use comparison operations (e.g., ">") to figure out aggregate state across all tenant connections
    /// </remarks>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "TenantConnectivityStatusContract")]
    public enum TenantConnectivityStatus
    {
        /// <summary>
        /// No TenantConnectivityStatus (default value automatically initialized by runtime)
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        LocalNetworkUnavailable,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        InternetConnectionUnavailable,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        CloudUnavailable,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        GatewayServiceUnavailable,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        CommunicationFailure,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        InvalidConnectionInformation,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        IncompatibleClient,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Reconfigure,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        TenantDisabled,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Normal
    }
}
