using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Invoices.Contracts.Data
{
    /// <summary>
    /// Invoice Balance
    /// </summary>
    public class InvoiceBalance : AbstractEntityInformation
    {
        /// <summary>
        /// Invoice External Id
        /// </summary>
        [ExternalIdReference]
        public string Invoice { get; set; }                           // id of invoice (link to Mongo for including invoice information in ClickToPayNow - s/b Guid?

        /// <summary>
        /// Invoice Number
        /// </summary>
        public string InvoiceNumber { get; set; }                       // Open Invoice

        /// <summary>
        /// Customer External Id
        /// </summary>
        [ExternalIdReference]
        public string Customer { get; set; }                          // several places access the old Invoice.Customer.<property> where Customer was an entity reference.  Now will be a guid.

        /// <summary>
        /// Invoice Date
        /// </summary>
        public System.DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Invoice Due Date
        /// </summary>
        public System.DateTime? InvoiceDueDate { get; set; }

        /// <summary>
        /// Discount Due Date
        /// </summary>
        public System.DateTime? DiscountDueDate { get; set; }

        /// <summary>
        /// Payment Discount
        /// </summary>
        public decimal PaymentDiscount { get; set; }                    // Open Invoice***

        /// <summary>
        /// Balance
        /// </summary>
        public decimal Balance { get; set; }                            // Open Invoice***

        /// <summary>
        /// Total
        /// </summary>
        public decimal Total { get; set; }                              // needed for SB&P, currently SB&P retrieves this from the Invoice model

    }
}

