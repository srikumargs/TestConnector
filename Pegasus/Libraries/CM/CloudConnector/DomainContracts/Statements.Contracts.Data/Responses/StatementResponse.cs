
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Statements.Contracts.Data.Responses
{
    /// <summary>
    /// Statement Response
    /// </summary>
    public class StatementResponse: Response
    {
        /// <summary>
        /// Statement 
        /// </summary>
        public Statement Statement { get; set; }
    }
}
