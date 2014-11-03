using Sage.Connector.ProcessExecution.AddinView.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// Application Notification EventArgs AddIn Adapter
    /// </summary>
    public class AppNotificationEventArgsAddInAdapter
    {
        /// <summary>
        /// Contract to View adapter for <see cref="AppNotificationEventArgs"/>
        /// </summary>
        /// <param name="contract"><see cref="IAppNotificationEventArgs"/></param>
        /// <returns>The <see cref="AppNotificationEventArgs"/></returns>
        internal static AppNotificationEventArgs ContractToViewAdapter(IAppNotificationEventArgs contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true)
                        && contract.GetType() == typeof(AppNotificationEventArgsViewToContractAddInAdapter)))
            {
                return ((AppNotificationEventArgsViewToContractAddInAdapter)(contract)).GetSourceView();
            }

            return new AppNotificationEventArgsContractToViewAddInAdapter(contract);

        }

        /// <summary>
        /// The View to Contract Adapter for <see cref="IAppNotificationEventArgs"/>
        /// </summary>
        /// <param name="view"><see cref="AppNotificationEventArgs"/></param>
        /// <returns>The <see cref="IAppNotificationEventArgs"/></returns>
        internal static IAppNotificationEventArgs ViewToContractAdapter(AppNotificationEventArgs view)
        {
            return view.GetType() == typeof(AppNotificationEventArgsContractToViewAddInAdapter) 
                ? ((AppNotificationEventArgsContractToViewAddInAdapter)(view)).GetSourceContract()
                : new AppNotificationEventArgsViewToContractAddInAdapter(view);
        }
    }
}
