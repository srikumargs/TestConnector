using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.AddinView.Events;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// Response Event Args View to Contract AddIn Adapter
    /// </summary>
    public class BackOfficeCompanyConfigViewToContractAddInAdapter : ContractBase,
        Sage.Connector.ProcessExecution.Interfaces.IBackOfficeCompanyConfiguration
    {

        private readonly BackOfficeCompanyConfiguration _view;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"><see cref="ResponseEventArgs"/></param>
        public BackOfficeCompanyConfigViewToContractAddInAdapter(BackOfficeCompanyConfiguration view)
        {
            _view = view;
        }

        /// <summary>
        /// Response Event Args Source View
        /// </summary>
        /// <returns></returns>
        internal BackOfficeCompanyConfiguration GetSourceView()
        {
            return _view;
        }

        /// <summary>
        /// Get view's back office id
        /// </summary>
        public string BackOfficeId
        {
            get { return _view.BackOfficeId; }

        }

        /// <summary>
        /// Get view's ConnectionCredentials
        /// </summary>
        public string ConnectionCredentials
        {
            get { return _view.ConnectionCredentials; }
        }

        /// <summary>
        /// Gets the data storage path.
        /// </summary>
        /// <value>
        /// The data storage path.
        /// </value>
        public string DataStoragePath
        {
            get { return _view.DataStoragePath; }
        }
    }
}
