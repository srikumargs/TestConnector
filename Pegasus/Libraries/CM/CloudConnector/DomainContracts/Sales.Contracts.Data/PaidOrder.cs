using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sage.Connector.Sales.Contracts.Data
{
    /// <summary>
    /// Paid Order
    /// </summary>
    public class PaidOrder : AbstractSalesDocument
    {
        private ICollection<PaidOrderDetail> _details;

        /// <summary>
        /// Tax Calculation provider.  
        /// 
        /// </summary>
        public TaxCalcProvider TaxCalcProvider { get; set; }

        /// <summary>
        /// Collection of Order Detail
        /// </summary>
        public ICollection<PaidOrderDetail> Details
        {
            get { return _details ?? (_details = new Collection<PaidOrderDetail>()); }
            set { _details = value; }
        }
    }
}
