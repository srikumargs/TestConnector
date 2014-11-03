using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.AddinView.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// App Notification Event Args View to Contract AddIn Adapter
    /// </summary>
    public class AppNotificationEventArgsViewToContractAddInAdapter : ContractBase, IAppNotificationEventArgs
    {

        private readonly AppNotificationEventArgs _view;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"><see cref="AppNotificationEventArgs"/></param>
        public AppNotificationEventArgsViewToContractAddInAdapter(AppNotificationEventArgs view)
        {
            _view = view;
        }

        /// <summary>
        /// The App Notification Event Args Data
        /// </summary>
        public string Data
        {
            get
            {
                return _view.Data;
            }
        }

        /// <summary>
        /// Get the App notification event args source view. 
        /// </summary>
        /// <returns><see cref="AppNotificationEventArgs"/></returns>
        internal AppNotificationEventArgs GetSourceView()
        {
            return _view;
        }
    }
}
