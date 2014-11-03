using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Statements.Contracts.Data.Requests;
using Sage.Connector.Statements.Contracts.Data.Responses;

namespace Sage.Connector.Statements.Contracts.BackOffice
{
    /// <summary>
    /// Process Statements
    /// </summary>
    public interface IProcessStatements : IBackOfficeSessionHandler
    {
        /// <summary>
        /// Perform any intializations required for performance reasons or otherwise. 
        /// Store
        /// </summary>
        /// <param name="statementsRequest">The <see cref="StatementsRequest"/> contains the list of customers 
        /// for which to process the statements based on statement date.</param>
        /// <returns><see cref="Response"/>containing the <see cref="Response"/> status information.
        /// None of the statements will not be processed if a Status of Failure is returned from this call.  </returns>
        Response InitializeProcessStatements(StatementsRequest statementsRequest);

        /// <summary>
        /// Get the statement from the request information
        /// </summary>
        /// <returns><see cref="StatementResponse"/>Set the Response Status appropriately for each statement, along with the Statement.</returns>
        StatementResponse GetNextStatement();
    }
}
