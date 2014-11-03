using System.Collections.Generic;
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Configuration.Contracts.Data.Responses
{
    /// <summary>
    /// Response for ValidateCompanyConnectionManagementCredentials call
    /// </summary>
    public class ValidateCompanyConnectionManagementCredentialsResponse : Response
    {
        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        /// <value>
        /// The credentials.
        /// </value>
        public IDictionary<string, string> Credentials { get; set; }

    }
}
