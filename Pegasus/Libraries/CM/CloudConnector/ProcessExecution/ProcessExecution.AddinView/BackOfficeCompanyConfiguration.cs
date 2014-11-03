using System;
using Sage.Connector.DomainContracts.BackOffice;

namespace Sage.Connector.ProcessExecution.AddinView
{
    /// <summary>
    /// Back office company configuration abstract
    /// </summary>
    public abstract class BackOfficeCompanyConfiguration : IBackOfficeCompanyConfiguration
    {

        /// <summary>
        /// Back Office Id is the id representing the specific back office.
        /// 
        /// TODO KMS: should this be IBackOffice config as opposed to company connection config? 
        /// TODO KMS: back office company config is access to the back office for a specific company and its 
        /// TODO KMS: configuration
        /// </summary>
        public abstract String BackOfficeId { get; }

        /// <summary>
        /// Get set the back office connection credentials as a json string
        /// Look at if we want to change this representation
        /// </summary>
        public abstract String ConnectionCredentials { get; }

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
        public abstract String DataStoragePath { get; }
    }

   
}
