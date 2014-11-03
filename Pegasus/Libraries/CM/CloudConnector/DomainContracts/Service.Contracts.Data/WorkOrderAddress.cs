
using System;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Service.Contracts.Data
{
    /// <summary>
    /// Address  -- this is used as information for Shipping address of a sales document.. 
    /// </summary>
    public class WorkOrderAddress 
    {

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
        /// TaxSchedule ExternalId
        /// </summary>
        /// 
        [ExternalIdReference]
        public String TaxSchedule { get; set; } 

     }

    /// <summary>
    /// Address Contact
    /// </summary>
    public class WorkOrderContact 
    {
        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }

    }
}
