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
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "CloudConnectivityStatusContract")]
    public enum CloudConnectivityStatus
    {
        /// <summary>
        /// No CloudConnectivityStatus (default value automatically initialized by runtime)
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Blackout > None
        /// </remarks>
        [EnumMember]
        Blackout,

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// LocalNetworkUnavailable > Blackout > None
        /// </remarks>
        [EnumMember]
        LocalNetworkUnavailable,

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// InternetConnectionUnavailable > LocalNetworkUnavailable > Blackout > None
        /// </remarks>
        [EnumMember]
        InternetConnectionUnavailable,

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// CloudUnavailable > InternetConnectionUnavailable > LocalNetworkUnavailable > Blackout > None
        /// </remarks>
        [EnumMember]
        CloudUnavailable,

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// GatewayUnavailable > CloudUnavailable > InternetConnectionUnavailable > LocalNetworkUnavailable > Blackout > None
        /// </remarks>
        [EnumMember]
        GatewayServiceUnavailable,

        /// <summary>
        /// Unknown situation, catch-all result
        /// </summary>
        /// <remarks>
        /// CommunicationFailure > GatewayUnavailable > CloudUnavailable > InternetConnectionUnavailable > LocalNetworkUnavailable > Blackout > None
        /// </remarks>
        [EnumMember]
        CommunicationFailure,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Normal
    }
}
