
using System;
using System.Collections.Generic;
using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.Interfaces;
using Sage.Connector.ProcessExecution.Interfaces.Events;


namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// App View to Contract Host Adapter
    /// </summary>
    public class AppViewToContractHostAdapter : System.AddIn.Pipeline.ContractBase, IAppContract
    {

        private readonly IApp _view;

        private readonly Dictionary<IAppNotificationEventHandler,
            EventHandler<AppNotificationEventArgs>> _appNotificationHandlers;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"><see cref="IApp"/></param>
        public AppViewToContractHostAdapter(IApp view)
        {
            _view = view;
            _appNotificationHandlers = new Dictionary<IAppNotificationEventHandler, EventHandler<AppNotificationEventArgs>>();
        }

        /// <summary>
        /// Add the App Notification event handler 
        /// </summary>
        /// <param name="handler"><see cref="IAppNotificationEventHandler"/></param>
        public virtual void AppNotificationEventAdd(IAppNotificationEventHandler handler)
        {
            EventHandler<AppNotificationEventArgs> adaptedHandler = new AppNotificationEventHandlerContractToViewHostAdapter(handler).Handler;
            _view.AppNotification += adaptedHandler;
            _appNotificationHandlers[handler] = adaptedHandler;
        }

        /// <summary>
        /// Remove the App Notification Event Handler
        /// </summary>
        /// <param name="handler"><see cref="IAppNotificationEventHandler"/></param>
        public virtual void AppNotificationEventRemove(IAppNotificationEventHandler handler)
        {
            EventHandler<AppNotificationEventArgs> adaptedHandler;
            if (!_appNotificationHandlers.TryGetValue(handler, out adaptedHandler)) return;

            _appNotificationHandlers.Remove(handler);
            _view.AppNotification -= adaptedHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ILoggingContract GetLogger()
        {
            return LoggingHostAdapter.ViewToContractAdapter(_view.GetLogger());
        }


        /// <summary>
        /// Get the App Source View
        /// </summary>
        /// <returns></returns>
        internal IApp GetSourceView()
        {
            return _view;
        }
    }
}
