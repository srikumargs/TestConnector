using System;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Sales.Contracts.Data
{
    /// <summary>
    /// Sales Document 
    /// </summary>
    public abstract class AbstractSalesDocument : AbstractEntityInformation
    {
        /// <summary>
        /// Cloud data.  Back office should not modify
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Sales Document Description
        /// </summary>
        public String Description { get; set; }                         //get
        // changed QuoteNumber to more generic DocumentNumber to share between quotes/orders (if schema remains same on other end need mapping

        /// <summary>
        /// Sales Document Number
        /// </summary>
        public int DocumentNumber { get; set; }                            //get (Sage300 only - used for logging errors for an invalid currencyCode that does match ERP)


        /// <summary>
        /// Customer Reference information
        /// </summary>
        public ExternalReference Customer { get; set; }                 //Customer.ExternalId get

        /// <summary>
        /// Shipping Address
        /// </summary>
        public Address ShippingAddress { get; set; }                    //ShippingAddress.PostalCode, .City, .StateProvince, .Country, .Name, .Street1-4, .Email, .Contact.FirstName, .Contact.LastName


        /// <summary>
        /// Sales Document Tax amount
        /// </summary>
        public decimal? Tax { get; set; }                      //set (quote only) //get (order with deposit - assume tax on order is gospel and use since payament was taken)

        /// <summary>
        /// Shipping and Handling
        /// </summary>
        public decimal? SandH { get; set; }                    //get //set (quote only)

        //changed QuoteTotal to more generic DocumentTotal to share between quotes/orders (if schema remains same on other end need mapping
        /// <summary>
        /// Document Total, for example Quote Total for a Quote Document, or Order Total for an Order document
        /// </summary>
        public decimal DocumentTotal { get; set; }                         //set (quote only)

        /// <summary>
        /// Subtotal amount
        /// </summary>
        public decimal SubTotal { get; set; }                           //set (quote only)

        /// <summary>
        /// Expiry Date
        /// </summary>
        public DateTime? ExpiryDate { get; set; }       //get //set (by 100 if null will set to order date (today)

        /// <summary>
        /// Submitted date
        /// </summary>
        public DateTime? SubmittedDate { get; set; }    //get //set (by 100 if null will set to order date (today)

        /// <summary>
        /// Discount percentage
        /// </summary>
        public decimal? DiscountPercent { get; set; }          //get   //set (quote only)


        /// <summary>
        /// Payment Informaton
        /// </summary>
        public SalesDocumentPayment Payment { get; set; }
    }

}
