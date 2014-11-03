using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Sales.Contracts.Data;

namespace Sage.Connector.Sales.Contracts.BackOffice
{
    /// <summary>
    /// The Process Quote interface 
    /// </summary>
    public interface IProcessQuote : IBackOfficeSessionHandler
    {
        /// <summary>
        /// Process the quote from the cloud. 
        /// 
        /// The purpose is to be able to price and provide accurate totals for the Quote
        /// regardless if the back office supports a Quote Entity.
        /// </summary>
        /// <param name="quote">The <see cref="Quote"/></param>
        /// <returns>The <see cref="Response"/></returns>
        Response ProcessQuote(Quote quote);
    }
}
