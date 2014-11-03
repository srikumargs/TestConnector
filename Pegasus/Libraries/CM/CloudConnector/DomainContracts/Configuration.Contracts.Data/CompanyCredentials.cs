using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage.Connector.Configuration.Contracts.Data
{
    /// <summary>
    /// Credentials for both connection management and company connection.
    /// </summary>
    public class CompanyCredentials
    {
        /// <summary>
        /// Company management credentials. These will be validated before processing the request
        /// </summary>
        public IDictionary<string, string> CompanyManagementCredentials;

        /// <summary>
        /// Company Connection Credentials. These will be supplied in an edit case. 
        /// </summary>
        public IDictionary<string, string> CompanyConnectionCredentials;
    }
}
