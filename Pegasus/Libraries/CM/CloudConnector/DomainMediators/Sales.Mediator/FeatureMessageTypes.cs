
namespace Sage.Connector.Sales.Mediator
{
    /// <summary>
    /// The supported set of features by the core domain mediator. 
    /// </summary>
    public static class FeatureMessageTypes
    {
        /// <summary>
        /// Sync Salespersons
        /// </summary>
        public const string SyncSalespersons = "SyncSalespersons";

        /// <summary>
        /// Sync Salespersons
        /// </summary>
        public const string SyncSalespersonCustomers = "SyncSalespersonCustomers";        
        
        /// <summary>
        /// Process Quote Message Type
        /// </summary>
        public const string ProcessQuote = "ProcessQuote";

        /// <summary>
        /// Process Paid Order Message Type
        /// </summary>
        public const string ProcessPaidOrder = "ProcessPaidOrder";
        /// <summary>
        /// Process Quote to Order Message Type
        /// </summary>
        public const string ProcessQuoteToOrder = "ProcessQuoteToOrder";

    }

   
}
