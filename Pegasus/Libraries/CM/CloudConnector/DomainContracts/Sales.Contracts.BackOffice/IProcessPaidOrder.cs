using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Sales.Contracts.Data;

namespace Sage.Connector.Sales.Contracts.BackOffice
{
    /// <summary>
    /// The Process Quote interface 
    /// </summary>
    public interface IProcessPaidOrder : IBackOfficeSessionHandler
    {
        /// <summary>
        /// Process the Paid Order from the cloud. 
        /// Handles the "Buy Now" feature for back offices to create an Invoice
        /// </summary>
        /// <param name="paidOrder">The <see cref="PaidOrder"/></param>
        /// <returns>The <see cref="Response"/></returns>
        Response ProcessPaidOrder(PaidOrder paidOrder);
    }
}
