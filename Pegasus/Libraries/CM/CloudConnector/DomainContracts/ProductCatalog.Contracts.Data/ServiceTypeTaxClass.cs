using System;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.ProductCatalog.Contracts.Data
{
    /// <summary>
    /// Service Type Tax Class
    /// </summary>
    public class ServiceTypeTaxClass : AbstractEntityInformation
    {
        /// <summary>
        /// Tax Code External Id
        /// </summary>
        [ExternalIdReference]
        public String TaxCode { get; set; }

        /// <summary>
        /// Tax Class
        /// </summary>
        public String TaxClass { get; set; }
    }

}
