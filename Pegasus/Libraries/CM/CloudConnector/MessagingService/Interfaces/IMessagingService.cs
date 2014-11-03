using System;
using System.Net.Security;
using System.Runtime.Serialization;
using System.ServiceModel;
using Sage.Connector.Cloud.Integration.Interfaces.WebAPI;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Interfaces.Faults;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.MessagingService.Interfaces
{
    /// <summary>
    /// Augmented tenant registration with error information
    /// </summary>
    [DataContract]
    public class TenantRegistrationWithErrorInfo : TenantRegistration
    {
        /// <summary>
        /// Whether tenant registration was succesfully retrieved
        /// </summary>
        [DataMember(Name="succeeded")]
        public bool Succeeded { get; set; }
        /// <summary>
        /// An error message for failed tenant registration
        /// </summary>
        [DataMember(Name="errormessage")]
        public String ErrorMessage { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IMessagingService
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

        /// <summary>
        /// Clouds the tenant registration.
        /// </summary>
        /// <param name="siteAddress">The site address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="backOfficeCompanyId">The back office company identifier.</param>
        /// <param name="authenticationToken">The authentication token required for registration</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof (ArgumentNullFault))]
        [FaultContract(typeof (ArgumentFault))]
        TenantRegistrationWithErrorInfo CloudTenantRegistration(Uri siteAddress, String tenantId, String backOfficeCompanyId, String authenticationToken);

        /// <summary>
        /// Clear the connector-tenant registration
        /// </summary>
        /// <param name="siteAddress"></param>
        /// <param name="tenantId"></param>
        /// <param name="authenticationToken"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof (ArgumentNullFault))]
        [FaultContract(typeof (ArgumentFault))]
        TenantRegistrationWithErrorInfo ClearConnectorTenantRegistration(Uri siteAddress, String tenantId, String authenticationToken);

        /// <summary>
        /// Clouds the tenant list.
        /// </summary>
        /// <param name="siteAddress">The site address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ArgumentNullFault))]
        [FaultContract(typeof(ArgumentFault))]
        Cloud.Integration.Interfaces.WebAPI.TenantInfo[] CloudTenantList(Uri siteAddress, String tenantId, String claim);
    }
}


