using Sage.Connector.ProcessExecution.Interfaces;
using Sage.Connector.ProcessExecution.Interfaces.Events;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// App Notification Event Args Host Adapter
    /// </summary>
    public class AppNotificationEventArgsHostAdapter
    {
        /// <summary>
        /// Contract to View Adapter
        /// </summary>
        /// <param name="contract"><see cref="IAppNotificationEventArgs"/></param>
        /// <returns></returns>
        internal static AppNotificationEventArgs ContractToViewAdapter(IAppNotificationEventArgs contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true)
                        && contract.GetType() == typeof(AppNotificationEventArgsViewToContractHostAdapter)))
            {
                return ((AppNotificationEventArgsViewToContractHostAdapter)(contract)).GetSourceView();
            }

            return new AppNotificationEventArgsContractToViewHostAdapter(contract);

        }

        /// <summary>
        /// View to Contract Adapter
        /// </summary>
        /// <param name="view"><see cref="AppNotificationEventArgs"/></param>
        /// <returns></returns>
        internal static IAppNotificationEventArgs ViewToContractAdapter(AppNotificationEventArgs view)
        {
            return view.GetType() == typeof(AppNotificationEventArgsContractToViewHostAdapter) 
                ? ((AppNotificationEventArgsContractToViewHostAdapter)(view)).GetSourceContract() 
                : new AppNotificationEventArgsViewToContractHostAdapter(view);
        }
    }
}
