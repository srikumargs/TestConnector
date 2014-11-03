using Sage.Connector.ProcessExecution.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// Response Event Args Host Adapter
    /// </summary>
    public class ResponseEventArgsHostAdapter
    {
        /// <summary>
        /// Contract to View Adapter
        /// </summary>
        /// <param name="contract"><see cref="IResponseEventArgs"/></param>
        /// <returns><see cref="ResponseEventArgs"/></returns>
        internal static ResponseEventArgs ContractToViewAdapter(IResponseEventArgs contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true)
                        && contract.GetType() == typeof(ResponseEventArgsViewToContractHostAdapter)))
            {
                return ((ResponseEventArgsViewToContractHostAdapter)(contract)).GetSourceView();
            }
            return new ResponseEventArgsContractToViewHostAdapter(contract);
        }

        /// <summary>
        /// View to Contract Adapter
        /// </summary>
        /// <param name="view"><see cref="ResponseEventArgs"/></param>
        /// <returns><see cref="IResponseEventArgs"/></returns>
        internal static IResponseEventArgs ViewToContractAdapter(ResponseEventArgs view)
        {
            return view.GetType() == typeof(ResponseEventArgsContractToViewHostAdapter) 
                ? ((ResponseEventArgsContractToViewHostAdapter)(view)).GetSourceContract() 
                : new ResponseEventArgsViewToContractHostAdapter(view);
        }
    }
}
