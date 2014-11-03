using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// Process Request AddIn Adapter
    /// </summary>
    public class ProcessRequestAddInAdapter
    {
        /// <summary>
        /// Process Request Contract to View Adapter
        /// </summary>
        /// <param name="contract"><see cref="IProcessRequestContract"/></param>
        /// <returns><see cref="IProcessRequest"/></returns>
        internal static IProcessRequest ContractToViewAdapter(IProcessRequestContract contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true)
                        && contract.GetType() == typeof(ProcessRequestViewToContractAddInAdapter)))
            {
                return ((ProcessRequestViewToContractAddInAdapter)(contract)).GetSourceView();
            }

            return new ProcessRequestContractToViewAddInAdapter(contract);

        }

        /// <summary>
        /// Process Request View to Contract Adapter
        /// </summary>
        /// <param name="view"><see cref="IProcessRequest"/></param>
        /// <returns><see cref="IProcessRequestContract"/></returns>
        internal static IProcessRequestContract ViewToContractAdapter(IProcessRequest view)
        {
            return view.GetType() == typeof(ProcessRequestContractToViewAddInAdapter) 
                ? ((ProcessRequestContractToViewAddInAdapter)(view)).GetSourceContract() 
                : new ProcessRequestViewToContractAddInAdapter(view);
        }
    }
}
