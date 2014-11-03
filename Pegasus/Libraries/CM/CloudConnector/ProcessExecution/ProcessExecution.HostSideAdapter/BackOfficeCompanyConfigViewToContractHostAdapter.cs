using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.Interfaces;
using IBackOfficeCompanyConfiguration = Sage.Connector.DomainContracts.BackOffice.IBackOfficeCompanyConfiguration;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// Response Event Args View To Contract Host Adapter
    /// </summary>
    public class BackOfficeCompanyConfigViewToContractHostAdapter : ContractBase, Sage.Connector.ProcessExecution.Interfaces.IBackOfficeCompanyConfiguration
    {
        private readonly IBackOfficeCompanyConfiguration _view;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"><see cref="BackOfficeCompanyConfiguration"/></param>
        public BackOfficeCompanyConfigViewToContractHostAdapter(IBackOfficeCompanyConfiguration view)
        {
            _view = view;
        }



        /// <summary>
        /// Get the Response Event Args Source View
        /// </summary>
        /// <returns><see cref="BackOfficeCompanyConfiguration"/></returns>
        internal IBackOfficeCompanyConfiguration GetSourceView()
        {
            return _view;
        }

        /// <summary>
        /// BackOffice Plugin Id
        /// </summary>
        public string BackOfficeId
        {
            get
            {
                return _view.BackOfficeId;
            }

        }

        /// <summary>
        /// ConnectionCredentials
        /// </summary>
        public string ConnectionCredentials
        {
            get
            {
                return _view.ConnectionCredentials;
            }
        }

        /// <summary>
        /// Gets the data storage path.
        /// </summary>
        /// <value>
        /// The data storage path.
        /// </value>
        public string DataStoragePath
        {
            get
            {
                return _view.DataStoragePath;
            }
        }
    }
}
