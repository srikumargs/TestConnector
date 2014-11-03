using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// App  Host Adapter
    /// </summary>
    public class AppHostAdapter
    {
        /// <summary>
        /// Contract to View Adapter
        /// </summary>
        /// <param name="contract"><see cref="IAppContract"/></param>
        /// <returns><see cref="IApp"/></returns>
        internal static IApp ContractToViewAdapter(IAppContract contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true)
                        && contract.GetType() == typeof(AppViewToContractHostAdapter)))
            {
                return ((AppViewToContractHostAdapter)(contract)).GetSourceView();
            }
      
                return new AppContractToViewHostAdapter(contract);
            
        }

        /// <summary>
        /// View to Contract Adapter
        /// </summary>
        /// <param name="view"><see cref="IApp"/></param>
        /// <returns><see cref="IAppContract"/></returns>
        internal static IAppContract ViewToContractAdapter(IApp view)
        {
            return view.GetType() == typeof(AppContractToViewHostAdapter) 
                ? ((AppContractToViewHostAdapter)(view)).GetSourceContract() 
                : new AppViewToContractHostAdapter(view);
        }
    }
}
