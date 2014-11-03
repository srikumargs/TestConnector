using System.Collections.Generic;
using Sage.Connector.Configuration.Contracts.Data.Interfaces;
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Configuration.Contracts.Data.Responses
{
    /// <summary>
    /// Return result for a CompanyManagementCredentialsNeeded call.
    /// </summary>
    public class CompanyManagementCredentialsNeededResponse : Response, ICompanyManagementCredentialsNeeded
    {
        /// <summary>
        /// Descriptions for the items to configure
        /// </summary>
        /// <remarks>
        /// If there are no visible descriptions no UI will be presented and default credentials will round trip.
        /// This allows the back office no concept of a an "admin" or management level to not present ux for it.
        /// This also allows back offices that do have an "admin" concept but do not wish to have the connector present it
        /// to provide the management credentials as the hidden defaults.
        /// </remarks>
        public IDictionary<string, object> Descriptions { get; set; }
        

        /// <summary>
        /// Default values for items to configure.
        /// </summary>
        /// <remarks>
        /// Note that by default any value here will round trip and be presented for validation unless changed by the user.
        /// Items not shown do to description values will be unchanged.
        /// </remarks>
        public IDictionary<string, string> CurrentValues {get; set;}
    }
}
