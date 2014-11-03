using Sage.Connector.LinkedSource;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Interfaces.Faults;
using System;
using System.Net.Security;
using System.ServiceModel;

namespace Sage.Connector.StateService.Interfaces
{
    /// <summary>
    /// CRUD Management of Premise-Cloud Configurations
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IFeatureService
    {

        /// <summary>
        /// Get the set Feature payload response
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ArgumentNullFault))]
        [FaultContract(typeof(ArgumentFault))]
        FeatureResponse GetFeatureResponse(String backOfficeId, String backOfficeCredentials, String featureId, String tenantId, String payload);

  
    }
}
