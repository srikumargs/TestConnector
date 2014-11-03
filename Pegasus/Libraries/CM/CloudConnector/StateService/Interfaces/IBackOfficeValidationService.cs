using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.ServiceModel;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Interfaces.Faults;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces
{
    /// <summary>
    /// CRUD Management of Premise-Cloud Configurations
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IBackOfficeValidationService
    {

        /// <summary>
        /// Get the set of available back office plugins
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BackOfficePluginsResponse GetBackOfficePlugins();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="companyConnectionCredentials"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ArgumentNullFault))]
        [FaultContract(typeof(ArgumentFault))]
        ValidateBackOfficeConnectionResponse ValidateBackOfficeConnection(String backOfficeId, IDictionary<string, string> companyConnectionCredentials);
        //note overlap with ValidateConnectionCredentials Pick one...

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="companyConnectionCredentials"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ArgumentNullFault))]
        [FaultContract(typeof(ArgumentFault))]
        ValidateBackOfficeConnectionResponse ValidateBackOfficeConnectionCredentialsAsString(String backOfficeId, string companyConnectionCredentials);
        //note overlap with ValidateConnectionCredentials Pick one...

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ArgumentNullFault))]
        [FaultContract(typeof(ArgumentFault))]
        ValidateBackOfficeAdminCredentialsResponse ValidateBackOfficeAdminCredentials(String backOfficeId, IDictionary<string, string> credentials);
        //note overlap with ValidateManagementCredentials Pick one...

        /// <summary>
        /// Get the Credentials to request the ability  to create a connection to the back office
        /// </summary>
        /// <returns>
        /// This is expected to return information about the need to check for administrator level credentials.
        /// However the back office may chose a lower level if appropriate.
        /// Additionally information about how to prompt for the needed credentials is expected to be returned.
        /// This information will be used by the connector to control access sensitive operations.
        /// These sensitive operations are connection creation, connection deletion, and other highly impact operations.        
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ArgumentNullFault))]
        [FaultContract(typeof(ArgumentFault))]
        ManagementCredentialsNeededResponse GetManagementCredentialsNeeded(string backOfficeId);

        /// <summary>
        /// Gets the credentials and configuration to specify a connection to a back office company.
        /// </summary>
        /// <returns>
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ArgumentNullFault))]
        [FaultContract(typeof(ArgumentFault))]
        ConnectionCredentialsNeededResponse GetConnectionCredentialsNeeded(string backOfficeId, IDictionary<string, string> companyManagementCredentials, IDictionary<string, string> companyConnectionCredentials);
    }
}
