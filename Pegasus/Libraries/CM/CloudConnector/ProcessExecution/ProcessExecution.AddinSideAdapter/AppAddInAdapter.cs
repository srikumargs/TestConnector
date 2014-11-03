using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// App AddIn Adapter handles the mapping from contract to View and View to contract for the
    /// <see cref="IAppContract "/> and <see cref="IApp"/> respectively.
    /// </summary>
    public class AppAddInAdapter
    {
        /// <summary>
        /// Adapts App contract to View
        /// </summary>
        /// <param name="contract"><see cref="IAppContract"/></param>
        /// <returns><see cref="IApp"/></returns>
        internal static IApp ContractToViewAdapter(IAppContract contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true)
                        && contract.GetType() == typeof(AppViewToContractAddInAdapter)))
            {
                return ((AppViewToContractAddInAdapter)(contract)).GetSourceView();
            }

            return new AppContractToViewAddInAdapter(contract);

        }

        /// <summary>
        /// Adapts View to Contract 
        /// </summary>
        /// <param name="view"><see cref="IApp"/></param>
        /// <returns><see cref="IAppContract"/></returns>
        internal static IAppContract ViewToContractAdapter(IApp view)
        {
            return view.GetType() == typeof(AppContractToViewAddInAdapter) 
                ? ((AppContractToViewAddInAdapter)(view)).GetSourceContract()
                : new AppViewToContractAddInAdapter(view);
        }
    }
}
