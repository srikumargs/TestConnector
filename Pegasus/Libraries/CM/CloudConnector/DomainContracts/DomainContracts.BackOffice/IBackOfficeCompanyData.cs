using System;
using System.Collections.Generic;

namespace Sage.Connector.DomainContracts.BackOffice
{
    /// <summary>
    /// BackOffice configuration 
    /// </summary>
    public interface IBackOfficeCompanyData
    {
        /// <summary>
        /// Back Office Id is the id representing the specific back office. (Sage300Erp, Sage300CRE, Sage100ERP, Sage100CRE etc)
        /// </summary>
        String BackOfficeId { get; }

        /// <summary>
        /// Connection values
        /// </summary>
        /// <remarks>
        /// Will contain whatever values are provided by plugin for editing by configuration tool.
        /// These are expected to include:
        ///  UserId,
        ///  Password, 
        ///  CompanyId,
        ///  etc if relevant for the back office.
        /// </remarks>
        IDictionary<String, String> ConnectionCredentials { get; }

    }
}
