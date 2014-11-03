using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sage.Connector.Sales.Contracts.Data
{
    /// <summary>
    /// This is the approval of a quote which will result in a sales order in the ERP system, using the unit price, tax, etc. of the quote when 
    /// processed on the ERP system.  Contrast this with an Order Request where the erp will process the same as a Quote Request, calculating custom
    /// pricing, discounts, tax, freight, etc.
    /// </summary>
    public class QuoteToOrder : AbstractSalesDocument
    {
        private ICollection<QuoteToOrderDetail> _details;

        /// <summary>
        /// Collection of Quote Detail
        /// </summary>
        public ICollection<QuoteToOrderDetail> Details
        {
            get { return _details??(_details= new Collection<QuoteToOrderDetail>()); }
            set { _details = value; }
        }
    }
}
