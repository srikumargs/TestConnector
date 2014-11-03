using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Service.Contracts.BackOffice.Responses;
using Sage.Connector.Service.Contracts.Data;

namespace Sage.Connector.Service.Contracts.BackOffice
{
    /// <summary>
    /// Process Work Order To Invoice
    /// </summary>
    public interface IProcessWorkOrderToInvoice : IBackOfficeSessionHandler
    {
        /// <summary>
        /// Create an invoice from the service work order
        /// </summary>
        /// <param name="workOrder">The <see cref="WorkOrder"/></param>
        /// <returns>The <see cref="Response"/>. A null response implies failure.</returns>
        WorkOrderToInvoiceResponse ProcessWorkOrder(WorkOrder workOrder);
    }
}
