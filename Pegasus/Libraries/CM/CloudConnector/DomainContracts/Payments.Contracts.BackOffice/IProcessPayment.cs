using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Payments.Contracts.Data;

namespace Sage.Connector.Payments.Contracts.BackOffice
{

    /// <summary>
    /// The Process Payment interface 
    /// </summary>
    public interface IProcessPayment : IBackOfficeSessionHandler
    {
       
        /// <summary>
        /// Process the Payment from the cloud. 
        /// Handles the "Buy Now" feature for back offices to process a payment
        /// </summary>
        /// <param name="payment">The <see cref="Payment"/></param>
        /// <returns>The <see cref="Response"/></returns>
        Response ProcessPayment(Payment payment);
    }

}
