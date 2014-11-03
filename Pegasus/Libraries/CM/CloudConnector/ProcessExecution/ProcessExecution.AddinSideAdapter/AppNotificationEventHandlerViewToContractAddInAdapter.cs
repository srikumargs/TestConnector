using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// App Notification Event Handler View to Contract AddIn Adapter
    /// </summary>
    public class AppNotificationEventHandlerViewToContractAddInAdapter : ContractBase, IAppNotificationEventHandler
    {

        private readonly object _view;

        private readonly System.Reflection.MethodInfo _event;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view">Object view</param>
        /// <param name="eventProp"><see cref=" System.Reflection.MethodInfo"/></param>
        public AppNotificationEventHandlerViewToContractAddInAdapter(object view, System.Reflection.MethodInfo eventProp)
        {
            _view = view;
            _event = eventProp;
        }

        /// <summary>
        /// App Notification Event Handler
        /// </summary>
        /// <param name="args"><see cref="IAppNotificationEventArgs"/></param>
        public void Handler(IAppNotificationEventArgs args)
        {
            AppNotificationEventArgsContractToViewAddInAdapter adaptedArgs = new AppNotificationEventArgsContractToViewAddInAdapter(args);
            object[] argsArray = new object[1];
            argsArray[0] = adaptedArgs;
            _event.Invoke(_view, argsArray);
        }

        /// <summary>
        /// Get Source View of the App Notification Event handler 
        /// </summary>
        /// <returns>The Source View</returns>
        internal object GetSourceView()
        {
            return _view;
        }
    }
}
