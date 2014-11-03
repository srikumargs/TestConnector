using System;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Customers.Contracts.Data
{

    /// <summary>
    /// Customer Tax Class 
    /// </summary>
    public class CustomerTaxClass : AbstractEntityInformation
    {
        /// <summary>
        /// Tax Code External ID
        /// </summary>
        [ExternalIdReference]
        public String TaxCode { get; set; }

        /// <summary>
        /// Tax Class
        /// </summary>
       public string TaxClass { get; set; }
    }
}
