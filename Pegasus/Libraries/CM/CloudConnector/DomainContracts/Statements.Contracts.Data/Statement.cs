using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Statements.Contracts.Data
{

    /// <summary>
    /// Statement
    /// </summary>
    public class Statement: AbstractEntityInformation
    {
        private ICollection<StatementDetail> _details;

        /// <summary>
        /// Statement Date
        /// </summary>
        public DateTime StatementDate { get; set; }

        /// <summary>
        /// Customer Reference
        /// </summary>
        public ExternalReference Customer { get; set; }         //Cloud worker role will need to obtain customerId based on ExternalId
       
        /// <summary>
        /// Customer Number
        /// </summary>
        public string CustomerNumber { get; set; }

        /// <summary>
        /// Customer Name
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Customer Address Street 1
        /// </summary>
        public string CustomerStreet1 { get; set; }

        /// <summary>
        /// Customer Address Street 2
        /// </summary>
        public string CustomerStreet2 { get; set; }

        /// <summary>
        /// Customer Address Street 3
        /// </summary>
        public string CustomerStreet3 { get; set; }

        /// <summary>
        /// Customer Address Street 4
        /// </summary>
        public string CustomerStreet4 { get; set; }

        /// <summary>
        /// Customer Address City
        /// </summary>
        public string CustomerCity { get; set; }

        /// <summary>
        /// Customer Address State or Province
        /// </summary>
        public string CustomerStateProvince { get; set; }

        /// <summary>
        /// Customer Address Postal Code
        /// </summary>
        public string CustomerPostalCode { get; set; }

        /// <summary>
        /// Customer Address Country
        /// </summary>
        public string CustomerCountry { get; set; }

        /// <summary>
        /// Credit Limit
        /// </summary>
        public decimal? CreditLimit { get; set; }

        /// <summary>
        /// Credit Available
        /// </summary>
        public decimal? CreditAvailable { get; set; }

        /// <summary>
        /// Contact Name
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// Salesperson Name
        /// </summary>
        public string SalesPersonName { get; set; }

        /// <summary>
        /// Standard Message
        /// </summary>
        public string StandardMessage { get; set; }

        /// <summary>
        /// Overdue Message
        /// </summary>
        public string OverdueMessage { get; set; }

        /// <summary>
        /// Current Balance
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Aging Balance 1
        /// </summary>
        public decimal AgingBalance1 { get; set; }

        /// <summary>
        /// Aging Balance 2
        /// </summary>
        public decimal AgingBalance2 { get; set; }

        /// <summary>
        /// Aging Balance 3
        /// </summary>
        public decimal AgingBalance3 { get; set; }

        /// <summary>
        /// Aging Balance 4
        /// </summary>
        public decimal AgingBalance4 { get; set; }

        /// <summary>
        /// Balance Due
        /// </summary>
        public decimal BalanceDue { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Collection of <see cref="StatementDetail"/>
        /// </summary>
        public ICollection<StatementDetail> Details
        {
            get { return _details ?? (_details = new Collection<StatementDetail>()); }
            set { _details = value; }
        }
    }

 

}
