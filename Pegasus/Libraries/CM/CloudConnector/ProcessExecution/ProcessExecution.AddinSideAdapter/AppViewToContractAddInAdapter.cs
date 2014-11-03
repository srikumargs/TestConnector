using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.AddinView.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// App View to Contract AddIn Adapter
    /// </summary>
    public class AppViewToContractAddInAdapter : ContractBase, IAppContract
    {

        private readonly IApp _view;

        private readonly Dictionary<IAppNotificationEventHandler, EventHandler<AppNotificationEventArgs>> _appNotificationHandlers;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="view"><see cref="IApp"/></param>
        public AppViewToContractAddInAdapter(IApp view)
        {
            _view = view;
            _appNotificationHandlers = new Dictionary<IAppNotificationEventHandler, EventHandler<AppNotificationEventArgs>>();
        }

        /// <summary>
        /// Add the AppNotification Event handler 
        /// </summary>
        /// <param name="handler"><see cref="IAppNotificationEventHandler"/> to be added</param>
        public virtual void AppNotificationEventAdd(IAppNotificationEventHandler handler)
        {
            EventHandler<AppNotificationEventArgs> adaptedHandler = new AppNotificationEventHandlerContractToViewAddInAdapter(handler).Handler;
            _view.AppNotification += adaptedHandler;
            _appNotificationHandlers[handler] = adaptedHandler;
        }

        /// <summary>
        /// Remove the App Notification Event handler
        /// </summary>
        /// <param name="handler"><see cref="IAppNotificationEventHandler"/> to be removed</param>
        public virtual void AppNotificationEventRemove(IAppNotificationEventHandler handler)
        {
            EventHandler<AppNotificationEventArgs> adaptedHandler;
            if (!_appNotificationHandlers.TryGetValue(handler, out adaptedHandler)) return;

            _appNotificationHandlers.Remove(handler);
            _view.AppNotification -= adaptedHandler;
        }

        /// <summary>
        /// get the App Source View
        /// </summary>
        /// <returns><see cref="IApp"/></returns>
        internal IApp GetSourceView()
        {
            return _view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ILoggingContract GetLogger()
        {
            return LoggingAddInAdapter.ViewToContractAdapter(_view.GetLogger());
        }
    }
}
