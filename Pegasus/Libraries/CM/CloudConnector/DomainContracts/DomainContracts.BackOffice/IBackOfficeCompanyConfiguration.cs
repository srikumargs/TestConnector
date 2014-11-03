using System;

namespace Sage.Connector.DomainContracts.BackOffice
{
    /// <summary>
    /// BackOffice configuration 
    /// </summary>
    public interface IBackOfficeCompanyConfiguration
    {
        /// <summary>
        /// Back Office Id is the id representing the specific back office. (Sage300Erp, Sage300CRE, Sage100ERP, Sage100CRE)
        /// </summary>
        String BackOfficeId { get;}

        /// <summary>
        /// Get the Connection Credentials
        /// </summary>
        String ConnectionCredentials { get; }

        /// <summary>
        /// Gets the data path.
        /// This is where the DM can store files for the tenant/request
        /// </summary>
        /// <value>
        /// The data path base.
        /// </value>
        /// <remarks>
        /// This value if populated is the data storage path available to the DM layer.
        /// </remarks>
        String DataStoragePath { get; }
    }
}
