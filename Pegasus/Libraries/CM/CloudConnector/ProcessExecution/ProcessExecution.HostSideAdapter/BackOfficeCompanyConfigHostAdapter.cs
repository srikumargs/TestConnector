using Sage.Connector.ProcessExecution.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// Response Event Args Host Adapter
    /// </summary>
    public class BackOfficeCompanyConfigHostAdapter
    {
        /// <summary>
        /// Contract to View Adapter
        /// </summary>
        /// <param name="contract"><see cref="IResponseEventArgs"/></param>
        /// <returns><see cref="DomainContracts.BackOffice.IBackOfficeCompanyConfiguration"/></returns>
        internal static DomainContracts.BackOffice.IBackOfficeCompanyConfiguration ContractToViewAdapter(IBackOfficeCompanyConfiguration contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true)
                        && contract.GetType() == typeof(BackOfficeCompanyConfigViewToContractHostAdapter)))
            {
                return ((BackOfficeCompanyConfigViewToContractHostAdapter)(contract)).GetSourceView();
            }
            return new BackOfficeCompanyConfigContractToViewHostAdapter(contract);
        }

        /// <summary>
        /// View to Contract Adapter
        /// </summary>
        /// <param name="view"><see cref="ResponseEventArgs"/></param>
        /// <returns><see cref="IResponseEventArgs"/></returns>
        internal static IBackOfficeCompanyConfiguration ViewToContractAdapter(DomainContracts.BackOffice.IBackOfficeCompanyConfiguration view)
        {
            return view.GetType() == typeof(BackOfficeCompanyConfigContractToViewHostAdapter)
                ? ((BackOfficeCompanyConfigContractToViewHostAdapter)(view)).GetSourceContract()
                : new BackOfficeCompanyConfigViewToContractHostAdapter(view);
        }
    }
}
