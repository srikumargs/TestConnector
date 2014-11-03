using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Service.Contracts.Data
{
    /// <summary>
    /// Work Order Contract
    /// </summary>
    public class WorkOrder
    {
        private ICollection<WorkOrderDetail> _details;

        /// <summary>
        /// Cloud Id
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// PO Number
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string PONumber { get; set; }                                    // get (100/300)

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }                                 // get (100/300 .Description - populates Comment)

        /// <summary>
        /// Customer Reference
        /// </summary>
        public ExternalReference Customer { get; set; }                            // get (100 .Customer.ExternalId) (300 .Customer.ExternalReference - yet quotes/orders uses Id??)

        /// <summary>
        /// Service Date
        /// </summary>
        public DateTime ServiceDate { get; set; }

        /// <summary>
        /// Work Order Approved Date
        /// </summary>
        public DateTime? ApprovedDate { get; set; }

        /// <summary>
        /// Approver
        /// </summary>
        public WorkOrderContact Approver { get; set; }

        /// <summary>
        /// Recipient
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Quick Code
        /// </summary>
        public string QuickCode { get; set; }                                   // get (300 only, why not 100? - set if description is blank or prepends desc)

        /// <summary>
        /// Service End Date
        /// </summary>
        public DateTime? ServiceEndDate { get; set; }           // get (100/300 - .ServiceEndDate - populates InvoiceDate)

        /// <summary>
        /// Completed Date
        /// </summary>
        public DateTime? CompletedDate { get; set; }


        /// <summary>
        /// Contact First Name
        /// </summary>
        public WorkOrderContact Contact { get; set; }                            // get (100/300 - used with below to poulate .SoldTo??)

        // put address into standard address object for consistency with other request contracts - used to be flattened out
        /// <summary>
        /// Work Order Location
        /// </summary>
        public WorkOrderAddress Location { get; set; }

        /// <summary>
        /// Billing Email 
        /// NOT USED send as readonly anyway
        /// </summary>
        public string BillingEmail { get; set; }

        /// <summary>
        /// Work Order Details
        /// </summary>
        public ICollection<WorkOrderDetail> Details
        {
            get { return _details ?? (_details = new Collection<WorkOrderDetail>()); }
            set { _details = value; }
        }

        /// <summary>
        /// Work Order Payment Information
        /// </summary>
        public WorkOrderPayment Payment { get; set; }

        /// <summary>
        /// Subtotal
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Total Amount
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Tax Amount
        /// </summary>
        public decimal Tax { get; set; }

        /// <summary>
        /// Tax Schedule
        /// </summary>
        public ExternalReference TaxSchedule { get; set; }

        /// <summary>
        /// Tax Calc Provider
        /// </summary>
        public TaxCalcProvider TaxCalcProvider { get; set; }

    }



}
