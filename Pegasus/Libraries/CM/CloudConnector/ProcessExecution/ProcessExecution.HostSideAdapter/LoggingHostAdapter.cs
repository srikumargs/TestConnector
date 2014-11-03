using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// 
    /// </summary>
    public class LoggingHostAdapter
    {

        internal static ILogging ContractToViewAdapter(ILoggingContract contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true)
                        && contract.GetType().Equals(typeof(LoggingViewToContractHostAdapter))))
            {
                return ((LoggingViewToContractHostAdapter)(contract)).GetSourceView();
            }
            else
            {
                return new LoggingContractToViewHostAdapter(contract);
            }
        }

        internal static ILoggingContract ViewToContractAdapter(ILogging view)
        {
            if (view.GetType().Equals(typeof(LoggingContractToViewHostAdapter)))
            {
                return ((LoggingContractToViewHostAdapter)(view)).GetSourceContract();
            }
            else
            {
                return new LoggingViewToContractHostAdapter(view);
            }
        }
    }
}
