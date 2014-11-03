using System;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Sales.Contracts.Data
{
    /// <summary>
    /// Sales Document Detail
    /// </summary>
    public abstract class AbstractSalesDocumentDetail: AbstractEntityInformation
    {
        /// <summary>
        /// Cloud Id not to be modified or set by back office
        /// </summary>
        public String Id { get; set; }
       
        /// <summary>
        /// Sales Document Detail Quantity, not to be set or modified by back office.
        /// </summary>
        public decimal Quantity { get; set; }                           //get
      
        /// <summary>
        /// item Price
        /// set (quote only, not set on order - although when we go straight to order request will need to) may b set for a quote but not an order
        /// </summary>
        public virtual decimal Price { get; set; }       
        
        /// <summary>
        /// Inventory Item not to be set or modified by back office.
        /// </summary>
        public ExternalReference InventoryItem { get; set; }           //get
    }
}
