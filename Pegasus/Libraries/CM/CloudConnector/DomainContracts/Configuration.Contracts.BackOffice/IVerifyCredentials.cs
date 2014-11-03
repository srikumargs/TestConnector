using System.Collections.Generic;
using Sage.Connector.Configuration.Contracts.Data.Responses;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.DomainContracts.BackOffice;

namespace Sage.Connector.Configuration.Contracts.BackOffice
{
    /// <summary>
    /// Support for getting and verifying credentials to setup back office connections.
    /// </summary>
    public interface IVerifyCredentials
    {
        /// <summary>
        /// Get the Credentials to request the ability  to create a connection to the back office
        /// </summary>
        /// <param name="sessionContext">session context to provide services</param>
        /// <returns>
        /// This is expected to return information about the need to check for administrator level credentials.
        /// However the back office may chose a lower level if appropriate.
        /// Additionally information about how to prompt for the needed credentials is expected to be returned.
        /// This information will be used by the connector to control access sensitive operations.
        /// These sensitive operations are connection creation, connection deletion, and other highly impact operations.        
        /// </returns>
         CompanyManagementCredentialsNeededResponse GetCompanyConnectionManagementCredentialsNeeded(ISessionContext sessionContext);

         /// <summary>
         /// Verify that the presented credentials will allow a connection to the back office be created.
         /// </summary>
         /// <param name="sessionContext">session context to provide services</param>
         /// <param name="companyManagementCredentials">company connection management credentials</param>
         /// <returns>if the credentials are valid and the possibly updated credentials for use</returns>
         ValidateCompanyConnectionManagementCredentialsResponse ValidateCompanyConnectionManagementCredentials(ISessionContext sessionContext, IDictionary<string, string> companyManagementCredentials);

         /// <summary>
         /// Gets the credentials and configuration to specify a connection to a back office company.
         /// </summary>
         /// <param name="sessionContext">session context to provide services</param>
         /// <param name="companyCredentials">Combination of management and prior connection credential. This enables edit cases to preserve already set values.</param>
         /// <returns>the needed credentials as descriptions and current values</returns>
        CompanyConnectionCredentialsNeededResponse GetCompanyConnectionCredentialsNeeded(ISessionContext sessionContext, CompanyCredentials companyCredentials);


        /// <summary>
        /// Validate that the presented credentials are allowed to access the back office company.
        /// </summary>
        /// <param name="sessionContext">session context to provide services</param>
        /// <param name="companyConnectionCredentials">company connection credentials</param>
        /// <returns>if the credentials are valid and the possibly updated credentials for use/persistence</returns>
         ValidateCompanyConnectionCredentialsResponse ValidateCompanyConnectionCredentials(ISessionContext sessionContext, IDictionary<string, string> companyConnectionCredentials);
    }
}
