using System;
using System.Net.Security;
using System.ServiceModel;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Interfaces.Faults;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface ITenantValidationService
    {
        /// <summary>
        /// Validate the tenant id and premise key as valid.
        /// </summary>
        /// <param name="siteAddress"></param>
        /// <param name="tenantId"></param>
        /// <param name="premiseId"></param>
        /// <param name="wireClaim"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ArgumentNullFault))]
        [FaultContract(typeof(ArgumentFault))]
        ValidateTenantConnectionResponse ValidateTenantConnection(String siteAddress, String tenantId, String premiseId, String wireClaim);
    }
}
