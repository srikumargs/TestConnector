
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.ProductCatalog.Contracts.Data
{
    /// <summary>
    /// Inventory Item 
    /// </summary>
    public class InventoryItem: AbstractEntityInformation
    {
        /// <summary>
        /// Item Number
        /// [StringLength(256)]
        ///[Required]
        /// </summary>
        public  string Sku { get; set; }


        /// <summary>
        /// Item Name
        /// [Required]
        /// [StringLength(256)]
        /// </summary>
        public  string Name { get; set; }

        /// <summary>
        /// [StringLength(256)]
        /// </summary>
        public  string Description { get; set; }

        /// <summary>
        /// List Price
        /// </summary>
        public  decimal? PriceStd { get; set; }

        /// <summary>
        /// Item standard cost
        /// </summary>
        public  decimal? CostStd { get; set; }

        /// <summary>
        /// Quantity Available
        /// </summary>
        public  decimal? Quantity { get; set; }


        /// <summary>
        /// [StringLength(256)]
        /// </summary>
        public  string UnitOfMeasure { get; set; }

        /// <summary>
        /// [StringLength(64)]
        /// [DisplayName("Tax class")]
        /// </summary>
        public  string TaxClass { get; set; }


    }
}
