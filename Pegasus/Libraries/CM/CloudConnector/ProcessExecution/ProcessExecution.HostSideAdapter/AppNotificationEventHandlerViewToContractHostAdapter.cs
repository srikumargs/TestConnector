using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// App Notification Event Handler View to Contract Host Adapter
    /// </summary>
    public class AppNotificationEventHandlerViewToContractHostAdapter : ContractBase, IAppNotificationEventHandler
    {
        
        private readonly object _view;
        
        private readonly System.Reflection.MethodInfo _event;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view">The view object</param>
        /// <param name="eventProp"><see cref="System.Reflection.MethodInfo"/></param>
        public AppNotificationEventHandlerViewToContractHostAdapter(object view, System.Reflection.MethodInfo eventProp)
        {
            _view = view;
            _event = eventProp;
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="args"><see cref="IAppNotificationEventArgs"/></param>
        public void Handler(IAppNotificationEventArgs args) {
            AppNotificationEventArgsContractToViewHostAdapter adaptedArgs = new AppNotificationEventArgsContractToViewHostAdapter(args);
            object[] argsArray = new object[1];
            argsArray[0] = adaptedArgs;
            _event.Invoke(_view, argsArray);
        }
        
        /// <summary>
        /// Get the App Notification Event Handler Source View
        /// </summary>
        /// <returns>the event handler view object</returns>
        internal object GetSourceView() {
            return _view;
        }
    }
}
