using System;
using System.ComponentModel.DataAnnotations;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Customers.Contracts.Data
{
    /// <summary>
    /// Customer Contact 
    /// </summary>
    public class CustomerContact: AbstractEntityInformation
    {

        //TODO: Unable to test at this point due to project changes.
        //TODO: concerned about circular object serialiation/deserialization if same object is 
        //TODO used from the customer.   May need to create ContactAddress
        //private ICollection<CustomerAddress> _addresses;

        ///// <summary>
        ///// Collection of Addresses
        ///// </summary>
        //public ICollection<CustomerAddress> Addresses
        //{
        //    get { return _addresses ?? (_addresses = new Collection<CustomerAddress>()); }
        //    set { _addresses = value; }
        //}

        /// <summary>
        /// Contact First Name
        /// </summary>
        public String FirstName { get; set; }

        /// <summary>
        /// Contact Last Name
        /// </summary>
        public String LastName { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public String Title { get; set; }


        /// <summary>
        /// Contact Work Phone
        /// </summary>
        public String PhoneWork { get; set; }

        /// <summary>
        /// Contact Work Email
        /// </summary>
        public String EmailWork { get; set; }

        /// <summary>
        /// Contact Mobile Phone
        /// </summary>
        public String PhoneMobile { get; set; }

        /// <summary>
        /// Contact Home Phone
        /// </summary>
        public String PhoneHome { get; set; }

        /// <summary>
        /// Contact Home Email
        /// </summary>
        [MaxLength(256, ErrorMessage = @"Email Personal cannot exceed 256 characters")]
        public String EmailPersonal { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public String Url { get; set; }
    }
}
