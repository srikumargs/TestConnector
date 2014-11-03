using System.Collections.Generic;
using Sage.Connector.Configuration.Contracts.Data.Interfaces;
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Configuration.Contracts.Data.Responses
{
    /// <summary>
    /// Return result for a CompanyConnectionCredentialsNeeded call.
    /// </summary>
    public class CompanyConnectionCredentialsNeededResponse : Response, ICompanyConnectionCredentialsNeeded
    {
        /// <summary>
        /// Descriptions of the credentials to gather, includes information to support the UI
        /// </summary>
        public IDictionary<string, object> Descriptions { get; set; }

        /// <summary>
        /// Current values for the credentials to gather.
        /// </summary>
        public IDictionary<string, string> CurrentValues { get; set; }
    }
}
