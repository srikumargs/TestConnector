using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Taxes.Contracts.Data
{
    /// <summary>
    /// Tax Code
    /// </summary>
    public class TaxCode: AbstractEntityInformation
    {
        private ICollection<TaxCodeDetail> _taxCodeDetails;
        private ICollection<TaxCodeClass> _taxCodeClasses;

        /// <summary>
        /// Tax Code Description
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Tax Code Short Description
        /// </summary>
        public String ShortDescription { get; set; }

        /// <summary>
        /// Minumum Tax 
        /// </summary>
        public decimal MinimumTax { get; set; }

        /// <summary>
        /// Maximum Tax
        /// </summary>
        public decimal MaximumTax { get; set; }

        /// <summary>
        /// Tax Code Details 
        /// </summary>
        public ICollection<TaxCodeDetail> TaxCodeDetails
        {
            get { return _taxCodeDetails ?? (_taxCodeDetails = new Collection<TaxCodeDetail>()); }
            set { _taxCodeDetails = value; }
        }

        /// <summary>
        /// Tax Code Classes
        /// </summary>
        public ICollection<TaxCodeClass> TaxCodeClasses
        {
            get { return _taxCodeClasses ?? (_taxCodeClasses = new Collection<TaxCodeClass>()); }
            set { _taxCodeClasses = value; }
        }
    }
}
