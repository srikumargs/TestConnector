using System;
using System.AddIn.Contract;

namespace Sage.Connector.ProcessExecution.Interfaces
{
    /// <summary>
    /// Back office company configuration contract
    /// </summary>
    public interface IBackOfficeCompanyConfiguration : IContract
    {   
        // TODO KMS: should this be IBackOffice config as opposed to company connection config? 
        // TODO        Back office config, we have specifics related to the back office that is independent
        // TODO        of the company.  
        // TODO        Back office company config is access to the back office for a specific company and its 
        // TODO        configuration
        // 

        /// <summary>
        /// Back Office Id is the id representing the specific back office. (Sage300ERP, Sage300CRE, Sage100ERP, Sage100CRE)
        ///</summary>
        String BackOfficeId { get;  }

        /// <summary>
        /// Credentials for the connection to the back office
        /// </summary>
        String ConnectionCredentials { get; }

        /// <summary>
        /// Gets the data storage path.
        /// </summary>
        /// <value>
        /// The data storage path.
        /// </value>
        String DataStoragePath { get; }
    }
}