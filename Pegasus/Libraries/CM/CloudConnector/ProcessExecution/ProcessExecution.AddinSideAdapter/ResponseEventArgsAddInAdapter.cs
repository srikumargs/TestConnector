using Sage.Connector.ProcessExecution.AddinView.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// Response Event Args AddIn Adapter
    /// </summary>
    public class ResponseEventArgsAddInAdapter
    {
        /// <summary>
        /// Contract to View Adapter for Response Event Args
        /// </summary>
        /// <param name="contract"><see cref="IResponseEventArgs"/></param>
        /// <returns><see cref="ResponseEventArgs"/></returns>
        internal static ResponseEventArgs ContractToViewAdapter(IResponseEventArgs contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true)
                        && contract.GetType() == typeof(ResponseEventArgsViewToContractAddInAdapter)))
            {
                return ((ResponseEventArgsViewToContractAddInAdapter)(contract)).GetSourceView();
            }

            return new ResponseEventArgsContractToViewAddInAdapter(contract);

        }

        /// <summary>
        /// View to Contract Adapter for Response Event Args
        /// </summary>
        /// <param name="view"><see cref="ResponseEventArgs"/></param>
        /// <returns><see cref="IResponseEventArgs"/></returns>
        internal static IResponseEventArgs ViewToContractAdapter(ResponseEventArgs view)
        {
            return view.GetType() == typeof(ResponseEventArgsContractToViewAddInAdapter) 
                ? ((ResponseEventArgsContractToViewAddInAdapter)(view)).GetSourceContract() 
                : new ResponseEventArgsViewToContractAddInAdapter(view);
        }
    }
}
