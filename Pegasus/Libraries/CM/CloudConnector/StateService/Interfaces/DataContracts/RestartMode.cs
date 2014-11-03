using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "RestartModeContract")]
    public enum RestartMode
    {
        /// <summary>
        /// No RestartMode (default value automatically initialized by runtime)
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        RestartIntervalExceeded,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        RestartIntervalSpecified
    }
}
