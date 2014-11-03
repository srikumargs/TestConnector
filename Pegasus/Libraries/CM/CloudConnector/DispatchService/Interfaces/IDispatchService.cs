using System.Collections.Generic;
using System.Net.Security;
using System.ServiceModel;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.DispatchService.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IDispatchService
    {
        /// <summary>
        /// Request cancellation of any in progress work for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [OperationContract]
        void CancelInProgressWork(string tenantId);

        /// <summary>
        /// Request cancellation of a specific request
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="activityTrackingContextId"></param>
        [OperationContract]
        void CancelWork(string tenantId, string activityTrackingContextId);

        /// <summary>
        /// The list of in progress work for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<RequestWrapper> InProgressWork(string tenantId);
    }
}