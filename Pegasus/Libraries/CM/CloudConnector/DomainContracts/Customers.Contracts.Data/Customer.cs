using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Customers.Contracts.Data
{
    /// <summary>
    /// Customer implementation
    /// </summary>
    public class Customer : AbstractEntityInformation
    {
        private ICollection<CustomerAddress> _addresses;
        private ICollection<CustomerContact> _contacts;
        private ICollection<CustomerTaxClass> _taxClasses;

        /// <summary>
        /// Customer Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is On Credit Hold
        /// </summary>
        public bool IsOnCreditHold { get; set; }

        /// <summary>
        /// Is Credit Limit used for this Customer
        /// </summary>
        public bool IsCreditLimitUsed { get; set; }

        /// <summary>
        /// Credit Limit for this customer. 
        /// </summary>
        public decimal CreditLimit { get; set; }

        /// <summary>
        /// Credit Available for this customer. 
        /// </summary>
        public decimal CreditAvailable { get; set; }

        /// <summary>
        /// Payment Terms established for this customer.
        /// </summary>
        public string PaymentTerms { get; set; }

        /// <summary>
        /// Tax Schedule External Id used as reference
        /// </summary>
        [ExternalIdReference]
        public string TaxSchedule { get; set; }

        /// <summary>
        /// Collection of <see cref="CustomerContact"/> Contacts for this Customer
        /// </summary>
        public ICollection<CustomerContact> Contacts
        {
            get { return _contacts ?? (_contacts = new Collection<CustomerContact>()); }
            set { _contacts = value; }
        }

        /// <summary>
        /// Collection of <see cref="CustomerAddress"/> Addresses for this Customer
        /// </summary>
        public ICollection<CustomerAddress> Addresses
        {
            get { return _addresses ?? (_addresses = new Collection<CustomerAddress>()); }
            set  { _addresses = value; }
        }

        /// <summary>
        /// Customer Tax Classes
        /// </summary>
        public ICollection<CustomerTaxClass> TaxClasses
        {
            get { return _taxClasses ?? (_taxClasses = new Collection<CustomerTaxClass>()); }
            set { _taxClasses = value; }
        }
    }
}
