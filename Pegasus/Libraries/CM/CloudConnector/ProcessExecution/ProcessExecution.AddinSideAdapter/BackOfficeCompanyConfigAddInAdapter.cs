using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.AddinView.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// Response Event Args AddIn Adapter
    /// </summary>
    public class BackOfficeCompanyConfigAddInAdapter
    {
        /// <summary>
        /// Contract to View Adapter for Response Event Args
        /// </summary>
        /// <param name="contract"><see cref="IResponseEventArgs"/></param>
        /// <returns><see cref="ResponseEventArgs"/></returns>
        internal static BackOfficeCompanyConfiguration ContractToViewAdapter(Sage.Connector.ProcessExecution.Interfaces.IBackOfficeCompanyConfiguration contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true)
                        && contract.GetType() == typeof(ResponseEventArgsViewToContractAddInAdapter)))
            {
                return ((BackOfficeCompanyConfigViewToContractAddInAdapter)(contract)).GetSourceView();
            }

            return new BackOfficeCompanyConfigContractToViewAddInAdapter(contract);

        }

        /// <summary>
        /// View to Contract Adapter for Response Event Args
        /// </summary>
        /// <param name="view"><see cref="ResponseEventArgs"/></param>
        /// <returns><see cref="IResponseEventArgs"/></returns>
        internal static Sage.Connector.ProcessExecution.Interfaces.IBackOfficeCompanyConfiguration ViewToContractAdapter(BackOfficeCompanyConfiguration view)
        {
            return view.GetType() == typeof(BackOfficeCompanyConfigContractToViewAddInAdapter)
                ? ((BackOfficeCompanyConfigContractToViewAddInAdapter)(view)).GetSourceContract()
                : new BackOfficeCompanyConfigViewToContractAddInAdapter(view);
        }
    }
}
