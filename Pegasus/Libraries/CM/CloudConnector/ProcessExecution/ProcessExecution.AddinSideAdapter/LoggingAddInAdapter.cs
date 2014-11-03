using System.Runtime.Remoting;
using Sage.Connector.ProcessExecution.AddinView;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// 
    /// </summary>
    public class LoggingAddInAdapter
    {
        internal static LoggerCore ContractToViewAdapter(ILoggingContract contract)
        {
            if (((RemotingServices.IsObjectOutOfAppDomain(contract) != true) 
                        && contract.GetType().Equals(typeof(LoggingViewToContractAddInAdapter)))) 
            {
                return ((LoggingViewToContractAddInAdapter)(contract)).GetSourceView();
            }
            else 
            {
                return new LoggingContractToViewAddInAdapter(contract);
            }
        }

        internal static ILoggingContract ViewToContractAdapter(LoggerCore view)
        {
            if (view.GetType().Equals(typeof(LoggingContractToViewAddInAdapter)))
            {
                return ((LoggingContractToViewAddInAdapter)(view)).GetSourceContract();
            }
            else
            {
                return new LoggingViewToContractAddInAdapter(view);
            }
        }
    }
}
