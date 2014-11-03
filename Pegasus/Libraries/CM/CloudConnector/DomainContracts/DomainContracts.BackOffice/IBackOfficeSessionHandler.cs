using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.DomainContracts.BackOffice
{
    /// <summary>
    /// Back Office Session interface
    /// </summary>
    public interface IBackOfficeSessionHandler
    {
        /// <summary>
        /// Begin session handler takes care of setup for communicating with the back office 
        /// for the given back office configuration.  
        /// Insert code here to connect to the erp and get ready for the feature task implementation
        /// </summary>
        /// <param name="sessionContext">session context to provide services</param>
        /// <param name="backOfficeConfiguration">Back office information, company connection credentials and such</param>
        /// <returns>Diagnoses or null if none</returns>
        Response BeginSession(ISessionContext sessionContext, IBackOfficeCompanyData backOfficeConfiguration);

        /// <summary>
        /// The session activity for the back office session is being ended. 
        /// Perform cleanup.
        /// </summary>
        void EndSession();
    }
}
