using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Statements.Contracts.Data
{
    /// <summary>
    /// Payment
    /// </summary>
    public class Payment
    {
        private ICollection<PaymentDetail> _details;

        /// <summary>
        /// Cloud Id
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Customer Reference
        /// </summary>
        public ExternalReference Customer { get; set; }

        /// <summary>
        /// Amount Paid
        /// </summary>
        public decimal AmountPaid { get; set; }

        /// <summary>
        /// Transaction Date
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Payment Type 
        /// </summary>
        public PaymentType Type { get; set; }

        /// <summary>
        /// Payment Reference
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Authorization code
        /// </summary>
        public string AuthorizationCode { get; set; }

        /// <summary>
        /// Last 4 digits of the Credit Card
        /// </summary>
        public string CreditCardLast4 { get; set; }

        /// <summary>
        /// Expiration Month
        /// Valid Values numeric month as string "01" thru "12"
        /// </summary>
        public string ExpirationMonth { get; set; }

        /// <summary>
        /// Expiration Year
        /// 4-digit year as string, ex "2014"
        /// </summary>
        public string ExpirationYear { get; set; }

        /// <summary>
        /// Collection Of Payment Details
        /// </summary>
        public ICollection<PaymentDetail> Details
        {
            get { return _details ?? (_details = new Collection<PaymentDetail>()); }
            set { _details = value; }
        } //can be empty if a statement
    }
}
