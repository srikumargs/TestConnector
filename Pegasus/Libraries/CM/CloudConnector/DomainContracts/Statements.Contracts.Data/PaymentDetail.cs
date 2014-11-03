using System;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Statements.Contracts.Data
{

    /// <summary>
    /// Payment Detail
    /// </summary>
    public class PaymentDetail
    {
        /// <summary>
        /// Cloud Id
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Invoice Reference
        /// </summary>
        public ExternalReference Invoice { get; set; }

        /// <summary>
        /// Applied Amount
        /// </summary>
        public decimal AppliedAmount { get; set; }
    }


}
