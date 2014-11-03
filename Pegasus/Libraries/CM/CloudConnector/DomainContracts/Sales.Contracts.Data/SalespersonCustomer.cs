using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Sales.Contracts.Data
{
    /// <summary>
    /// Salesperson Customers 
    /// </summary>
    public class SalespersonCustomer : AbstractEntityInformation
    {
        /// <summary>
        /// Salesperson
        /// </summary>
        [ExternalIdReference]
        public String Salesperson { get; set; }

        /// <summary>
        /// Customer
        /// </summary>
        [ExternalIdReference]
        public String Customer { get; set; }
    }
}
