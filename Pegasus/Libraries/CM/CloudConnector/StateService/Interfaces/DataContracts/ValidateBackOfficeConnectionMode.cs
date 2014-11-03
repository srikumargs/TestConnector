using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ValidateBackOfficeConnectionModeContract")]
    public enum ValidateBackOfficeConnectionMode
    {
        /// <summary>
        /// No ValidateBackOfficeConnectionMode (default value automatically initialized by runtime)
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        TestOnly,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        ValidateAndUpdateState
    }
}
