using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.Interfaces;
using Sage.Connector.ProcessExecution.Interfaces.Events;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// App Notification Event Args View to Contract Host Adapter
    /// </summary>
    public class AppNotificationEventArgsViewToContractHostAdapter : ContractBase, IAppNotificationEventArgs
    {

        private readonly AppNotificationEventArgs _view;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"><see cref="AppNotificationEventArgs"/></param>
        public AppNotificationEventArgsViewToContractHostAdapter(AppNotificationEventArgs view)
        {
            _view = view;
        }

        /// <summary>
        /// App Notification Data
        /// </summary>
        public string Data
        {
            get
            {
                return _view.Data;
            }
        }

        /// <summary>
        /// Get the App Notification Event Args Source View
        /// </summary>
        /// <returns><see cref="AppNotificationEventArgs"/></returns>
        internal AppNotificationEventArgs GetSourceView()
        {
            return _view;
        }
    }
}
