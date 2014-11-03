using System;
using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "IntegrationEnabledStatusContract")]
    public enum IntegrationEnabledStatus
    {
        /// <summary>
        /// No IntegrationEnabledStatus (default value automatically initialized by runtime)
        /// </summary>
        [EnumMember]
        None = 0x0,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        CloudGetRequests = 0x1,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        BackOfficeProcessing = 0x2,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        CloudPutResponses = 0x4,
    }
}
