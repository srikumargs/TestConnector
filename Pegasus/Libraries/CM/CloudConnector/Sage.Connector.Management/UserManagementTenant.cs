using System;

namespace Sage.Connector.Management
{
    /// <summary>
    /// Class to expose the Tenant List from the cloud. 
    /// Data is ultimately sourced from User management team.
    /// </summary>
    public class UserManagementTenant
    {
        /// <summary>
        /// Gets or sets the tenant unique identifier.
        /// </summary>
        /// <value>
        /// The tenant unique identifier.
        /// </value>
        public Guid TenantGuid { get; internal set; }

        /// <summary>
        /// Gets or sets the name of the user management tenant.
        /// </summary>
        /// <value>
        /// The name of the user management tenant.
        /// </value>
        public string TenantName { get; internal set; }

        /// <summary>
        /// Gets or sets the registered connector id
        /// </summary>
        public string RegisteredConnectorId { get; internal set; }

        /// <summary>
        /// Gets or sets the registered company id
        /// </summary>
        public string RegisteredCompanyId { get; internal set; }
    }
}