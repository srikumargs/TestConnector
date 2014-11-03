using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ConnectorUpdateStatusContract")]
    public enum ConnectorUpdateStatus
    {
        /// <summary>
        /// No ConnectorUpdateStatus (default value automatically initialized by runtime)
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        UpdateAvailable,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        UpdateRequired
    }
}
