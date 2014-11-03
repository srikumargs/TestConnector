using System;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Sales.Contracts.Data
{
    /// <summary>
    /// Address  -- this is used as information for Shipping address of a sales document.. 
    /// </summary>
    public class Address 
    {
        /// <summary>
        /// Cloud id - not to be modified or set by back office
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Address Name 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Street 1
        /// </summary>
        public string Street1 { get; set; }

        /// <summary>
        /// Street 2
        /// </summary>
        public string Street2 { get; set; }


        /// <summary>
        /// Street 3
        /// </summary>
        public string Street3 { get; set; }

        /// <summary>
        /// Street 4
        /// </summary>
        public string Street4 { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// State Province
        /// </summary>
        public string StateProvince { get; set; }

        /// <summary>
        /// Postal Code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; } //could flatten this out??

        /// <summary>
        /// Address Contact
        /// </summary>
        public AddressContact Contact { get; set; }


    }

    /// <summary>
    /// Address Contact
    /// </summary>
    public class AddressContact 
    {
        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Work Phone
        /// </summary>
        public string PhoneWork { get; set; }

        /// <summary>
        /// Email Work 
        /// </summary>
        public string EmailWork { get; set; }
    }
}
