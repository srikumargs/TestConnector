using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Sales.Contracts.Data
{
    /// <summary>
    /// Salesperson 
    /// </summary>
    public class Salesperson : AbstractEntityInformation
    {

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email Address
        /// </summary>
        public string Email { get; set; }
    }
}
