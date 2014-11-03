using System;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Statements.Contracts.BackOffice
{
    /// <summary>
    /// StatementRequest
    /// </summary>
    public class StatementRequest
    {
        /// <summary>
        /// Statement Processing Date
        /// </summary>
        public DateTime StatementDate { get; set; }

        /// <summary>
        /// Collection of Customer External References
        /// </summary>
        public ExternalReference CustomerReference { get; set; }
    }
}
