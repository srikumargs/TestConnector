using System.Collections.Generic;
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Configuration.Contracts.Data.Responses
{
    /// <summary>
    /// Response for ValidateCompanyConnectionCredentials call
    /// </summary>
    public class ValidateCompanyConnectionCredentialsResponse : Response
    {
        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        /// <value>
        /// The credentials.
        /// </value>
        public IDictionary<string, string> Credentials { get; set; }

        /// <summary>
        /// This should be the user facing name of the back office company associated with the credentials
        /// </summary>
        public string CompanyNameForDisplay { get; set; }

        /// <summary>
        /// Identifier that uniquely identifies a given back office company.
        /// This is used to help make sure that two connections do not inadvertently point to the same back office company. 
        /// If a connection is made, destroyed and recreated it is expected that the Identifier will be consistent.
        /// </summary>
        public string CompanyUnqiueIdentifier { get; set; }
    }
}
