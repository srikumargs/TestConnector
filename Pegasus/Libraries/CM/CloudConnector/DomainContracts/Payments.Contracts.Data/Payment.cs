using Sage.Connector.DomainContracts.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sage.Connector.Payments.Contracts.Data
{
    /// <summary>
    /// Payment
    /// </summary>
    public class Payment : AbstractEntityInformation
    {
        private ICollection<PaymentDetail> _details;

        /// <summary>
        /// Cloud Id not to be modified.
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Customer reference
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
        public String Reference { get; set; }

        /// <summary>
        /// Authorization code
        /// </summary>
        public String AuthorizationCode { get; set; }

        /// <summary>
        /// Last 4 digits for the Credit Card
        /// </summary>
        public String CreditCardLast4 { get; set; }

        /// <summary>
        /// Expiration Month 
        /// Valid Values numeric month as string "01" thru "12"
        /// </summary>
        public String ExpirationMonth { get; set; }

        /// <summary>
        /// Expiration 4-digit Year 
        /// 4 digit year as string, ex "2014"
        /// </summary>
        public String ExpirationYear { get; set; }

        /// <summary>
        /// Payment detail.  Will be empty if statement 
        /// </summary>
        public ICollection<PaymentDetail> Details
        {
            get { return _details ?? (_details = new Collection<PaymentDetail>()); }
            set { _details = value; }
        } 
    }

}
