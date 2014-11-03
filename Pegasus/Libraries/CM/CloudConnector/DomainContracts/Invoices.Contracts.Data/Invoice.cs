using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Invoices.Contracts.Data
{
    /// <summary>
    /// Invoice contract
    /// </summary>
    public class Invoice : AbstractEntityInformation
    {
        private ICollection<InvoiceDetail> _details;

        /// <summary>
        /// Invoice Nubmer
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Invoice Due Date
        /// </summary>
        public DateTime? InvoiceDueDate { get; set; }

        /// <summary>
        /// Invoice Date
        /// </summary>
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Discount Due Date
        /// </summary>
        public DateTime? DiscountDueDate { get; set; }

        /// <summary>
        /// Customer External Id 
        /// </summary>
        [ExternalIdReference]
        public String Customer { get; set; }


        /// <summary>
        /// Customer External Reference as the displayable back office key
        /// </summary>
        public string CustomerExternalReference { get; set; } 

        /// <summary>
        /// PO Number
        /// </summary>
        public string PoNumber { get; set; }

        /// <summary>
        /// Comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Taxable amount
        /// </summary>
        public decimal TaxableAmt { get; set; } //new to support future tax enhancements - previously just sub-total was used

        /// <summary>
        /// Non Taxable Amount
        /// </summary>
        public decimal NonTaxableAmt { get; set; }

        /// <summary>
        /// Subtotal
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Discount Amount
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Freight Amount
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// Miscellaneous amount 
        /// </summary>
        public decimal Miscellaneous { get; set; }

        /// <summary>
        /// Taxes Amount
        /// </summary>
        public decimal Taxes { get; set; }

        /// <summary>
        /// Total Amount
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Order Number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Order Date
        /// </summary>
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// Salesperson Name
        /// </summary>
        public string SalespersonName { get; set; }


        /// <summary>
        /// Salesperson External Reference as the displayable back office key
        /// </summary>
        public string SalespersonExternalReference { get; set; } 

        /// <summary>
        /// Ship Date
        /// </summary>

        public DateTime? ShipDate { get; set; }

        /// <summary>
        /// Ship Via
        /// </summary>
        public string ShipVia { get; set; }

        /// <summary>
        /// Terms 
        /// </summary>
        public string Terms { get; set; }

        /// <summary>
        /// FOB
        /// </summary>
        public string Fob { get; set; }
        //public decimal Payment { get; set; }                  //these three fields moved to InvoiceBalance table (remains in SQL? OR could leave here if we want to do a PATCH on subsequent syncs of balance info
        //public decimal PaymentDiscount { get; set; }
        //public decimal Balance { get; set; }


        /// <summary>
        /// Bill To Tname
        /// </summary>
        public string BillToName { get; set; }

        /// <summary>
        /// Bill To First Name
        /// </summary>
        public string BillToFirstName { get; set; }             // this is kind of bogus - this becomes FirstName: Beach House LastName: Homes - recommend adding BillToName - and changing the meaning of these to billto.contact.Firstname       

        /// <summary>
        /// Bill To Last Name
        /// </summary>
        public string BillToLastName { get; set; }

        /// <summary>
        /// Bill To Street 1
        /// </summary>
        public string BillToStreet1 { get; set; }

        /// <summary>
        /// Bill  To Street 2
        /// </summary>
        public string BillToStreet2 { get; set; }
        /// <summary>
        /// Bill  To Street 3
        /// </summary>
        public string BillToStreet3 { get; set; }
        /// <summary>
        /// Bill  To Street 4
        /// </summary>
        public string BillToStreet4 { get; set; }

        /// <summary>
        /// Bill To City
        /// </summary>
        public string BillToCity { get; set; }

        /// <summary>
        /// Bill To State or Province
        /// </summary>
        public string BillToStateProvince { get; set; }

        /// <summary>
        /// Bill to Postal Code
        /// </summary>
        public string BillToPostalCode { get; set; }

        /// <summary>
        /// Bill to Country
        /// </summary>
        public string BillToCountry { get; set; }

        /// <summary>
        /// Bill To Phone
        /// </summary>
        public string BillToPhone { get; set; }

        /// <summary>
        /// Ship To Name
        /// </summary>
        public string ShipToName { get; set; }

        /// <summary>
        /// Ship To First Name
        /// </summary>
        public string ShipToFirstName { get; set; }

        /// <summary>
        /// Ship to Last Name
        /// </summary>
        public string ShipToLastName { get; set; }             // this is kind of bogus - this becomes FirstName: Beach House LastName: Homes - recommend adding ShipToName - and changing the meaning of these to shipto.contact.Firstname                            

        /// <summary>
        /// Ship To Street 1
        /// </summary>
        public string ShipToStreet1 { get; set; }

        /// <summary>
        /// Ship To Street 2
        /// </summary>
        public string ShipToStreet2 { get; set; }

        /// <summary>
        /// Ship To Street 3
        /// </summary>
        public string ShipToStreet3 { get; set; }

        /// <summary>
        /// Ship To Street 4
        /// </summary>
        public string ShipToStreet4 { get; set; }

        /// <summary>
        /// Ship To City
        /// </summary>
        public string ShipToCity { get; set; }

        /// <summary>
        /// Ship to State Province
        /// </summary>
        public string ShipToStateProvince { get; set; }

        /// <summary>
        /// Ship To Postal Code
        /// </summary>
        public string ShipToPostalCode { get; set; }

        /// <summary>
        /// Ship To Country
        /// </summary>
        public string ShipToCountry { get; set; }

        /// <summary>
        /// Ship To Phone
        /// </summary>
        public string ShipToPhone { get; set; }

        /// <summary>
        /// Bill To Email
        /// </summary>
        public string BillToEmail { get; set; }

        /// <summary>
        /// Ship To Email
        /// </summary>
        public string ShipToEmail { get; set; }

        /// <summary>
        /// Document Type
        /// </summary>
        public DocumentType Type { get; set; }                          //see enum below (needed for SBP to only send invoices)

        /// <summary>
        /// Document Source
        /// </summary>
        public DocumentSource DocumentSource { get; set; }

        //used to distinguish whether or not from order processing or ar transaction.                         
        //public  int Status { get; set; }                                //removed - hold over for when MobileService was POSTing directly to Invoice resource

        /// <summary>
        /// Invoice Line Details
        /// </summary>
        public ICollection<InvoiceDetail> Details
        {
            get { return _details ?? (_details = new Collection<InvoiceDetail>()); }
            set { _details = value; }
        } 
    }

}
