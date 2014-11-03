using System;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Service.Contracts.Data
{
    /// <summary>
    /// Work Order Detail Contract
    /// </summary>
    public  class WorkOrderDetail
    {
        /// <summary>
        /// Cloud Id
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Rate
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Taxable 
        /// NOT USED 
        /// </summary>
        public bool Taxable { get; set; }    
        
        /// <summary>
        /// Service Type Reference
        /// </summary>
        public ExternalReference ServiceType { get; set; }

        /// <summary>
        /// Short Description
        /// NOT USED 
        /// </summary>
        public string ShortDescription { get; set; }    
        
        /// <summary>
        /// Work Order Total
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// Unit of Measure
        /// </summary>
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// Discount Amount
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Is Rate Overridden?
        /// </summary>
        public bool IsRateOverridden { get; set; }
    }
}
