using System;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Taxes.Contracts.Data
{
    /// <summary>
    /// Tax Code Detail 
    /// </summary>
    public class TaxCodeDetail: AbstractEntityInformation
    {
        /// <summary>
        /// Item Tax Class
        /// </summary>
        public String ItemTaxClass { get; set; }

        /// <summary>
        /// Customer Tax Class
        /// </summary>
        public String CustomerTaxClass { get; set; }

        /// <summary>
        /// Tax Code Rate
        /// </summary>
        public decimal Rate { get; set; }
    }
}
