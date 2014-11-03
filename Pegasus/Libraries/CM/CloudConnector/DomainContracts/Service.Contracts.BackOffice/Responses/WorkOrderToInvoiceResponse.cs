using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Service.Contracts.BackOffice.Responses
{
    /// <summary>
    /// Work Order to Invoice Response
    /// </summary>
    public class WorkOrderToInvoiceResponse: Response
    {

        /// <summary>
        /// The Invoice number to be set once created from the work order.
        /// A null value assumes failure.  
        /// </summary>
        public string DocumentReference { get; set; }                           // set (100/300- ERP invoice number)
    }
}
