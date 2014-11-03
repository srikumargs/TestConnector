using System.Collections.Generic;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Statements.Contracts.Data.Responses;

namespace Sage.Connector.Statements.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// Statements Response
    /// </summary>
    public class StatementsResponse : Response
    {
        /// <summary>
        /// Collection of Statement Responses
        /// </summary>
        public ICollection<StatementResponse> StatementResponses { get; set; }
    }
}
