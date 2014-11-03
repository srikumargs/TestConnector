using System.Collections.Generic;

namespace Sage.Connector.Configuration.Contracts.Data.Interfaces
{
    /// <summary>
    /// Company Connection Management Credentials needed
    /// This includes descriptions and current values
    /// </summary>
    public interface ICompanyManagementCredentialsNeeded
    {
        /// <summary>
        ///  Descriptions of the credentials to gather, includes information to support the UI
        /// </summary>
        IDictionary<string, object> Descriptions { get; set; }
        
        /// <summary>
        /// Current values for the credentials to gather.
        /// </summary>
        IDictionary<string, string> CurrentValues { get; set; }
    }
}
