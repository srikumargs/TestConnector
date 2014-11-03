using System;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Taxes.Contracts.Data
{
    /// <summary>
    /// Tax Code Class
    /// </summary>
    public class TaxCodeClass: AbstractEntityInformation
    {
        /// <summary>
        /// Tax Class Type for this tax code class
        /// </summary>
        public TaxClassTypes ClassType { get; set; }

        /// <summary>
        /// Sequence
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Tax Class
        /// </summary>
        public String TaxClass { get; set; }

        /// <summary>
        /// Tax Code Class Description
        /// </summary>
        public String Description { get; set; }
    }
}
