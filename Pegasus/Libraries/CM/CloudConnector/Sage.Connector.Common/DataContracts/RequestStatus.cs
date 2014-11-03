using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.Common.DataContracts
{
    /// <summary>
    /// TODO:  v1ok - evaluate consolidation of ActivityEntryStatus (an internal implementation detail of the Connector service) with the RequestStatus
    /// parallel to it located in Sage.Connector.Commmon (which is used to surface request status information to the Monitor tray app)
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "RequestStatusContract")]
    public enum RequestStatus
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
        InProgress,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        InProgressBindableWorkProcessing,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        InProgressMediationBoundWorkProcessing,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        CompletedWithSuccessResponse,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        CompletedWithErrorResponse,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        CompletedWithCancelResponse
    }
}
