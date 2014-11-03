using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.ProductCatalog.Contracts.Data
{
    /// <summary>
    /// Service Type is an item that is not inventory tracked, such as labor
    /// </summary>
    public class ServiceType: AbstractEntityInformation
    {
        private ICollection<ServiceTypeTaxClass> _taxClasses;

        /// <summary>
        /// Description
        /// </summary>
        public  string Description { get; set; }

        /// <summary>
        /// Short Description
        /// </summary>
        public  string ShortDescription { get; set; }

        /// <summary>
        /// Unit of Measure, ex. Hours
        /// </summary>
        public  string UnitOfMeasure { get; set; }

        /// <summary>
        /// Rate
        /// </summary>
        public  decimal Rate { get; set; }

        /// <summary>
        /// Taxable?  
        /// </summary>
        public  bool Taxable { get; set; }

       /// <summary>
       /// Tax Class
        ///[StringLength(64)]
        ///[DisplayName("Tax Class")
       /// </summary>
        public string TaxClass { get; set; }

        /// <summary>
        /// Tax Classes
        /// </summary>
        public ICollection<ServiceTypeTaxClass> TaxClasses
        {
            get { return _taxClasses ?? (_taxClasses = new Collection<ServiceTypeTaxClass>()); }
            set { _taxClasses = value; }
        }
    }
}
