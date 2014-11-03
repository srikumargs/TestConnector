using System;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Customers.Contracts.Data
{
    /// <summary>
    /// Customer Address Implemenation
    /// </summary>
    public class CustomerAddress : AbstractEntityInformation
    {

        /// <summary>
        /// The <see cref="AddressType"/> of the Customer Address
        /// </summary>
        public AddressType Type { get; set; }

        /// <summary>
        /// The Name of the Customer Address
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Street Address Line 1
        /// </summary>
        public String Street1 { get; set; }

        /// <summary>
        /// Street Address Line 2
        /// </summary>
        public String Street2 { get; set; }

        /// <summary>
        /// Street Address Line 3
        /// </summary>
        public String Street3 { get; set; }

        /// <summary>
        /// Street Address Line 4
        /// </summary>
        public String Street4 { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public String City { get; set; }

        /// <summary>
        /// State or Province
        /// </summary>
        public String StateProvince { get; set; }

        /// <summary>
        /// Postal Code
        /// </summary>
        public String PostalCode { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public String Country { get; set; }

        /// <summary>
        /// Address Phone
        /// </summary>
        public String Phone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public String URL { get; set; }

        /// <summary>
        /// Tax Schedule External Id
        /// </summary>
        [ExternalIdReference]
        public String TaxSchedule { get; set; }

        /// <summary>
        /// Contact ExternalId
        /// Set this to the primary contact at the address 
        /// </summary>
        [ExternalIdReference]
        public String Contact { get; set; }


    }
}
