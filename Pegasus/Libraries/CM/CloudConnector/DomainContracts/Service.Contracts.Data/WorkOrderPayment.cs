namespace Sage.Connector.Service.Contracts.Data
{
    /// <summary>
    /// Payment information for the Sales Document 
    /// </summary>
    public class WorkOrderPayment
    {
        //potential different types (PaidCC, PaidOther, OnAccount)

        /// <summary>
        /// Payment Method 
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; } 
        
        /// <summary>
        /// Amount paid
        /// </summary>
        public decimal AmountPaid { get; set; }

        /// <summary>
        /// Payment Reference
        /// </summary>
        public string PaymentReference { get; set; }

        /// <summary>
        /// Authorization Code
        /// </summary>
        public string AuthorizationCode { get; set; }

        /// <summary>
        /// Last four digits of the Credit Card
        /// </summary>
        public string CreditCardLast4 { get; set; }

        /// <summary>
        /// Expiration Month
        /// TODO KMS: what is the value supposed to be  "01" or ?? 
        /// </summary>
        public string ExpirationMonth { get; set; }

        /// <summary>
        /// Expiration Year
        /// 4-digit year as a string, ex "2014"
        /// </summary>
        public string ExpirationYear { get; set; }
    }
}
