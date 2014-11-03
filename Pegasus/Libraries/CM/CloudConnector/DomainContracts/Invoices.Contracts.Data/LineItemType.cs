
namespace Sage.Connector.Invoices.Contracts.Data
{
    /// <summary>
    /// Line Item Types
    /// </summary>
    public enum LineItemType
     {
        /// <summary>
        /// Inventory Item 
        /// </summary>
         InventoryItem = 0,       
     
        /// <summary>
        /// Non-Inventory
        /// </summary>
         Miscellaneous = 1,

        /// <summary>
        /// Comment Line
        /// </summary>
         Comment = 2,

        /// <summary>
        /// Other
        /// </summary>
         Other = 3
     }

}
