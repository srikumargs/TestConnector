using System;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Taxes.Contracts.Data
{
    /// <summary>
    /// Tax Schedule Detail
    /// </summary>
    public class TaxScheduleDetail : AbstractEntityInformation
    {
        /// <summary>
        /// Tax Code External Id used as reference for Tax Code
        /// </summary>
        [ExternalIdReference]
        public String TaxCode { get; set; }

        /// <summary>
        /// Tax Code Sequence
        /// </summary>
        public int TaxCodeSequence { get; set; }


    }
}
