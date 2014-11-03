using System;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Invoices.Contracts.Data
{
    /// <summary>
    /// Invoice Detail
    /// </summary>
    public class InvoiceDetail : AbstractEntityInformation
    {
 
        /// <summary>
        /// Item
        /// </summary>
        [ExternalIdReference]
        public String Item { get; set; }     // will want to change to ItemID once we combine service types and inventory ids into a single table (Mobile Service - OE/IC integration)
        
        /// <summary>
        /// Line Item Type
        /// </summary>
        public LineItemType LineItemType { get; set; }

        /// <summary>
        /// Item Number 
        /// </summary>
        public string ItemNumber { get; set; }

        /// <summary>
        /// Item Description
        /// </summary>
        public string ItemDescription { get; set; }

        /// <summary>
        /// Unit Of Measure
        /// </summary>
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Quantity Shipped
        /// </summary>
        public decimal QuantityShipped { get; set; }

        /// <summary>
        /// Quantity Back Ordered
        /// </summary>
        public decimal QuantityBackOrdered { get; set; }

        /// <summary>
        /// Quantity Shippped from the Base Unit of Measure
        /// This is the actual quantity shipped on the invoice in terms of the base unit of measure (e.g. SalesUM * UMConversion)
        /// </summary>
        public decimal QuantityShippedBaseUom { get; set; } // this is the actual quantity shipped on the invoice in terms of the base unit of measure (e.g. SalesUM * UMConversion)
        
        /// <summary>
        /// Warehouse
        /// </summary>
        public string Warehouse { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Discount
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public decimal Total { get; set; }

    }
  
}
