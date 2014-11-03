using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sage.Connector.Sales.Contracts.Data
{
    /// <summary>
    /// Quote payload
    /// </summary>
    public class Quote : AbstractSalesDocument
    {
        private ICollection<QuoteDetail> _details;

        /// <summary>
        /// Collection of Quote Detail
        /// </summary>
        public ICollection<QuoteDetail> Details
        {
            get { return _details ?? (_details = new Collection<QuoteDetail>()); }
            set { _details = value; }
        }
    }
}