using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Sales.Contracts.Data;

namespace Sage.Connector.Sales.Contracts.BackOffice
{
    /// <summary>
    /// The Process Quote to Order interface 
    /// 
    /// Used to create a back office Order from the Quote information. 
    /// </summary>
    public interface IProcessQuoteToOrder : IBackOfficeSessionHandler
    {
        /// <summary>
        /// This is the approval of a quote which will result in a sales order in the ERP system, using the unit price, tax, etc. of the quote when 
        /// processed on the ERP system.  Contrast this with an Order Request where the erp will process the same as a Quote Request, calculating custom
        /// pricing, discounts, tax, freight, etc.
        /// </summary>
        /// <param name="quote">The <see cref="QuoteToOrder"/></param>
        /// <returns>The <see cref="Response"/></returns>
        Response ProcessQuoteToOrder(QuoteToOrder quote);
    }
}
