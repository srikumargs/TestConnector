using Sage.Connector.DomainContracts.Data;
using System;

namespace Sage.Connector.Payments.Contracts.Data
{
    /// <summary>
    /// Payment Details
    /// </summary>
    public  class PaymentDetail : AbstractEntityInformation
    {
        /// <summary>
        /// Cloud Id not to be modified
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Invoice External reference information
        /// </summary>
        public ExternalReference Invoice { get; set; }

        /// <summary>
        /// Applied Amount
        /// </summary>
        public decimal AppliedAmount { get; set; }
    }
}
