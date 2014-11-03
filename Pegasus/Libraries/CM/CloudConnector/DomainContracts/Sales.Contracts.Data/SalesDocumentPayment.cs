namespace Sage.Connector.Sales.Contracts.Data
{
    /// <summary>
    /// Payment information for the Sales Document 
    /// </summary>
    public class SalesDocumentPayment
    {
        /// <summary>
        /// Amount paid
        /// </summary>
        public decimal AmountPaid { get; set; }

        /// <summary>
        /// Payment Method 
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; }

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
        /// Valid Values numeric month as string "01" thru "12"
        /// </summary>
        public string ExpirationMonth { get; set; }

        /// <summary>
        /// Expiration Year
        /// 4-digit year, ex "2014", ex "2014", 
        /// </summary>
        public string ExpirationYear { get; set; }
    }
}
